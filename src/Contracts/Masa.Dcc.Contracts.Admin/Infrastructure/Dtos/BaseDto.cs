﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Dcc.Contracts.Admin.Infrastructure.Dtos
{
    public class BaseDto
    {
        public Guid Creator { get; set; }

        public DateTime CreationTime { get; set; }

        public Guid Modifier { get; set; }

        public DateTime ModificationTime { get; set; }
    }
}
