// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using D2RCompanion.Core.Traderie.Domain;
using D2RCompanion.UI.ViewModels;
using D2RCompanion.ViewModels;

namespace D2RCompanion.UI.Views
{
    /// <summary>
    /// Interaction logic for OverlayViewContent.xaml
    /// </summary>
    public partial class OverlayViewContent : UserControl
    {
        public OverlayViewContent()
        {
            InitializeComponent();
        }

        private void SetHandlers()
        {
            // Set full screen on load
            Loaded += OnWindowLoaded;

            // Click anywhere on overlay window
            

            // Content panel stops click bubbling
            ContentPanel.MouseDown += (s, e) => e.Handled = true;
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
          
            Width = SystemParameters.PrimaryScreenWidth;
            Height = SystemParameters.PrimaryScreenHeight;

            IsHitTestVisible = true; // click-through background initially
        }

        private void OnBackgroundClicked(object sender, MouseButtonEventArgs e)
        {
            // If click came from content panel → ignore
            if (IsClickInsideContent(e))
                return;

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
            if (DataContext is OverlayContentViewModel vm)
                vm.RuneInfoHoverEnter();
        }

        private void RuneHoverLeave(object sender, MouseEventArgs e)
        {
            if (DataContext is OverlayContentViewModel vm)
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
