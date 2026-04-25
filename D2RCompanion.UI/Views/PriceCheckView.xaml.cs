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
using D2RCompanion.UI.ViewModels;

namespace D2RCompanion.UI.Views
{
    /// <summary>
    /// Interaction logic for PriceCheckView.xaml
    /// </summary>
    public partial class PriceCheckView : UserControl
    {
        public PriceCheckView()
        {
            InitializeComponent();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            ((PriceCheckViewModel)DataContext).Close();
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var scrollViewer = sender as ScrollViewer;
            if (scrollViewer == null) return;

            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RuneHoverEnter(object sender, MouseEventArgs e)
        {
            if (DataContext is PriceCheckViewModel vm)
                vm.RuneInfoHoverEnter();
        }

        private void RuneHoverLeave(object sender, MouseEventArgs e)
        {
            if (DataContext is PriceCheckViewModel vm)
                vm.RuneInfoHoverLeave();
        }

        private void ContentPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            ((PriceCheckViewModel)DataContext).Close();
        }
    }
}
