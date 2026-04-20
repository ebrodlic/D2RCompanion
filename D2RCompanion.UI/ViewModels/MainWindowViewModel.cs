using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace D2RCompanion.UI.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        //TODO move this as provider into App class, to be used further on in services
        public string AppName { get; } = "D2RCompanion"; 

        public string AppDisplayName { get; } = "D2R Companion";

        public string AppVersion { get; } = "";

        public string AppDisplayVersion { get; } = "";

        public MainWindowViewModel()
        {
            AppVersion = GetVersion();
            AppDisplayVersion = $"v{AppVersion}";
        }

        public string GetVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }
}
