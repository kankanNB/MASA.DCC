﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Dcc.Service.Admin.Domain.App.Services
{
    public class ConfigObjectDomainService : DomainService
    {
        private readonly DccDbContext _context;
        private readonly IConfigObjectReleaseRepository _configObjectReleaseRepository;
        private readonly IConfigObjectRepository _configObjectRepository;
        private readonly IAppConfigObjectRepository _appConfigObjectRepository;
        private readonly IBizConfigObjectRepository _bizConfigObjectRepository;
        private readonly IBizConfigRepository _bizConfigRepository;
        private readonly IPublicConfigObjectRepository _publicConfigObjectRepository;
        private readonly IPublicConfigRepository _publicConfigRepository;
        private readonly IMultilevelCacheClient _memoryCacheClient;
        private readonly IPmClient _pmClient;
        private readonly DaprClient _daprClient;
        private readonly IUnitOfWork _unitOfWork;

        public ConfigObjectDomainService(
            IDomainEventBus eventBus,
            DccDbContext context,
            IConfigObjectReleaseRepository configObjectReleaseRepository,
            IConfigObjectRepository configObjectRepository,
            IAppConfigObjectRepository appConfigObjectRepository,
            IBizConfigObjectRepository bizConfigObjectRepository,
            IBizConfigRepository bizConfigRepository,
            IPublicConfigObjectRepository publicConfigObjectRepository,
            IPublicConfigRepository publicConfigRepository,
            IMultilevelCacheClient memoryCacheClient,
            IPmClient pmClient,
            DaprClient daprClient,
            IUnitOfWork unitOfWork) : base(eventBus)
        {
            _context = context;
            _configObjectReleaseRepository = configObjectReleaseRepository;
            _configObjectRepository = configObjectRepository;
            _appConfigObjectRepository = appConfigObjectRepository;
            _bizConfigObjectRepository = bizConfigObjectRepository;
            _bizConfigRepository = bizConfigRepository;
            _publicConfigObjectRepository = publicConfigObjectRepository;
            _publicConfigRepository = publicConfigRepository;
            _memoryCacheClient = memoryCacheClient;
            _pmClient = pmClient;
            _daprClient = daprClient;
            _unitOfWork = unitOfWork;
        }

        public async Task AddConfigObjectAsync(List<AddConfigObjectDto> configObjectDtos)
        {
            List<ConfigObject> configObjects = new();
            foreach (var configObjectDto in configObjectDtos)
            {
                var configObject = new ConfigObject(
                    configObjectDto.Name,
                    configObjectDto.FormatLabelCode,
                    configObjectDto.Type,
                    configObjectDto.Content,
                    configObjectDto.TempContent,
                    configObjectDto.RelationConfigObjectId,
                    configObjectDto.FromRelation,
                    configObjectDto.Encryption);

                configObjects.Add(configObject);

                if (configObjectDto.Type == ConfigObjectType.Public)
                {
                    configObject.SetPublicConfigObject(configObjectDto.ObjectId, configObjectDto.EnvironmentClusterId);
                }
                else if (configObjectDto.Type == ConfigObjectType.App)
                {
                    configObject.SetAppConfigObject(configObjectDto.ObjectId, configObjectDto.EnvironmentClusterId);
                }
                else if (configObjectDto.Type == ConfigObjectType.Biz)
                {
                    configObject.SetBizConfigObject(configObjectDto.ObjectId, configObjectDto.EnvironmentClusterId);
                }
            }

            await _configObjectRepository.AddRangeAsync(configObjects);
        }

        public async Task RemoveConfigObjectAsync(RemoveConfigObjectDto dto)
        {
            var configObjectEntity = await _configObjectRepository.FindAsync(p => p.Id == dto.ConfigObjectId)
                ?? throw new UserFriendlyException("Config object does not exist");

            await _configObjectRepository.RemoveAsync(configObjectEntity);

            var key = $"{dto.EnvironmentName}-{dto.ClusterName}-{dto.AppId}-{configObjectEntity.Name}";
            await _memoryCacheClient.RemoveAsync<PublishReleaseModel>(key.ToLower());
        }

        public async Task UpdateConfigObjectContentAsync(UpdateConfigObjectContentDto dto)
        {
            var configObject = await _configObjectRepository.FindAsync(configObject => configObject.Id == dto.ConfigObjectId)
                ?? throw new UserFriendlyException("Config object does not exist");

            if (dto.FormatLabelCode.Trim().ToLower() != "properties")
            {
                string content = dto.Content;
                if (configObject.Encryption)
                {
                    content = await EncryptContentAsync(dto.Content);
                }

                configObject.UpdateContent(content);
                configObject.UnRelation();
            }
            else
            {
                string content = configObject.Content;
                if (configObject.Encryption && configObject.Content != "[]")
                {
                    content = await DecryptContentAsync(configObject.Content);
                }

                var propertyEntities = JsonSerializer.Deserialize<List<ConfigObjectPropertyContentDto>>(content) ?? new();
                if (dto.AddConfigObjectPropertyContent.Any())
                {
                    propertyEntities.AddRange(dto.AddConfigObjectPropertyContent);
                }
                if (dto.DeleteConfigObjectPropertyContent.Any())
                {
                    propertyEntities.RemoveAll(prop => dto.DeleteConfigObjectPropertyContent.Select(prop => prop.Key).Contains(prop.Key));
                }
                if (dto.EditConfigObjectPropertyContent.Any())
                {
                    propertyEntities.RemoveAll(prop => dto.EditConfigObjectPropertyContent.Select(prop => prop.Key).Contains(prop.Key));
                    propertyEntities.AddRange(dto.EditConfigObjectPropertyContent);
                }

                content = JsonSerializer.Serialize(propertyEntities);
                if (configObject.Encryption)
                {
                    content = await EncryptContentAsync(content);
                }
                configObject.UpdateContent(content);
            }

            await _configObjectRepository.UpdateAsync(configObject);
        }

        private async Task<string> EncryptContentAsync(string content)
        {
            var config = await _daprClient.GetSecretAsync("localsecretstore", "dcc-config");
            var secret = config["dcc-config-secret"];

            var encryptContent = AesUtils.Encrypt(content, secret, FillType.Left);
            return encryptContent;
        }

        private async Task<string> DecryptContentAsync(string content)
        {
            var config = await _daprClient.GetSecretAsync("localsecretstore", "dcc-config");
            var secret = config["dcc-config-secret"];

            var encryptContent = AesUtils.Decrypt(content, secret, FillType.Left);
            return encryptContent;
        }

        public async Task CloneConfigObjectAsync(CloneConfigObjectDto dto)
        {
            //add
            await CloneConfigObjectsAsync(dto.ConfigObjects, dto.ToObjectId);

            //update
            var envClusterIds = dto.CoverConfigObjects.Select(c => c.EnvironmentClusterId);

            IEnumerable<ConfigObject> needRemove = new List<ConfigObject>();
            if (dto.ConfigObjectType == ConfigObjectType.App)
            {
                var appConfigObjects = await _appConfigObjectRepository.GetListAsync(
                    app => app.AppId == dto.ToObjectId && envClusterIds.Contains(app.EnvironmentClusterId));
                needRemove = await _configObjectRepository.GetListAsync(c => appConfigObjects.Select(app => app.ConfigObjectId).Contains(c.Id));
            }
            else if (dto.ConfigObjectType == ConfigObjectType.Biz)
            {
                var bizConfigObjects = await _bizConfigObjectRepository.GetListAsync(
                    biz => biz.BizConfigId == dto.ToObjectId && envClusterIds.Contains(biz.EnvironmentClusterId));
                needRemove = await _configObjectRepository.GetListAsync(c => bizConfigObjects.Select(biz => biz.ConfigObjectId).Contains(c.Id));
            }
            else if (dto.ConfigObjectType == ConfigObjectType.Public)
            {
                var publicConfigObjects = await _publicConfigObjectRepository.GetListAsync(
                    publicConfig => publicConfig.PublicConfigId == dto.ToObjectId && envClusterIds.Contains(publicConfig.EnvironmentClusterId));
                needRemove = await _configObjectRepository.GetListAsync(c => publicConfigObjects.Select(publicConfig => publicConfig.ConfigObjectId).Contains(c.Id));
            }

            await _configObjectRepository.RemoveRangeAsync(needRemove);
            await CloneConfigObjectsAsync(dto.CoverConfigObjects, dto.ToObjectId);
        }

        private async Task CloneConfigObjectsAsync(List<AddConfigObjectDto> configObjects, int appId)
        {
            await CheckConfigObjectDuplication(configObjects, appId);

            List<ConfigObject> cloneConfigObjects = new();
            foreach (var configObjectDto in configObjects)
            {
                var configObject = new ConfigObject(
                    configObjectDto.Name,
                    configObjectDto.FormatLabelCode,
                    configObjectDto.Type,
                    configObjectDto.Content,
                    configObjectDto.TempContent,
                    encryption: configObjectDto.Encryption);
                cloneConfigObjects.Add(configObject);

                if (configObjectDto.Type == ConfigObjectType.Public)
                {
                    configObject.SetPublicConfigObject(appId, configObjectDto.EnvironmentClusterId);
                }
                else if (configObjectDto.Type == ConfigObjectType.App)
                {
                    configObject.SetAppConfigObject(appId, configObjectDto.EnvironmentClusterId);
                }
                else if (configObjectDto.Type == ConfigObjectType.Biz)
                {
                    configObject.SetBizConfigObject(appId, configObjectDto.EnvironmentClusterId);
                }

                if (configObject.Encryption)
                {
                    var encryptConten = await EncryptContentAsync(configObject.Content);
                    configObject.UpdateContent(encryptConten);
                }
            }
            await _configObjectRepository.AddRangeAsync(cloneConfigObjects);
        }

        private async Task CheckConfigObjectDuplication(List<AddConfigObjectDto> configObjects, int appId)
        {
            if (configObjects?.Count > 0)
            {
                var configType = configObjects.First().Type;
                var configObjectNames = configObjects.Select(e => e.Name);
                switch (configType)
                {
                    case ConfigObjectType.Public:
                        var allPublicConfigs = await _publicConfigObjectRepository.GetListByPublicConfigIdAsync(appId);
                        foreach (var item in configObjects)
                        {
                            if (allPublicConfigs.Any(e => e.EnvironmentClusterId == item.EnvironmentClusterId && e.ConfigObject.Name == item.Name))
                            {
                                throw new UserFriendlyException($"Configuration Name '{item.Name}' already exist in the environment cluster '{item.EnvironmentClusterId}'.");
                            }
                        }
                        break;
                    case ConfigObjectType.Biz:
                        var bizConfigObjects = await _bizConfigObjectRepository.GetListByBizConfigIdAsync(appId);
                        foreach (var item in configObjects)
                        {
                            if (bizConfigObjects.Any(e => e.EnvironmentClusterId == item.EnvironmentClusterId && e.ConfigObject.Name == item.Name))
                            {
                                throw new UserFriendlyException($"Configuration Name '{item.Name}' already exist in the environment cluster '{item.EnvironmentClusterId}'.");
                            }
                        }
                        break;
                    case ConfigObjectType.App:
                        var allAppConfigObjects = await _appConfigObjectRepository.GetListByAppIdAsync(appId);
                        foreach (var item in configObjects)
                        {
                            if (allAppConfigObjects.Any(e => e.EnvironmentClusterId == item.EnvironmentClusterId && e.ConfigObject.Name == item.Name))
                            {
                                throw new UserFriendlyException($"Configuration Name '{item.Name}' already exist for environment cluster's '{item.EnvironmentClusterId}'.  ");
                            }
                        }
                        break;
                }
            }
        }

        public async Task AddConfigObjectReleaseAsync(AddConfigObjectReleaseDto dto)
        {
            var configObject = (await _configObjectRepository.FindAsync(
                configObject => configObject.Id == dto.ConfigObjectId)) ?? throw new Exception("Config object does not exist");

            configObject.AddContent(configObject.Content, configObject.Content);
            await _configObjectRepository.UpdateAsync(configObject);

            await _configObjectReleaseRepository.AddAsync(new ConfigObjectRelease(
                   dto.ConfigObjectId,
                   dto.Name,
                   dto.Comment,
                   configObject.Content)
               );

            var relationConfigObjects = await _configObjectRepository.GetRelationConfigObjectWithReleaseHistoriesAsync(configObject.Id);
            if (relationConfigObjects.Any())
            {
                var allEnvClusters = await _pmClient.ClusterService.GetEnvironmentClustersAsync();
                var appConfigObjects = await _appConfigObjectRepository.GetListAsync(app => relationConfigObjects.Select(c => c.Id).Contains(app.ConfigObjectId));
                var apps = await _pmClient.AppService.GetListAsync();

                foreach (var item in appConfigObjects)
                {
                    var envCluster = allEnvClusters.FirstOrDefault(c => c.Id == item.EnvironmentClusterId) ?? new();
                    var app = apps.FirstOrDefault(a => a.Id == item.AppId) ?? new();
                    var relationConfigObject = relationConfigObjects.First(c => c.Id == item.ConfigObjectId);
                    var key = $"{envCluster.EnvironmentName}-{envCluster.ClusterName}-{app.Identity}-{relationConfigObject.Name}";

                    if (relationConfigObject.FormatLabelCode.ToLower() == "properties")
                    {
                        var appRelease = relationConfigObject.ConfigObjectRelease.OrderByDescending(c => c.Id).FirstOrDefault();
                        if (appRelease == null)
                        {
                            await _memoryCacheClient.SetAsync(key.ToLower(), new PublishReleaseModel
                            {
                                Content = dto.Content,
                                FormatLabelCode = configObject.FormatLabelCode,
                                Encryption = configObject.Encryption
                            });
                        }
                        else
                        {
                            //compare
                            var publicContents = JsonSerializer.Deserialize<List<ConfigObjectPropertyContentDto>>(dto.Content) ?? new();
                            var appContents = JsonSerializer.Deserialize<List<ConfigObjectPropertyContentDto>>(appRelease.Content) ?? new();

                            var exceptContent = publicContents.ExceptBy(appContents.Select(c => c.Key), content => content.Key).ToList();
                            var content = appContents.Union(exceptContent).ToList();

                            var releaseContent = new PublishReleaseModel
                            {
                                Content = JsonSerializer.Serialize(content),
                                FormatLabelCode = configObject.FormatLabelCode,
                                Encryption = configObject.Encryption
                            };
                            await _memoryCacheClient.SetAsync(key.ToLower(), releaseContent);
                        }
                    }
                    else
                    {
                        var releaseContent = new PublishReleaseModel
                        {
                            Content = dto.Content,
                            FormatLabelCode = configObject.FormatLabelCode,
                            Encryption = configObject.Encryption
                        };
                        await _memoryCacheClient.SetAsync(key.ToLower(), releaseContent);
                    }
                }
            }
            else
            {
                //add redis cache
                //TODO: encryption value
                var key = $"{dto.EnvironmentName}-{dto.ClusterName}-{dto.Identity}-{configObject.Name}";
                if (configObject.Encryption)
                {
                    dto.Content = await EncryptContentAsync(dto.Content);
                }
                var releaseContent = new PublishReleaseModel
                {
                    Content = dto.Content,
                    FormatLabelCode = configObject.FormatLabelCode,
                    Encryption = configObject.Encryption
                };
                await _memoryCacheClient.SetAsync(key.ToLower(), releaseContent);
            }
        }

        public async Task RollbackConfigObjectReleaseAsync(RollbackConfigObjectReleaseDto rollbackDto)
        {
            var latestConfigObjectRelease = await _context.Set<ConfigObjectRelease>()
                .Where(cor => cor.ConfigObjectId == rollbackDto.ConfigObjectId)
                .OrderByDescending(cor => cor.Id)
                .FirstOrDefaultAsync();

            var rollbackToEntity = await _configObjectReleaseRepository.FindAsync(
                ocr => ocr.Id == rollbackDto.RollbackToReleaseId);

            if (latestConfigObjectRelease == null || rollbackToEntity == null)
            {
                throw new Exception("要回滚的版本不存在");
            }

            if (rollbackDto.RollbackToReleaseId == latestConfigObjectRelease.FromReleaseId)
            {
                throw new UserFriendlyException("该版本已作废");
            }
            if (rollbackToEntity.Version == latestConfigObjectRelease.Version)
            {
                throw new UserFriendlyException("两个版本相同");
            }

            //rollback
            //add
            await _configObjectReleaseRepository.AddAsync(new ConfigObjectRelease(
                     rollbackToEntity.ConfigObjectId,
                     rollbackToEntity.Name,
                     $"由 {latestConfigObjectRelease.Name} 回滚至 {rollbackToEntity.Name}",
                     rollbackToEntity.Content,
                     rollbackToEntity.Version,
                     latestConfigObjectRelease.Id
                 ));

            //Invalid rollback entity
            latestConfigObjectRelease.Invalid();
            await _configObjectReleaseRepository.UpdateAsync(latestConfigObjectRelease);

            //Update ConfigObject entity
            var configObject = (await _configObjectRepository.FindAsync(config => config.Id == rollbackDto.ConfigObjectId))!;
            configObject.AddContent(configObject.Content, rollbackToEntity.Content);
            await _configObjectRepository.UpdateAsync(configObject);
        }

        public async Task UpdateConfigObjectAsync(string environmentName, string clusterName, string appId,
            string configObjectName,
            object value)
        {
            var configObject = await _configObjectRepository.FindAsync(config => config.Name == configObjectName) ?? throw new UserFriendlyException("ConfigObject does not exist");

            string newValue = JsonSerializer.Serialize(value);

            if (configObject.Encryption)
            {
                newValue = await EncryptContentAsync(newValue);
            }

            configObject.UpdateContent(newValue);

            await _configObjectRepository.UpdateAsync(configObject);

            var releaseModel = new AddConfigObjectReleaseDto
            {
                Type = ReleaseType.MainRelease,
                ConfigObjectId = configObject.Id,
                Name = "通过Sdk发布",
                EnvironmentName = environmentName,
                ClusterName = clusterName,
                Identity = appId,
                Content = newValue
            };

            await AddConfigObjectReleaseAsync(releaseModel);
        }

        public async Task InitConfigObjectAsync(string environmentName, string clusterName, string appId,
            Dictionary<string, string> configObjects,
            bool isEncryption)
        {
            var envs = await _pmClient.EnvironmentService.GetListAsync();
            var env = envs.FirstOrDefault(e => e.Name.ToLower() == environmentName.ToLower());
            if (env == null)
                throw new UserFriendlyException("Environment does not exist");
            var clusters = await _pmClient.ClusterService.GetListByEnvIdAsync(env.Id);
            var cluster = clusters.FirstOrDefault(c => c.Name.ToLower() == clusterName.ToLower());
            if (cluster == null)
                throw new UserFriendlyException("Cluster does not exist");
            foreach (var configObject in configObjects)
            {
                var configObjectName = configObject.Key;
                var key = $"{environmentName}-{clusterName}-{appId}-{configObjectName}".ToLower();
                var redisData = await _memoryCacheClient.GetAsync<PublishReleaseModel?>(key);
                if (redisData != null)
                {
                    continue;
                }

                string content = configObject.Value;
                if (isEncryption)
                    content = await EncryptContentAsync(content);
                var newConfigObject = new ConfigObject(
                    configObjectName,
                    "Json",
                    ConfigObjectType.App,
                    content,
                    "{}",
                encryption: isEncryption);

                var publicConfig = await _publicConfigRepository.FindAsync(publicConfig => publicConfig.Identity == appId);
                if (publicConfig != null)
                {
                    newConfigObject.SetPublicConfigObject(publicConfig.Id, cluster.EnvironmentClusterId);
                }
                else
                {
                    var bizConfig = await _bizConfigRepository.FindAsync(bizConfig => bizConfig.Identity == appId);
                    if (bizConfig != null)
                    {
                        newConfigObject.SetBizConfigObject(bizConfig.Id, cluster.EnvironmentClusterId);
                    }
                    else
                    {
                        var app = await _pmClient.AppService.GetByIdentityAsync(appId);
                        if (app != null)
                        {
                            newConfigObject.SetAppConfigObject(app.Id, cluster.EnvironmentClusterId);
                        }
                        else
                        {
                            throw new UserFriendlyException("AppId Error");
                        }
                    }
                }
                await _configObjectRepository.AddAsync(newConfigObject);
                await _unitOfWork.SaveChangesAsync();

                var releaseModel = new AddConfigObjectReleaseDto
                {
                    Type = ReleaseType.MainRelease,
                    ConfigObjectId = newConfigObject.Id,
                    Name = "通过Sdk发布",
                    EnvironmentName = environmentName,
                    ClusterName = clusterName,
                    Identity = appId,
                    Content = configObject.Value,
                };
                await AddConfigObjectReleaseAsync(releaseModel);
            }
        }

        public async Task<string> RefreshConfigObjectToRedisAsync()
        {
            var configObjectInfo = await _configObjectRepository.GetNewestConfigObjectReleaseWithAppInfo();
            var apps = await _pmClient.AppService.GetListAsync();
            var envClusters = await _pmClient.ClusterService.GetEnvironmentClustersAsync();
            var publicConfig = (await _publicConfigRepository.GetListAsync()).FirstOrDefault()
                ?? throw new UserFriendlyException("PublicConfig is null");

            configObjectInfo.ForEach(config =>
            {
                if (config.ConfigObject.Type == ConfigObjectType.App)
                {
                    apps.Where(app => app.Id == config.ObjectId).ToList().ForEach(app =>
                    {
                        app.EnvironmentClusters.ForEach(async envCluster =>
                        {
                            if (envCluster.Id == config.EnvironmentClusterId)
                            {
                                var key = $"{envCluster.EnvironmentName}-{envCluster.ClusterName}-{app.Identity}-{config.ConfigObject.Name}";
                                await _memoryCacheClient.SetAsync(key.ToLower(), new PublishReleaseModel
                                {
                                    Content = config.ConfigObject.Content,
                                    FormatLabelCode = config.ConfigObject.FormatLabelCode,
                                    Encryption = config.ConfigObject.Encryption
                                });
                            }
                        });
                    });
                }
                else if (config.ConfigObject.Type == ConfigObjectType.Public)
                {
                    envClusters.Where(ec => ec.Id == config.EnvironmentClusterId).ToList().ForEach(async envCluster =>
                    {
                        var key = $"{envCluster.EnvironmentName}-{envCluster.ClusterName}-{publicConfig.Identity}-{config.ConfigObject.Name}";
                        await _memoryCacheClient.SetAsync(key.ToLower(), new PublishReleaseModel
                        {
                            Content = config.ConfigObject.Content,
                            FormatLabelCode = config.ConfigObject.FormatLabelCode,
                            Encryption = config.ConfigObject.Encryption
                        });
                    });
                }
            });

            return "success";
        }
    }
}
