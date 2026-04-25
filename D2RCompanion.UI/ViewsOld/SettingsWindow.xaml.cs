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
using System.Windows.Shapes;
using D2RCompanion.UI.ViewModels;

namespace D2RCompanion.UI.Views
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow(SettingsViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            // Cancel the close event
            e.Cancel = true;

            // Hide the window instead of closing it
            this.Hide();
        }
    }
}
