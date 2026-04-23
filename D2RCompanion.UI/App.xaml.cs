using System.IO;
using System.Windows;
using CommunityToolkit.Mvvm.Messaging;
using D2RCompanion.Core.Items;
using D2RCompanion.Services;
using D2RCompanion.UI.AppCore;
using D2RCompanion.UI.Messages;
using D2RCompanion.UI.Services;
using D2RCompanion.UI.Traderie;
using D2RCompanion.UI.Util;
using D2RCompanion.UI.ViewModels;
using D2RCompanion.UI.Views;
using D2RCompanion.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace D2RCompanion.UI;

public partial class App : Application
{
    private IConfiguration _config = null!;
    private IServiceProvider _provider = null!;

    private AppInfo _appInfo = null!;
    private AppPaths _appPaths = null!;
  
    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        SetupConfig();
        SetupCore();
        SetupLogging();
        SetupDI();

        InitializeView();

        var logger = _provider.GetRequiredService<ILogger<App>>();
        logger.LogInformation("Application Started");

        _ = InitializeBackgroundAsync();
    }

    private void SetupConfig()
    {
        _config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
    }
    private void SetupCore()
    {
        _appInfo = new AppInfo();
        _appPaths = new AppPaths(_appInfo);

        _appPaths.EnsureCreated();
    }
    private void SetupLogging()
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(_config)
            .WriteTo.File(
                Path.Combine(_appPaths.Logs, "log-.txt"),
                rollingInterval: RollingInterval.Day)
            .CreateLogger();
    }

    private void SetupDI()
    {
        var services = new ServiceCollection();

        // Core
        services.AddSingleton(_config);
        services.AddSingleton(_appInfo);
        services.AddSingleton(_appPaths);

        services.AddOptions();

        // Logging
        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.AddSerilog(Log.Logger, dispose: true);
        });

        services.AddSingleton<IItemBaseNameProvider, FileItemBaseNameProvider>();


        // Services
        services.AddSingleton<SettingsService>();        
        services.AddSingleton<ScreenshotService>();
        services.AddSingleton<PipelineService>();
        services.AddSingleton<HotkeyService>();
        services.AddSingleton<TraderieClient>();

        // TODO: temporarily to debug saved images:
        services.AddSingleton<CacheService>();

        //extra
        services.AddSingleton(new OcrService("Models/d2r_tooltip_crnn_best.onnx"));
     

        // UI
        services.AddSingleton<MainWindow>();
        services.AddTransient<MainViewModel>();

        services.AddSingleton<OverlayWindow>();
        services.AddTransient<OverlayViewModel>();

        services.AddSingleton<SettingsWindow>();
        services.AddTransient<SettingsViewModel>();

        services.AddSingleton<TraderieWindow>();

        _provider = services.BuildServiceProvider();
    }

    private void InitializeView()
    {
        var mainWindow = _provider.GetRequiredService<MainWindow>();

        MainWindow = mainWindow;
        MainWindow.Show();
    }

    private async Task InitializeBackgroundAsync()
    {
        try
        {
            var settingsTask = Task.Run(() => _provider.GetRequiredService<SettingsService>().Initialize());
            var ocrTask = Task.Run(() => _provider.GetRequiredService<OcrService>().InitializeAsync());

            await Task.WhenAll(settingsTask, ocrTask);

            var traderieWindow = _provider.GetRequiredService<TraderieWindow>();

            await traderieWindow.Preload();
            await traderieWindow.InitializeAsync();

            await Task.Delay(300);

            // Try to obtain session info for future use
            await traderieWindow.TryLoadSessionAsync();

            //while (!traderieWindow.IsLoggedIn)
            //{
            //    traderieWindow.Show();
            //    await Task.Delay(5000);
            //}

            // If no session info, show the traderie window so user can log in for session data
            if (!traderieWindow.IsLoggedIn)
                traderieWindow.Show();

            WeakReferenceMessenger.Default.Send(new AppReadyMessage());
        }
        catch (Exception ex)
        {

        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        var logger = _provider.GetRequiredService<ILogger<App>>();
        logger.LogInformation("Application Exit");

        Log.CloseAndFlush();

        if (MainWindow is MainWindow main)
            main.Cleanup();

        base.OnExit(e);
    }
}

