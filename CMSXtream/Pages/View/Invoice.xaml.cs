using System;
using System.Globalization;
using System.Linq;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using XtreamDataAccess;

namespace CMSXtream.Pages.View
{
    /// <summary>
    /// Interaction logic for Invoice.xaml
    /// </summary>
    public partial class Invoice : UserControl
    {
        public Int32 studentID { get; set; }
        public String studentName { get; set; }
        public String studentTag { get; set; }
        public Invoice()
        {
            InitializeComponent();
            txtPrintDate.Text =DateTime.Today.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        }
        public void LoadFormContaint()
        {
            try
            {
                StudentDA _clsPayment = new StudentDA();
                _clsPayment.STD_ID = studentID;
                System.Data.DataSet ds = _clsPayment.StudentInvoice();

                System.Data.DataTable dt0 = ds.Tables[0];
                System.Data.DataTable dt1 = ds.Tables[1];
                System.Data.DataTable dt2 = ds.Tables[2];
                txtName.Text = studentID + " " + studentName + " " + studentTag;
                txtName.IsReadOnly = true;
                txtStudent.Text = ds.Tables[3].Rows[0][0].ToString();
                txtSinhalaFont.Text = ds.Tables[3].Rows[0][1].ToString();

                if (dt0.Rows.Count > 0)
                {
                    grdMainDetails.ItemsSource = dt0.DefaultView;
                }
                else
                {
                    grdMainDetails.ItemsSource = null;
                }

                if (dt1.Rows.Count > 0)
                {
                    grdPayments.ItemsSource = dt1.DefaultView;
                }
                else
                {
                    grdPayments.ItemsSource = null;
                }

                if (dt2.Rows.Count > 0)
                {
                    grpDelayPay.Visibility = Visibility.Visible;
                    grdDelayCarges.ItemsSource = dt2.DefaultView;
                }
                else
                {
                    grpDelayPay.Visibility = Visibility.Hidden;
                    grdDelayCarges.ItemsSource = null;
                }


            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            try {

                Clipboard.SetText(txtName.Text);

                ScrollInvoice.ScrollToVerticalOffset(0);
                using (var printServer = new LocalPrintServer())
                {
                    var dialog = new PrintDialog();
                    //Find the PDF printer
                    var qs = printServer.GetPrintQueues();
                    var queue = qs.FirstOrDefault(q => q.Name.Contains("PDF"));
                    if (queue == null) {                        
                        dialog.ShowDialog();
                    }
                    dialog.PrintTicket.PageOrientation = PageOrientation.Portrait;
                    dialog.PrintQueue = queue; 
                    dialog.PrintVisual(print, "Invoise");
                    ((FirstFloor.ModernUI.Windows.Controls.ModernWindow)this.Parent).Close();
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
