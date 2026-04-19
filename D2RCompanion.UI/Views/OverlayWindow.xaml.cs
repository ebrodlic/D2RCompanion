using D2RCompanion.Core.Traderie.DTO;
using D2RCompanion.Core.Traderie.Domain;
using D2RCompanion.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using D2RCompanion.UI.ViewModels;

namespace D2RCompanion.UI.Views
{
    /// <summary>
    /// Interaction logic for OverlayWindow.xaml
    /// </summary>
    public partial class OverlayWindow : Window
    {
        public OverlayViewModel ViewModel { get; set; } = new();

        public OverlayWindow()
        {
            InitializeComponent();
            SetHandlers();

            DataContext = ViewModel;
        }

        private void SetHandlers()
        {
            // Set full screen on load
            Loaded += OnWindowLoaded;

            // Click anywhere on overlay window
            Root.MouseDown += OnBackgroundClicked;

            // Content panel stops click bubbling
            ContentPanel.MouseDown += (s, e) => e.Handled = true;
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            Left = 0;
            Top = 0;
            Width = SystemParameters.PrimaryScreenWidth;
            Height = SystemParameters.PrimaryScreenHeight;

            IsHitTestVisible = true; // click-through background initially
        }

        private void OnBackgroundClicked(object sender, MouseButtonEventArgs e)
        {
            // If click came from content panel → ignore
            if (IsClickInsideContent(e))
                return;

            ViewModel.Reset();
            HideOverlay();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Reset();
            HideOverlay();
        }

        private bool IsClickInsideContent(MouseButtonEventArgs e)
        {
            var source = e.OriginalSource as DependencyObject;

            while (source != null)
            {
                if (source == ContentPanel)
                    return true;

                source = VisualTreeHelper.GetParent(source);
            }

            return false;
        }

        // Call this from your main window after OCR + trade parsing
        public void UpdateValues(List<string> ocrLines, List<Trade> trades)
        {
            ViewModel.OcrLines.Clear();
            foreach (var line in ocrLines)
                ViewModel.OcrLines.Add(new OcrLine(line, false));

            ViewModel.Trades.Clear();
            foreach (var trade in trades)
                ViewModel.Trades.Add(trade);
          
        }

        public void UpdateValues(List<string> ocrLines)
        {
            ViewModel.OcrLines.Clear();
            foreach (var line in ocrLines)
                ViewModel.OcrLines.Add(new OcrLine(line, false));
        }

        public void UpdateValues(List<Trade> trades)
        {
            ViewModel.Trades.Clear();
            foreach (var trade in trades)
                ViewModel.Trades.Add(trade);
            


            ViewModel.RecalculateActivity();
            ViewModel.RefreshPriceGroupsDisplay();
            ViewModel.RefreshPricePrediction();
        }

        public void UpdateValues(TradeStatistics stats)
        {
            //ViewModel.Statistics.Clear();
            ViewModel.Statistics = stats;
        }

        public void ShowOverlay()
        {
            if (!IsVisible)
                Show();

            // Bring to front of game
            Topmost = true;
            Activate();

            TradesScrollViewer.ScrollToTop();
        }
        public void HideOverlay()
        {
            Visibility = Visibility.Hidden;
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var scrollViewer = sender as ScrollViewer;
            if (scrollViewer == null) return;

            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void RuneHoverEnter(object sender, MouseEventArgs e)
        {
            if (DataContext is OverlayViewModel vm)
                vm.RuneInfoHoverEnter();
        }

        private void RuneHoverLeave(object sender, MouseEventArgs e)
        {
            if (DataContext is OverlayViewModel vm)
                vm.RuneInfoHoverLeave();
        }

        public void DisplayText(string text)
        {
            //OcrText.Text = text;

            //Visibility = Visibility.Visible;
            //Activate(); // bring on top of game
        }

        // TODO - not sure about this for now
        public void DisplayPrices(List<Trade> trades)
        {
            //PriceText.Text = string.Join("\n", prices);
        
            //Visibility = Visibility.Visible;
            //Activate(); // bring on top of game
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                HideOverlay();
        }

        private void ContentPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

    }
}
