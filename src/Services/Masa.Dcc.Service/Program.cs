﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.Contrib.Dispatcher.IntegrationEvents.Dapr;
using Masa.Utils.Caching.DistributedMemory.DependencyInjection;
using Masa.Utils.Caching.Redis.DependencyInjection;
using Masa.Utils.Caching.Redis.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDaprClient();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.Authority = "";
    options.RequireHttpsMetadata = false;
    options.Audience = "";
});

var redisOptions = builder.Configuration.GetSection("redis").Get<RedisConfigurationOptions>();
builder.Services.AddMasaRedisCache(redisOptions)
                .AddMasaMemoryCache();

//builder.Services.AddPmClient("https://pm-service-dev.masastack.com/");
builder.Services.AddPmClient("http://localhost:19401/");

var app = builder.Services
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddTransient(typeof(IMiddleware<>), typeof(LogMiddleware<>))
    .AddFluentValidation(options =>
    {
        options.RegisterValidatorsFromAssemblyContaining<Program>();
    })
    .AddTransient(typeof(IMiddleware<>), typeof(ValidatorMiddleware<>))
    .AddDomainEventBus(options =>
    {
        options.UseDaprEventBus<IntegrationEventLogService>(options => options.UseEventLog<DccDbContext>())
               .UseEventBus()
               .UseUoW<DccDbContext>(dbOptions => dbOptions.UseSqlServer().UseSoftDelete())
               .UseEventLog<DccDbContext>()
               .UseRepository<DccDbContext>();
    })
    .AddServices(builder);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseCloudEvents();
app.UseEndpoints(endpoints =>
{
    endpoints.MapSubscribeHandler();
});
app.UseHttpsRedirection();

app.Run();
