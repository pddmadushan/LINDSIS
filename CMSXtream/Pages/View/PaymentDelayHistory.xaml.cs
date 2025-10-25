using System;
using System.Windows;
using System.Windows.Controls;
using XtreamDataAccess;

namespace CMSXtream.Pages.View
{
    /// <summary>
    /// Interaction logic for PaymentDelayHistory.xaml
    /// </summary>
    public partial class PaymentDelayHistory : UserControl
    {
        public Int32 studentID { get; set; }
        public PaymentDelayHistory()
        {
            InitializeComponent();
        }

        public void LoadFormContaint()
        {
            try
            {
                StudentDA _clsPayment = new StudentDA();
                _clsPayment.STD_ID = studentID;
                System.Data.DataTable table = _clsPayment.StudentDelayPaymentHistory().Tables[0];
                if (table.Rows.Count > 0)
                {
                    grdAttHistory.ItemsSource = table.DefaultView;
                }
                else
                {
                    grdAttHistory.ItemsSource = null;
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
