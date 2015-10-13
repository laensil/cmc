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

        int indexOfDay = 0;
        int indexOfMonth = 0;
        string store = Environment.GetEnvironmentVariable("Location");
        string[] days = { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15",
                            "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31" };

        Dictionary<string, string> monthsWithNames = new Dictionary<string, string>();

        MySqlConnection myConn;
                
        string id;

        #region Constructors

        public Repairs(MySqlConnection myconn)
        {
            InitializeComponent();
            myConn = myconn;
            FillCombo();
            DateTime now = DateTime.Now;
            int year = now.Year;
            string u = year.ToString().Remove(0, 2);
            txtDatetimeID.Text = u + now.Month.ToString("00") + now.Day.ToString("00") + now.Hour.ToString("00") + now.Minute.ToString("00") + now.Second.ToString("00");

            SetDates();

            //UpdateRepairButton.Visibility = Visibility.Hidden;
            if (txtDatetimeID.Text.Length > 0)
            {
                btnSave.Visibility = Visibility.Visible;
            }
            else
            {
                btnSave.Visibility = Visibility.Hidden;
            }
            //DeleteRepairButton.Visibility = Visibility.Hidden;
            //ClearRepairButton.Visibility = Visibility.Hidden;

            PopulateMonthsDictionary(monthsWithNames);
        }

        public Repairs(string id, MySqlConnection myconn)
        {
            InitializeComponent();
            myConn = myconn;
            FillCombo();
            cboIDs.SelectedValue = id;
            cboIDs_Change(cboIDs, new EventArgs());
            btnSave.Visibility = Visibility.Hidden;
            SetDates();

            PopulateMonthsDictionary(monthsWithNames);
        }

        #endregion

        protected void SetDates()
        {
            cboDateQuotedDays.ItemsSource = days;
            cboDateQuotedMonth.ItemsSource = monthsWithNames.Keys;
            txtDateQuotedYear.Text = DateTime.Now.Year.ToString();

            cboPaidDateDays.ItemsSource = days;
            cboPaidDateMonth.ItemsSource = monthsWithNames.Keys;
            txtPaidDateYear.Text = DateTime.Now.Year.ToString();

            cboPODateDays.ItemsSource = days;
            cboPODateMonth.ItemsSource = monthsWithNames.Keys;
            txtPODateYear.Text = DateTime.Now.Year.ToString();
        }

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

        protected string MergePODate()
        {
            return cboPODateDays.Text + "/" + monthsWithNames[cboPODateMonth.Text] + "/" + txtPODateYear.Text;
        }

        protected string MergePaidDate()
        {
            return cboPaidDateDays.Text + "/" + monthsWithNames[cboPaidDateMonth.Text] + "/" + txtPaidDateYear.Text;
        }

        protected string MergeQuoteDate()
        {
            return cboDateQuotedDays.Text + "/" + monthsWithNames[cboDateQuotedMonth.Text] + "/" + txtDateQuotedYear.Text;
        }

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
                    //txtPOdate.Text = po_date;

                    if (po_date != null && !po_date.Equals(""))
                    {
                        indexOfDay = po_date.IndexOf("/");
                        if (indexOfDay > 0)
                        {
                            cboPODateDays.Text = po_date.Substring(0, indexOfDay);
                            indexOfMonth = po_date.IndexOf("/", indexOfDay + 1);
                            if (indexOfMonth > indexOfDay)
                            {
                                cboPODateMonth.Text = po_date.Substring(indexOfDay + 1, (indexOfMonth - indexOfDay) - 1);
                                txtPODateYear.Text = po_date.Substring(indexOfMonth + 1);
                            }
                        }
                    }
                    else
                    {
                        cboPODateDays.Text = "";
                        cboPODateMonth.Text = "";
                        txtPODateYear.Text = "";
                    }

                    txtIMEI.Text = imei;
                    txtPOid.Text = po_id;
                    txtOtherAcc.Text = other_acc;
                    txtIssuesDesc.Text = issues_description;
                    txtBackupDetails.Text = backup_specify;
                    txtQuote.Text = quote_price;
                    //txtDateQuoted.Text = quote_date;

                    if (quote_date != null && !quote_date.Equals(""))
                    {
                        indexOfDay = quote_date.IndexOf("/");
                        if (indexOfDay > 0)
                        {
                            cboDateQuotedDays.Text = quote_date.Substring(0, indexOfDay);
                            indexOfMonth = quote_date.IndexOf("/", indexOfDay + 1);
                            if (indexOfMonth > indexOfDay)
                            {
                                cboDateQuotedMonth.Text = quote_date.Substring(indexOfDay + 1, (indexOfMonth - indexOfDay) - 1);
                                txtDateQuotedYear.Text = quote_date.Substring(indexOfMonth + 1);
                            }
                        }
                    }
                    else
                    {
                        cboDateQuotedDays.Text = "";
                        cboDateQuotedMonth.Text = "";
                        txtDateQuotedYear.Text = "";
                    }

                    txtFaultTestedColl.Text = fault_tested_coll;
                    //txtPaidDate.Text = paid_date;

                    if (paid_date != null && !paid_date.Equals(""))
                    {
                        indexOfDay = paid_date.IndexOf("/");
                        if (indexOfDay > 0)
                        {
                            cboPaidDateDays.Text = paid_date.Substring(0, indexOfDay);
                            indexOfMonth = paid_date.IndexOf("/", indexOfDay + 1);
                            if (indexOfMonth > indexOfDay)
                            {
                                cboPaidDateMonth.Text = paid_date.Substring(indexOfDay + 1, (indexOfMonth - indexOfDay) - 1);
                                txtPaidDateYear.Text = paid_date.Substring(indexOfMonth + 1);
                            }
                        }
                    }
                    else
                    {
                        cboPaidDateDays.Text = "";
                        cboPaidDateMonth.Text = "";
                        txtPaidDateYear.Text = "";
                    }

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
            //System.Windows.Forms.DialogResult answer = MessageBox.Show("Menu?", "", System.Windows.Forms.MessageBoxButtons.YesNo);

            //if (answer == System.Windows.Forms.DialogResult.Yes)
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



            //TODO AWC - Ive temp removed contact number because it wasnt saving, please correct DB and insert contact num back




            //define the command text
            mySQLcommand.CommandText = "INSERT INTO cmc_repairs_repair(datetime_id, completed, customer_name, item, item_with_customer, " +
                "rwpa, details, charger, password, po_date, bag, imei, po_id, sim, pa, pa_inf, mem_card, rtc, rtc_inf, other_acc, issues_description, " +
