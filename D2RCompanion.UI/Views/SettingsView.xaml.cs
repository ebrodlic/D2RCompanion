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
    /// Interaction logic for SettingsUserControl.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            ((SettingsViewModel)DataContext).CloseSettings();
        }

        public void TabMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedTab = (ListBoxItem)((ListBox)sender).SelectedItem;

            if (selectedTab != null)
            {
                // Load the appropriate content based on the selected tab
                switch (selectedTab.Content.ToString())
                {
                    //case "Tab 1":
                    //    ContentArea.Content = new Tab1Content();  // Load Tab 1 content
                    //    break;
                    //case "Tab 2":
                    //    ContentArea.Content = new Tab2Content();  // Load Tab 2 content
                    //    break;
                    //case "Tab 3":
                    //    ContentArea.Content = new Tab3Content();  // Load Tab 3 content (not implemented)
                    //    break;
                    //case "Tab 4":
                    //    ContentArea.Content = new Tab4Content();  // Load Tab 4 content (not implemented)
                    //    break;
                }
            }
        }
    }
}
