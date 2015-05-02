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
using CMCrepairs.Barcode;
using DYMO.Label.Framework;
using Label = DYMO.Label.Framework.Com.Label;
using Dymo;
#endregion

namespace CMCrepairs
{
    /// <summary>
    /// Interaction logic for Repairs.xaml
    /// </summary>
    public partial class Repairs : UserControl, ISwitchable
    {
        DymoLabels myLabel;
        DymoAddIn myDymoAddin;
        bool isConnOpen = false;

        int[] days = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31 };
        int[] months = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

        //MySqlConnection myConn = new MySqlConnection("server=localhost; user id=root;password=CMCsales;database=test;persist security info=false");
        MySqlConnection myConn = new MySqlConnection("server=localhost; user id=root;password=root;database=test;persist security info=false");

        string id;

        #region Constructors

        public Repairs()
        {
            InitializeComponent();
            FillCombo();
            DateTime now = DateTime.Now;
            int year = now.Year;
            string u = year.ToString().Remove(0, 2);
            txtDatetimeID.Text = u + now.Month.ToString("00") + now.Day.ToString("00") + now.Hour.ToString("00") + now.Minute.ToString("00") + now.Second.ToString("00");

            //UpdateRepairButton.Visibility = Visibility.Hidden;
            if (txtDatetimeID.Text.Length > 0)
            {
                SaveRepairsForm.Visibility = Visibility.Visible;
            }
            else
            {
                SaveRepairsForm.Visibility = Visibility.Hidden;
            }
            //DeleteRepairButton.Visibility = Visibility.Hidden;
            //ClearRepairButton.Visibility = Visibility.Hidden;
        }

        public Repairs(string id)
        {
            InitializeComponent();
            FillCombo();
            cboIDs.SelectedValue = id;
            cboIDs_Change(cboIDs, new EventArgs());
            SaveRepairsForm.Visibility = Visibility.Hidden;
        }

        #endregion

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
            mySQLcommand.CommandText = "SELECT * FROM cmc_repairs_repair WHERE datetime_id=@datetime_id";

            DateTime mydate = DateTime.Now;

            //add values provided by user

            if (txtDatetimeID.Text.Equals(null) || txtDatetimeID.Text.Equals(""))
                MessageBox.Show("This cannot be null");
            else
                mySQLcommand.Parameters.AddWithValue("@datetime_id", txtDatetimeID.Text);

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

        #region fill combo box
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
                mySQLcommand.CommandText = "SELECT * FROM cmc_repairs_repair ORDER BY datetime_id DESC ;";

                MySqlDataReader mySQLDataReader;
                mySQLDataReader = mySQLcommand.ExecuteReader();

