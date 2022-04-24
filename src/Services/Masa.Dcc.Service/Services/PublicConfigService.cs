﻿using Microsoft.AspNetCore.Mvc;

namespace Masa.Dcc.Service.Services;

public class PublicConfigService : ServiceBase
{
    public PublicConfigService(IServiceCollection services) : base(services)
    {
        App.MapPost("api/v1/publicConfig", AddAsync);
        App.MapPut("api/v1/publicConfig", UpdateAsync);
        App.MapDelete("api/v1/publicConfig/{Id}", RemoveAsync);
    }

    public async Task AddAsync(IEventBus eventBus, AddPublicConfigDto dto)
    {
        await eventBus.PublishAsync(new AddPublicConfigCommand(dto));
    }

    public async Task UpdateAsync(IEventBus eventBus, UpdatePublicConfigDto dto)
    {
        await eventBus.PublishAsync(new UpdatePublicConfigCommand(dto));
    }

    public async Task RemoveAsync(IEventBus eventBus, [FromQuery] int Id)
    {
        await eventBus.PublishAsync(new RemovePublicConfigCommand(Id));
    }
}
