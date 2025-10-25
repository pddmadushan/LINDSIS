using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using XtreamDataAccess;

namespace CMSXtream.Pages.DataEntry
{
    /// <summary>
    /// Interaction logic for PaymetType.xaml
    /// </summary>
    public partial class PaymetType : UserControl
    {
        public Int32 CLS_ID;
        public Int32 STD_ID;
        public Int32 CLS_PAY_TYPE;     
        public double CLS_AMOUNT;

        public PaymetType()
        {
            InitializeComponent();            
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StudentDA _clsStudent = new StudentDA();
                _clsStudent.STD_ID = STD_ID;
                _clsStudent.CLS_ID = CLS_ID;
                double stdFee = 0;
                Int32 payType = 0;

                if (chkClsFee.IsChecked.Value)
                {
                    payType = 0;
                    stdFee = double.Parse(txtAmount.Text.ToString());
                }
                else
                {
                    payType = 1;
                    stdFee = double.Parse(txtCauseFee.Text.ToString());
                }

                _clsStudent.UpdatePaymentType(payType, stdFee);
                CLS_PAY_TYPE = payType;
                CLS_AMOUNT = stdFee;
                ((FirstFloor.ModernUI.Windows.Controls.ModernWindow)this.Parent).Close();

            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        public void loadFormDetails()
        {
            try
            {
                StudentDA _clsStudent = new StudentDA();
                _clsStudent.CLS_ID = CLS_ID;
                System.Data.DataTable table = _clsStudent.SelectClassPayType();
                if (table.Rows.Count > 0)
                {
                    txtAmount.Text = table.Rows[0]["CLS_FEE"].ToString();
                    txtCauseFee.Text = table.Rows[0]["COUSE_FREE"].ToString(); 
                }
                else
                {
                    txtAmount.Text = "0";
                    txtCauseFee.Text = "0";
                }
                if(CLS_PAY_TYPE == 0)
                {
                    chkClsFee.IsChecked = true;
                }
                else
                {
                    chkCouseFee.IsChecked = true;
                }
            }
            catch(Exception ex){
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);         
            }
        }

         

    }
}
