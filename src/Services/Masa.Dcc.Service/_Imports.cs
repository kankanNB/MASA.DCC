﻿global using FluentValidation;
global using FluentValidation.AspNetCore;
global using Masa.BuildingBlocks.BasicAbility.Pm;
global using Masa.BuildingBlocks.BasicAbility.Pm.Model;
global using Masa.BuildingBlocks.Data.Contracts;
global using Masa.BuildingBlocks.Data.UoW;
global using Masa.BuildingBlocks.Ddd.Domain.Entities.Auditing;
global using Masa.BuildingBlocks.Ddd.Domain.Events;
global using Masa.BuildingBlocks.Ddd.Domain.Repositories;
global using Masa.BuildingBlocks.Dispatcher.Events;
global using Masa.Contrib.Ddd.Domain;
global using Masa.Contrib.Ddd.Domain.Events;
global using Masa.Contrib.Ddd.Domain.Repository.EF;
global using Masa.Contrib.Dispatcher.Events;
global using Masa.Contrib.Dispatcher.Events.Enums;
global using Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EF;
global using Masa.Contrib.ReadWriteSpliting.Cqrs.Commands;
global using Masa.Contrib.Service.MinimalAPIs;
global using Masa.Dcc.Contracts.Admin.Enum;
global using Masa.Dcc.Service.Application.Orders.Commands;
global using Masa.Dcc.Service.Application.Orders.Queries;
global using Masa.Dcc.Service.Domain.Events;
global using Masa.Dcc.Service.Domain.Repositories;
global using Masa.Dcc.Service.Domain.Services;
global using Masa.Dcc.Service.Infrastructure.Middleware;
global using Masa.Utils.Data.EntityFrameworkCore;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.OpenApi.Models;
global using System.ComponentModel.DataAnnotations;
global using System.ComponentModel.DataAnnotations.Schema;
