using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using D2RCompanion.Services;
using D2RCompanion.UI.Services;

namespace D2RCompanion.ViewModels
{
    public class SettingsViewModel : ObservableObject
    {
        private readonly SettingsService _service;
        public SettingsViewModel(SettingsService service)
        {
            _service = service;
        }

        public bool AutoCheckForUpdates
        {
            get => _service.Settings.AutoCheckForUpdates;
            set
            {
                if (_service.Settings.AutoCheckForUpdates != value) // Compare the new value with the current value
                {
                    _service.Settings.AutoCheckForUpdates = value; // Update the setting
                    SaveSettings(); // Save the settings immediately
                    OnPropertyChanged(); // Notify the UI of the change
                }
            } 
        }

        public bool SaveImagesToDisk
        {
            get => _service.Settings.SaveImagesToDisk;
            set
            {
                if (_service.Settings.SaveImagesToDisk != value) // Compare the new value with the current value
                {
                    _service.Settings.SaveImagesToDisk = value; // Update the setting
                    SaveSettings(); // Save the settings immediately
                    OnPropertyChanged(); // Notify the UI of the change
                }
            }
        }
        public IRelayCommand SaveCommand => new RelayCommand(SaveSettings);

        private void SaveSettings()
        {
            _service.Save();
        }
    }
}
