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
        //public DymoAddInClass DymoAddIn;
        //public DymoLabelsClass DymoLabels;
        //private ILabel _label;
        DymoLabels myLabel;
        DymoAddIn myDymoAddin;
        bool isConnOpen = false;
        //Dictionary<int, string> idsAndDateSold = new Dictionary<int, string>();
        //MySqlConnection myConn = new MySqlConnection("server=localhost; user id=root;password=CMCsales;database=test;persist security info=false");
        MySqlConnection myConn = new MySqlConnection("server=localhost; user id=root;password=root;database=test;persist security info=false");
        //MySqlConnection myConn = new MySqlConnection("server=sql3.freesqldatabase.com;uid=sql370941;pwd=fU9*jL4%;database=sql370941;persist security info=false");
        string id;

        //private System.Windows.Forms.OpenFileDialog openFileDialog1;
        //OpenFileDialog openFileDialog1 = new OpenFileDialog();
        
        #region Constructors

        public Repairs()
        {
            InitializeComponent();
            //Load();
            FillCombo();
            DateTime now = DateTime.Now;
            int year = now.Year;
            txtDatetimeID.Text = year.ToString() + now.Month.ToString("00") + now.Day.ToString("00") + now.Hour.ToString("00") + now.Minute.ToString("00") + now.Second.ToString("00");

            UpdateRepairButton.Visibility = Visibility.Hidden;
            if (txtDatetimeID.Text.Length > 0)
            {
                SaveRepairsForm.Visibility = Visibility.Visible;
            }
            else
            {
                SaveRepairsForm.Visibility = Visibility.Hidden;
            }
            DeleteRepairButton.Visibility = Visibility.Hidden;
            ClearRepairButton.Visibility = Visibility.Hidden;
            //WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
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
            //myConn = new MySqlConnection("server=sql3.freesqldatabase.com;uid=sql370941;pwd=fU9*jL4%;database=sql370941;persist security info=false");
            //MySqlConnection myConn = new MySqlConnection("server=localhost; user id=root;password=CMCsales;database=test;persist security info=false");

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
                //int priceSoldNull = mySQLDataReader.GetOrdinal("price_sold");

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
                //define the connection reference


                ///TODO - AWC - Change below pwd // CMCSales

                //MySqlConnection myConn = new MySqlConnection("server=localhost; user id=root;password=root;database=test;persist security info=false");
                //MySqlConnection myConn = new MySqlConnection("server=localhost; user id=root;password=CMCsales;database=test;persist security info=false");

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
        private void cboIDs_Change(object sender, EventArgs e)///////////////////////////////////////item not showing
        {
            //MessageBox.Show(cboIDs.Text);

            //define the connection reference


            ///TODO - AWC - Change below pwd


            //MySqlConnection myConn = new MySqlConnection("server=localhost; user id=root;password=root;database=test;persist security info=false");
            //MySqlConnection myConn = new MySqlConnection("server=localhost; user id=root;password=CMCsales;database=test;persist security info=false");

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
                    string contact_num = mySQLDataReader.IsDBNull(contact_numNull) ? null : mySQLDataReader.GetInt32("contact_num").ToString();
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
                    //cboItem.Text = item;
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

            //MessageBox.Show("Saving data to the database");

            //define the connection reference


            ///TODO - AWC - Change below pwd

            //MySqlConnection myConn = new MySqlConnection("server=localhost; user id=root;password=root;database=test;persist security info=false");
            //MySqlConnection myConn = new MySqlConnection("server=localhost; user id=root;password=CMCsales;database=test;persist security info=false");
            
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

            //DateTime mydate = DateTime.Now;

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

            MessageBox.Show("Saved");

            //Switcher.Switch(new Repairs());
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

        #region Refresh method
        private void Refresh()
        {
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

            cboIDs.SelectedIndex = -1;
        }
        #endregion

        #region Clear Button
        private void ClearRepairButton_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }
        #endregion

        #region Delete Button
        private void DeleteRepairButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Deleting this record from the database");

            //DateTime now = DateTime.Now;
            //String theDate = "" + now.Year + now.Month + now.Day + now.Hour + now.Minute + now.Second;

            //define the connection reference


            ///TODO - AWC - Change below pwd


            //MySqlConnection myConn = new MySqlConnection("server=localhost; user id=root;password=root;database=test;persist security info=false");
            //MySqlConnection myConn = new MySqlConnection("server=localhost; user id=root;password=CMCsales;database=test;persist security info=false");

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

            MessageBox.Show("Info DELETED!! Congrats");

            Switcher.Switch(new Repairs());
        }
        #endregion

        #region Update Button
        private void UpdateRepairButton_Click(object sender, RoutedEventArgs e)
        {
            if (Validation())
            {
                MessageBox.Show("Updating this record in the database");

                //DateTime now = DateTime.Now;
                //String theDate = "" + now.Year + now.Month + now.Day + now.Hour + now.Minute + now.Second;
                //MessageBox.Show("datetime is " + theDate);
                //MessageBox.Show("" + dtp.Value.ToShortDateString());

                //define the connection reference



                ///TODO - AWC - Change below pwd


                //MySqlConnection myConn = new MySqlConnection("server=localhost; user id=root;password=root;database=test;persist security info=false");
                //MySqlConnection myConn = new MySqlConnection("server=localhost; user id=root;password=CMCsales;database=test;persist security info=false");

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

                    //DateTime mydate = DateTime.Now;

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

                Refresh();

                MessageBox.Show("Info UPDATED!! Congrats");
                //MessageBox.Show("ID is: " + theDate);
            }
            //else { }
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
            //TODO - AWC - readonly fields if ID is blank
            //if (txtDatetimeID.Text.Length == 0)
            //{
            //    Refresh();
            //    txtCustName.IsReadOnly = true;
            //}
            //else
            //{
            //    txtCustName.IsReadOnly = false;
            //}
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

        //private void btnBarcode_Click(object sender, RoutedEventArgs e)
        //{

        //    string PrtNames = DymoAddIn.GetDymoPrinters();

        //    if (PrtNames != null)
        //    {
        //        // parse the result
        //        int i = PrtNames.IndexOf('|');
        //        while (i >= 0)
        //        {
        //            cboItem.Items.Add(PrtNames.Substring(0, i));
        //            PrtNames = PrtNames.Remove(0, i + 1);
        //            i = PrtNames.IndexOf('|');
        //        }
        //        if (PrtNames.Length > 0)
        //            cboItem.Items.Add(PrtNames);

        //        PrtNames = DymoAddIn.GetCurrentPrinterName();
        //        if (PrtNames != null)
        //            cboItem.SelectedIndex = cboItem.Items.IndexOf(PrtNames);
        //        else
        //            cboItem.SelectedIndex = 0;
        //    }
        //    //Switcher.Switch(new BarcodeWindow());

        //    //DymoAddInClass _dymoAddin = new DymoAddInClass();
        //    //DymoLabelsClass _dymoLabel = new DymoLabelsClass();

        //    //var label = DYMO.Label.Framework.Label.Open("/CMCrepairs;component/Images/LargeAddressTestLabel.label");
            
        //    //label.SetObjectText("lblFirstName", firstName);
        //    //label.SetObjectText("lblLastName", lastName);
        //    //label.Print("DYMO LabelWriter 450");
            

        //    //var label = DYMO.Label.Framework.Label.Open("/CMCrepairs;component/Images/LargeAddressTestLabel.label");
        //    //label.SetObjectText("lblBrand", "iPhone 4");
        //    //label.SetObjectText("lblGrade", "A");
        //    //label.SetObjectText("BARCODE", "54321");
        //    //label.SetObjectText("lblPrice", "199.99");
        //    //label.Print("DYMO LabelWriter 450");
        //}








        //private void SetupLabelObject()
        //{
        //    // clear edit control
        //    txtDetails.Clear();

        //    // clear all items first
        //    cboItem.Items.Clear();

        //    if (_label == null)
        //        return;

        //    foreach (string objName in _label.ObjectNames)
        //        if (!string.IsNullOrEmpty(objName))
        //            cboItem.Items.Add(objName);

        //    if (cboItem.Items.Count > 0)
        //        cboItem.SelectedIndex = 0;
        //}

        ////private void SetupTraySelection()
        ////{
        ////    // enable/disable tray based on selected printer

        ////    TrayCmb.Enabled = false; // disable by default

        ////    string printerName = LabelWriterCmb.Text;
        ////    ILabelWriterPrinter printer = Framework.GetPrinters()[printerName] as ILabelWriterPrinter;
        ////    TrayCmb.Enabled = printer != null && printer.IsTwinTurbo;
        ////}

        //private void SetupLabelWriterSelection()
        //{
        //    // clear all items first
        //    comboBox1.Items.Clear();

        //    foreach (IPrinter printer in Framework.GetPrinters())
        //        comboBox1.Items.Add(printer.Name);

        //    if (comboBox1.Items.Count > 0)
        //        comboBox1.SelectedIndex = 0;
        //}

        //private void Load()
        //{
        //    //Application.ThreadException += OnThreadException;

        //    //TrayCmb.SelectedIndex = 0;

        //    // populate label objects
        //    SetupLabelObject();

        //    // obtain the currently selected printer
        //    SetupLabelWriterSelection();

        //    //UpdateControls();
        //}

        //private void bOpenFileDialog_Click(object sender, RoutedEventArgs e)
        //{
        //    openFileDialog1 = new System.Windows.Forms.OpenFileDialog();

        //    if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        //    {
        //        _label = Framework.Open(openFileDialog1.FileName);

        //        // show the file name
        //        txtBackupDetails.Text = openFileDialog1.FileName;

        //        // populate label objects
        //        SetupLabelObject();

        //        // setup paper tray selection
        //        //SetupLabelWriterSelection(false);

        //        //UpdateControls();
        //    }
        //}

        //public enum DialogResult
        //{
        //    // Summary:
        //    //     Nothing is returned from the dialog box. This means that the modal dialog
        //    //     continues running.
        //    None = 0,
        //    //
        //    // Summary:
        //    //     The dialog box return value is OK (usually sent from a button labeled OK).
        //    OK = 1,
        //    //
        //    // Summary:
        //    //     The dialog box return value is Cancel (usually sent from a button labeled
        //    //     Cancel).
        //    Cancel = 2,
        //    //
        //    // Summary:
        //    //     The dialog box return value is Abort (usually sent from a button labeled
        //    //     Abort).
        //    Abort = 3,
        //    //
        //    // Summary:
        //    //     The dialog box return value is Retry (usually sent from a button labeled
        //    //     Retry).
        //    Retry = 4,
        //    //
        //    // Summary:
        //    //     The dialog box return value is Ignore (usually sent from a button labeled
        //    //     Ignore).
        //    Ignore = 5,
        //    //
        //    // Summary:
        //    //     The dialog box return value is Yes (usually sent from a button labeled Yes).
        //    Yes = 6,
        //    //
        //    // Summary:
        //    //     The dialog box return value is No (usually sent from a button labeled No).
        //    No = 7,
        //}

        //private void btnBarcode_Click2(object sender, System.EventArgs e)
        //{
        //    IPrinter printer = Framework.GetPrinters()[comboBox1.Text];
        //    if (printer is ILabelWriterPrinter)
        //    {
        //        ILabelWriterPrintParams printParams = null;
        //        ILabelWriterPrinter labelWriterPrinter = printer as ILabelWriterPrinter;
        //        //if (labelWriterPrinter.IsTwinTurbo)
        //        //{
        //        //    printParams = new LabelWriterPrintParams();
        //        //    printParams.RollSelection = (RollSelection)Enum.Parse(typeof(RollSelection), TrayCmb.Text);
        //        //}

        //        _label.Print(printer, printParams);
        //    }
        //    else
        //        _label.Print(printer); // print with default params
        //}

        //private void ObjectDataEdit_Leave(object sender, System.EventArgs e)
        //{
        //    //DymoLabels.SetField(ObjectNameCmb.Text, ObjectDataEdit.Text);
        //    _label.SetObjectText(cboItem.Text, txtDetails.Text);
        //}

        ////private void UpdateControls()
        ////{
        ////    cboItem.Enabled = cboItem.Items.Count > 0;
        ////    ObjectDataEdit.Enabled = cboItem.Items.Count > 0 && !string.IsNullOrEmpty(cboItem.Text);
        ////    PrintLabelBtn.Enabled = _label != null && !string.IsNullOrEmpty(PrintLabelBtn.Text);
        ////}

        //private void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{

        //}

        private void btnBarcode_Click(object sender, RoutedEventArgs e)
        {
            //Switcher.Switch(new BarcodeWindow(txtDatetimeID.Text));

            //SetID();

            myDymoAddin = new DymoAddIn();
            myLabel = new DymoLabels();
            //if (myDymoAddin.Open(@"C:\Users\Aaron\Documents\DYMO Label\Labels\Repair.label"))
            //{
            //    myLabel.SetField("lblItem", txtDatetimeID.Text);
            //    myLabel.SetField("lblDate", txtDateQuoted.Text);
            //    myLabel.SetField("lbl_ID", id.ToString());
            //    myLabel.SetField("lblDateTime", txtDatetimeID.Text);
            //    myDymoAddin.StartPrintJob();
            //    myDymoAddin.Print(1, false);
            //    myDymoAddin.EndPrintJob();
            //}

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

                        //if (!listOfIDs.Contains(txtDateSold.Text))
                        //if (idsAndDateSold.ContainsValue(txtDateSold.Text))
                        //{
                            //btnUpdate_Click(this, new RoutedEventArgs());
                        //}
                        //else
                        //{
                            //MessageBox.Show("Barcode has printed - item not updated - please check Date Sold ID");
                        //}

                        //string combine = txtBrand.Text + " " + txtModel.Text;

                        //MessageBox.Show(id);

                        //MessageBox.Show(txtDatetimeID.Text + " " + txtDateQuoted.Text + " " + id.ToString());

                        myLabel.SetField("lblItem", txtDatetimeID.Text);
                        myLabel.SetField("lblDate", txtDateQuoted.Text);
                        myLabel.SetField("lbl_ID", id.ToString());
                        myLabel.SetField("lblDateTime", txtDatetimeID.Text);
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
    }
}
