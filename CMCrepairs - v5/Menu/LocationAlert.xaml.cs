using System;
using System.Collections.Generic;
using System.Linq;
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

using System.ComponentModel;
using MessageBox = System.Windows.Forms.MessageBox;
using MySql.Data.MySqlClient;
using CMCrepairs.Menu;
using System.Text.RegularExpressions;
using Dymo;
using System.Collections;

namespace CMCrepairs
{
    /// <summary>
    /// Interaction logic for LocationAlert.xaml
    /// </summary>
    public partial class LocationAlert : UserControl, ISwitchable
    {
        public LocationAlert()
        {
            InitializeComponent();
        }


        #region Textbox textchanged
        private void txtLocation_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtLocation.Text.Equals(null) || txtLocation.Text.Equals(""))
            {
                btnSaveLocationForm.Visibility = Visibility.Hidden;
               // btnClear.Visibility = Visibility.Hidden;
                
            }
            else
            {
                btnSaveLocationForm.Visibility = Visibility.Visible;
                //btnClear.Visibility = Visibility.Visible;
            }
        }
        #endregion

        private void btnSaveLocationForm_Click(object sender, RoutedEventArgs e)
        {
            if (txtLocation.Text == null || txtLocation.Text == "")
            {
                MessageBox.Show("Please enter shop location before proceeding");
            }
            else if(txtLocation.Text!=null || txtLocation.Text !=""){
            //set environment variable so pernament location can be set for this particular install
                System.Windows.Forms.DialogResult answer = MessageBox.Show("Are these details correct?", "", System.Windows.Forms.MessageBoxButtons.YesNo);
                if (answer == System.Windows.Forms.DialogResult.Yes)
                {
                    Environment.SetEnvironmentVariable("Location", txtLocation.Text, EnvironmentVariableTarget.User);
                    Switcher.Switch(new MainWindow());
                }
            }
            
    }

        

        #region ISwitchable Members
        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }


        #endregion
    }
}
