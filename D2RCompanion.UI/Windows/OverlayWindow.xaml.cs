using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CommunityToolkit.Mvvm.Messaging;
using D2RCompanion.Core.Traderie.Domain;
using D2RCompanion.Core.Traderie.DTO;
using D2RCompanion.UI.Messages;
using D2RCompanion.UI.Services;
using D2RCompanion.UI.ViewModels;
using D2RCompanion.UI.Views;
using D2RCompanion.ViewModels;

namespace D2RCompanion.UI.Windows
{
    /// <summary>
    /// Interaction logic for OverlayWindow.xaml
    /// </summary>
    public partial class OverlayWindow : Window
    {
        private readonly HotkeyService _hotkeys;
        private readonly TraderieWebViewControl _traderieWebView;

        public OverlayWindow(
            OverlayViewModel vm,
            HotkeyService hotkeys,
            TraderieWebViewControl traderieWebView
            )
        {
            _hotkeys = hotkeys;
            _traderieWebView = traderieWebView;

            Loaded += OnWindowLoaded;
            InitializeComponent();

            DataContext = vm;
        }
        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            SetFullscreen();
            SetHandlers();
            SetMessageHandlers();
            SetHotkeys();

            CreateTraderieWebViewControl();
        }

        private void SetMessageHandlers()
        {
            WeakReferenceMessenger.Default.Register<OverlayVisibilityRequestMessage>(this, (r, m) =>
            {
                if (m.IsVisible)
                {
                    Show();
                }
                else
                {
                    Hide();
                }
            });
        }

        private async Task CreateTraderieWebViewControl()
        {
            Grid.SetColumn(_traderieWebView, 1);

            Root.Children.Add(_traderieWebView);

            // Set to visible to trigger the inital webview load
            _traderieWebView.Visibility = Visibility.Visible;

            await _traderieWebView.InitializeAsync();
            await Task.Delay(300);
            await _traderieWebView.TryLoadSessionAsync();

            _traderieWebView.Visibility = Visibility.Collapsed;

            //if (!_traderieWebView.IsLoggedIn)
            //{
            //    _traderieWebView.Visibility = Visibility.Visible;
            //}
        }

        private void SetFullscreen()
        {
            Left = 0;
            Top = 0;
            Width = SystemParameters.PrimaryScreenWidth;
            Height = SystemParameters.PrimaryScreenHeight;
        }

        private void SetHandlers()
        {
            // Click anywhere on overlay window
            Root.MouseDown += OnBackgroundClicked;
        }

        private void SetHotkeys()
        {
            var hwnd = new WindowInteropHelper(this).Handle;

            _hotkeys.Initialize(hwnd);
            _hotkeys.Register(Key.Space, ModifierKeys.Shift, ToggleVisibility);

            _hotkeys.Register(Key.D, ModifierKeys.Control, () =>
            {

            });
        }

        private void CheckTraderieStatus()
        {
            //TraderieStatus.Text = _traderieWebView.IsLoggedIn ? "Logged In" : "NOT Logged In";
          
        }

        private async void ToggleVisibility()
        {
            if (Visibility == Visibility.Visible)
                Hide();
            else
            {
                Show();
            }
               
        }

        private void OnCheckStatusClick(object sender, RoutedEventArgs e)
        {
            CheckTraderieStatus();
        }

     

        private void OnBackgroundClicked(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource == Root)
            {
                Hide();
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Hide();
        }
    }
}
