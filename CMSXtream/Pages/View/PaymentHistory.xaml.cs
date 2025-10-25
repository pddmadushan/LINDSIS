using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using XtreamDataAccess;

namespace CMSXtream.Pages.View
{
    /// <summary>
    /// Interaction logic for PaymentHistory.xaml
    /// </summary>
    public partial class PaymentHistory : UserControl
    {
        public Int32 introducerID { get; set; }
        public Double PaybleAmount { get; set; }

        public PaymentHistory()
        {
            InitializeComponent();
            dtpReleaseDate.SelectedDate = DateTime.Now;            
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        public void LoadFormContaint()
        {
            StudentDA _clsPayment = new StudentDA();
            _clsPayment.INTRODUCER_ID = introducerID;
            System.Data.DataTable table = _clsPayment.SelectIntroducerPayment().Tables[0];
            if (table.Rows.Count > 0)
            {
                grdPaymentHistory.ItemsSource = table.DefaultView;
            }
            else
            {
                grdPaymentHistory.ItemsSource = null;
            }
            if (PaybleAmount == 0)
            {
                btnRelease.IsEnabled = false;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtReleasePayment.Text.Trim() == "")
                {
                    MessageBox.Show("Release amount is required!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.No);
                    return;
                }
                if (double.Parse(txtReleasePayment.Text.Trim()) <= 0)
                {
                    MessageBox.Show("Please define a valid amount!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.No);
                    return;
                }
                if ((double.Parse(txtReleasePayment.Text.Trim()) > PaybleAmount))
                {
                    MessageBox.Show("Maximum payble amount is " + PaybleAmount.ToString(), StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.No);
                    return;
                }

                MessageBoxResult result = MessageBox.Show("Do you want to release a payment?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    DoPaymentDA _clsPayment = new DoPaymentDA();
                    _clsPayment.STD_ID = introducerID;
                    _clsPayment.MODIFY_USER = StaticProperty.LoginUserID;
                    _clsPayment.RELEASE_AMOUNT = double.Parse(txtReleasePayment.Text.Trim());
                    _clsPayment.RELEASE_DATE = dtpReleaseDate.SelectedDate.Value;
                    _clsPayment.RELEASE_COMMENT = txtComment.Text.ToString();
                    MessageBox.Show(_clsPayment.InsertReleasePayment(), StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                    ((FirstFloor.ModernUI.Windows.Controls.ModernWindow)this.Parent).Close();
                    //LoadFormContaint();
                    //txtReleasePayment.Text = "";
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }
    }
}
