﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

global using System.ComponentModel.DataAnnotations;
global using System.ComponentModel.DataAnnotations.Schema;
global using System.Linq.Expressions;
global using System.Text.Encodings.Web;
global using System.Text.Json;
global using System.Text.Unicode;
global using System.Xml.Linq;
global using Dapr.Client;
global using FluentValidation;
global using FluentValidation.AspNetCore;
global using Mapster;
global using Masa.BuildingBlocks.Authentication.Identity;
global using Masa.BuildingBlocks.Caching;
global using Masa.BuildingBlocks.Data.UoW;
global using Masa.BuildingBlocks.Ddd.Domain.Entities.Full;
global using Masa.BuildingBlocks.Ddd.Domain.Events;
global using Masa.BuildingBlocks.Ddd.Domain.Repositories;
global using Masa.BuildingBlocks.Ddd.Domain.Services;
global using Masa.BuildingBlocks.Dispatcher.Events;
global using Masa.BuildingBlocks.Dispatcher.IntegrationEvents;
global using Masa.BuildingBlocks.Globalization.I18n;
global using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Commands;
global using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Queries;
global using Masa.BuildingBlocks.StackSdks.Auth.Contracts;
global using Masa.BuildingBlocks.StackSdks.Auth.Contracts.Consts;
global using Masa.BuildingBlocks.StackSdks.Config;
global using Masa.BuildingBlocks.StackSdks.Dcc.Contracts.Model;
global using Masa.BuildingBlocks.StackSdks.Middleware;
global using Masa.BuildingBlocks.StackSdks.Pm;
global using Masa.BuildingBlocks.StackSdks.Pm.Enum;
global using Masa.BuildingBlocks.StackSdks.Pm.Model;
global using Masa.Contrib.Caching.Distributed.StackExchangeRedis;
global using Masa.Contrib.Ddd.Domain.Repository.EFCore;
global using Masa.Contrib.Dispatcher.Events;
global using Masa.Contrib.StackSdks.Config;
global using Masa.Contrib.StackSdks.Middleware;
global using Masa.Contrib.StackSdks.Tsc;
global using Masa.Dcc.Contracts.Admin.App.Dtos;
global using Masa.Dcc.Contracts.Admin.App.Enums;
global using Masa.Dcc.Contracts.Admin.Label.Dtos;
global using Masa.Dcc.Service.Admin.Application.App.Commands;
global using Masa.Dcc.Service.Admin.Application.App.Queries;
global using Masa.Dcc.Service.Admin.Application.Label.Commands;
global using Masa.Dcc.Service.Admin.Application.Label.Queries;
global using Masa.Dcc.Service.Admin.Domain.App.Aggregates;
global using Masa.Dcc.Service.Admin.Domain.App.Repositories;
global using Masa.Dcc.Service.Admin.Domain.App.Services;
global using Masa.Dcc.Service.Admin.Domain.Label.Aggregates;
global using Masa.Dcc.Service.Admin.Domain.Label.Repositories;
global using Masa.Dcc.Service.Admin.Domain.Label.Services;
global using Masa.Dcc.Service.Admin.Migrations;
global using Masa.Dcc.Service.Infrastructure;
global using Masa.Utils.Configuration.Json;
global using Masa.Utils.Security.Cryptography;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.EntityFrameworkCore;
global using Newtonsoft.Json.Linq;
global using System.Xml.Linq;
global using FluentValidation.Resources;
