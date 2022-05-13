﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Dcc.Service.Admin.Application.App.Queries
{
    public record ConfigObjectsQuery(int EnvClusterId, ConfigObjectType Type, string ConfigObjectName) : Query<List<ConfigObjectDto>>
    {
        public override List<ConfigObjectDto> Result { get; set; } = new();
    }
}
