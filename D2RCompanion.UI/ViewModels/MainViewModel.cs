using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using D2RCompanion.UI.AppCore;
using D2RCompanion.UI.Messages;
using D2RCompanion.UI.Services;
using Microsoft.Extensions.Logging;

namespace D2RCompanion.UI.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        public string AppDisplayName { get; }
        public string AppDisplayVersion { get; }

        [ObservableProperty]
        private bool isReady;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string statusMessage = "Starting...";

        private PipelineService _pipeline;
        private ILogger _logger;

        public MainViewModel(AppInfo appInfo, PipelineService pipelineService, ILogger<MainViewModel> logger)
        {
            AppDisplayName = appInfo.Name;
            AppDisplayVersion = $"v{appInfo.Version}";

            _pipeline = pipelineService;
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
            _logger.LogInformation("Command: RunPipelineAsync");

            if (!IsReady) return;
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var result = await _pipeline.RunAsync();

                _logger.LogInformation("Pipeline completed, about to send PipelineResultReadyMessage");

                WeakReferenceMessenger.Default.Send(new PipelineResultReadyMessage(result));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
