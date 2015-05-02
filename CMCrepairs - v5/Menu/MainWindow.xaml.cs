#region Imports
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
#endregion

namespace CMCrepairs
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : UserControl, ISwitchable
    {
        string location;

        #region Constructor
        public MainWindow()
        {
            location = Environment.GetEnvironmentVariable("Location", EnvironmentVariableTarget.User);
            InitializeComponent();
        }
        #endregion

        #region Buttons
        private void RepairsButton_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new Repairs());
        }

        private void StockButton_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new Stock());
        }

        private void SaleButton_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new Sale());
        }
        #endregion

        #region ISwitchable Members
        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}