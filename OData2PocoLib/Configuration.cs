﻿// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco;
#nullable disable
public class Configuration
{
    public OdataConnectionString ConnectionString { get; set; }
    public PocoSetting Setting { get; set; }
}
#nullable restore