<<<<<<< HEAD
                "backup_specify, quote_price, quote_date, fault_tested_coll, paid_date, paid, store)" +
                "values(@datetime_id, @completed, @customer_name, @contact_num, @item, @item_with_customer, " +
=======
                "backup_specify, quote_price, quote_date, fault_tested_coll, paid_date, paid)" +
                "values(@datetime_id, @completed, @customer_name, @item, @item_with_customer, " +
>>>>>>> origin/master
                "@rwpa, @details, @charger, @password, @po_date, @bag, @imei, @po_id, @sim, @pa, @pa_inf, @mem_card, @rtc, @rtc_inf, @other_acc, " +
                "@issues_description, @backup_specify, @quote_price, @quote_date, @fault_tested_coll, @paid_date, @paid, @store)";

            //add values provided by user
            mySQLcommand.Parameters.AddWithValue("@datetime_id", txtDatetimeID.Text);
            mySQLcommand.Parameters.AddWithValue("@completed", chbCompleted.IsChecked.Value);

            if (txtCustName.Text.Equals(null) || txtCustName.Text.Equals(""))
                mySQLcommand.Parameters.AddWithValue("@customer_name", "");
            else
                mySQLcommand.Parameters.AddWithValue("@customer_name", txtCustName.Text);

            //if (txtContactNum.Text.Equals(null) || txtContactNum.Text.Equals(""))
            //    mySQLcommand.Parameters.AddWithValue("@contact_num", 0);
            //else
            //    mySQLcommand.Parameters.AddWithValue("@contact_num", int.Parse(txtContactNum.Text));

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

            //if (txtPOdate.Text.Equals(null) || txtPOdate.Text.Equals(""))
            //mySQLcommand.Parameters.AddWithValue("@po_date", "");
            //else
            //mySQLcommand.Parameters.AddWithValue("@po_date", txtPOdate.Text);

            if ((cboPODateDays.Text.Equals(null) || cboPODateDays.Text.Equals("")) ||
                (cboPODateMonth.Text.Equals(null) || cboPODateMonth.Text.Equals("")) ||
                (txtPODateYear.Text.Equals(null) || txtPODateYear.Text.Equals("")))
                mySQLcommand.Parameters.AddWithValue("@po_date", "");
            else
                mySQLcommand.Parameters.AddWithValue("@po_date", MergePODate());

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

            //if (txtDateQuoted.Text.Equals(null) || txtDateQuoted.Text.Equals(""))
            //mySQLcommand.Parameters.AddWithValue("@quote_date", "");
            //else
            //mySQLcommand.Parameters.AddWithValue("@quote_date", txtDateQuoted.Text);

            if ((cboDateQuotedDays.Text.Equals(null) || cboDateQuotedDays.Text.Equals("")) ||
                (cboDateQuotedMonth.Text.Equals(null) || cboDateQuotedMonth.Text.Equals("")) ||
                (txtDateQuotedYear.Text.Equals(null) || txtDateQuotedYear.Text.Equals("")))
                mySQLcommand.Parameters.AddWithValue("@quote_date", "");
            else
                mySQLcommand.Parameters.AddWithValue("@quote_date", MergeQuoteDate());

            if (txtFaultTestedColl.Text.Equals(null) || txtFaultTestedColl.Text.Equals(""))
                mySQLcommand.Parameters.AddWithValue("@fault_tested_coll", "");
            else
                mySQLcommand.Parameters.AddWithValue("@fault_tested_coll", txtFaultTestedColl.Text);

            //if (txtPaidDate.Text.Equals(null) || txtPaidDate.Text.Equals(""))
            //mySQLcommand.Parameters.AddWithValue("@paid_date", "");
            //else
            //mySQLcommand.Parameters.AddWithValue("@paid_date", txtPaidDate.Text);

            if ((cboPaidDateDays.Text.Equals(null) || cboPaidDateDays.Text.Equals("")) ||
                (cboPaidDateMonth.Text.Equals(null) || cboPaidDateMonth.Text.Equals("")) ||
                (txtPaidDateYear.Text.Equals(null) || txtPaidDateYear.Text.Equals("")))
                mySQLcommand.Parameters.AddWithValue("@paid_date", "");
            else
                mySQLcommand.Parameters.AddWithValue("@paid_date", MergePaidDate());

            if (txtPaid.Text.Equals(null) || txtPaid.Text.Equals(""))
                mySQLcommand.Parameters.AddWithValue("@paid", 0);
            else
                mySQLcommand.Parameters.AddWithValue("@paid", Double.Parse(txtPaid.Text));
            if (store.Equals(null) || store.Equals(""))
                mySQLcommand.Parameters.AddWithValue("@store", "Unknown");
            else
                mySQLcommand.Parameters.AddWithValue("@store", store);

            try
            {
                mySQLcommand.ExecuteNonQuery();
                MessageBox.Show("Information Saved");
                Switcher.Switch(new Repairs(myConn));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Did not save. If it's a duplicate entry, try updating instead\nError Message: '" + ex.Message + "'");
            }

            //close the connection
            myConn.Close();
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
            Switcher.Switch(new SearchID("cmc_repairs_repair", myConn));
        }
        #endregion

        #region Clear Button
        private void ClearRepairButton_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new Repairs(myConn));
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

                Switcher.Switch(new Repairs(myConn));
            }
        }
        #endregion

        #region Update Button
        private void UpdateRepairButton_Click(object sender, RoutedEventArgs e)
        {
            if (Validation())
            {
                MessageBox.Show("Updating record");

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
                    mySQLcommand.CommandText = "UPDATE cmc_repairs_repair SET datetime_id=@datetime_id, completed=@completed, customer_name=@customer_name, item=@item, item_with_customer=@item_with_customer, " +
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

                    //if (txtContactNum.Text.Equals(null) || txtContactNum.Text.Equals(""))
                    //    mySQLcommand.Parameters.AddWithValue("@contact_num", "");
                    //else
                    //    mySQLcommand.Parameters.AddWithValue("@contact_num", txtContactNum.Text);

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

                    //if (txtPOdate.Text.Equals(null) || txtPOdate.Text.Equals(""))
                    //mySQLcommand.Parameters.AddWithValue("@po_date", "");
                    //else
                    //mySQLcommand.Parameters.AddWithValue("@po_date", txtPOdate.Text);

                    if ((cboPODateDays.Text.Equals(null) || cboPODateDays.Text.Equals("")) ||
                        (cboPODateMonth.Text.Equals(null) || cboPODateMonth.Text.Equals("")) ||
                        (txtPODateYear.Text.Equals(null) || txtPODateYear.Text.Equals("")))
                        mySQLcommand.Parameters.AddWithValue("@po_date", "");
                    else
                        mySQLcommand.Parameters.AddWithValue("@po_date", MergePODate());

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

                    //if (txtDateQuoted.Text.Equals(null) || txtDateQuoted.Text.Equals(""))
                    //mySQLcommand.Parameters.AddWithValue("@quote_date", "");
                    //else
                    //mySQLcommand.Parameters.AddWithValue("@quote_date", txtDateQuoted.Text);

                    if ((cboDateQuotedDays.Text.Equals(null) || cboDateQuotedDays.Text.Equals("")) ||
                        (cboDateQuotedMonth.Text.Equals(null) || cboDateQuotedMonth.Text.Equals("")) ||
                        (txtDateQuotedYear.Text.Equals(null) || txtDateQuotedYear.Text.Equals("")))
                        mySQLcommand.Parameters.AddWithValue("@quote_date", "");
                    else
                        mySQLcommand.Parameters.AddWithValue("@quote_date", MergeQuoteDate());

                    if (txtFaultTestedColl.Text.Equals(null) || txtFaultTestedColl.Text.Equals(""))
                        mySQLcommand.Parameters.AddWithValue("@fault_tested_coll", "");
                    else
                        mySQLcommand.Parameters.AddWithValue("@fault_tested_coll", txtFaultTestedColl.Text);

                    //if (txtPaidDate.Text.Equals(null) || txtPaidDate.Text.Equals(""))
                    //mySQLcommand.Parameters.AddWithValue("@paid_date", "");
                    //else
                    //mySQLcommand.Parameters.AddWithValue("@paid_date", txtPaidDate.Text);

                    if ((cboPaidDateDays.Text.Equals(null) || cboPaidDateDays.Text.Equals("")) ||
                        (cboPaidDateMonth.Text.Equals(null) || cboPaidDateMonth.Text.Equals("")) ||
                        (txtPaidDateYear.Text.Equals(null) || txtPaidDateYear.Text.Equals("")))
                        mySQLcommand.Parameters.AddWithValue("@paid_date", "");
                    else
                        mySQLcommand.Parameters.AddWithValue("@paid_date", MergePaidDate());

                    if (txtPaid.Text.Equals(null) || txtPaid.Text.Equals(""))
                        mySQLcommand.Parameters.AddWithValue("@paid", 0);
                    else
                        mySQLcommand.Parameters.AddWithValue("@paid", Double.Parse(txtPaid.Text));

                  

                    //mySQLcommand.ExecuteNonQuery();
                    MySqlDataReader mySQLDataReader;
                    mySQLDataReader = mySQLcommand.ExecuteReader();

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
                    //UpdateRepairButton.Visibility = Visibility.Visible;
                    //SaveRepairsForm.Visibility = Visibility.Hidden;
                    //DeleteRepairButton.Visibility = Visibility.Visible;
                    //ClearRepairButton.Visibility = Visibility.Visible;
                }
                else
                {
                    //UpdateRepairButton.Visibility = Visibility.Hidden;

                    //if (txtDatetimeID.Text.Equals(null) || txtDatetimeID.Text.Equals(""))
                    //{
                    //    SaveRepairsForm.Visibility = Visibility.Hidden;
                    //    DeleteRepairButton.Visibility = Visibility.Hidden;
                    //    ClearRepairButton.Visibility = Visibility.Hidden;
                    //}
                    //else
                    //{
                    //    SaveRepairsForm.Visibility = Visibility.Visible;
                    //    DeleteRepairButton.Visibility = Visibility.Visible;
                    //    ClearRepairButton.Visibility = Visibility.Visible;
                    //}
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
                        (MergeQuoteDate().Equals("//")) || (txtCustName.Text.Equals(null) || txtCustName.Text.Equals("")) ||
                        (txtIssuesDesc.Text.Equals(null) || txtIssuesDesc.Text.Equals("")) || (txtContactNum.Text.Equals(null) ||
                        txtContactNum.Text.Equals("")))
                    {
                        MessageBox.Show("ID/Date Quoted/Customer Name/Issue Desc/Contact Num is empty");
                    }
                    else
                    {
                        SetID();

                        MessageBox.Show("Remember to save/update after label has printed");

                        myLabel.SetField("lblItem", txtDatetimeID.Text);
                        myLabel.SetField("lblDate", MergeQuoteDate());
                        myLabel.SetField("lblContactName", txtCustName.Text);
                        myLabel.SetField("lblItemDetails", txtIssuesDesc.Text);
                        myLabel.SetField("lblContactNum", txtContactNum.Text);
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
        #endregion


        #region Lock/Unlock Buttons

        private void LockSaveButton()
        {
            btnSave.IsEnabled = false;
        }

        private void UnlockSaveButton()
        {
            btnSave.IsEnabled = true;
        }

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

        private void LockCompletedButton()
        {
            btnCompleted.IsEnabled = false;
        }

        private void UnlockCompletedButton()
        {
            btnCompleted.IsEnabled = true;
        }

        #endregion


        private void LockControls()
        {
            txtDatetimeID.IsEnabled = false;
            txtCustName.IsEnabled = false;
            txtContactNum.IsEnabled = false;
            cboItem.IsEnabled = false;
            chbItemCust.IsEnabled = false;
            chbRWPA.IsEnabled = false;
            txtDetails.IsEnabled = false;
            chbCharger.IsEnabled = false;
            txtPassword.IsEnabled = false;
            cboPODateDays.IsEnabled = false;
            cboPODateMonth.IsEnabled = false;
            txtPODateYear.IsEnabled = false;
            chbBag.IsEnabled = false;
            txtIMEI.IsEnabled = false;
            txtPOid.IsEnabled = false;
            chbSIM.IsEnabled = false;
            chbPA.IsEnabled = false;
            chbPAinf.IsEnabled = false;
            chbMemCard.IsEnabled = false;
            chbRTC.IsEnabled = false;
            chbRTCinf.IsEnabled = false;
            txtOtherAcc.IsEnabled = false;
            txtIssuesDesc.IsEnabled = false;
            txtBackupDetails.IsEnabled = false;
            txtQuote.IsEnabled = false;
            cboDateQuotedDays.IsEnabled = false;
            cboDateQuotedMonth.IsEnabled = false;
            txtDateQuotedYear.IsEnabled = false;
            txtFaultTestedColl.IsEnabled = false;
            cboPaidDateDays.IsEnabled = false;
            cboPaidDateMonth.IsEnabled = false;
            txtPaidDateYear.IsEnabled = false;
            txtPaid.IsEnabled = false;
        }

        private void UnlockControls()
        {
            txtDatetimeID.IsEnabled = true;
            txtCustName.IsEnabled = true;
            txtContactNum.IsEnabled = true;
            cboItem.IsEnabled = true;
            chbItemCust.IsEnabled = true;
            chbRWPA.IsEnabled = true;
            txtDetails.IsEnabled = true;
            chbCharger.IsEnabled = true;
            txtPassword.IsEnabled = true;
            cboPODateDays.IsEnabled = true;
            cboPODateMonth.IsEnabled = true;
            txtPODateYear.IsEnabled = true;
            chbBag.IsEnabled = true;
            txtIMEI.IsEnabled = true;
            txtPOid.IsEnabled = true;
            chbSIM.IsEnabled = true;
            chbPA.IsEnabled = true;
            chbPAinf.IsEnabled = true;
            chbMemCard.IsEnabled = true;
            chbRTC.IsEnabled = true;
            chbRTCinf.IsEnabled = true;
            txtOtherAcc.IsEnabled = true;
            txtIssuesDesc.IsEnabled = true;
            txtBackupDetails.IsEnabled = true;
            txtQuote.IsEnabled = true;
            cboDateQuotedDays.IsEnabled = true;
            cboDateQuotedMonth.IsEnabled = true;
            txtDateQuotedYear.IsEnabled = true;
            txtFaultTestedColl.IsEnabled = true;
            cboPaidDateDays.IsEnabled = true;
            cboPaidDateMonth.IsEnabled = true;
            txtPaidDateYear.IsEnabled = true;
            txtPaid.IsEnabled = true;
        }


        private void btnCompleted_Click(object sender, RoutedEventArgs e)
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
                mySQLcommand.CommandText = "UPDATE cmc_repairs_repair SET datetime_id=@datetime_id, completed=@completed, customer_name=@customer_name, item=@item, item_with_customer=@item_with_customer, " +
                    "rwpa=@rwpa, details=@details, charger=@charger, password=@password, po_date=@po_date, bag=@bag, imei=@imei, po_id=@po_id, sim=@sim, pa=@pa, pa_inf=@pa_inf, mem_card=@mem_card, rtc=@rtc, rtc_inf=@rtc_inf, other_acc=@other_acc, " +
                    "issues_description=@issues_description, backup_specify=@backup_specify, quote_price=@quote_price, quote_date=@quote_date, fault_tested_coll=@fault_tested_coll, paid_date=@paid_date, paid=@paid " +
                    "WHERE datetime_id=@datetime_id";

                //add values provided by user
                if (txtDatetimeID.Text.Equals(null) || txtDatetimeID.Text.Equals(""))
                {
                    MessageBox.Show("ID cannot be blank");
                    return;
                }
                else
                    mySQLcommand.Parameters.AddWithValue("@datetime_id", txtDatetimeID.Text);

                if (txtPaid.Text.Equals(null) || txtPaid.Text.Equals(""))
                {
                    System.Windows.Forms.DialogResult answer = MessageBox.Show("Amount Paid is blank, Are you sure you want to mark this item as Completed?", "", System.Windows.Forms.MessageBoxButtons.YesNo);

                    if (answer == System.Windows.Forms.DialogResult.No)
                        return;
                }

                chbCompleted.IsChecked = true;
                mySQLcommand.Parameters.AddWithValue("@completed", true);

                if (txtCustName.Text.Equals(null) || txtCustName.Text.Equals(""))
                    mySQLcommand.Parameters.AddWithValue("@customer_name", "");
                else
                    mySQLcommand.Parameters.AddWithValue("@customer_name", txtCustName.Text);

                //if (txtContactNum.Text.Equals(null) || txtContactNum.Text.Equals(""))
                //    mySQLcommand.Parameters.AddWithValue("@contact_num", "");
                //else
                //    mySQLcommand.Parameters.AddWithValue("@contact_num", txtContactNum.Text);

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

                if ((cboPODateDays.Text.Equals(null) || cboPODateDays.Text.Equals("")) ||
                    (cboPODateMonth.Text.Equals(null) || cboPODateMonth.Text.Equals("")) ||
                    (txtPODateYear.Text.Equals(null) || txtPODateYear.Text.Equals("")))
                    mySQLcommand.Parameters.AddWithValue("@po_date", "");
                else
                    mySQLcommand.Parameters.AddWithValue("@po_date", MergePODate());

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

                if ((cboDateQuotedDays.Text.Equals(null) || cboDateQuotedDays.Text.Equals("")) ||
                    (cboDateQuotedMonth.Text.Equals(null) || cboDateQuotedMonth.Text.Equals("")) ||
                    (txtDateQuotedYear.Text.Equals(null) || txtDateQuotedYear.Text.Equals("")))
                    mySQLcommand.Parameters.AddWithValue("@quote_date", "");
                else
                    mySQLcommand.Parameters.AddWithValue("@quote_date", MergeQuoteDate());

                if (txtFaultTestedColl.Text.Equals(null) || txtFaultTestedColl.Text.Equals(""))
                    mySQLcommand.Parameters.AddWithValue("@fault_tested_coll", "");
                else
                    mySQLcommand.Parameters.AddWithValue("@fault_tested_coll", txtFaultTestedColl.Text);

                if ((cboPaidDateDays.Text.Equals(null) || cboPaidDateDays.Text.Equals("")) ||
                    (cboPaidDateMonth.Text.Equals(null) || cboPaidDateMonth.Text.Equals("")) ||
                    (txtPaidDateYear.Text.Equals(null) || txtPaidDateYear.Text.Equals("")))
                    mySQLcommand.Parameters.AddWithValue("@paid_date", "");
                else
                    mySQLcommand.Parameters.AddWithValue("@paid_date", MergePaidDate());

                if (txtPaid.Text.Equals(null) || txtPaid.Text.Equals(""))
                    mySQLcommand.Parameters.AddWithValue("@paid", 0);
                else
                    mySQLcommand.Parameters.AddWithValue("@paid", Double.Parse(txtPaid.Text));

                //mySQLcommand.ExecuteNonQuery();
                MySqlDataReader mySQLDataReader;
                mySQLDataReader = mySQLcommand.ExecuteReader();

                //close the connection
                myConn.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void chbCompleted_Checked(object sender, RoutedEventArgs e)
        {
            if (chbCompleted.IsChecked == true)
            {
                LockControls();
                LockBarcodeButton();
                LockDeleteButton();
                LockSaveButton();
                LockUpdateButton();
                LockCompletedButton();
            }
            else
            {
                UnlockControls();
                UnlockBarcodeButton();
                UnlockDeleteButton();
                UnlockSaveButton();
                UnlockUpdateButton();
                UnlockCompletedButton();
            }
        }
    }
}