using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace D2RCompanion.UI.AppCore
{
    public class AppInfo
    {
        public string Id { get; }
        public string Name { get; }
        public string Version { get; }
        public AppInfo()
        {
            var assembly = Assembly.GetExecutingAssembly();

            Id = "D2RCompanion";

            Name = assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title
                ?? "D2R Companion";

            Version = assembly
                .GetName()
                .Version?
                .ToString()
                ?? "unknown";
        }
    }
}
