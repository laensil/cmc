﻿#region imports
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
    /// Interaction logic for Stock.xaml
    /// </summary>
    public partial class Stock : UserControl, ISwitchable
    {
        DymoLabels myLabel;
        DymoAddIn myDymoAddin;
        ArrayList listOfIDs = new ArrayList();
        Dictionary<int, string> idsAndDateSold = new Dictionary<int, string>();

        MySqlConnection myConn;

        int indexOfDay = 0;
        int indexOfMonth = 0;

        string[] days = { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15",
                            "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31" };

        Dictionary<string, string> monthsWithNames = new Dictionary<string, string>();

        string id;

        bool isConnOpen = false;

        #region Constructors
        public Stock(MySqlConnection myconn)
        {
            InitializeComponent();
            myConn = myconn;
            FillCombo();

            txtDateSold.Focus();
            //btnUpdate.Visibility = Visibility.Hidden;

            SetDates();

            //open the connection
            if (myConn.State != System.Data.ConnectionState.Open)
                myConn.Open();

            //define the command reference
            MySqlCommand mySQLcommand = new MySqlCommand();

            //define the connection used by the command object
            mySQLcommand.Connection = myConn;

            //define the command text
            mySQLcommand.CommandText = "SELECT * FROM cmc_repairs_stock ORDER BY id ASC ;";

            MySqlDataReader mySQLDataReader;
            mySQLDataReader = mySQLcommand.ExecuteReader();

            while (mySQLDataReader.Read())
            {
                string date_sold = mySQLDataReader.GetString("date_sold");
                int ids = mySQLDataReader.GetInt32("id");
                listOfIDs.Add(date_sold);
                idsAndDateSold.Add(ids, date_sold);
            }

            //close the connection
            myConn.Close();

            PopulateMonthsDictionary(monthsWithNames);
        }

        public Stock(string id, MySqlConnection myconn)
        {
            InitializeComponent();
            FillCombo();
            cboIDs.SelectedValue = id;
            cboIDs_Change(cboIDs, new EventArgs());

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
                int ids = mySQLDataReader.GetInt32("id");
                listOfIDs.Add(date_sold);
                idsAndDateSold.Add(ids, date_sold);
            }

            //close the connection
            myConn.Close();

            PopulateMonthsDictionary(monthsWithNames);
        }
        #endregion

        #region PopuldateMonthsDictionary
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

        #region Set Dates
        protected void SetDates()
        {
            cboBuyDateDays.ItemsSource = days;
            cboBuyDateMonth.ItemsSource = monthsWithNames.Keys;
            txtBuyDateYear.Text = DateTime.Now.Year.ToString();
        }
        #endregion

        #region MergeDate
        protected string MergeDate()
        {
            return cboBuyDateDays.Text + "/" + monthsWithNames[cboBuyDateMonth.Text] + "/" + txtBuyDateYear.Text;
        }
        #endregion

        #region SetID for barcode
        private void SetID()
        {
            //open the connection
            if (myConn.State != System.Data.ConnectionState.Open)
                myConn.Open();

            //define the command reference
            MySqlCommand mySQLcommand = new MySqlCommand();

            //define the connection used by the command object
            mySQLcommand.Connection = myConn;

            ////define the command text
            mySQLcommand.CommandText = "SELECT * FROM cmc_repairs_stock WHERE date_sold=@date_sold";

            //add values provided by user

            if (txtDateSold.Text.Equals(null) || txtDateSold.Text.Equals(""))
                MessageBox.Show("This cannot be null");
            else
                mySQLcommand.Parameters.AddWithValue("@date_sold", txtDateSold.Text);

            //mySQLcommand.ExecuteNonQuery();
            MySqlDataReader mySQLDataReader;
            mySQLDataReader = mySQLcommand.ExecuteReader();

            while (mySQLDataReader.Read())
            {
                id = mySQLDataReader.GetString("id");
            }

            //close the connection
            myConn.Close();
        }
        #endregion

        #region Fill Combo
        void FillCombo()
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
                mySQLcommand.CommandText = "SELECT * FROM cmc_repairs_stock ORDER BY date_sold DESC ;";

                MySqlDataReader mySQLDataReader;
                mySQLDataReader = mySQLcommand.ExecuteReader();

                while (mySQLDataReader.Read())
                {
                    string date_sold = mySQLDataReader.GetString("date_sold");
                    cboIDs.Items.Add(date_sold);
                }

                //close the connection
                myConn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region cboIDs On Change
        private void cboIDs_Change(object sender, EventArgs e)
        {
            //define the command reference
            MySqlCommand mySQLcommand = new MySqlCommand();

            //define the connection used by the command object
            mySQLcommand.Connection = myConn;

            //define the command text
            mySQLcommand.CommandText = "SELECT * FROM cmc_repairs_stock WHERE date_sold='" + cboIDs.Text + "' ;";

            MySqlDataReader mySQLDataReader;

            try
            {
                //open the connection
                if (myConn.State != System.Data.ConnectionState.Open)
                    myConn.Open();
                mySQLDataReader = mySQLcommand.ExecuteReader();

                while (mySQLDataReader.Read())
                {
                    int priceSoldNull = mySQLDataReader.GetOrdinal("price_sold");
                    int gradeNull = mySQLDataReader.GetOrdinal("grade");
                    int checkmendNull = mySQLDataReader.GetOrdinal("checkmend");
                    int itemNull = mySQLDataReader.GetOrdinal("item");
                    int otherItemNull = mySQLDataReader.GetOrdinal("other_item");
                    int brandNull = mySQLDataReader.GetOrdinal("brand");
                    //int modelNull = mySQLDataReader.GetOrdinal("model");
                    int imeiNull = mySQLDataReader.GetOrdinal("imei");
                    int networkNull = mySQLDataReader.GetOrdinal("network");
                    int otherAccNull = mySQLDataReader.GetOrdinal("other_acc");
                    int secSealNumNull = mySQLDataReader.GetOrdinal("sec_seal_num");
                    int warrantyNull = mySQLDataReader.GetOrdinal("warranty");
                    int additionalNoticeNull = mySQLDataReader.GetOrdinal("additional_notice");
                    int buyDateNull = mySQLDataReader.GetOrdinal("buy_date");
                    int boughtPriceNull = mySQLDataReader.GetOrdinal("bought_price");
                    int nameNull = mySQLDataReader.GetOrdinal("name");
                    int addressNull = mySQLDataReader.GetOrdinal("address");
                    int contactNumNull = mySQLDataReader.GetOrdinal("contact_num");
                    int postCodeNull = mySQLDataReader.GetOrdinal("post_code");
                    int displayPriceNull = mySQLDataReader.GetOrdinal("display_price");
                    int statusOfItemNull = mySQLDataReader.GetOrdinal("status_of_item");

                    string date_sold = mySQLDataReader.GetString("date_sold");
                    string now_sold = mySQLDataReader.GetInt32("now_sold").ToString();
                    string price_sold = mySQLDataReader.IsDBNull(priceSoldNull) ? null : mySQLDataReader.GetDecimal("price_sold").ToString();
                    string grade = mySQLDataReader.IsDBNull(gradeNull) ? null : mySQLDataReader.GetString("grade");
                    string checkmend = mySQLDataReader.IsDBNull(checkmendNull) ? null : mySQLDataReader.GetString("checkmend");
                    string item = mySQLDataReader.IsDBNull(itemNull) ? null : mySQLDataReader.GetString("item");
                    string other_item = mySQLDataReader.IsDBNull(otherItemNull) ? null : mySQLDataReader.GetString("other_item");
                    string brand = mySQLDataReader.IsDBNull(brandNull) ? null : mySQLDataReader.GetString("brand");
                    //string model = mySQLDataReader.IsDBNull(modelNull) ? null : mySQLDataReader.GetString("model");
                    string imei = mySQLDataReader.IsDBNull(imeiNull) ? null : mySQLDataReader.GetString("imei");
                    string network = mySQLDataReader.IsDBNull(networkNull) ? null : mySQLDataReader.GetString("network");
                    string charger = mySQLDataReader.GetInt32("charger").ToString();
                    string other_acc = mySQLDataReader.IsDBNull(otherAccNull) ? null : mySQLDataReader.GetString("other_acc");
                    string sec_seal_num = mySQLDataReader.IsDBNull(secSealNumNull) ? null : mySQLDataReader.GetString("sec_seal_num");
                    string box = mySQLDataReader.GetInt32("box").ToString();
                    string warranty = mySQLDataReader.IsDBNull(warrantyNull) ? null : mySQLDataReader.GetString("warranty");
                    string mem_card = mySQLDataReader.GetInt32("mem_card").ToString();
                    string additional_notice = mySQLDataReader.IsDBNull(additionalNoticeNull) ? null : mySQLDataReader.GetString("additional_notice");
                    string buy_date = mySQLDataReader.IsDBNull(buyDateNull) ? null : mySQLDataReader.GetString("buy_date");
                    string bought_price = mySQLDataReader.IsDBNull(boughtPriceNull) ? null : mySQLDataReader.GetDecimal("bought_price").ToString();
                    string name = mySQLDataReader.IsDBNull(nameNull) ? null : mySQLDataReader.GetString("name");
                    string address = mySQLDataReader.IsDBNull(addressNull) ? null : mySQLDataReader.GetString("address");
                    string contact_num = mySQLDataReader.IsDBNull(contactNumNull) ? null : mySQLDataReader.GetString("contact_num");
                    string post_code = mySQLDataReader.IsDBNull(postCodeNull) ? null : mySQLDataReader.GetString("post_code");
                    string display_price = mySQLDataReader.IsDBNull(displayPriceNull) ? null : mySQLDataReader.GetString("display_price");
                    string status_of_item = mySQLDataReader.IsDBNull(statusOfItemNull) ? null : mySQLDataReader.GetString("status_of_item");

                    if (now_sold.Equals("1"))
                    {
                        chbNowSold.IsChecked = true;
                    }
                    else
                    {
                        chbNowSold.IsChecked = false;
                    }
                    isConnOpen = true;
                    txtDateSold.Text = date_sold;
                    isConnOpen = false;
                    txtPriceSold.Text = price_sold;
                    cboGrade.SelectedValue = grade;
                    cboCheckMEND.SelectedValue = checkmend;
                    cboItem.Text = item;
                    txtOtherItem.Text = other_item;
                    txtBrand.Text = brand;
                    //txtModel.Text = model;
                    txtIMEI.Text = imei;
                    txtNetwork.Text = network;

                    if (charger.Equals("1"))
                    {
                        chbCharger.IsChecked = true;
                    }
                    else
                    {
                        chbCharger.IsChecked = false;
                    }
                    txtOtherAcc.Text = other_acc;
                    txtSecSealNum.Text = sec_seal_num;

                    if (box.Equals("1"))
                    {
                        chbBox.IsChecked = true;
                    }
                    else
                    {
                        chbBox.IsChecked = false;
                    }
                    cboWarranty.Text = warranty;

                    if (mem_card.Equals("1"))
                    {
                        chbMemCard.IsChecked = true;
                    }
                    else
                    {
                        chbMemCard.IsChecked = false;
                    }
                    txtAdditionalNotice.Text = additional_notice;
                    //txtBuyDate.Text = buy_date;

                    if (buy_date != null && !buy_date.Equals(""))
                    {
                        indexOfDay = buy_date.IndexOf("/");
                        if (indexOfDay > 0)
                        {
                            cboBuyDateDays.Text = buy_date.Substring(0, indexOfDay);
                            indexOfMonth = buy_date.IndexOf("/", indexOfDay + 1);
                            if (indexOfMonth > indexOfDay)
                            {
                                int indexOfMonthValue = -1;
                                string monthBuyDateNumber = buy_date.Substring(indexOfDay + 1, (indexOfMonth - indexOfDay) - 1);

                                if (int.TryParse(monthBuyDateNumber, out indexOfMonthValue))
                                {
                                    cboBuyDateMonth.SelectedIndex = indexOfMonthValue - 1;
                                }

                                //bool k = int.TryParse(buy_date.Substring(indexOfDay + 1, (indexOfMonth - indexOfDay) - 1), out indexOfMonthValue);

                                //cboBuyDateMonth.SelectedIndex = indexOfMonthValue;
                                txtBuyDateYear.Text = buy_date.Substring(indexOfMonth + 1);
                            }
                        }
                    }
                    else
                    {
                        cboBuyDateDays.Text = "";
                        cboBuyDateMonth.Text = "";
                        txtBuyDateYear.Text = "";
                    }

                    txtBoughtPrice.Text = bought_price;
                    txtName.Text = name;
                    txtAddress.Text = address;
                    txtContactNum.Text = contact_num;
                    txtPostCode.Text = post_code;
                    txtDisplayPrice.Text = display_price;
                    cboStatusOfItem.SelectedValue = status_of_item;
                }

                //close the connection
                myConn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region Basic Validation
        private bool Validation()
        {
            bool val = false;
            int errorCounter = 0;
            errorCounter = Regex.Matches(txtDateSold.Text, @"[a-zA-Z]").Count;
            if (txtDateSold.Text.Equals(null) || txtDateSold.Text.Equals("") || errorCounter > 0)
            {
                MessageBox.Show("Date Sold cannot be blank or contain characters. Please try again");
            }
            else
                errorCounter = Regex.Matches(txtPriceSold.Text, @"[a-zA-Z]").Count + Regex.Matches(txtBoughtPrice.Text, @"[a-zA-Z]").Count + Regex.Matches(txtContactNum.Text, @"[a-zA-Z]").Count + Regex.Matches(txtDisplayPrice.Text, @"[a-zA-Z]").Count;
            if (errorCounter > 0)
                MessageBox.Show("Price Sold/Bought Price/Contact Number/Display Price cannot contain characters. Please try again");
            else
                val = true;
            return val;
        }
        #endregion

        //#region Save Click
        //private void SaveStockForm_Click(object sender, RoutedEventArgs e)
        //{
        //    if (Validation())
        //    {
        //        MessageBox.Show("Saving data to the database");

        //        //DateTimePicker dtp = this.wfhBuyDateWrapper.Child as DateTimePicker;
        //        //DateTimePicker dtp2 = this.wfhDateSoldWrapper.Child as DateTimePicker;
        //        //DateTime now = DateTime.Now;
        //        //String theDate = "" + now.Year + now.Month + now.Day + now.Hour + now.Minute + now.Second;
        //        //MessageBox.Show("datetime is " + theDate);
        //        //MessageBox.Show("" + dtp.Value.ToShortDateString());

        //        //define the connection reference



        //        ///TODO - AWC - Change below pwd



        //        MySqlConnection myConn = new MySqlConnection("server=localhost; user id=root;password=CMCsales;database=test;persist security info=false");

        //        try
        //        {
        //            //open the connection
        //            if (myConn.State != System.Data.ConnectionState.Open)
        //                myConn.Open();

        //            //define the command reference
        //            MySqlCommand mySQLcommand = new MySqlCommand();

        //            //define the connection used by the command object
        //            mySQLcommand.Connection = myConn;

        //            //define the command text
        //            mySQLcommand.CommandText = "INSERT INTO cmc_repairs_stock(now_sold, date_sold, " +
        //                "price_sold, grade, checkmend, item, other_item, brand, model, imei, network, " +
        //                "charger, other_acc, sec_seal_num, box, warranty, mem_card, additional_notice, " +
        //                "buy_date, bought_price, name, address, contact_num, post_code)" +
        //                "values(@now_sold, @date_sold, @price_sold, @grade, @checkmend, @item, @other_item, " +
        //                "@brand, @model, @imei, @network, @charger, @other_acc, @sec_seal_num, @box, @warranty, " +
        //                "@mem_card, @addtional_notice, @buy_date, @bought_price, @name, @address, @contact_num, @post_code)";

        //            //DateTime mydate = DateTime.Now;

        //            //add values provided by user

        //            mySQLcommand.Parameters.AddWithValue("@now_sold", chbNowSold.IsChecked.Value);

        //            if (txtDateSold.Text.Equals(null) || txtDateSold.Text.Equals(""))
        //                MessageBox.Show("DateSold cannot be null. Please try again");
        //            else
        //                mySQLcommand.Parameters.AddWithValue("@date_sold", txtDateSold.Text);

        //            if (txtPriceSold.Text.Equals(null) || txtPriceSold.Text.Equals(""))
        //                mySQLcommand.Parameters.AddWithValue("@price_sold", 0);
        //            else
        //                mySQLcommand.Parameters.AddWithValue("@price_sold", decimal.Parse(txtPriceSold.Text));

        //            if (cboGrade.Text.Equals(null) || cboGrade.Text.Equals(""))
        //                mySQLcommand.Parameters.AddWithValue("@grade", "");
        //            else
        //                mySQLcommand.Parameters.AddWithValue("@grade", cboGrade.Text);

        //            if (cboCheckMEND.Text.Equals(null) || cboCheckMEND.Text.Equals(""))
        //                mySQLcommand.Parameters.AddWithValue("@checkmend", "");
        //            else
        //                mySQLcommand.Parameters.AddWithValue("@checkmend", cboCheckMEND.Text);

        //            if (cboItem.Text.Equals(null) || cboItem.Text.Equals(""))
        //                mySQLcommand.Parameters.AddWithValue("@item", "");
        //            else
        //                mySQLcommand.Parameters.AddWithValue("@item", cboItem.Text);

        //            if (txtOtherItem.Text.Equals(null) || txtOtherItem.Text.Equals(""))
        //                mySQLcommand.Parameters.AddWithValue("@other_item", "");
        //            else
        //                mySQLcommand.Parameters.AddWithValue("@other_item", txtOtherItem.Text);

        //            if (txtBrand.Text.Equals(null) || txtBrand.Text.Equals(""))
        //                mySQLcommand.Parameters.AddWithValue("@brand", "");
        //            else
        //                mySQLcommand.Parameters.AddWithValue("@brand", txtBrand.Text);

        //            if (txtModel.Text.Equals(null) || txtModel.Text.Equals(""))
        //                mySQLcommand.Parameters.AddWithValue("@model", "");
        //            else
        //                mySQLcommand.Parameters.AddWithValue("@model", txtModel.Text);

        //            if (txtIMEI.Text.Equals(null) || txtIMEI.Text.Equals(""))
        //                mySQLcommand.Parameters.AddWithValue("@imei", "");
        //            else
        //                mySQLcommand.Parameters.AddWithValue("@imei", txtIMEI.Text);

        //            if (txtNetwork.Text.Equals(null) || txtNetwork.Text.Equals(""))
        //                mySQLcommand.Parameters.AddWithValue("@network", "");
        //            else
        //                mySQLcommand.Parameters.AddWithValue("@network", txtNetwork.Text);

        //            mySQLcommand.Parameters.AddWithValue("@charger", chbCharger.IsChecked.Value);

        //            if (txtOtherAcc.Text.Equals(null) || txtOtherAcc.Text.Equals(""))
        //                mySQLcommand.Parameters.AddWithValue("@other_acc", "");
        //            else
        //                mySQLcommand.Parameters.AddWithValue("@other_acc", txtOtherAcc.Text);

        //            if (txtSecSealNum.Text.Equals(null) || txtSecSealNum.Text.Equals(""))
        //                mySQLcommand.Parameters.AddWithValue("@sec_seal_num", "");
        //            else
        //                mySQLcommand.Parameters.AddWithValue("@sec_seal_num", txtSecSealNum.Text);

        //            mySQLcommand.Parameters.AddWithValue("@box", chbBox.IsChecked.Value);

        //            if (cboWarranty.Text.Equals(null) || cboWarranty.Text.Equals(""))
        //                mySQLcommand.Parameters.AddWithValue("@warranty", "");
        //            else
        //                mySQLcommand.Parameters.AddWithValue("@warranty", cboWarranty.Text);

        //            mySQLcommand.Parameters.AddWithValue("@mem_card", chbMemCard.IsChecked.Value);

        //            if (txtAdditionalNotice.Text.Equals(null) || txtAdditionalNotice.Text.Equals(""))
        //                mySQLcommand.Parameters.AddWithValue("@addtional_notice", "");
        //            else
        //                mySQLcommand.Parameters.AddWithValue("@addtional_notice", txtAdditionalNotice.Text);

        //            //mySQLcommand.Parameters.AddWithValue("@buy_date", dtp.Value.ToShortDateString());

        //            if (txtBuyDate.Text.Equals(null) || txtBuyDate.Text.Equals(""))
        //                mySQLcommand.Parameters.AddWithValue("@buy_date", "");
        //            else
        //                mySQLcommand.Parameters.AddWithValue("@buy_date", txtBuyDate.Text);

        //            if (txtBoughtPrice.Text.Equals(null) || txtBoughtPrice.Text.Equals(""))
        //                mySQLcommand.Parameters.AddWithValue("@bought_price", 0);
        //            else
        //                mySQLcommand.Parameters.AddWithValue("@bought_price", decimal.Parse(txtBoughtPrice.Text));

        //            if (txtName.Text.Equals(null) || txtName.Text.Equals(""))
        //                mySQLcommand.Parameters.AddWithValue("@name", "");
        //            else
        //                mySQLcommand.Parameters.AddWithValue("@name", txtName.Text);

        //            if (txtAddress.Text.Equals(null) || txtAddress.Text.Equals(""))
        //                mySQLcommand.Parameters.AddWithValue("@address", "");
        //            else
        //                mySQLcommand.Parameters.AddWithValue("@address", txtAddress.Text);

        //            if (txtContactNum.Text.Equals(null) || txtContactNum.Text.Equals(""))
        //                mySQLcommand.Parameters.AddWithValue("@contact_num", 0);
        //            else
        //                mySQLcommand.Parameters.AddWithValue("@contact_num", Int32.Parse(txtContactNum.Text));

        //            if (txtPostCode.Text.Equals(null) || txtPostCode.Text.Equals(""))
        //                mySQLcommand.Parameters.AddWithValue("@post_code", "");
        //            else
        //                mySQLcommand.Parameters.AddWithValue("@post_code", txtPostCode.Text);

        //            //mySQLcommand.ExecuteNonQuery();
        //            MySqlDataReader mySQLDataReader;
        //            mySQLDataReader = mySQLcommand.ExecuteReader();

        //            //close the connection
        //            myConn.Close();

        //            //empty the textboxes
        //            refresh();

        //            MessageBox.Show("Info Added!! Congrats");

        //            Switcher.Switch(new Stock());

        //            //MessageBox.Show("ID is: " + theDate);

        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show(ex.Message);
        //        }
        //    }
        //    else
        //    {
        //    }
        //}
        //#endregion

        #region ISwitchable Members
        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Search Button Click
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new SearchID("cmc_repairs_stock", myConn));
        }
        #endregion

        #region Exit Button Click
        private void ExitButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //System.Windows.Forms.DialogResult answer = MessageBox.Show("Menu?", "", System.Windows.Forms.MessageBoxButtons.YesNo);

            //if (answer == System.Windows.Forms.DialogResult.Yes)
            Switcher.Switch(new MainWindow());
        }
        #endregion

        #region Update Click
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (Validation())
            {
                //DateTimePicker dtp = this.wfhBuyDateWrapper.Child as DateTimePicker;
                //DateTimePicker dtp2 = this.wfhDateSoldWrapper.Child as DateTimePicker;
                //DateTime now = DateTime.Now;
                //String theDate = "" + now.Year + now.Month + now.Day + now.Hour + now.Minute + now.Second;
                //MessageBox.Show("datetime is " + theDate);
                //MessageBox.Show("" + dtp.Value.ToShortDateString());

                try
                {
                    //open the connection
                    if (myConn.State != System.Data.ConnectionState.Open)
                        myConn.Open();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                //define the command reference
                MySqlCommand mySQLcommand = new MySqlCommand();

                //define the connection used by the command object
                mySQLcommand.Connection = myConn;

                //define the command text
                mySQLcommand.CommandText = "UPDATE cmc_repairs_stock SET now_sold=@now_sold, date_sold=@date_sold, price_sold=@price_sold, grade=@grade, checkmend=@checkmend, item=@item, other_item=@other_item, " +
                    "brand=@brand, imei=@imei, network=@network, charger=@charger, other_acc=@other_acc, sec_seal_num=@sec_seal_num, box=@box, warranty=@warranty, " +
                    "mem_card=@mem_card, additional_notice=@addtional_notice, buy_date=@buy_date, bought_price=@bought_price, name=@name, address=@address, contact_num=@contact_num, post_code=@post_code, " +
                    "display_price=@display_price, status_of_item=@status_of_item " +
                    "WHERE date_sold=@date_sold";

                //add values provided by user

                mySQLcommand.Parameters.AddWithValue("@now_sold", chbNowSold.IsChecked.Value);

                if (txtDateSold.Text.Equals(null) || txtDateSold.Text.Equals(""))
                    MessageBox.Show("This cannot be null");
                else
                    mySQLcommand.Parameters.AddWithValue("@date_sold", txtDateSold.Text);

                if (txtPriceSold.Text.Equals(null) || txtPriceSold.Text.Equals(""))
                    mySQLcommand.Parameters.AddWithValue("@price_sold", 0);
                else
                    mySQLcommand.Parameters.AddWithValue("@price_sold", decimal.Parse(txtPriceSold.Text));

                if (cboGrade.Text.Equals(null) || cboGrade.Text.Equals(""))
                    mySQLcommand.Parameters.AddWithValue("@grade", "");
                else
                    mySQLcommand.Parameters.AddWithValue("@grade", cboGrade.Text);

                if (cboCheckMEND.Text.Equals(null) || cboCheckMEND.Text.Equals(""))
                    mySQLcommand.Parameters.AddWithValue("@checkmend", "");
                else
                    mySQLcommand.Parameters.AddWithValue("@checkmend", cboCheckMEND.Text);

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

                //mySQLcommand.Parameters.AddWithValue("@buy_date", dtp.Value.ToShortDateString());

                //if (txtBuyDate.Text.Equals(null) || txtBuyDate.Text.Equals(""))
                //    mySQLcommand.Parameters.AddWithValue("@buy_date", "");
                //else
                //    mySQLcommand.Parameters.AddWithValue("@buy_date", txtBuyDate.Text);

                if ((cboBuyDateDays.Text.Equals(null) || cboBuyDateDays.Text.Equals("")) ||
                        (cboBuyDateMonth.Text.Equals(null) || cboBuyDateMonth.Text.Equals("")) ||
                        (txtBuyDateYear.Text.Equals(null) || txtBuyDateYear.Text.Equals("")))
                    mySQLcommand.Parameters.AddWithValue("@buy_date", "");
                else
                    mySQLcommand.Parameters.AddWithValue("@buy_date", MergeDate());

                if (txtBoughtPrice.Text.Equals(null) || txtBoughtPrice.Text.Equals(""))
                    mySQLcommand.Parameters.AddWithValue("@bought_price", 0);
                else
                    mySQLcommand.Parameters.AddWithValue("@bought_price", decimal.Parse(txtBoughtPrice.Text));

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

                if (txtDisplayPrice.Text.Equals(null) || txtDisplayPrice.Text.Equals(""))
                    mySQLcommand.Parameters.AddWithValue("@display_price", 0);
                else
                    mySQLcommand.Parameters.AddWithValue("@display_price", decimal.Parse(txtDisplayPrice.Text));

                if (cboStatusOfItem.Text.Equals(null) || cboStatusOfItem.Text.Equals(""))
                    mySQLcommand.Parameters.AddWithValue("@status_of_item", "");
                else
                    mySQLcommand.Parameters.AddWithValue("@status_of_item", cboStatusOfItem.Text);

                //mySQLcommand.ExecuteNonQuery();
                MySqlDataReader mySQLDataReader;
                try
                {
                    mySQLDataReader = mySQLcommand.ExecuteReader();
                    MessageBox.Show("Successfully updated information");
                }
                catch (Exception)
                {
                }

                //close the connection
                myConn.Close();

            }
        }
        #endregion

        #region Delete Click
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //open the connection
                if (myConn.State != System.Data.ConnectionState.Open)
                    myConn.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            //define the command reference
            MySqlCommand mySQLcommand = new MySqlCommand();

            //define the connection used by the command object
            mySQLcommand.Connection = myConn;

            ////define the command text
            mySQLcommand.CommandText = "DELETE FROM cmc_repairs_stock WHERE date_sold=@date_sold";

            //add values provided by user

            if (txtDateSold.Text.Equals(null) || txtDateSold.Text.Equals(""))
                MessageBox.Show("This cannot be null");
            else
                mySQLcommand.Parameters.AddWithValue("@date_sold", txtDateSold.Text);

            //mySQLcommand.ExecuteNonQuery();
            MySqlDataReader mySQLDataReader;
            mySQLDataReader = mySQLcommand.ExecuteReader();

            while (mySQLDataReader.Read())
            {

            }

            //close the connection
            myConn.Close();

            MessageBox.Show("Information Deleted");

            Switcher.Switch(new Stock(myConn));
        }
        #endregion

        #region Clear Click
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new Stock(myConn));
        }
        #endregion

        #region Textbox textchanged
        private void txtDateSold_TextChanged(object sender, TextChangedEventArgs e)
        {
            int num2;
            bool num = Int32.TryParse(txtDateSold.Text.ToString(), out num2);

            if (num && !isConnOpen)
            {

                if ((!txtDateSold.Text.Equals(null) || !txtDateSold.Text.Equals("")) && (cboIDs.Items.Contains(txtDateSold.Text) || idsAndDateSold.ContainsKey(Int32.Parse(txtDateSold.Text.ToString()))))
                {
                    try
                    {
                        if (num)
                        {
                            if (idsAndDateSold.ContainsKey(Int32.Parse(txtDateSold.Text.ToString())))
                            {
                                //MessageBox.Show("Yes made it! : " + idsAndDateSold[Int32.Parse(txtDateSold.Text)]);
                                txtDateSold.Text = idsAndDateSold[Int32.Parse(txtDateSold.Text)];
                            }
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Could not convert Date Sold ID to Number - Please try again");
                    }

                    cboIDs.Text = txtDateSold.Text;
                    cboIDs_Change(this, new EventArgs());
                    SetID();
                    //MessageBox.Show(id);
                    btnUpdate.Visibility = Visibility.Visible;
                    //SaveStockForm.Visibility = Visibility.Hidden;
                    //btnDelete.Visibility = Visibility.Visible;
                    btnClear.Visibility = Visibility.Visible;
                }
                else
                {
                    btnUpdate.Visibility = Visibility.Hidden;

                    if (txtDateSold.Text.Equals(null) || txtDateSold.Text.Equals(""))
                    {
                        //SaveStockForm.Visibility = Visibility.Hidden;
                        //btnDelete.Visibility = Visibility.Hidden;
                        btnClear.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        //SaveStockForm.Visibility = Visibility.Visible;
                        //btnDelete.Visibility = Visibility.Visible;
                        btnClear.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        private void txtPriceSold_TextChanged(object sender, TextChangedEventArgs e)
        {
            int errorCounter = 0;
            errorCounter = Regex.Matches(txtPriceSold.Text, @"[a-zA-Z]").Count;
            if (errorCounter > 0)
            {
                MessageBox.Show("Price Sold cannot contain characters. Please try again");
                txtPriceSold.Text = txtPriceSold.Text.Substring(0, txtPriceSold.Text.Length - 1);
            }
        }

        private void txtBoughtPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            int errorCounter = 0;
            errorCounter = Regex.Matches(txtBoughtPrice.Text, @"[a-zA-Z]").Count;
            if (errorCounter > 0)
            {
                MessageBox.Show("Bought Price cannot contain characters. Please try again");
                txtBoughtPrice.Text = txtBoughtPrice.Text.Substring(0, txtBoughtPrice.Text.Length - 1);
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

        private void txtDisplayPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            int errorCounter = 0;
            errorCounter = Regex.Matches(txtDisplayPrice.Text, @"[a-zA-Z]").Count;
            if (errorCounter > 0)
            {
                MessageBox.Show("Display Price cannot contain characters. Please try again");
                txtDisplayPrice.Text = txtDisplayPrice.Text.Substring(0, txtDisplayPrice.Text.Length - 1);
            }
        }
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
                        (txtDateSold.Text.ToString().Equals(null) || txtDateSold.Text.ToString().Equals("")) ||
                        (txtNetwork.Text.ToString().Equals(null) || txtNetwork.Text.ToString().Equals("")) ||
                        (txtDisplayPrice.Text.ToString().Equals(null) || txtDisplayPrice.Text.ToString().Equals("")))
                    {
                        MessageBox.Show("Sale Date, Brand, Network or Display Price is empty");
                    }
                    else
                    {
                        SetID();

                        if (idsAndDateSold.ContainsValue(txtDateSold.Text))
                        {
                            btnUpdate_Click(this, new RoutedEventArgs());
                        }
                        else
                        {
                            MessageBox.Show("Barcode has printed - item not updated - please check ID");
                        }

                        string combine = txtBrand.Text;// +" " + txtModel.Text;
                        string combineAddDetailsNetwork = txtAdditionalNotice.Text + "  " + txtNetwork.Text;

                        myLabel.SetField("lblBrand", combine.ToString());
                        myLabel.SetField("lblSaleDate", txtDateSold.Text);
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

        #region Lock/Unlock Controls
        private void LockControls()
        {
            txtPriceSold.IsEnabled = false;
            cboGrade.IsEnabled = false;
            cboCheckMEND.IsEnabled = false;
            cboItem.IsEnabled = false;
            txtOtherItem.IsEnabled = false;
            txtBrand.IsEnabled = false;
            //txtModel.IsEnabled = false;
            txtIMEI.IsEnabled = false;
            txtNetwork.IsEnabled = false;
            txtOtherAcc.IsEnabled = false;
            txtSecSealNum.IsEnabled = false;
            chbBox.IsEnabled = false;
            cboWarranty.IsEnabled = false;
            chbMemCard.IsEnabled = false;
            txtAdditionalNotice.IsEnabled = false;
            cboBuyDateDays.IsEnabled = false;
            cboBuyDateMonth.IsEnabled = false;
            txtBuyDateYear.IsEnabled = false;
            txtBoughtPrice.IsEnabled = false;
            txtName.IsEnabled = false;
            txtAddress.IsEnabled = false;
            txtContactNum.IsEnabled = false;
            txtPostCode.IsEnabled = false;
            txtDisplayPrice.IsEnabled = false;
            chbCharger.IsEnabled = false;
        }


        private void UnLockControls()
        {
            txtPriceSold.IsEnabled = true;
            cboGrade.IsEnabled = true;
            cboCheckMEND.IsEnabled = true;
            cboItem.IsEnabled = true;
            txtOtherItem.IsEnabled = true;
            txtBrand.IsEnabled = true;
            //txtModel.IsEnabled = true;
            txtIMEI.IsEnabled = true;
            txtNetwork.IsEnabled = true;
            txtOtherAcc.IsEnabled = true;
            txtSecSealNum.IsEnabled = true;
            chbBox.IsEnabled = true;
            cboWarranty.IsEnabled = true;
            chbMemCard.IsEnabled = true;
            txtAdditionalNotice.IsEnabled = true;
            cboBuyDateDays.IsEnabled = true;
            cboBuyDateMonth.IsEnabled = true;
            txtBuyDateYear.IsEnabled = true;
            txtBoughtPrice.IsEnabled = true;
            txtName.IsEnabled = true;
            txtAddress.IsEnabled = true;
            txtContactNum.IsEnabled = true;
            txtPostCode.IsEnabled = true;
            txtDisplayPrice.IsEnabled = true;
            chbCharger.IsEnabled = true;
        }
        #endregion

        #region Lock/Unlock Buttons

        private void LockUpdateButton()
        {
            btnUpdate.IsEnabled = false;
        }

        private void UnlockUpdateButton()
        {
            btnUpdate.IsEnabled = true;
        }

        private void LockDeleteButton()
        {
            btnDelete.IsEnabled = false;
        }

        private void UnlockDeleteButton()
        {
            btnDelete.IsEnabled = true;
        }

        private void LockClearButton()
        {
            btnClear.IsEnabled = false;
        }

        private void UnlockClearButton()
        {
            btnClear.IsEnabled = true;
        }

        private void LockBarcodeButton()
        {
            btnBarcode.IsEnabled = false;
        }

        private void UnlockBarcodeButton()
        {
            btnBarcode.IsEnabled = true;
        }

        private void LockSoldButton()
        {
            btnSold.IsEnabled = false;
        }

        private void UnlockSoldButton()
        {
            btnSold.IsEnabled = true;
        }

        #endregion

        #region Sold Button Click
        private void btnSold_Click(object sender, RoutedEventArgs e)
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
                mySQLcommand.CommandText = "UPDATE cmc_repairs_stock SET now_sold=@now_sold, date_sold=@date_sold, price_sold=@price_sold, grade=@grade, checkmend=@checkmend, item=@item, other_item=@other_item, " +
                    "brand=@brand, imei=@imei, network=@network, charger=@charger, other_acc=@other_acc, sec_seal_num=@sec_seal_num, box=@box, warranty=@warranty, " +
                    "mem_card=@mem_card, additional_notice=@addtional_notice, buy_date=@buy_date, bought_price=@bought_price, name=@name, address=@address, contact_num=@contact_num, post_code=@post_code, display_price=@display_price, status_of_item=@status_of_item " +
                    "WHERE date_sold=@date_sold";

                //add values provided by user
                if (txtDateSold.Text.Equals(null) || txtDateSold.Text.Equals(""))
                {
                    MessageBox.Show("ID cannot be blank");
                    return;
                }
                else
                    mySQLcommand.Parameters.AddWithValue("@date_sold", txtDateSold.Text);

                if ((txtName.Text.Equals(null) || txtName.Text.Equals("")) || (txtContactNum.Text.Equals(null) || txtContactNum.Text.Equals("")))
                {
                    System.Windows.Forms.DialogResult answer = MessageBox.Show("Customer name and contact name is blank, Are you sure you want to mark this item as sold?", "", System.Windows.Forms.MessageBoxButtons.YesNo);

                    if (answer == System.Windows.Forms.DialogResult.No)
                        return;
                }

                chbNowSold.IsChecked = true;
                mySQLcommand.Parameters.AddWithValue("@now_sold", true);

                if (txtPriceSold.Text.Equals(null) || txtPriceSold.Text.Equals(""))
                    mySQLcommand.Parameters.AddWithValue("@price_sold", 0);
                else
                    mySQLcommand.Parameters.AddWithValue("@price_sold", decimal.Parse(txtPriceSold.Text));

                if (cboGrade.Text.Equals(null) || cboGrade.Text.Equals(""))
                    mySQLcommand.Parameters.AddWithValue("@grade", "");
                else
                    mySQLcommand.Parameters.AddWithValue("@grade", cboGrade.Text);

                if (cboCheckMEND.Text.Equals(null) || cboCheckMEND.Text.Equals(""))
                    mySQLcommand.Parameters.AddWithValue("@checkmend", "");
                else
                    mySQLcommand.Parameters.AddWithValue("@checkmend", cboCheckMEND.Text);

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

                if ((cboBuyDateDays.Text.Equals(null) || cboBuyDateDays.Text.Equals("")) ||
                        (cboBuyDateMonth.Text.Equals(null) || cboBuyDateMonth.Text.Equals("")) ||
                        (txtBuyDateYear.Text.Equals(null) || txtBuyDateYear.Text.Equals("")))
                    mySQLcommand.Parameters.AddWithValue("@buy_date", "");
                else
                    mySQLcommand.Parameters.AddWithValue("@buy_date", MergeDate());

                if (txtBoughtPrice.Text.Equals(null) || txtBoughtPrice.Text.Equals(""))
                    mySQLcommand.Parameters.AddWithValue("@bought_price", 0);
                else
                    mySQLcommand.Parameters.AddWithValue("@bought_price", decimal.Parse(txtBoughtPrice.Text));

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

                if (txtDisplayPrice.Text.Equals(null) || txtDisplayPrice.Text.Equals(""))
                    mySQLcommand.Parameters.AddWithValue("@display_price", 0);
                else
                    mySQLcommand.Parameters.AddWithValue("@display_price", decimal.Parse(txtDisplayPrice.Text));

                if (cboStatusOfItem.Text.Equals(null) || cboStatusOfItem.Text.Equals(""))
                    mySQLcommand.Parameters.AddWithValue("@status_of_item", "");
                else
                    mySQLcommand.Parameters.AddWithValue("@status_of_item", cboStatusOfItem.Text);

                //mySQLcommand.ExecuteNonQuery();
                MySqlDataReader mySQLDataReader;
                try
                {
                    mySQLDataReader = mySQLcommand.ExecuteReader();
                    MessageBox.Show("Successfully updated information");
                }
                catch (Exception)
                {
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            //close the connection
            myConn.Close();
        }
        #endregion

        #region NowSold Checked
        private void chbNowSold_Checked(object sender, RoutedEventArgs e)
        {
            if (chbNowSold.IsChecked == true)
            {
                LockControls();
                LockBarcodeButton();
                LockDeleteButton();
                LockSoldButton();
                LockUpdateButton();
            }
            else
            {
                UnLockControls();
                UnlockBarcodeButton();
                UnlockDeleteButton();
                UnlockSoldButton();
                UnlockUpdateButton();
            }
        }
        #endregion
    }
}