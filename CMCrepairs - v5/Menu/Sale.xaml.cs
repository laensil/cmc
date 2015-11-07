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
using System.Windows.Shapes;
using System.ComponentModel;
using MessageBox = System.Windows.Forms.MessageBox;
using MySql.Data.MySqlClient;
using CMCrepairs.Menu;
using System.Text.RegularExpressions;
using Dymo;
using System.Collections;
#endregion

namespace CMCrepairs
{
    /// <summary>
    /// Interaction logic for Sale.xaml
    /// </summary>
    public partial class Sale : UserControl, ISwitchable
    {
        DymoLabels myLabel;
        DymoAddIn myDymoAddin;
        ArrayList listOfIDs = new ArrayList();

        MySqlConnection myConn;

        string id;

        string[] days = { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15",
                            "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31" };

        Dictionary<string, string> monthsWithNames = new Dictionary<string, string>();

        #region Constructor
        public Sale(MySqlConnection myconn)
        {
            InitializeComponent();

            myConn = myconn;

            SetSaleDate();

            DateTime now = DateTime.Now;
            int year = now.Year;
            string u = year.ToString().Remove(0, 2);
            txtSaleDate.Text = u + now.Month.ToString("00") + now.Day.ToString("00") + now.Hour.ToString("00") + now.Minute.ToString("00") + now.Second.ToString("00");

            if (txtSaleDate.Text.Length > 0)
            {
                btnSaveSaleForm.Visibility = Visibility.Visible;
            }
            else
            {
                btnSaveSaleForm.Visibility = Visibility.Hidden;
            }

            //open the connection
            if (myConn.State != System.Data.ConnectionState.Open)
                myConn.Open();

            //define the command reference
            MySqlCommand mySQLcommand = new MySqlCommand();

            //define the connection used by the command object
            mySQLcommand.Connection = myConn;

            //define the command text
            mySQLcommand.CommandText = "SELECT * FROM cmc_repairs_stock ORDER BY date_sold ASC ;";

            MySqlDataReader mySQLDataReader;
            mySQLDataReader = mySQLcommand.ExecuteReader();

            while (mySQLDataReader.Read())
            {
                string date_sold = mySQLDataReader.GetString("date_sold");
                listOfIDs.Add(date_sold);
            }

            //close the connection
            myConn.Close();

            PopulateMonthsDictionary(monthsWithNames);
        }
        #endregion

        #region Set Sale Date
        protected void SetSaleDate()
        {
            cboSaleDateDays.ItemsSource = days;
            cboSaleDateMonth.ItemsSource = monthsWithNames.Keys;
            txtSaleDateYear.Text = DateTime.Now.Year.ToString();
        }
        #endregion

        #region Merge Date
        protected string MergeDate()
        {
            //if
                return cboSaleDateDays.Text + "/" + monthsWithNames[cboSaleDateMonth.Text] + "/" + txtSaleDateYear.Text;
        }
        #endregion

        #region Basic Validation
        private bool Validation()
        {
            bool val = false;
            int errorCounter = 0;
            errorCounter = Regex.Matches(txtSaleDate.Text, @"[a-zA-Z]").Count;
            if (txtSaleDate.Text.Equals(null) || txtSaleDate.Text.Equals("") || errorCounter > 0)
            {
                MessageBox.Show("Sale Date cannot be blank or contain characters. Please try again");
            }
            else
                errorCounter = Regex.Matches(txtPrice.Text, @"[a-zA-Z]").Count + Regex.Matches(txtDisplayPrice.Text, @"[a-zA-Z]").Count
                    + Regex.Matches(txtContactNum.Text, @"[a-zA-Z]").Count;
            if (errorCounter > 0)
                MessageBox.Show("Price/Display Price/Contact Number cannot contain characters. Please try again");
            else
                val = true;
            return val;
        }
        #endregion

