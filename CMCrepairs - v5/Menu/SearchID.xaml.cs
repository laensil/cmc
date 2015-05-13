#region imports
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
using MySql.Data.MySqlClient;
using System.Data;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;
using UserControl = System.Windows.Controls.UserControl;
using TextBox = System.Windows.Controls.TextBox;
using DataGrid = System.Windows.Forms.DataGrid;
using System.ComponentModel;
#endregion

namespace CMCrepairs.Menu
{
    /// <summary>
    /// Interaction logic for SearchID.xaml
    /// </summary>
    public partial class SearchID : UserControl, ISwitchable
    {
        #region Wee Variables
        string preScreen;
        DataSet ds;
        #endregion

        MySqlConnection myConn = new MySqlConnection("server=sql21.hostinger.co.uk ;uid=u741972762_admin;password=cmcshop1;database=u741972762_cmcdb;persist security info=false");
        //***** switch when using database on local computer
        //MySqlConnection myConn = new MySqlConnection("server=localhost; user id=root;password=CMCsales;database=test;persist security info=false");
       
        #region Constructor
        public SearchID(string prevScreen)
        {
            preScreen = prevScreen;
            InitializeComponent();

            ///Repair Checkboxes
            chbCompleted.Visibility = Visibility.Collapsed;
            chbItemWithCustomer.Visibility = Visibility.Collapsed;
            chbRWPA.Visibility = Visibility.Collapsed;

            //Stock Checkboxes
            chbNowSold.Visibility = Visibility.Collapsed;

            btnLoad_Click(null, new RoutedEventArgs());
        }
        #endregion

