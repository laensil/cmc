using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace CMCrepairs
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : UserControl, ISwitchable
    {
        //private Sales viewBuyWindow = new Sales();
        //private Repairs viewRepairsWindow = new Repairs();
        //private Stock viewStockWindow = new Stock();

        //protected override void OnClosing(CancelEventArgs e)
        //{
        //    //base.OnClosing(e);
        //    //e.Cancel = true;
        //}

        public MainWindow()
        {
            InitializeComponent();
            //WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }

        //protected override void OnClosed(EventArgs e)
        //{
        //    //base.OnClosed(e);

        //    Application.Current.Shutdown();
        //}

        //private void ViewBuyWindow_Closed(object sender, EventArgs e)
        //{
        //    //Sales viewBuyWindow = sender as Sales;
        //    //this.viewBuyWindow = null;
        //}

        private void RepairsButton_Click(object sender, RoutedEventArgs e)
        {

            Switcher.Switch(new Repairs());
            //Repairs window = this.viewRepairsWindow;
            //if (!window.IsVisible)
            //{
            //    window.Show();
            //    //window.Owner = this;
            //}
            //else
            //{
            //    window.Hide();
            //}
        }

        private void StockButton_Click(object sender, RoutedEventArgs e)
        {

            Switcher.Switch(new Stock());
            //if (this.viewStockWindow == null)
            //{
            //    this.viewStockWindow = new Stock();
            //    this.viewStockWindow.Closed += ViewStockWindow_Closed;
            //}

            //Stock window = this.viewStockWindow;
            //if (!window.IsVisible)
            //{
            //    //window.Show();
            //    window.ShowDialog();
            //   // window.Owner = this;
            //}
            //else
            //{
            //    window.Hide();
            //}
        }

        //private void ViewStockWindow_Closed(object sender, EventArgs e)
        //{
        //    //Stock viewStockWindow = sender as Stock;
        //    //this.viewStockWindow = null;
        //}

        //private void ShutdownButton_Click(object sender, RoutedEventArgs e)
        //{
        //   // base.OnClosed(e);

        //    //Application.Current.Shutdown();
        //}

        #region ISwitchable Members
        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }


        #endregion

        private void SaleButton_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new Sale());
        }
    }
}
