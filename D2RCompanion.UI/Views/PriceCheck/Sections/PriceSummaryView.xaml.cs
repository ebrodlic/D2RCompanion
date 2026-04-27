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

namespace D2RCompanion.UI.Views.PriceCheck.Sections
{
    /// <summary>
    /// Interaction logic for PriceSummaryView.xaml
    /// </summary>
    public partial class PriceSummaryView : UserControl
    {
        public PriceSummaryView()
        {
            InitializeComponent();
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
    }
}