        #region Search Button
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedRow = dgdIDs.SelectedCells[0].ToString();
                DataRowView drv = (DataRowView)dgdIDs.SelectedItems[0];
                if (preScreen.Equals("cmc_repairs_stock"))
                    Switcher.Switch(new Stock(drv["Date Sold"].ToString()));
                else if (preScreen.Equals("cmc_repairs_repair"))
                    Switcher.Switch(new Repairs(drv["ID"].ToString()));
                else
                    MessageBox.Show("[SearchID-0] Sorry there has been an error."
                + " Please take note of error code and the steps taken to get the error.");
            }
            catch (Exception)
            {
                MessageBox.Show("Please select an item to search.");
            }
        }
        #endregion

        #region ISwitchable Members

        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (preScreen.Equals("cmc_repairs_stock"))
                Switcher.Switch(new Stock());
            else if (preScreen.Equals("cmc_repairs_repair"))
                Switcher.Switch(new Repairs());
            else
                MessageBox.Show("There has been an error. Cannot cancel!");
        }

        #endregion

        #region Load Button
        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            //define the command reference
            MySqlCommand mySQLcommand = new MySqlCommand();

            //define the connection used by the command object
            mySQLcommand.Connection = myConn;

            if (preScreen.Equals("cmc_repairs_stock"))
            {
                chbNowSold.Visibility = Visibility.Visible;

                ////define the command text
                mySQLcommand.CommandText = "SELECT date_sold AS 'Date Sold', brand AS 'Brand', model AS 'Model', network AS 'Network', name AS 'Name', address AS 'Address' FROM " + preScreen + " ORDER BY date_sold DESC;";
            }
            else if (preScreen.Equals("cmc_repairs_repair"))
            {
                chbCompleted.Visibility = Visibility.Visible;
                chbItemWithCustomer.Visibility = Visibility.Visible;
                chbRWPA.Visibility = Visibility.Visible;

                ////define the command text
                mySQLcommand.CommandText = "SELECT datetime_id AS 'ID', customer_name AS 'Customer Name', contact_num AS 'Contact Num', item AS 'Item', details AS 'Details', quote_price AS 'Price Quoted' FROM " + preScreen + " ORDER BY datetime_id DESC;";
            }
            else
            {
                MessageBox.Show("What screen did you come from?");
            }
            try
            {
                //open the connection
                if (myConn.State != System.Data.ConnectionState.Open)
                    myConn.Open();

                MySqlDataAdapter mySQLDataAd = new MySqlDataAdapter(mySQLcommand);
                DataSet ds = new DataSet();
                mySQLDataAd.Fill(ds, "LoadDataBinding");
                dgdIDs.DataContext = ds;
                //open the connection
                if (myConn.State != System.Data.ConnectionState.Open)
                    myConn.Open();

                //close the connection
                myConn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        private void dgdIDs_DoubleClick(object sender, EventArgs e)
        {
            btnSearch_Click(this, new RoutedEventArgs());
        }

        private void txtOne_TextChanged(object sender, TextChangedEventArgs e)
        {
            //define the command reference
            MySqlCommand mySQLcommand = new MySqlCommand();

            //define the connection used by the command object
            mySQLcommand.Connection = myConn;

            if (preScreen.Equals("cmc_repairs_stock"))
            {
                //define the command text
                //mySQLcommand.CommandText = "SELECT date_sold AS 'Date Sold', brand AS 'Brand', model AS 'Model', network AS 'Network', name AS 'Name', address AS 'Address' FROM " + preScreen + " WHERE date_sold LIKE '%" + txtOne.Text + "%';";

                if ((bool)chbNowSold.IsChecked)
                {
                    mySQLcommand.CommandText = "SELECT date_sold AS 'Date Sold', brand AS 'Brand', model AS 'Model', network AS 'Network', name AS 'Name', address AS 'Address' FROM " + preScreen + " WHERE now_sold = 1 AND date_sold LIKE '%" + txtOne.Text + "%';";
                }
                else
                    mySQLcommand.CommandText = "SELECT date_sold AS 'Date Sold', brand AS 'Brand', model AS 'Model', network AS 'Network', name AS 'Name', address AS 'Address' FROM " + preScreen + " WHERE date_sold LIKE '%" + txtOne.Text + "%';";
            }
            else if (preScreen.Equals("cmc_repairs_repair"))
            {
                if ((bool)chbItemWithCustomer.IsChecked && (bool)chbRWPA.IsChecked && (bool)chbCompleted.IsChecked)
                {
                    mySQLcommand.CommandText = "SELECT datetime_id AS 'ID', customer_name AS 'Customer Name', contact_num AS 'Contact Num', item AS 'Item', details AS 'Details', quote_price AS 'Price Quoted' FROM " + preScreen + " WHERE completed = 1 AND item_with_customer = 1 AND rwpa = 1 AND datetime_id LIKE '%" + txtOne.Text + "%';";
                }
                else if ((bool)chbItemWithCustomer.IsChecked && (bool)chbRWPA.IsChecked)
                {
                    mySQLcommand.CommandText = "SELECT datetime_id AS 'ID', customer_name AS 'Customer Name', contact_num AS 'Contact Num', item AS 'Item', details AS 'Details', quote_price AS 'Price Quoted' FROM " + preScreen + " WHERE completed = 1 AND item_with_customer = 1 AND rwpa = 1";
                }
                else if ((bool)chbItemWithCustomer.IsChecked && (bool)chbCompleted.IsChecked)
                {
                    mySQLcommand.CommandText = "SELECT datetime_id AS 'ID', customer_name AS 'Customer Name', contact_num AS 'Contact Num', item AS 'Item', details AS 'Details', quote_price AS 'Price Quoted' FROM " + preScreen + " WHERE completed = 1 AND item_with_customer = 1 AND datetime_id LIKE '%" + txtOne.Text + "%';";
                }
                else if ((bool)chbRWPA.IsChecked && (bool)chbCompleted.IsChecked)
                {
                    mySQLcommand.CommandText = "SELECT datetime_id AS 'ID', customer_name AS 'Customer Name', contact_num AS 'Contact Num', item AS 'Item', details AS 'Details', quote_price AS 'Price Quoted' FROM " + preScreen + " WHERE completed = 1 AND rwpa = 1 AND datetime_id LIKE '%" + txtOne.Text + "%';";
                }
                else if ((bool)chbItemWithCustomer.IsChecked)
                {
                    mySQLcommand.CommandText = "SELECT datetime_id AS 'ID', customer_name AS 'Customer Name', contact_num AS 'Contact Num', item AS 'Item', details AS 'Details', quote_price AS 'Price Quoted' FROM " + preScreen + " WHERE completed = 1 AND datetime_id LIKE '%" + txtOne.Text + "%';";
                }
                else if ((bool)chbRWPA.IsChecked)
                {
                    mySQLcommand.CommandText = "SELECT datetime_id AS 'ID', customer_name AS 'Customer Name', contact_num AS 'Contact Num', item AS 'Item', details AS 'Details', quote_price AS 'Price Quoted' FROM " + preScreen + " WHERE rwpa = 1 AND datetime_id LIKE '%" + txtOne.Text + "%';";
                }
                else if ((bool)chbCompleted.IsChecked)
                {
                    mySQLcommand.CommandText = "SELECT datetime_id AS 'ID', customer_name AS 'Customer Name', contact_num AS 'Contact Num', item AS 'Item', details AS 'Details', quote_price AS 'Price Quoted' FROM " + preScreen + " WHERE completed = 1 AND datetime_id LIKE '%" + txtOne.Text + "%';";
                }
                else
                    mySQLcommand.CommandText = "SELECT datetime_id AS 'ID', customer_name AS 'Customer Name', contact_num AS 'Contact Num', item AS 'Item', details AS 'Details', quote_price AS 'Price Quoted' FROM " + preScreen + " WHERE datetime_id LIKE '%" + txtOne.Text + "%';";
            }
            try
            {
                //open the connection
                if (myConn.State != System.Data.ConnectionState.Open)
                    myConn.Open();

                MySqlDataAdapter mySQLDataAd = new MySqlDataAdapter(mySQLcommand);
                ds = new DataSet();
                mySQLDataAd.Fill(ds, "LoadDataBinding");
                dgdIDs.DataContext = ds;
                //open the connection
                if (myConn.State != System.Data.ConnectionState.Open)
                    myConn.Open();

                //close the connection
                myConn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #region Repairs Screen Checkboxes
        private void chbCompleted_Checked(object sender, RoutedEventArgs e)
        {
            //define the command reference
            MySqlCommand mySQLcommand = new MySqlCommand();

            //define the connection used by the command object
            mySQLcommand.Connection = myConn;

            if ((bool)chbItemWithCustomer.IsChecked && (bool)chbRWPA.IsChecked && txtOne.Text.Length > 0)
            {
                mySQLcommand.CommandText = "SELECT datetime_id AS 'ID', customer_name AS 'Customer Name', contact_num AS 'Contact Num', item AS 'Item', details AS 'Details', quote_price AS 'Price Quoted' FROM " + preScreen + " WHERE completed = 1 AND item_with_customer = 1 AND rwpa = 1 AND datetime_id LIKE '%" + txtOne.Text + "%';";
            }
            else if ((bool)chbItemWithCustomer.IsChecked && (bool)chbRWPA.IsChecked)
            {
                mySQLcommand.CommandText = "SELECT datetime_id AS 'ID', customer_name AS 'Customer Name', contact_num AS 'Contact Num', item AS 'Item', details AS 'Details', quote_price AS 'Price Quoted' FROM " + preScreen + " WHERE completed = 1 AND item_with_customer = 1 AND rwpa = 1";
            }
            else if ((bool)chbItemWithCustomer.IsChecked && txtOne.Text.Length > 0)
            {
                mySQLcommand.CommandText = "SELECT datetime_id AS 'ID', customer_name AS 'Customer Name', contact_num AS 'Contact Num', item AS 'Item', details AS 'Details', quote_price AS 'Price Quoted' FROM " + preScreen + " WHERE completed = 1 AND item_with_customer = 1 AND datetime_id LIKE '%" + txtOne.Text + "%';";
            }
            else if ((bool)chbRWPA.IsChecked && txtOne.Text.Length > 0)
            {
                mySQLcommand.CommandText = "SELECT datetime_id AS 'ID', customer_name AS 'Customer Name', contact_num AS 'Contact Num', item AS 'Item', details AS 'Details', quote_price AS 'Price Quoted' FROM " + preScreen + " WHERE completed = 1 AND rwpa = 1 AND datetime_id LIKE '%" + txtOne.Text + "%';";
            }
            else if ((bool)chbItemWithCustomer.IsChecked)
            {
                mySQLcommand.CommandText = "SELECT datetime_id AS 'ID', customer_name AS 'Customer Name', contact_num AS 'Contact Num', item AS 'Item', details AS 'Details', quote_price AS 'Price Quoted' FROM " + preScreen + " WHERE completed = 1 AND item_with_customer = 1";
            }
            else if ((bool)chbRWPA.IsChecked)
            {
                mySQLcommand.CommandText = "SELECT datetime_id AS 'ID', customer_name AS 'Customer Name', contact_num AS 'Contact Num', item AS 'Item', details AS 'Details', quote_price AS 'Price Quoted' FROM " + preScreen + " WHERE completed = 1 AND rwpa = 1";
            }
            else if (txtOne.Text.Length > 0)
            {
                mySQLcommand.CommandText = "SELECT datetime_id AS 'ID', customer_name AS 'Customer Name', contact_num AS 'Contact Num', item AS 'Item', details AS 'Details', quote_price AS 'Price Quoted' FROM " + preScreen + " WHERE completed = 1 AND datetime_id LIKE '%" + txtOne.Text + "%';";
            }
            else
            {
                mySQLcommand.CommandText = "SELECT datetime_id AS 'ID', customer_name AS 'Customer Name', contact_num AS 'Contact Num', item AS 'Item', details AS 'Details', quote_price AS 'Price Quoted' FROM " + preScreen + " WHERE completed = 1";
            }

            try
            {
                //open the connection
                if (myConn.State != System.Data.ConnectionState.Open)
                    myConn.Open();

                MySqlDataAdapter mySQLDataAd = new MySqlDataAdapter(mySQLcommand);
                ds = new DataSet();
                mySQLDataAd.Fill(ds, "LoadDataBinding");
                dgdIDs.DataContext = ds;
                //open the connection
                if (myConn.State != System.Data.ConnectionState.Open)
                    myConn.Open();

                //close the connection
                myConn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void chbCompleted_Unchecked(object sender, RoutedEventArgs e)
        {
            //define the command reference
            MySqlCommand mySQLcommand = new MySqlCommand();

            //define the connection used by the command object
            mySQLcommand.Connection = myConn;

            if ((bool)chbItemWithCustomer.IsChecked && (bool)chbRWPA.IsChecked && txtOne.Text.Length > 0)
            {
                mySQLcommand.CommandText = "SELECT datetime_id AS 'ID', customer_name AS 'Customer Name', contact_num AS 'Contact Num', item AS 'Item', details AS 'Details', quote_price AS 'Price Quoted' FROM " + preScreen + " WHERE item_with_customer = 1 AND rwpa = 1 AND datetime_id LIKE '%" + txtOne.Text + "%';";
            }
            else if ((bool)chbItemWithCustomer.IsChecked && (bool)chbRWPA.IsChecked)
            {
                mySQLcommand.CommandText = "SELECT datetime_id AS 'ID', customer_name AS 'Customer Name', contact_num AS 'Contact Num', item AS 'Item', details AS 'Details', quote_price AS 'Price Quoted' FROM " + preScreen + " WHERE item_with_customer = 1 AND rwpa = 1";
            }
            else if ((bool)chbItemWithCustomer.IsChecked && txtOne.Text.Length > 0)
            {
                mySQLcommand.CommandText = "SELECT datetime_id AS 'ID', customer_name AS 'Customer Name', contact_num AS 'Contact Num', item AS 'Item', details AS 'Details', quote_price AS 'Price Quoted' FROM " + preScreen + " WHERE item_with_customer = 1 AND datetime_id LIKE '%" + txtOne.Text + "%';";
            }
            else if ((bool)chbRWPA.IsChecked && txtOne.Text.Length > 0)
            {
                mySQLcommand.CommandText = "SELECT datetime_id AS 'ID', customer_name AS 'Customer Name', contact_num AS 'Contact Num', item AS 'Item', details AS 'Details', quote_price AS 'Price Quoted' FROM " + preScreen + " WHERE rwpa = 1 AND datetime_id LIKE '%" + txtOne.Text + "%';";
            }
            else if ((bool)chbItemWithCustomer.IsChecked)
            {
                mySQLcommand.CommandText = "SELECT datetime_id AS 'ID', customer_name AS 'Customer Name', contact_num AS 'Contact Num', item AS 'Item', details AS 'Details', quote_price AS 'Price Quoted' FROM " + preScreen + " WHERE item_with_customer = 1";
            }
            else if ((bool)chbRWPA.IsChecked)
            {
                mySQLcommand.CommandText = "SELECT datetime_id AS 'ID', customer_name AS 'Customer Name', contact_num AS 'Contact Num', item AS 'Item', details AS 'Details', quote_price AS 'Price Quoted' FROM " + preScreen + " WHERE rwpa = 1";
            }
            else if (txtOne.Text.Length > 0)
            {
                mySQLcommand.CommandText = "SELECT datetime_id AS 'ID', customer_name AS 'Customer Name', contact_num AS 'Contact Num', item AS 'Item', details AS 'Details', quote_price AS 'Price Quoted' FROM " + preScreen + " WHERE datetime_id LIKE '%" + txtOne.Text + "%';";
            }
            else
            {
                mySQLcommand.CommandText = "SELECT datetime_id AS 'ID', customer_name AS 'Customer Name', contact_num AS 'Contact Num', item AS 'Item', details AS 'Details', quote_price AS 'Price Quoted' FROM " + preScreen + ";";
            }

            try
            {
                //open the connection
                if (myConn.State != System.Data.ConnectionState.Open)
                    myConn.Open();

                MySqlDataAdapter mySQLDataAd = new MySqlDataAdapter(mySQLcommand);
                ds = new DataSet();
                mySQLDataAd.Fill(ds, "LoadDataBinding");
                dgdIDs.DataContext = ds;
                //open the connection
                if (myConn.State != System.Data.ConnectionState.Open)
                    myConn.Open();

                //close the connection
                myConn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void chbItemWithCustomer_Checked(object sender, RoutedEventArgs e)
        {
            //define the command reference
            MySqlCommand mySQLcommand = new MySqlCommand();

            //define the connection used by the command object
            mySQLcommand.Connection = myConn;

            if ((bool)chbCompleted.IsChecked && (bool)chbRWPA.IsChecked && txtOne.Text.Length > 0)
            {
                mySQLcommand.CommandText = "SELECT datetime_id AS 'ID', customer_name AS 'Customer Name', contact_num AS 'Contact Num', item AS 'Item', details AS 'Details', quote_price AS 'Price Quoted' FROM " + preScreen + " WHERE completed = 1 AND item_with_customer = 1 AND rwpa = 1 AND datetime_id LIKE '%" + txtOne.Text + "%';";
            }
            else if ((bool)chbCompleted.IsChecked && (bool)chbRWPA.IsChecked)
            {
                mySQLcommand.CommandText = "SELECT datetime_id AS 'ID', customer_name AS 'Customer Name', contact_num AS 'Contact Num', item AS 'Item', details AS 'Details', quote_price AS 'Price Quoted' FROM " + preScreen + " WHERE completed = 1 AND item_with_customer = 1 AND rwpa = 1";
            }
            else if ((bool)chbCompleted.IsChecked && txtOne.Text.Length > 0)
            {
                mySQLcommand.CommandText = "SELECT datetime_id AS 'ID', customer_name AS 'Customer Name', contact_num AS 'Contact Num', item AS 'Item', details AS 'Details', quote_price AS 'Price Quoted' FROM " + preScreen + " WHERE completed = 1 AND item_with_customer = 1 AND datetime_id LIKE '%" + txtOne.Text + "%';";
            }
            else if ((bool)chbRWPA.IsChecked && txtOne.Text.Length > 0)
            {
                mySQLcommand.CommandText = "SELECT datetime_id AS 'ID', customer_name AS 'Customer Name', contact_num AS 'Contact Num', item AS 'Item', details AS 'Details', quote_price AS 'Price Quoted' FROM " + preScreen + " WHERE item_with_customer = 1 AND rwpa = 1 AND datetime_id LIKE '%" + txtOne.Text + "%';";
            }
            else if ((bool)chbCompleted.IsChecked)
            {
                mySQLcommand.CommandText = "SELECT datetime_id AS 'ID', customer_name AS 'Customer Name', contact_num AS 'Contact Num', item AS 'Item', details AS 'Details', quote_price AS 'Price Quoted' FROM " + preScreen + " WHERE completed = 1 AND item_with_customer = 1";
            }
            else if ((bool)chbRWPA.IsChecked)
            {
                mySQLcommand.CommandText = "SELECT datetime_id AS 'ID', customer_name AS 'Customer Name', contact_num AS 'Contact Num', item AS 'Item', details AS 'Details', quote_price AS 'Price Quoted' FROM " + preScreen + " WHERE item_with_customer = 1 AND rwpa = 1";
            }
            else if (txtOne.Text.Length > 0)
            {
                mySQLcommand.CommandText = "SELECT datetime_id AS 'ID', customer_name AS 'Customer Name', contact_num AS 'Contact Num', item AS 'Item', details AS 'Details', quote_price AS 'Price Quoted' FROM " + preScreen + " WHERE item_with_customer = 1 AND datetime_id LIKE '%" + txtOne.Text + "%';";
            }
            else
            {
                mySQLcommand.CommandText = "SELECT datetime_id AS 'ID', customer_name AS 'Customer Name', contact_num AS 'Contact Num', item AS 'Item', details AS 'Details', quote_price AS 'Price Quoted' FROM " + preScreen + " WHERE item_with_customer = 1";
            }

            try
            {
                //open the connection
                if (myConn.State != System.Data.ConnectionState.Open)
                    myConn.Open();

                MySqlDataAdapter mySQLDataAd = new MySqlDataAdapter(mySQLcommand);
                ds = new DataSet();
                mySQLDataAd.Fill(ds, "LoadDataBinding");
                dgdIDs.DataContext = ds;
                //open the connection
                if (myConn.State != System.Data.ConnectionState.Open)
                    myConn.Open();

                //close the connection
                myConn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void chbItemWithCustomer_Unchecked(object sender, RoutedEventArgs e)
        {
            //define the command reference
            MySqlCommand mySQLcommand = new MySqlCommand();

            //define the connection used by the command object
            mySQLcommand.Connection = myConn;

            if ((bool)chbCompleted.IsChecked && (bool)chbRWPA.IsChecked && txtOne.Text.Length > 0)
            {
                mySQLcommand.CommandText = "SELECT datetime_id AS 'ID', customer_name AS 'Customer Name', contact_num AS 'Contact Num', item AS 'Item', details AS 'Details', quote_price AS 'Price Quoted' FROM " + preScreen + " WHERE completed = 1 AND rwpa = 1 AND datetime_id LIKE '%" + txtOne.Text + "%';";
            }
            else if ((bool)chbCompleted.IsChecked && (bool)chbRWPA.IsChecked)
            {
                mySQLcommand.CommandText = "SELECT datetime_id AS 'ID', customer_name AS 'Customer Name', contact_num AS 'Contact Num', item AS 'Item', details AS 'Details', quote_price AS 'Price Quoted' FROM " + preScreen + " WHERE completed = 1 AND rwpa = 1";
            }
            else if ((bool)chbCompleted.IsChecked && txtOne.Text.Length > 0)
            {
                mySQLcommand.CommandText = "SELECT datetime_id AS 'ID', customer_name AS 'Customer Name', contact_num AS 'Contact Num', item AS 'Item', details AS 'Details', quote_price AS 'Price Quoted' FROM " + preScreen + " WHERE completed = 1 AND datetime_id LIKE '%" + txtOne.Text + "%';";
            }
            else if ((bool)chbRWPA.IsChecked && txtOne.Text.Length > 0)
            {
                mySQLcommand.CommandText = "SELECT datetime_id AS 'ID', customer_name AS 'Customer Name', contact_num AS 'Contact Num', item AS 'Item', details AS 'Details', quote_price AS 'Price Quoted' FROM " + preScreen + " WHERE rwpa = 1 AND datetime_id LIKE '%" + txtOne.Text + "%';";
            }
            else if ((bool)chbCompleted.IsChecked)
            {
                mySQLcommand.CommandText = "SELECT datetime_id AS 'ID', customer_name AS 'Customer Name', contact_num AS 'Contact Num', item AS 'Item', details AS 'Details', quote_price AS 'Price Quoted' FROM " + preScreen + " WHERE completed = 1";
            }
            else if ((bool)chbRWPA.IsChecked)
            {
                mySQLcommand.CommandText = "SELECT datetime_id AS 'ID', customer_name AS 'Customer Name', contact_num AS 'Contact Num', item AS 'Item', details AS 'Details', quote_price AS 'Price Quoted' FROM " + preScreen + " WHERE rwpa = 1";
            }
            else if (txtOne.Text.Length > 0)
            {
                mySQLcommand.CommandText = "SELECT datetime_id AS 'ID', customer_name AS 'Customer Name', contact_num AS 'Contact Num', item AS 'Item', details AS 'Details', quote_price AS 'Price Quoted' FROM " + preScreen + " WHERE datetime_id LIKE '%" + txtOne.Text + "%';";
            }
            else
            {
                mySQLcommand.CommandText = "SELECT datetime_id AS 'ID', customer_name AS 'Customer Name', contact_num AS 'Contact Num', item AS 'Item', details AS 'Details', quote_price AS 'Price Quoted' FROM " + preScreen + ";";
            }

            try
            {
                //open the connection
                if (myConn.State != System.Data.ConnectionState.Open)
                    myConn.Open();

                MySqlDataAdapter mySQLDataAd = new MySqlDataAdapter(mySQLcommand);
                ds = new DataSet();
                mySQLDataAd.Fill(ds, "LoadDataBinding");
                dgdIDs.DataContext = ds;
                //open the connection
                if (myConn.State != System.Data.ConnectionState.Open)
                    myConn.Open();

                //close the connection
                myConn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void chbRWPA_Checked(object sender, RoutedEventArgs e)
        {
            //define the command reference
            MySqlCommand mySQLcommand = new MySqlCommand();

            //define the connection used by the command object
            mySQLcommand.Connection = myConn;

            if ((bool)chbCompleted.IsChecked && (bool)chbItemWithCustomer.IsChecked && txtOne.Text.Length > 0)
            {
                mySQLcommand.CommandText = "SELECT datetime_id AS 'ID', customer_name AS 'Customer Name', contact_num AS 'Contact Num', item AS 'Item', details AS 'Details', quote_price AS 'Price Quoted' FROM " + preScreen + " WHERE completed = 1 AND item_with_customer = 1 AND rwpa = 1 AND datetime_id LIKE '%" + txtOne.Text + "%';";
            }
            else if ((bool)chbCompleted.IsChecked && (bool)chbItemWithCustomer.IsChecked)
            {
                mySQLcommand.CommandText = "SELECT datetime_id AS 'ID', customer_name AS 'Customer Name', contact_num AS 'Contact Num', item AS 'Item', details AS 'Details', quote_price AS 'Price Quoted' FROM " + preScreen + " WHERE completed = 1 AND item_with_customer = 1 AND rwpa = 1";
            }
            else if ((bool)chbCompleted.IsChecked && txtOne.Text.Length > 0)
            {
                mySQLcommand.CommandText = "SELECT datetime_id AS 'ID', customer_name AS 'Customer Name', contact_num AS 'Contact Num', item AS 'Item', details AS 'Details', quote_price AS 'Price Quoted' FROM " + preScreen + " WHERE completed = 1 AND rwpa = 1 AND datetime_id LIKE '%" + txtOne.Text + "%';";
            }
            else if ((bool)chbItemWithCustomer.IsChecked && txtOne.Text.Length > 0)
            {
                mySQLcommand.CommandText = "SELECT datetime_id AS 'ID', customer_name AS 'Customer Name', contact_num AS 'Contact Num', item AS 'Item', details AS 'Details', quote_price AS 'Price Quoted' FROM " + preScreen + " WHERE item_with_customer = 1 AND rwpa = 1 AND datetime_id LIKE '%" + txtOne.Text + "%';";
            }
            else if ((bool)chbCompleted.IsChecked)
            {
                mySQLcommand.CommandText = "SELECT datetime_id AS 'ID', customer_name AS 'Customer Name', contact_num AS 'Contact Num', item AS 'Item', details AS 'Details', quote_price AS 'Price Quoted' FROM " + preScreen + " WHERE completed = 1 AND rwpa = 1";
            }
            else if ((bool)chbItemWithCustomer.IsChecked)
            {
                mySQLcommand.CommandText = "SELECT datetime_id AS 'ID', customer_name AS 'Customer Name', contact_num AS 'Contact Num', item AS 'Item', details AS 'Details', quote_price AS 'Price Quoted' FROM " + preScreen + " WHERE item_with_customer = 1 AND rwpa = 1";
            }
            else if (txtOne.Text.Length > 0)
            {
                mySQLcommand.CommandText = "SELECT datetime_id AS 'ID', customer_name AS 'Customer Name', contact_num AS 'Contact Num', item AS 'Item', details AS 'Details', quote_price AS 'Price Quoted' FROM " + preScreen + " WHERE rwpa = 1 AND datetime_id LIKE '%" + txtOne.Text + "%';";
            }
            else
            {
                mySQLcommand.CommandText = "SELECT datetime_id AS 'ID', customer_name AS 'Customer Name', contact_num AS 'Contact Num', item AS 'Item', details AS 'Details', quote_price AS 'Price Quoted' FROM " + preScreen + " WHERE rwpa = 1";
            }

            try
            {
                //open the connection
                if (myConn.State != System.Data.ConnectionState.Open)
                    myConn.Open();

                MySqlDataAdapter mySQLDataAd = new MySqlDataAdapter(mySQLcommand);
                ds = new DataSet();
                mySQLDataAd.Fill(ds, "LoadDataBinding");
                dgdIDs.DataContext = ds;
                //open the connection
                if (myConn.State != System.Data.ConnectionState.Open)
                    myConn.Open();

                //close the connection
                myConn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void chbRWPA_Unchecked(object sender, RoutedEventArgs e)
        {
            //define the command reference
            MySqlCommand mySQLcommand = new MySqlCommand();

            //define the connection used by the command object
            mySQLcommand.Connection = myConn;

            if ((bool)chbCompleted.IsChecked && (bool)chbItemWithCustomer.IsChecked && txtOne.Text.Length > 0)
            {
                mySQLcommand.CommandText = "SELECT datetime_id AS 'ID', customer_name AS 'Customer Name', contact_num AS 'Contact Num', item AS 'Item', details AS 'Details', quote_price AS 'Price Quoted' FROM " + preScreen + " WHERE completed = 1 AND item_with_customer = 1 AND datetime_id LIKE '%" + txtOne.Text + "%';";
            }
            else if ((bool)chbCompleted.IsChecked && (bool)chbItemWithCustomer.IsChecked)
            {
                mySQLcommand.CommandText = "SELECT datetime_id AS 'ID', customer_name AS 'Customer Name', contact_num AS 'Contact Num', item AS 'Item', details AS 'Details', quote_price AS 'Price Quoted' FROM " + preScreen + " WHERE completed = 1 AND item_with_customer = 1";
            }
            else if ((bool)chbCompleted.IsChecked && txtOne.Text.Length > 0)
            {
                mySQLcommand.CommandText = "SELECT datetime_id AS 'ID', customer_name AS 'Customer Name', contact_num AS 'Contact Num', item AS 'Item', details AS 'Details', quote_price AS 'Price Quoted' FROM " + preScreen + " WHERE completed = 1 AND datetime_id LIKE '%" + txtOne.Text + "%';";
            }
            else if ((bool)chbItemWithCustomer.IsChecked && txtOne.Text.Length > 0)
            {
                mySQLcommand.CommandText = "SELECT datetime_id AS 'ID', customer_name AS 'Customer Name', contact_num AS 'Contact Num', item AS 'Item', details AS 'Details', quote_price AS 'Price Quoted' FROM " + preScreen + " WHERE item_with_customer = 1 AND datetime_id LIKE '%" + txtOne.Text + "%';";
            }
            else if ((bool)chbCompleted.IsChecked)
            {
                mySQLcommand.CommandText = "SELECT datetime_id AS 'ID', customer_name AS 'Customer Name', contact_num AS 'Contact Num', item AS 'Item', details AS 'Details', quote_price AS 'Price Quoted' FROM " + preScreen + " WHERE completed = 1";
            }
            else if ((bool)chbItemWithCustomer.IsChecked)
            {
                mySQLcommand.CommandText = "SELECT datetime_id AS 'ID', customer_name AS 'Customer Name', contact_num AS 'Contact Num', item AS 'Item', details AS 'Details', quote_price AS 'Price Quoted' FROM " + preScreen + " WHERE chbItemWithCustomer = 1";
            }
            else if (txtOne.Text.Length > 0)
            {
                mySQLcommand.CommandText = "SELECT datetime_id AS 'ID', customer_name AS 'Customer Name', contact_num AS 'Contact Num', item AS 'Item', details AS 'Details', quote_price AS 'Price Quoted' FROM " + preScreen + " WHERE datetime_id LIKE '%" + txtOne.Text + "%';";
            }
            else
            {
                mySQLcommand.CommandText = "SELECT datetime_id AS 'ID', customer_name AS 'Customer Name', contact_num AS 'Contact Num', item AS 'Item', details AS 'Details', quote_price AS 'Price Quoted' FROM " + preScreen + ";";
            }

            try
            {
                //open the connection
                if (myConn.State != System.Data.ConnectionState.Open)
                    myConn.Open();

                MySqlDataAdapter mySQLDataAd = new MySqlDataAdapter(mySQLcommand);
                ds = new DataSet();
                mySQLDataAd.Fill(ds, "LoadDataBinding");
                dgdIDs.DataContext = ds;
                //open the connection
                if (myConn.State != System.Data.ConnectionState.Open)
                    myConn.Open();

                //close the connection
                myConn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region Stock Screen Checkboxes
        private void chbNowSold_Checked(object sender, RoutedEventArgs e)
        {
            //define the command reference
            MySqlCommand mySQLcommand = new MySqlCommand();

            //define the connection used by the command object
            mySQLcommand.Connection = myConn;

            if (txtOne.Text.Length > 0)
            {
                mySQLcommand.CommandText = "SELECT date_sold AS 'Date Sold', brand AS 'Brand', model AS 'Model', network AS 'Network', name AS 'Name', address AS 'Address' FROM " + preScreen + " WHERE now_sold = 1 AND date_sold LIKE '%" + txtOne.Text + "%';";
            }
            else
                //define the command text
                mySQLcommand.CommandText = "SELECT date_sold AS 'Date Sold', brand AS 'Brand', model AS 'Model', network AS 'Network', name AS 'Name', address AS 'Address' FROM " + preScreen + " WHERE now_sold = 1";

            try
            {
                //open the connection
                if (myConn.State != System.Data.ConnectionState.Open)
                    myConn.Open();

                MySqlDataAdapter mySQLDataAd = new MySqlDataAdapter(mySQLcommand);
                ds = new DataSet();
                mySQLDataAd.Fill(ds, "LoadDataBinding");
                dgdIDs.DataContext = ds;
                //open the connection
                if (myConn.State != System.Data.ConnectionState.Open)
                    myConn.Open();

                //close the connection
                myConn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void chbNowSold_Unchecked(object sender, RoutedEventArgs e)
        {
            //define the command reference
            MySqlCommand mySQLcommand = new MySqlCommand();

            //define the connection used by the command object
            mySQLcommand.Connection = myConn;

            if (txtOne.Text.Length > 0)
            {
                mySQLcommand.CommandText = "SELECT date_sold AS 'Date Sold', brand AS 'Brand', model AS 'Model', network AS 'Network', name AS 'Name', address AS 'Address' FROM " + preScreen + " WHERE date_sold LIKE '%" + txtOne.Text + "%';";
            }
            else
                ////define the command text
                mySQLcommand.CommandText = "SELECT date_sold AS 'Date Sold', brand AS 'Brand', model AS 'Model', network AS 'Network', name AS 'Name', address AS 'Address' FROM " + preScreen + ";";

            try
            {
                //open the connection
                if (myConn.State != System.Data.ConnectionState.Open)
                    myConn.Open();

                MySqlDataAdapter mySQLDataAd = new MySqlDataAdapter(mySQLcommand);
                ds = new DataSet();
                mySQLDataAd.Fill(ds, "LoadDataBinding");
                dgdIDs.DataContext = ds;
                //open the connection
                if (myConn.State != System.Data.ConnectionState.Open)
                    myConn.Open();

                //close the connection
                myConn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        //private void txtTwo_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    //define the connection reference

        //    //TODO - AWC - pwd


        //    MySqlConnection myConn = new MySqlConnection("server=localhost; user id=root;password=CMCsales;database=test;persist security info=false");

        //    //define the command reference
        //    MySqlCommand mySQLcommand = new MySqlCommand();

        //    //define the connection used by the command object
        //    mySQLcommand.Connection = myConn;

        //    ////define the command text
        //    mySQLcommand.CommandText = "SELECT date_sold AS 'Date Sold', brand AS 'Brand', model AS 'Model', network AS 'Network', name AS 'Name', address AS 'Address' FROM " + preScreen + " WHERE brand LIKE '%" + txtTwo.Text + "%';";

        //    try
        //    {
        //        //open the connection
        //        if (myConn.State != System.Data.ConnectionState.Open)
        //            myConn.Open();

        //        MySqlDataAdapter mySQLDataAd = new MySqlDataAdapter(mySQLcommand);
        //        ds = new DataSet();
        //        mySQLDataAd.Fill(ds, "LoadDataBinding");
        //        dgdIDs.DataContext = ds;
        //        //open the connection
        //        if (myConn.State != System.Data.ConnectionState.Open)
        //            myConn.Open();

        //        //close the connection
        //        myConn.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}

        //private void txtThree_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    //define the connection reference
        //    MySqlConnection myConn = new MySqlConnection("server=localhost; user id=root;password=CMCsales;database=test;persist security info=false");

        //    //define the command reference
        //    MySqlCommand mySQLcommand = new MySqlCommand();

        //    //define the connection used by the command object
        //    mySQLcommand.Connection = myConn;

        //    ////define the command text
        //    mySQLcommand.CommandText = "SELECT date_sold AS 'Date Sold', brand AS 'Brand', model AS 'Model', network AS 'Network', name AS 'Name', address AS 'Address' FROM " + preScreen + " WHERE date_sold LIKE '%" + txtThree.Text + "%';";

        //    try
        //    {
        //        //open the connection
        //        if (myConn.State != System.Data.ConnectionState.Open)
        //            myConn.Open();

        //        MySqlDataAdapter mySQLDataAd = new MySqlDataAdapter(mySQLcommand);
        //        ds = new DataSet();
        //        mySQLDataAd.Fill(ds, "LoadDataBinding");
        //        dgdIDs.DataContext = ds;
        //        //open the connection
        //        if (myConn.State != System.Data.ConnectionState.Open)
        //            myConn.Open();

        //        //close the connection
        //        myConn.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}

        //private void txtFour_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    //define the connection reference
        //    MySqlConnection myConn = new MySqlConnection("server=localhost; user id=root;password=CMCsales;database=test;persist security info=false");

        //    //define the command reference
        //    MySqlCommand mySQLcommand = new MySqlCommand();

        //    //define the connection used by the command object
        //    mySQLcommand.Connection = myConn;

        //    ////define the command text
        //    mySQLcommand.CommandText = "SELECT date_sold AS 'Date Sold', brand AS 'Brand', model AS 'Model', network AS 'Network', name AS 'Name', address AS 'Address' FROM " + preScreen + " WHERE date_sold LIKE '%" + txtFour.Text + "%';";

        //    try
        //    {
        //        //open the connection
        //        if (myConn.State != System.Data.ConnectionState.Open)
        //            myConn.Open();

        //        MySqlDataAdapter mySQLDataAd = new MySqlDataAdapter(mySQLcommand);
        //        ds = new DataSet();
        //        mySQLDataAd.Fill(ds, "LoadDataBinding");
        //        dgdIDs.DataContext = ds;
        //        //open the connection
        //        if (myConn.State != System.Data.ConnectionState.Open)
        //            myConn.Open();

        //        //close the connection
        //        myConn.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}

        //private void txtFive_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    //define the connection reference
        //    MySqlConnection myConn = new MySqlConnection("server=localhost; user id=root;password=CMCsales;database=test;persist security info=false");

        //    //define the command reference
        //    MySqlCommand mySQLcommand = new MySqlCommand();

        //    //define the connection used by the command object
        //    mySQLcommand.Connection = myConn;

        //    ////define the command text
        //    mySQLcommand.CommandText = "SELECT date_sold AS 'Date Sold', brand AS 'Brand', model AS 'Model', network AS 'Network', name AS 'Name', address AS 'Address' FROM " + preScreen + " WHERE date_sold LIKE '%" + txtFive.Text + "%';";

        //    try
        //    {
        //        //open the connection
        //        if (myConn.State != System.Data.ConnectionState.Open)
        //            myConn.Open();

        //        MySqlDataAdapter mySQLDataAd = new MySqlDataAdapter(mySQLcommand);
        //        ds = new DataSet();
        //        mySQLDataAd.Fill(ds, "LoadDataBinding");
        //        dgdIDs.DataContext = ds;
        //        //open the connection
        //        if (myConn.State != System.Data.ConnectionState.Open)
        //            myConn.Open();

        //        //close the connection
        //        myConn.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}

        //private void txtSix_TextChanged(object sender, TextChangedEventArgs e)
        //{

        //    //define the connection reference
        //    MySqlConnection myConn = new MySqlConnection("server=localhost; user id=root;password=CMCsales;database=test;persist security info=false");

        //    //define the command reference
        //    MySqlCommand mySQLcommand = new MySqlCommand();

        //    //define the connection used by the command object
        //    mySQLcommand.Connection = myConn;

        //    ////define the command text
        //    mySQLcommand.CommandText = "SELECT date_sold AS 'Date Sold', brand AS 'Brand', model AS 'Model', network AS 'Network', name AS 'Name', address AS 'Address' FROM " + preScreen + " WHERE date_sold LIKE '%" + txtSix.Text + "%';";

        //    try
        //    {
        //        //open the connection
        //        if (myConn.State != System.Data.ConnectionState.Open)
        //            myConn.Open();

        //        MySqlDataAdapter mySQLDataAd = new MySqlDataAdapter(mySQLcommand);
        //        ds = new DataSet();
        //        mySQLDataAd.Fill(ds, "LoadDataBinding");
        //        dgdIDs.DataContext = ds;
        //        //open the connection
        //        if (myConn.State != System.Data.ConnectionState.Open)
        //            myConn.Open();

        //        //close the connection
        //        myConn.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}
    }
}