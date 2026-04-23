using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using D2RCompanion.Core.Items;
using D2RCompanion.Core.Pipelines;
using D2RCompanion.Pipelines;
using D2RCompanion.UI.AppCore;
using D2RCompanion.UI.Services;
using D2RCompanion.UI.Traderie;
using D2RCompanion.UI.Util;
using D2RCompanion.UI.ViewModels;
using D2RCompanion.ViewModels;
using Microsoft.Extensions.Logging;


namespace D2RCompanion.UI.Views
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;
        private readonly ILogger<MainWindow> _logger;

        private readonly OverlayWindow _overlayWindow;
        private readonly SettingsWindow _settingsWindow;
        private readonly TraderieWindow _traderieWindow;

        // Icons and Tray
        private NotifyIcon _trayIcon = null!;

        //Managers
        private HotkeyService _hotkeys = null!;


        // We run these here so we have the STA thread to init Traderie, as well as never closing windows for tray behaviour
        public MainWindow(
            MainViewModel vm,
            OverlayWindow overlayWindow,
            SettingsWindow settingsWindow,
            TraderieWindow traderieWindow,
            HotkeyService hotkeys,
            ILogger<MainWindow> logger)
        {
            InitializeComponent();

            _viewModel = vm;
            _hotkeys = hotkeys;
            _logger = logger;

            _overlayWindow = overlayWindow;
            _settingsWindow = settingsWindow;
            _traderieWindow = traderieWindow;

            DataContext = vm;

            SetupTray();

            Loaded += OnWindowLoaded;
        }

        private void SetupTray()
        {
            _trayIcon = new NotifyIcon()
            {
                Icon = new Icon(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "icon-16x16.ico")),
                Visible = true,
                Text = _viewModel.AppDisplayName
            };

            var menu = new ContextMenuStrip();
            menu.Items.Add("Open", null, (s, e) => { Show(); });
            menu.Items.Add("Show Overlay", null, (s, e) => { ShowOverlayWindow(); });
            menu.Items.Add("Show Browser", null, (s, e) => { ShowTraderieWindow(); });
            menu.Items.Add("Settings", null, (s, e) => { ShowSettingsWindow(); });
            menu.Items.Add("Exit", null, (s, e) => { System.Windows.Application.Current.Shutdown(); });

            _trayIcon.ContextMenuStrip = menu;
            _trayIcon.DoubleClick += (s, e) => { this.Show(); };
        }      

        // Show the overlay window
        private void ShowOverlayWindow()
        {
            _overlayWindow.Show();
        }

        // Show the settings window
        private void ShowSettingsWindow()
        {        
            _settingsWindow.Show();
        }

        // Show the Traderie window
        private void ShowTraderieWindow()
        {
            _traderieWindow.Show();
        }

        private async void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            // TODO get rid: hack for now
            _overlayWindow.Show();
            _overlayWindow.Hide();


            SetupHotkeys();
        }

        private void SetupHotkeys()
        {
            var hwnd = new WindowInteropHelper(this).Handle;

            _hotkeys.Initialize(hwnd);

            _hotkeys.Register(Key.D, ModifierKeys.Control,
                () => _viewModel.RunPipelineCommand.Execute(null));
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Only start drag on left mouse button
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            // Instead of closing the window, just hide it, sends to tray
            this.Hide();

            // Optional: show balloon tip from tray
            if (_trayIcon != null)
            {
                //_trayIcon.ShowBalloonTip(1000, "D2R Price Checker", "Application minimized to tray", ToolTipIcon.Info);
            }
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown(); // ensures full exit
        }

        public void Cleanup()
        {
            _trayIcon?.Dispose();
        }

        private void MinimizeToTray()
        {
            Hide();
        }
    }
}