                while (mySQLDataReader.Read())
                {
                    string date_sold = mySQLDataReader.GetString("datetime_id");
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
            mySQLcommand.CommandText = "SELECT * FROM cmc_repairs_repair WHERE datetime_id='" + cboIDs.Text + "' ;";
            //MessageBox.Show(cboIDs.Text);
            MySqlDataReader mySQLDataReader;

            try
            {
                //open the connection
                if (myConn.State != System.Data.ConnectionState.Open)
                    myConn.Open();
                mySQLDataReader = mySQLcommand.ExecuteReader();

                while (mySQLDataReader.Read())
                {
                    //int datetime_idNull = mySQLDataReader.GetOrdinal("datetime_id");
                    int customer_nameNull = mySQLDataReader.GetOrdinal("customer_name");
                    int contact_numNull = mySQLDataReader.GetOrdinal("contact_num");
                    int itemNull = mySQLDataReader.GetOrdinal("item");
                    int detailsNull = mySQLDataReader.GetOrdinal("details");
                    int passwordNull = mySQLDataReader.GetOrdinal("password");
                    int po_dateNull = mySQLDataReader.GetOrdinal("po_date");
                    int imeiNull = mySQLDataReader.GetOrdinal("imei");
                    int po_idNull = mySQLDataReader.GetOrdinal("po_id");
                    int other_accNull = mySQLDataReader.GetOrdinal("other_acc");
                    int issues_descriptionNull = mySQLDataReader.GetOrdinal("issues_description");
                    int backup_specifyNull = mySQLDataReader.GetOrdinal("backup_specify");
                    int quote_priceNull = mySQLDataReader.GetOrdinal("quote_price");
                    int quote_dateNull = mySQLDataReader.GetOrdinal("quote_date");
                    int fault_tested_collNull = mySQLDataReader.GetOrdinal("fault_tested_coll");
                    int paid_dateNull = mySQLDataReader.GetOrdinal("paid_date");
                    int paidNull = mySQLDataReader.GetOrdinal("paid");

                    string datetime_id = mySQLDataReader.GetString("datetime_id");
                    string customer_name = mySQLDataReader.IsDBNull(customer_nameNull) ? null : mySQLDataReader.GetString("customer_name");
                    string contact_num = mySQLDataReader.IsDBNull(contact_numNull) ? null : mySQLDataReader.GetString("contact_num");
                    string item = mySQLDataReader.IsDBNull(itemNull) ? null : mySQLDataReader.GetString("item");
                    string details = mySQLDataReader.IsDBNull(detailsNull) ? null : mySQLDataReader.GetString("details");
                    string password = mySQLDataReader.IsDBNull(passwordNull) ? null : mySQLDataReader.GetString("password");
                    string po_date = mySQLDataReader.IsDBNull(po_dateNull) ? null : mySQLDataReader.GetString("po_date");
                    string imei = mySQLDataReader.IsDBNull(imeiNull) ? null : mySQLDataReader.GetString("imei");
                    string po_id = mySQLDataReader.IsDBNull(po_idNull) ? null : mySQLDataReader.GetString("po_id");
                    string other_acc = mySQLDataReader.IsDBNull(other_accNull) ? null : mySQLDataReader.GetString("other_acc");
                    string issues_description = mySQLDataReader.IsDBNull(issues_descriptionNull) ? null : mySQLDataReader.GetString("issues_description");
                    string backup_specify = mySQLDataReader.IsDBNull(backup_specifyNull) ? null : mySQLDataReader.GetString("backup_specify");
                    string quote_price = mySQLDataReader.IsDBNull(quote_priceNull) ? null : mySQLDataReader.GetDecimal("quote_price").ToString();
                    string quote_date = mySQLDataReader.IsDBNull(quote_dateNull) ? null : mySQLDataReader.GetString("quote_date");
                    string fault_tested_coll = mySQLDataReader.IsDBNull(fault_tested_collNull) ? null : mySQLDataReader.GetString("fault_tested_coll");
                    string paid_date = mySQLDataReader.IsDBNull(paid_dateNull) ? null : mySQLDataReader.GetString("paid_date");
                    string paid = mySQLDataReader.IsDBNull(paidNull) ? null : mySQLDataReader.GetDecimal("paid").ToString();

                    string completed = mySQLDataReader.GetInt32("completed").ToString();
                    string item_with_customer = mySQLDataReader.GetInt32("item_with_customer").ToString();
                    string rwpa = mySQLDataReader.GetInt32("rwpa").ToString();
                    string pa = mySQLDataReader.GetInt32("pa").ToString();
                    string pa_inf = mySQLDataReader.GetInt32("pa_inf").ToString();
                    string rtc = mySQLDataReader.GetInt32("rtc").ToString();
                    string rtc_inf = mySQLDataReader.GetInt32("rtc_inf").ToString();
                    string charger = mySQLDataReader.GetInt32("charger").ToString();
                    string bag = mySQLDataReader.GetInt32("bag").ToString();
                    string sim = mySQLDataReader.GetInt32("sim").ToString();
                    string mem_card = mySQLDataReader.GetInt32("mem_card").ToString();

                    if (completed.Equals("1"))
                    {
                        chbCompleted.IsChecked = true;
                    }
                    else
                    {
                        chbCompleted.IsChecked = false;
                    }

                    if (item_with_customer.Equals("1"))
                    {
                        chbItemCust.IsChecked = true;
                    }
                    else
                    {
                        chbItemCust.IsChecked = false;
                    }

                    if (rwpa.Equals("1"))
                    {
                        chbRWPA.IsChecked = true;
                    }
                    else
                    {
                        chbRWPA.IsChecked = false;
                    }

                    if (pa.Equals("1"))
                    {
                        chbPA.IsChecked = true;
                    }
                    else
                    {
                        chbPA.IsChecked = false;
                    }

                    if (pa_inf.Equals("1"))
                    {
                        chbPAinf.IsChecked = true;
                    }
                    else
                    {
                        chbPAinf.IsChecked = false;
                    }

                    if (rtc.Equals("1"))
                    {
                        chbRTC.IsChecked = true;
                    }
                    else
                    {
                        chbRTC.IsChecked = false;
                    }

                    if (rtc_inf.Equals("1"))
                    {
                        chbRTCinf.IsChecked = true;
                    }
                    else
                    {
                        chbRTCinf.IsChecked = false;
                    }

                    if (charger.Equals("1"))
                    {
                        chbCharger.IsChecked = true;
                    }
                    else
                    {
                        chbCharger.IsChecked = false;
                    }

                    if (bag.Equals("1"))
                    {
                        chbBag.IsChecked = true;
                    }
                    else
                    {
                        chbBag.IsChecked = false;
                    }

                    if (sim.Equals("1"))
                    {
                        chbSIM.IsChecked = true;
                    }
                    else
                    {
                        chbSIM.IsChecked = false;
                    }

                    if (mem_card.Equals("1"))
                    {
                        chbMemCard.IsChecked = true;
                    }
                    else
                    {
                        chbMemCard.IsChecked = false;
                    }
                    isConnOpen = true;
                    txtDatetimeID.Text = datetime_id;
                    isConnOpen = false;
                    txtCustName.Text = customer_name;
                    txtContactNum.Text = contact_num;
                    txtDetails.Text = details;
                    txtPassword.Text = password;
                    txtPOdate.Text = po_date;
                    txtIMEI.Text = imei;
                    txtPOid.Text = po_id;
                    txtOtherAcc.Text = other_acc;
                    txtIssuesDesc.Text = issues_description;
                    txtBackupDetails.Text = backup_specify;
                    txtQuote.Text = quote_price;
                    txtDateQuoted.Text = quote_date;
                    txtFaultTestedColl.Text = fault_tested_coll;
                    txtPaidDate.Text = paid_date;
                    txtPaid.Text = paid;
                    cboItem.Text = item;
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

        #region Exit/Back Button
        private void ExitRepairButton_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new MainWindow());
        }
        #endregion

        #region Save Button
        private void SaveRepairsForm_Click(object sender, RoutedEventArgs e)
        {
            //DateTimePicker dtp = this.wfhDateTimePickerWrapper.Child as DateTimePicker;
            //DateTime now = DateTime.Now;
            //String theDate = "" + now.Year + now.Month + now.Day + now.Hour + now.Minute + now.Second;
            //MessageBox.Show("datetime is " + theDate);
            //MessageBox.Show("" + dtp.Value.ToShortDateString());

            //open the connection
            if (myConn.State != System.Data.ConnectionState.Open)
                myConn.Open();

            //define the command reference
            MySqlCommand mySQLcommand = new MySqlCommand();

            //define the connection used by the command object
            mySQLcommand.Connection = myConn;

            //define the command text
            mySQLcommand.CommandText = "INSERT INTO cmc_repairs_repair(datetime_id, completed, customer_name, contact_num, item, item_with_customer, " +
                "rwpa, details, charger, password, po_date, bag, imei, po_id, sim, pa, pa_inf, mem_card, rtc, rtc_inf, other_acc, issues_description, " +
                "backup_specify, quote_price, quote_date, fault_tested_coll, paid_date, paid)" +
                "values(@datetime_id, @completed, @customer_name, @contact_num, @item, @item_with_customer, " +
                "@rwpa, @details, @charger, @password, @po_date, @bag, @imei, @po_id, @sim, @pa, @pa_inf, @mem_card, @rtc, @rtc_inf, @other_acc, " +
                "@issues_description, @backup_specify, @quote_price, @quote_date, @fault_tested_coll, @paid_date, @paid)";

            //add values provided by user
            mySQLcommand.Parameters.AddWithValue("@datetime_id", txtDatetimeID.Text);
            mySQLcommand.Parameters.AddWithValue("@completed", chbCompleted.IsChecked.Value);

            if (txtCustName.Text.Equals(null) || txtCustName.Text.Equals(""))
                mySQLcommand.Parameters.AddWithValue("@customer_name", "");
            else
                mySQLcommand.Parameters.AddWithValue("@customer_name", txtCustName.Text);

            if (txtContactNum.Text.Equals(null) || txtContactNum.Text.Equals(""))
                mySQLcommand.Parameters.AddWithValue("@contact_num", 0);
            else
                mySQLcommand.Parameters.AddWithValue("@contact_num", Int32.Parse(txtContactNum.Text));

            if (cboItem.Text.Equals(null) || cboItem.Text.Equals(""))
                mySQLcommand.Parameters.AddWithValue("@item", "");
            else
                mySQLcommand.Parameters.AddWithValue("@item", cboItem.Text);

            mySQLcommand.Parameters.AddWithValue("@item_with_customer", chbItemCust.IsChecked.Value);
            mySQLcommand.Parameters.AddWithValue("@rwpa", chbRWPA.IsChecked.Value);

            if (txtDetails.Text.Equals(null) || txtDetails.Text.Equals(""))
                mySQLcommand.Parameters.AddWithValue("@details", "");
            else
                mySQLcommand.Parameters.AddWithValue("@details", txtDetails.Text);

            mySQLcommand.Parameters.AddWithValue("@charger", chbCharger.IsChecked.Value);

            if (txtPassword.Text.Equals(null) || txtPassword.Text.Equals(""))
                mySQLcommand.Parameters.AddWithValue("@password", "");
            else
                mySQLcommand.Parameters.AddWithValue("@password", txtPassword.Text);

            if (txtPOdate.Text.Equals(null) || txtPOdate.Text.Equals(""))
                mySQLcommand.Parameters.AddWithValue("@po_date", "");
            else
                mySQLcommand.Parameters.AddWithValue("@po_date", txtPOdate.Text);

            mySQLcommand.Parameters.AddWithValue("@bag", chbBag.IsChecked.Value);

            if (txtIMEI.Text.Equals(null) || txtIMEI.Text.Equals(""))
                mySQLcommand.Parameters.AddWithValue("@imei", "");
            else
                mySQLcommand.Parameters.AddWithValue("@imei", txtIMEI.Text);

            if (txtPOid.Text.Equals(null) || txtPOid.Text.Equals(""))
                mySQLcommand.Parameters.AddWithValue("@po_id", "");
            else
                mySQLcommand.Parameters.AddWithValue("@po_id", txtPOid.Text);

            mySQLcommand.Parameters.AddWithValue("@sim", chbSIM.IsChecked.Value);
            mySQLcommand.Parameters.AddWithValue("@pa", chbPA.IsChecked.Value);
            mySQLcommand.Parameters.AddWithValue("@pa_inf", chbPAinf.IsChecked.Value);
            mySQLcommand.Parameters.AddWithValue("@mem_card", chbMemCard.IsChecked.Value);
            mySQLcommand.Parameters.AddWithValue("@rtc", chbRTC.IsChecked.Value);
            mySQLcommand.Parameters.AddWithValue("@rtc_inf", chbRTCinf.IsChecked.Value);

            if (txtOtherAcc.Text.Equals(null) || txtOtherAcc.Text.Equals(""))
                mySQLcommand.Parameters.AddWithValue("@other_acc", "");
            else
                mySQLcommand.Parameters.AddWithValue("@other_acc", txtOtherAcc.Text);

            if (txtIssuesDesc.Text.Equals(null) || txtIssuesDesc.Text.Equals(""))
                mySQLcommand.Parameters.AddWithValue("@issues_description", "");
            else
                mySQLcommand.Parameters.AddWithValue("@issues_description", txtIssuesDesc.Text);

            if (txtBackupDetails.Text.Equals(null) || txtBackupDetails.Text.Equals(""))
                mySQLcommand.Parameters.AddWithValue("@backup_specify", "");
            else
                mySQLcommand.Parameters.AddWithValue("@backup_specify", txtBackupDetails.Text);

            if (txtQuote.Text.Equals(null) || txtQuote.Text.Equals(""))
                mySQLcommand.Parameters.AddWithValue("@quote_price", 0);
            else
                mySQLcommand.Parameters.AddWithValue("@quote_price", Double.Parse(txtQuote.Text));

            if (txtDateQuoted.Text.Equals(null) || txtDateQuoted.Text.Equals(""))
                mySQLcommand.Parameters.AddWithValue("@quote_date", "");
            else
                mySQLcommand.Parameters.AddWithValue("@quote_date", txtDateQuoted.Text);

            if (txtFaultTestedColl.Text.Equals(null) || txtFaultTestedColl.Text.Equals(""))
                mySQLcommand.Parameters.AddWithValue("@fault_tested_coll", "");
            else
                mySQLcommand.Parameters.AddWithValue("@fault_tested_coll", txtFaultTestedColl.Text);

            if (txtPaidDate.Text.Equals(null) || txtPaidDate.Text.Equals(""))
                mySQLcommand.Parameters.AddWithValue("@paid_date", "");
            else
                mySQLcommand.Parameters.AddWithValue("@paid_date", txtPaidDate.Text);

            if (txtPaid.Text.Equals(null) || txtPaid.Text.Equals(""))
                mySQLcommand.Parameters.AddWithValue("@paid", 0);
            else
                mySQLcommand.Parameters.AddWithValue("@paid", Double.Parse(txtPaid.Text));

            mySQLcommand.ExecuteNonQuery();
            //close the connection
            myConn.Close();

            //empty the textboxes
            txtDatetimeID.Text = null;
            chbCompleted.IsChecked = false;
            txtCustName.Text = null;
            txtContactNum.Text = null;
            cboItem.SelectedIndex = -1;
            chbItemCust.IsChecked = false;
            chbRWPA.IsChecked = false;
            txtDetails.Text = null;
            chbCharger.IsChecked = false;
            txtPassword.Text = null;
            txtPOdate.Text = null;
            chbBag.IsChecked = false;
            txtIMEI.Text = null;
            txtPOid.Text = null;
            chbSIM.IsChecked = false;
            chbPA.IsChecked = false;
            chbPAinf.IsChecked = false;
            chbMemCard.IsChecked = false;
            chbRTC.IsChecked = false;
            chbRTCinf.IsChecked = false;
            txtOtherAcc.Text = null;
            txtIssuesDesc.Text = null;
            txtBackupDetails.Text = null;
            txtQuote.Text = null;
            txtDateQuoted.Text = null;
            txtFaultTestedColl.Text = null;
            txtPaidDate.Text = null;
            txtPaid.Text = null;

            FillCombo();

            MessageBox.Show("Information Saved");
        }
        #endregion

        #region ISwitchable Members
        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Search Button
        private void SearchRepairButton_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new SearchID("cmc_repairs_repair"));
        }
        #endregion

        #region Clear Button
        private void ClearRepairButton_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new Repairs());
        }
        #endregion

        #region Delete Button
        private void DeleteRepairButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.DialogResult answer = MessageBox.Show("Are you sure?", "", System.Windows.Forms.MessageBoxButtons.YesNo);

            if (answer == System.Windows.Forms.DialogResult.Yes)
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

                    ////define the command text
                    mySQLcommand.CommandText = "DELETE FROM cmc_repairs_repair WHERE datetime_id=@datetime_id";

                    //DateTime mydate = DateTime.Now;

                    //values provided by user

                    if (txtDatetimeID.Text.Equals(null) || txtDatetimeID.Text.Equals(""))
                        MessageBox.Show("This cannot be null");
                    else
                        mySQLcommand.Parameters.AddWithValue("@datetime_id", txtDatetimeID.Text);

                    //mySQLcommand.ExecuteNonQuery();
                    MySqlDataReader mySQLDataReader;
                    mySQLDataReader = mySQLcommand.ExecuteReader();

                    while (mySQLDataReader.Read())
                    {

                    }

                    //close the connection
                    myConn.Close();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                MessageBox.Show("Record Deleted");

                Switcher.Switch(new Repairs());
            }
        }
        #endregion

        #region Update Button
        private void UpdateRepairButton_Click(object sender, RoutedEventArgs e)
        {
            if (Validation())
            {
                MessageBox.Show("Updating this record in the database");

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
                    mySQLcommand.CommandText = "UPDATE cmc_repairs_repair SET datetime_id=@datetime_id, completed=@completed, customer_name=@customer_name, contact_num=@contact_num, item=@item, item_with_customer=@item_with_customer, " +
                        "rwpa=@rwpa, details=@details, charger=@charger, password=@password, po_date=@po_date, bag=@bag, imei=@imei, po_id=@po_id, sim=@sim, pa=@pa, pa_inf=@pa_inf, mem_card=@mem_card, rtc=@rtc, rtc_inf=@rtc_inf, other_acc=@other_acc, " +
                        "issues_description=@issues_description, backup_specify=@backup_specify, quote_price=@quote_price, quote_date=@quote_date, fault_tested_coll=@fault_tested_coll, paid_date=@paid_date, paid=@paid " +
                        "WHERE datetime_id=@datetime_id";

                    //add values provided by user
                    mySQLcommand.Parameters.AddWithValue("@datetime_id", txtDatetimeID.Text);
                    mySQLcommand.Parameters.AddWithValue("@completed", chbCompleted.IsChecked.Value);

                    if (txtCustName.Text.Equals(null) || txtCustName.Text.Equals(""))
                        mySQLcommand.Parameters.AddWithValue("@customer_name", "");
                    else
                        mySQLcommand.Parameters.AddWithValue("@customer_name", txtCustName.Text);

                    if (txtContactNum.Text.Equals(null) || txtContactNum.Text.Equals(""))
                        mySQLcommand.Parameters.AddWithValue("@contact_num", "");
                    else
                        mySQLcommand.Parameters.AddWithValue("@contact_num", txtContactNum.Text);

                    if (cboItem.Text.Equals(null) || cboItem.Text.Equals(""))
                        mySQLcommand.Parameters.AddWithValue("@item", "");
                    else
                        mySQLcommand.Parameters.AddWithValue("@item", cboItem.Text);

                    mySQLcommand.Parameters.AddWithValue("@item_with_customer", chbItemCust.IsChecked.Value);
                    mySQLcommand.Parameters.AddWithValue("@rwpa", chbRWPA.IsChecked.Value);

                    if (txtDetails.Text.Equals(null) || txtDetails.Text.Equals(""))
                        mySQLcommand.Parameters.AddWithValue("@details", "");
                    else
                        mySQLcommand.Parameters.AddWithValue("@details", txtDetails.Text);

                    mySQLcommand.Parameters.AddWithValue("@charger", chbCharger.IsChecked.Value);

                    if (txtPassword.Text.Equals(null) || txtPassword.Text.Equals(""))
                        mySQLcommand.Parameters.AddWithValue("@password", "");
                    else
                        mySQLcommand.Parameters.AddWithValue("@password", txtPassword.Text);

                    if (txtPOdate.Text.Equals(null) || txtPOdate.Text.Equals(""))
                        mySQLcommand.Parameters.AddWithValue("@po_date", "");
                    else
                        mySQLcommand.Parameters.AddWithValue("@po_date", txtPOdate.Text);

                    mySQLcommand.Parameters.AddWithValue("@bag", chbBag.IsChecked.Value);

                    if (txtIMEI.Text.Equals(null) || txtIMEI.Text.Equals(""))
                        mySQLcommand.Parameters.AddWithValue("@imei", "");
                    else
                        mySQLcommand.Parameters.AddWithValue("@imei", txtIMEI.Text);

                    if (txtPOid.Text.Equals(null) || txtPOid.Text.Equals(""))
                        mySQLcommand.Parameters.AddWithValue("@po_id", "");
                    else
                        mySQLcommand.Parameters.AddWithValue("@po_id", txtPOid.Text);

                    mySQLcommand.Parameters.AddWithValue("@sim", chbSIM.IsChecked.Value);
                    mySQLcommand.Parameters.AddWithValue("@pa", chbPA.IsChecked.Value);
                    mySQLcommand.Parameters.AddWithValue("@pa_inf", chbPAinf.IsChecked.Value);
                    mySQLcommand.Parameters.AddWithValue("@mem_card", chbMemCard.IsChecked.Value);
                    mySQLcommand.Parameters.AddWithValue("@rtc", chbRTC.IsChecked.Value);
                    mySQLcommand.Parameters.AddWithValue("@rtc_inf", chbRTCinf.IsChecked.Value);

                    if (txtOtherAcc.Text.Equals(null) || txtOtherAcc.Text.Equals(""))
                        mySQLcommand.Parameters.AddWithValue("@other_acc", "");
                    else
                        mySQLcommand.Parameters.AddWithValue("@other_acc", txtOtherAcc.Text);

                    if (txtIssuesDesc.Text.Equals(null) || txtIssuesDesc.Text.Equals(""))
                        mySQLcommand.Parameters.AddWithValue("@issues_description", "");
                    else
                        mySQLcommand.Parameters.AddWithValue("@issues_description", txtIssuesDesc.Text);

                    if (txtBackupDetails.Text.Equals(null) || txtBackupDetails.Text.Equals(""))
                        mySQLcommand.Parameters.AddWithValue("@backup_specify", "");
                    else
                        mySQLcommand.Parameters.AddWithValue("@backup_specify", txtBackupDetails.Text);

                    if (txtQuote.Text.Equals(null) || txtQuote.Text.Equals(""))
                        mySQLcommand.Parameters.AddWithValue("@quote_price", 0);
                    else
                        mySQLcommand.Parameters.AddWithValue("@quote_price", Double.Parse(txtQuote.Text));

                    if (txtDateQuoted.Text.Equals(null) || txtDateQuoted.Text.Equals(""))
                        mySQLcommand.Parameters.AddWithValue("@quote_date", "");
                    else
                        mySQLcommand.Parameters.AddWithValue("@quote_date", txtDateQuoted.Text);

                    if (txtFaultTestedColl.Text.Equals(null) || txtFaultTestedColl.Text.Equals(""))
                        mySQLcommand.Parameters.AddWithValue("@fault_tested_coll", "");
                    else
                        mySQLcommand.Parameters.AddWithValue("@fault_tested_coll", txtFaultTestedColl.Text);

                    if (txtPaidDate.Text.Equals(null) || txtPaidDate.Text.Equals(""))
                        mySQLcommand.Parameters.AddWithValue("@paid_date", "");
                    else
                        mySQLcommand.Parameters.AddWithValue("@paid_date", txtPaidDate.Text);

                    if (txtPaid.Text.Equals(null) || txtPaid.Text.Equals(""))
                        mySQLcommand.Parameters.AddWithValue("@paid", 0);
                    else
                        mySQLcommand.Parameters.AddWithValue("@paid", Double.Parse(txtPaid.Text));

                    //mySQLcommand.ExecuteNonQuery();
                    MySqlDataReader mySQLDataReader;
                    mySQLDataReader = mySQLcommand.ExecuteReader();

                    while (mySQLDataReader.Read())
                    {

                    }

                    //close the connection
                    myConn.Close();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                MessageBox.Show("Information Updated");
            }
        }
        #endregion

        #region Basic Validation
        private bool Validation()
        {
            bool val = false;
            int errorCounter = 0;
            errorCounter = Regex.Matches(txtDatetimeID.Text, @"[a-zA-Z]").Count;
            if (txtDatetimeID.Text.Equals(null) || txtDatetimeID.Text.Equals("") || errorCounter > 0)
            {
                MessageBox.Show("ID cannot be blank or contain characters. Please try again");
            }
            else
                errorCounter = Regex.Matches(txtContactNum.Text, @"[a-zA-Z]").Count + Regex.Matches(txtQuote.Text, @"[a-zA-Z]").Count + Regex.Matches(txtPaid.Text, @"[a-zA-Z]").Count;

            if (errorCounter > 0)
                MessageBox.Show("Price Sold/Bought Price/Contact Number cannot contain characters. Please try again");
            else
                val = true;
            return val;
        }
        #endregion

        #region Textbox textchanged
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

        private void txtQuote_TextChanged(object sender, TextChangedEventArgs e)
        {
            int errorCounter = 0;
            errorCounter = Regex.Matches(txtQuote.Text, @"[a-zA-Z]").Count;
            if (errorCounter > 0)
            {
                MessageBox.Show("Quote Price cannot contain characters. Please try again");
                txtQuote.Text = txtQuote.Text.Substring(0, txtQuote.Text.Length - 1);
            }
        }

        private void txtPaid_TextChanged(object sender, TextChangedEventArgs e)
        {
            int errorCounter = 0;
            errorCounter = Regex.Matches(txtPaid.Text, @"[a-zA-Z]").Count;
            if (errorCounter > 0)
            {
                MessageBox.Show("Paid Price cannot contain characters. Please try again");
                txtPaid.Text = txtPaid.Text.Substring(0, txtPaid.Text.Length - 1);
            }
        }

        private void txtDatetimeID_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!isConnOpen)
            {
                if (cboIDs.Items.Contains(txtDatetimeID.Text))
                {
                    cboIDs.Text = txtDatetimeID.Text;
                    cboIDs_Change(this, new EventArgs());
                    UpdateRepairButton.Visibility = Visibility.Visible;
                    SaveRepairsForm.Visibility = Visibility.Hidden;
                    DeleteRepairButton.Visibility = Visibility.Visible;
                    ClearRepairButton.Visibility = Visibility.Visible;
                }
                else
                {
                    UpdateRepairButton.Visibility = Visibility.Hidden;

                    if (txtDatetimeID.Text.Equals(null) || txtDatetimeID.Text.Equals(""))
                    {
                        SaveRepairsForm.Visibility = Visibility.Hidden;
                        DeleteRepairButton.Visibility = Visibility.Hidden;
                        ClearRepairButton.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        SaveRepairsForm.Visibility = Visibility.Visible;
                        DeleteRepairButton.Visibility = Visibility.Visible;
                        ClearRepairButton.Visibility = Visibility.Visible;
                    }
                }
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
                if (myDymoAddin.Open(@"C:\Labels\Repair.label"))
                {
                    if ((txtDatetimeID.Text.ToString().Equals(null) || txtDatetimeID.Text.ToString().Equals("")) ||
                        (txtDateQuoted.Text.ToString().Equals(null) || txtDateQuoted.Text.ToString().Equals("")))
                    {
                        MessageBox.Show("ID or the quoted date is empty");
                    }
                    else
                    {
                        SetID();

                        MessageBox.Show("Remember to save/update!");

                        //myLabel.SetField("lblItem", txtDatetimeID.Text);
                        //myLabel.SetField("lblDate", txtDateQuoted.Text);
                        //myLabel.SetField("lblContactName", txtCustName.Text);
                        //myLabel.SetField("lblItemDetails", txtIssuesDesc.Text);
                        //myLabel.SetField("lblContactNum", txtContactNum.Text);
                        //myLabel.SetField("lbl_ID", id.ToString());
                        //myLabel.SetField("lblDateTime", txtDatetimeID.Text);
                        //myDymoAddin.StartPrintJob();
                        //myDymoAddin.Print(1, false);
                        //myDymoAddin.EndPrintJob();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("There is an error with the label, please check all fields and try again.");
            }
        }
        #endregion
    }
}