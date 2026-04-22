using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using D2RCompanion.UI.AppCore;
using D2RCompanion.UI.Messages;
using D2RCompanion.UI.Services;
using Microsoft.Extensions.Logging;

namespace D2RCompanion.UI.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private ILogger _logger;
        public string AppDisplayName { get; }
        public string AppDisplayVersion { get; }

        [ObservableProperty]
        private bool isReady;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string statusMessage = "Starting...";

        public MainWindowViewModel(AppInfo appInfo, ILogger<MainWindowViewModel> logger)
        {
            AppDisplayName = appInfo.Name;
            AppDisplayVersion = $"v{appInfo.Version}";

            _logger = logger;

            WeakReferenceMessenger.Default.Register<AppReadyMessage>(this, (recipient, message) =>
            {
                // This will be called when the message is sent
                _logger.LogInformation("AppReadyMessage received!");

                SetAppReady();
            });
        }

        private void SetAppReady()
        {
            IsReady = true;
            StatusMessage = "Ready";
        }

        [RelayCommand]
        public async Task RunPipelineAsync()
        {
            if (!IsReady) return;
            if (isBusy) return;

            isBusy = true;

            //    try
            //    {
            //        await Task.Delay(2000);


            //        //var result = await _pipeline.RunAsync();

            //        //PipelineCompleted?.Invoke(result);

            //        //store result if needed
            //    }
            //    catch (Exception ex)
            //    {

            //    }
            //    finally
            //    {
            //        IsRunning = false;
            //    }
        }
    }
}
