using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;


namespace D2RPriceChecker.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SplashWindow : Window
    {
        private NotifyIcon _trayIcon = null!;

        public SplashWindow()
        {
            InitializeComponent();
            SetupTray();
        }

        private void SetupTray()
        {
            _trayIcon = new NotifyIcon();
            _trayIcon.Icon = new Icon(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "icon-16x16.ico"));
            _trayIcon.Visible = true;
            _trayIcon.Text = "D2R Price Checker";

            var menu = new ContextMenuStrip();
            menu.Items.Add("Open", null, (s, e) => { this.Show(); });
            menu.Items.Add("Exit", null, (s, e) => { System.Windows.Application.Current.Shutdown(); });
            _trayIcon.ContextMenuStrip = menu;

            _trayIcon.DoubleClick += (s, e) => { this.Show(); };
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Only start drag on left mouse button
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            // Instead of closing the window, just hide it
            this.Hide();

            // Optional: show balloon tip from tray
            if (_trayIcon != null)
            {
                //_trayIcon.ShowBalloonTip(1000, "D2R Price Checker", "Application minimized to tray", ToolTipIcon.Info);
            }
        }

        private void MinimizeToTray()
        {
            this.Hide();
        }
    }
}
