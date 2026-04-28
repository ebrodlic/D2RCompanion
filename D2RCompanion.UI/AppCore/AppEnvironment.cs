// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace D2RCompanion.UI.AppCore
{
    public class AppEnvironment
    {
        public bool IsDevelopment { get; }

        public AppEnvironment()
        {
            var env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

            IsDevelopment =
                env == "Development" ||
                Debugger.IsAttached;
        }
    }
}