        #region Save Click
        private void btnSaveSaleForm_Click(object sender, RoutedEventArgs e)
        {
            if (Validation())
            {
                try
                {
                    //open the connection
                    if (myConn.State != System.Data.ConnectionState.Open)
                        myConn.Open();

                    //define the command reference
                    MySqlCommand mySQLcommand = new MySqlCommand();

                    //define the connection used by the command object
                    mySQLcommand.Connection = myConn;

                    //define the command text
                    mySQLcommand.CommandText = "INSERT INTO cmc_repairs_stock(now_sold, date_sold, " +
                        "price_sold, grade, item, other_item, brand, imei, network, " +
                        "charger, other_acc, sec_seal_num, box, warranty, mem_card, additional_notice, bought_date, from_screen, display_price, " +
                        "name, address, contact_num, post_code)" +
                        "values(@now_sold, @date_sold, @price_sold, @grade, @item, @other_item, " +
                        "@brand, @imei, @network, @charger, @other_acc, @sec_seal_num, @box, @warranty, " +
                        "@mem_card, @addtional_notice, @bought_date, @from_screen, @display_price, @name, @address, @contact_num, @post_code)";

                    //add values provided by user

                    mySQLcommand.Parameters.AddWithValue("@now_sold", 0);

                    if (txtSaleDate.Text.Equals(null) || txtSaleDate.Text.Equals(""))
                        MessageBox.Show("Sale Date cannot be null. Please try again");
                    else
                        mySQLcommand.Parameters.AddWithValue("@date_sold", txtSaleDate.Text);

                    if (txtPrice.Text.Equals(null) || txtPrice.Text.Equals(""))
                        mySQLcommand.Parameters.AddWithValue("@price_sold", 0);
                    else
                        mySQLcommand.Parameters.AddWithValue("@price_sold", decimal.Parse(txtPrice.Text));

                    if (cboGrade.Text.Equals(null) || cboGrade.Text.Equals(""))
                        mySQLcommand.Parameters.AddWithValue("@grade", "");
                    else
                        mySQLcommand.Parameters.AddWithValue("@grade", cboGrade.Text);

                    if (cboItem.Text.Equals(null) || cboItem.Text.Equals(""))
                        mySQLcommand.Parameters.AddWithValue("@item", "");
                    else
                        mySQLcommand.Parameters.AddWithValue("@item", cboItem.Text);

                    if (txtOtherItem.Text.Equals(null) || txtOtherItem.Text.Equals(""))
                        mySQLcommand.Parameters.AddWithValue("@other_item", "");
                    else
                        mySQLcommand.Parameters.AddWithValue("@other_item", txtOtherItem.Text);

                    if (txtBrand.Text.Equals(null) || txtBrand.Text.Equals(""))
                        mySQLcommand.Parameters.AddWithValue("@brand", "");
                    else
                        mySQLcommand.Parameters.AddWithValue("@brand", txtBrand.Text);

                    //if (txtModel.Text.Equals(null) || txtModel.Text.Equals(""))
                    //    mySQLcommand.Parameters.AddWithValue("@model", "");
                    //else
                    //    mySQLcommand.Parameters.AddWithValue("@model", txtModel.Text);

                    if (txtIMEI.Text.Equals(null) || txtIMEI.Text.Equals(""))
                        mySQLcommand.Parameters.AddWithValue("@imei", "");
                    else
                        mySQLcommand.Parameters.AddWithValue("@imei", txtIMEI.Text);

                    if (txtNetwork.Text.Equals(null) || txtNetwork.Text.Equals(""))
                        mySQLcommand.Parameters.AddWithValue("@network", "");
                    else
                        mySQLcommand.Parameters.AddWithValue("@network", txtNetwork.Text);

                    mySQLcommand.Parameters.AddWithValue("@charger", chbCharger.IsChecked.Value);

                    if (txtOtherAcc.Text.Equals(null) || txtOtherAcc.Text.Equals(""))
                        mySQLcommand.Parameters.AddWithValue("@other_acc", "");
                    else
                        mySQLcommand.Parameters.AddWithValue("@other_acc", txtOtherAcc.Text);

                    if (txtSecSealNum.Text.Equals(null) || txtSecSealNum.Text.Equals(""))
                        mySQLcommand.Parameters.AddWithValue("@sec_seal_num", "");
                    else
                        mySQLcommand.Parameters.AddWithValue("@sec_seal_num", txtSecSealNum.Text);

                    mySQLcommand.Parameters.AddWithValue("@box", chbBox.IsChecked.Value);

                    if (cboWarranty.Text.Equals(null) || cboWarranty.Text.Equals(""))
                        mySQLcommand.Parameters.AddWithValue("@warranty", "");
                    else
                        mySQLcommand.Parameters.AddWithValue("@warranty", cboWarranty.Text);

                    mySQLcommand.Parameters.AddWithValue("@mem_card", chbMemCard.IsChecked.Value);

                    if (txtAdditionalNotice.Text.Equals(null) || txtAdditionalNotice.Text.Equals(""))
                        mySQLcommand.Parameters.AddWithValue("@addtional_notice", "");
                    else
                        mySQLcommand.Parameters.AddWithValue("@addtional_notice", txtAdditionalNotice.Text);

                    //if (txtDate.Text.Equals(null) || txtDate.Text.Equals(""))
                    //    mySQLcommand.Parameters.AddWithValue("@bought_date", "");
                    //else
                    //    mySQLcommand.Parameters.AddWithValue("@bought_date", txtDate.Text);

                    if ((cboSaleDateDays.Text.Equals(null) || cboSaleDateDays.Text.Equals(""))
                        || (cboSaleDateMonth.Text.Equals(null) || cboSaleDateMonth.Text.Equals(""))
                        || (txtSaleDateYear.Text.Equals(null) || txtSaleDateYear.Text.Equals("")))
                        mySQLcommand.Parameters.AddWithValue("@bought_date", "");
                    else
                        mySQLcommand.Parameters.AddWithValue("@bought_date", MergeDate());

                    mySQLcommand.Parameters.AddWithValue("@from_screen", "cmc_repairs_sale");

                    if (txtDisplayPrice.Text.Equals(null) || txtDisplayPrice.Text.Equals(""))
                        mySQLcommand.Parameters.AddWithValue("@display_price", 0);
                    else
                        mySQLcommand.Parameters.AddWithValue("@display_price", decimal.Parse(txtDisplayPrice.Text));

                    if (txtName.Text.Equals(null) || txtName.Text.Equals(""))
                        mySQLcommand.Parameters.AddWithValue("@name", "");
                    else
                        mySQLcommand.Parameters.AddWithValue("@name", txtName.Text);

                    if (txtAddress.Text.Equals(null) || txtAddress.Text.Equals(""))
                        mySQLcommand.Parameters.AddWithValue("@address", "");
                    else
                        mySQLcommand.Parameters.AddWithValue("@address", txtAddress.Text);

                    if (txtContactNum.Text.Equals(null) || txtContactNum.Text.Equals(""))
                        mySQLcommand.Parameters.AddWithValue("@contact_num", "");
                    else
                        mySQLcommand.Parameters.AddWithValue("@contact_num", txtContactNum.Text);

                    if (txtPostCode.Text.Equals(null) || txtPostCode.Text.Equals(""))
                        mySQLcommand.Parameters.AddWithValue("@post_code", "");
                    else
                        mySQLcommand.Parameters.AddWithValue("@post_code", txtPostCode.Text);

                    //mySQLcommand.ExecuteNonQuery();
                    MySqlDataReader mySQLDataReader;
                    mySQLDataReader = mySQLcommand.ExecuteReader();

                    //close the connection
                    myConn.Close();

                    MessageBox.Show("Information Saved");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        #endregion

        #region ISwitchable Members
        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }

        private void ExitButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //System.Windows.Forms.DialogResult answer = MessageBox.Show("Menu?", "", System.Windows.Forms.MessageBoxButtons.YesNo);

            //if (answer == System.Windows.Forms.DialogResult.Yes)
            Switcher.Switch(new MainWindow());
        }
        #endregion

        #region Clear Click
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new Sale(myConn));
        }
        #endregion

