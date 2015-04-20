using System;
using System.Windows;
using System.Windows.Controls;

namespace CMCrepairs
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class PageSwitcher : Window
    {
        public PageSwitcher()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            Switcher.pageSwitcher = this;
            string location;

            location = Environment.GetEnvironmentVariable("Location", EnvironmentVariableTarget.User);
            if (location == null || location == "")
            {
               
                Switcher.Switch(new LocationAlert());


            }
            else if (location != null || location != "")
            {
                Switcher.Switch(new MainWindow());
            }
        }

        public void Navigate(UserControl nextPage)
        {
            this.Content = nextPage;
        }

        public void Navigate(UserControl nextPage, object state)
        {
            this.Content = nextPage;
            ISwitchable s = nextPage as ISwitchable;

            if (s != null)
                s.UtilizeState(state);
            else
                throw new ArgumentException("NextPage is not ISwitchable! "
                  + nextPage.Name.ToString());
        }
    }
}