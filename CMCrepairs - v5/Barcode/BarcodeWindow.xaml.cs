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
using MessageBox = System.Windows.Forms.MessageBox;
using MySql.Data.MySqlClient;
using Dymo;
using DYMO.Label.Framework;

namespace CMCrepairs.Barcode
{
    /// <summary>
    /// Interaction logic for BarcodeWindow.xaml
    /// </summary>
    public partial class BarcodeWindow : UserControl, ISwitchable
    {
        //public DymoAddInClass DymoAddIn;
        //public DymoLabelsClass DymoLabels;
        private ILabel _label;
        private System.Windows.Forms.OpenFileDialog openFileDialog;

        string _test;

        public BarcodeWindow(string test)
        {
            InitializeComponent();
            _test = test;
            Load();

        }

        #region ISwitchable Members
        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }
        #endregion

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new MainWindow());
        }

        private void SetupLabelObject()
        {
            // clear edit control
            txtDetail.Clear();

            // clear all items first
            cboLabelPart.Items.Clear();

            if (_label == null)
                return;

            foreach (string objName in _label.ObjectNames)
                if (!string.IsNullOrEmpty(objName))
                    cboLabelPart.Items.Add(objName);

            if (cboLabelPart.Items.Count > 0)
                cboLabelPart.SelectedIndex = 0;
        }

        private void SetupLabelWriterSelection()
        {
            // clear all items first
            cboPrinterName.Items.Clear();

            foreach (IPrinter printer in Framework.GetPrinters())
                cboPrinterName.Items.Add(printer.Name);

            if (cboPrinterName.Items.Count > 0)
                cboPrinterName.SelectedIndex = 0;
        }

        private void Load()
        {
            // populate label objects
            SetupLabelObject();

            // obtain the currently selected printer
            SetupLabelWriterSelection();
        }

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            openFileDialog = new System.Windows.Forms.OpenFileDialog();

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _label = Framework.Open(openFileDialog.FileName);

                // show the file name
                txtOpenLabelFile.Text = openFileDialog.FileName;

                // populate label objects
                SetupLabelObject();
            }
        }

        public enum DialogResult
        {
            // Summary:
            //     Nothing is returned from the dialog box. This means that the modal dialog
            //     continues running.
            None = 0,
            //
            // Summary:
            //     The dialog box return value is OK (usually sent from a button labeled OK).
            OK = 1,
            //
            // Summary:
            //     The dialog box return value is Cancel (usually sent from a button labeled
            //     Cancel).
            Cancel = 2,
            //
            // Summary:
            //     The dialog box return value is Abort (usually sent from a button labeled
            //     Abort).
            Abort = 3,
            //
            // Summary:
            //     The dialog box return value is Retry (usually sent from a button labeled
            //     Retry).
            Retry = 4,
            //
            // Summary:
            //     The dialog box return value is Ignore (usually sent from a button labeled
            //     Ignore).
            Ignore = 5,
            //
            // Summary:
            //     The dialog box return value is Yes (usually sent from a button labeled Yes).
            Yes = 6,
            //
            // Summary:
            //     The dialog box return value is No (usually sent from a button labeled No).
            No = 7,
        }

        DymoLabels myLabel;
        DymoAddIn myDymoAddin;

        private void btnPrintBarcode_Click(object sender, System.EventArgs e)
        {
            //myDymoAddin = new DymoAddIn();
            //myLabel = new DymoLabels();
            IPrinter printer = Framework.GetPrinters()[cboPrinterName.Text];
            if (printer is ILabelWriterPrinter)
            {

                ////_label.SetObjectText("BARCODE_TEST", "54321");
                //ILabelWriterPrintParams printParams = null;
                //ILabelWriterPrinter labelWriterPrinter = printer as ILabelWriterPrinter;
                //_label.Print(printer, printParams);
            }
            else
                _label.Print(printer); // print with default params



            myDymoAddin = new DymoAddIn();
            myLabel = new DymoLabels();
            if (myDymoAddin.Open(@"C:\Users\Aaron\Documents\DYMO Label\Labels\SmallNameBadgeTest.label"))
            {
                myLabel.SetField("lblPrice", _test);
                myDymoAddin.StartPrintJob();
                myDymoAddin.Print(1, false);
                myDymoAddin.EndPrintJob();
            }
        }

        private void ObjectDataEdit_Leave(object sender, System.EventArgs e)
        {
            //DymoLabels.SetField(ObjectNameCmb.Text, ObjectDataEdit.Text);
            _label.SetObjectText(cboLabelPart.Text, txtDetail.Text);
        }

        //private void FormLoad(object sender, EventArgs e)
        //{
        //    /////////////////////////////////////
        //    // Encode The Data
        //    /////////////////////////////////////
        //    Barcodes bb = new Barcodes();
        //    bb.BarcodeType = Barcodes.BarcodeEnum.Code39;
        //    bb.Data = "123";
        //    //bb.CheckDigit = Barcodes.YesNoEnum.Yes;
        //    bb.encode();

        //    int thinWidth;
        //    int thickWidth;

        //    thinWidth = 3;
        //    thickWidth = 3 * thinWidth;

        //    string outputString = bb.EncodedData;
        //    string humanText = bb.HumanText;

        //    /////////////////////////////////////
        //    // Draw The Barcode
        //    /////////////////////////////////////
        //    int len = outputString.Length;
        //    int currentPos = 10;
        //    int currentTop = 10;
        //    int currentColor = 0;
        //    for (int i = 0; i < len; i++)
        //    {
        //        Rectangle rect = new Rectangle();
        //        rect.Height = 100;
        //        if (currentColor == 0)
        //        {
        //            currentColor = 1;
        //            rect.Fill = new SolidColorBrush(Colors.Black);

        //        }
        //        else
        //        {
        //            currentColor = 0;
        //            rect.Fill = new SolidColorBrush(Colors.White);

        //        }
        //        Canvas.SetLeft(rect, currentPos);
        //        Canvas.SetTop(rect, currentTop);

        //        if (outputString[i] == 't')
        //        {
        //            rect.Width = thinWidth;
        //            currentPos += thinWidth;

        //        }
        //        else if (outputString[i] == 'w')
        //        {
        //            rect.Width = thickWidth;
        //            currentPos += thickWidth;

        //        }
        //        mainCanvas.Children.Add(rect);

        //    }

        //    /////////////////////////////////////
        //    // Add the Human Readable Text
        //    /////////////////////////////////////
        //    TextBlock tb = new TextBlock();
        //    tb.Text = humanText;
        //    tb.FontSize = 32;
        //    tb.FontFamily = new FontFamily("Courier New");
        //    Rect rx = new Rect(0, 0, 0, 0);
        //    tb.Arrange(rx);
        //    Canvas.SetLeft(tb, (currentPos - tb.ActualWidth) / 2);
        //    Canvas.SetTop(tb, currentTop + 205);
        //    mainCanvas.Children.Add(tb);
        //}
    }
}