        #region Textbox textchanged
        private void txtSaleDate_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtSaleDate.Text.Equals(null) || txtSaleDate.Text.Equals(""))
            {
                btnSaveSaleForm.Visibility = Visibility.Hidden;
                btnClear.Visibility = Visibility.Hidden;
            }
            else
            {
                btnSaveSaleForm.Visibility = Visibility.Visible;
                btnClear.Visibility = Visibility.Visible;
            }
        }

        private void txtPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            int errorCounter = 0;
            errorCounter = Regex.Matches(txtPrice.Text, @"[a-zA-Z]").Count;
            if (errorCounter > 0)
            {
                MessageBox.Show("Price cannot contain characters. Please try again");
                txtPrice.Text = txtPrice.Text.Substring(0, txtPrice.Text.Length - 1);
            }
        }

        private void txtContactNum_TextChanged(object sender, TextChangedEventArgs e)
        {
            int errorCounter = 0;
            errorCounter = Regex.Matches(txtContactNum.Text, @"[a-zA-Z]").Count;
            if (errorCounter > 0)
            {
                MessageBox.Show("Contact Number cannot contain characters. Please try again");
                txtContactNum.Text = txtContactNum.Text.Substring(0, txtContactNum.Text.Length - 1);
            }
        }

        //private void txtBoughtPrice_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    int errorCounter = 0;
        //    errorCounter = Regex.Matches(txtBoughtPrice.Text, @"[a-zA-Z]").Count;
        //    if (errorCounter > 0)
        //    {
        //        MessageBox.Show("Bought Price cannot contain characters. Please try again");
        //        txtBoughtPrice.Text = txtBoughtPrice.Text.Substring(0, txtBoughtPrice.Text.Length - 1);
        //    }
        //}

        //private void txtContactNum_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    int errorCounter = 0;
        //    errorCounter = Regex.Matches(txtContactNum.Text, @"[a-zA-Z]").Count;
        //    if (errorCounter > 0)
        //    {
        //        MessageBox.Show("Contact Number cannot contain characters. Please try again");
        //        txtContactNum.Text = txtContactNum.Text.Substring(0, txtContactNum.Text.Length - 1);
        //    }
        //}
        #endregion

        #region Barcode Button
        private void btnBarcode_Click(object sender, RoutedEventArgs e)
        {
            //Switcher.Switch(new BarcodeWindow());




            myDymoAddin = new DymoAddIn();
            myLabel = new DymoLabels();

            try
            {
                if (myDymoAddin.Open(@"C:\Labels\SaleSmall.label"))
                {
                    if ((txtBrand.Text.ToString().Equals(null) || txtBrand.Text.ToString().Equals("")) ||
                        //(txtModel.Text.ToString().Equals(null) || txtModel.Text.ToString().Equals("")) ||
                        (txtSaleDate.Text.ToString().Equals(null) || txtSaleDate.Text.ToString().Equals("")) ||
                        (txtNetwork.Text.ToString().Equals(null) || txtNetwork.Text.ToString().Equals("")) ||
                        (txtDisplayPrice.Text.ToString().Equals(null) || txtDisplayPrice.Text.ToString().Equals("")))
                    {
                        MessageBox.Show("Sale Date, Brand, Model, Network or DisplayPrice is empty");
                    }
                    else
                    {
                        if (!listOfIDs.Contains(txtSaleDate.Text))
                        {
                            btnSaveSaleForm_Click(this, new RoutedEventArgs());
                        }
                        else
                        {
                            MessageBox.Show("Barcode has printed and Item already has been saved");
                        }

                        string combine = txtBrand.Text;// +" " + txtModel.Text;
                        string combineAddDetailsNetwork = txtAdditionalNotice.Text + "  " + txtNetwork.Text;

                        //open the connection
                        if (myConn.State != System.Data.ConnectionState.Open)
                            myConn.Open();

                        //define the command reference
                        MySqlCommand mySQLcommand = new MySqlCommand();

                        //define the connection used by the command object
                        mySQLcommand.Connection = myConn;

                        ////define the command text
                        mySQLcommand.CommandText = "SELECT * FROM cmc_repairs_stock WHERE date_sold=@date_sold";

                        DateTime mydate = DateTime.Now;

                        //add values provided by user

                        if (txtSaleDate.Text.Equals(null) || txtSaleDate.Text.Equals(""))
                            MessageBox.Show("This cannot be null");
                        else
                            mySQLcommand.Parameters.AddWithValue("@date_sold", txtSaleDate.Text);

                        //mySQLcommand.ExecuteNonQuery();
                        MySqlDataReader mySQLDataReader;
                        mySQLDataReader = mySQLcommand.ExecuteReader();

                        while (mySQLDataReader.Read())
                        {
                            id = mySQLDataReader.GetString("id");
                        }

                        //close the connection
                        myConn.Close();

                        myLabel.SetField("lblBrand", combine.ToString());
                        myLabel.SetField("lblSaleDate", txtSaleDate.Text);
                        myLabel.SetField("lbl_ID", id.ToString());
                        myLabel.SetField("lblNetwork", combineAddDetailsNetwork.ToString());
                        myLabel.SetField("lblPrice", "£" + txtDisplayPrice.Text);
                        myDymoAddin.StartPrintJob();
                        myDymoAddin.Print(1, false);
                        myDymoAddin.EndPrintJob();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("There is an error with the label, please check all fields and try again.");
            }
        }
        #endregion

        #region Populate Months Dictionary
        public Dictionary<string, string> PopulateMonthsDictionary(Dictionary<string, string> months)
        {
            months.Add("Jan", "01");
            months.Add("Feb", "02");
            months.Add("Mar", "03");
            months.Add("Apr", "04");
            months.Add("May", "05");
            months.Add("Jun", "06");
            months.Add("Jul", "07");
            months.Add("Aug", "08");
            months.Add("Sep", "09");
            months.Add("Oct", "10");
            months.Add("Nov", "11");
            months.Add("Dec", "12");

            return months;
        }
        #endregion

        #region Lock/Unlock Buttons

        private void LockSaveButton()
        {
            btnSaveSaleForm.IsEnabled = false;
        }

        private void UnlockSaveButton()
        {
            btnSaveSaleForm.IsEnabled = true;
        }

        private void LockBarcodeButton()
        {
            btnBarcode.IsEnabled = false;
        }

        private void UnlockBarcodeButton()
        {
            btnBarcode.IsEnabled = true;
        }

        private void LockClearButton()
        {
            btnClear.IsEnabled = false;
        }

        private void UnlockClearButton()
        {
            btnClear.IsEnabled = true;
        }

        #endregion
    }
}