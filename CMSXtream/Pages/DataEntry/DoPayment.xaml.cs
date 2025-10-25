using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using XtreamDataAccess;

namespace CMSXtream.Pages.DataEntry
{
    /// <summary>
    /// Interaction logic for DoPayment.xaml
    /// </summary>
    public partial class DoPayment : UserControl
    {
        public Int32 studentId { get; set; }
        public Int32 classID { get; set; }
        public int paidYear { get; set; }
        public int PaidMonth { get; set; }

        public Boolean cardDeliverd { get; set; }
        public string className { get; set; }
        public string yearMonth { get; set; }
        public string classFee { get; set; }
        public DateTime clsDate { get; set; }

        public DoPayment()
        {
            InitializeComponent();
            if (StaticProperty.LoginisAdmin != "1")
            {
                dtpDatePicke.SelectedDate = DateTime.Now;
                dtpDatePicke.IsEnabled = false;
                clmBtndelete.Visibility = Visibility.Collapsed;
                btnDeleteCalssfree.Visibility = Visibility.Collapsed;
            }
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        public void LoadFormContaint()
        {
            lblClass.Content = className;
            lblYearMonth.Content = yearMonth.Replace("1999",string.Empty);
            txtAmount.Text = classFee;
            chkCardIssue.IsChecked = cardDeliverd;
            //dtpDatePicke.SelectedDate = clsDate;
            rdoBtn.IsChecked = false;
            BindPaymentHistoryGrid();            
        }
        public void BindPaymentHistoryGrid()
        {
            try
            {
                DoPaymentDA _clsPayment = new DoPaymentDA();
                _clsPayment.STD_ID = studentId;

                if (lblClass.Visibility == System.Windows.Visibility.Visible)
                {
                    _clsPayment.CLS_ID = classID;
                    _clsPayment.PAID_YEAR = paidYear;
                    _clsPayment.PAID_MONTH = PaidMonth;
                }
                else
                {
                    _clsPayment.CLS_ID = int.Parse(cmbClsGroup.SelectedValue.ToString()); ;
                    _clsPayment.PAID_YEAR = dtpPayMonth.SelectedDate.Value.Year;
                    _clsPayment.PAID_MONTH = dtpPayMonth.SelectedDate.Value.Month;
                }

                System.Data.DataTable table = _clsPayment.SelectPayment().Tables[0];
                if (table.Rows.Count > 0)
                {
                    grdPayHistory.ItemsSource = table.DefaultView;
                }
                else
                {
                    grdPayHistory.ItemsSource = null;
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }
        private void btnDoPayment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dtpDatePicke.SelectedDate == null)
                {
                    MessageBox.Show("Date is required!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.No);
                    return;
                }

                double paidYearamt = getPaidAmount(dtpDatePicke.SelectedDate.Value);

                if ((double.Parse(txtAmount.Text.Trim()) <= paidYearamt))
                {
                    MessageBox.Show("Full payment has already been done!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.No);
                    return;
                }

                if (rdoBtn.IsChecked.Value == false)
                {
                    if (txtPayment.Text.Trim() == "")
                    {
                        MessageBox.Show("Payment amount is required!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.No);
                        return;
                    }
                    if (double.Parse(txtPayment.Text.Trim()) < 0)
                    {
                        MessageBox.Show("Please define a valid amount!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.No);
                        return;
                    }

                    if ((double.Parse(txtAmount.Text.Trim()) - paidYearamt) < double.Parse(txtPayment.Text.Trim()))
                    {
                        MessageBox.Show("Payment should not be greater than balance to paid!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.No);
                        return;
                    }
                }

                MessageBoxResult result = MessageBox.Show("Do you want to add payment?", "Add Payment Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {

                    DoPaymentDA _clsPayment = new DoPaymentDA();
                    _clsPayment.STD_ID = studentId;
                    if (lblClass.Visibility == System.Windows.Visibility.Visible)
                    {
                        _clsPayment.CLS_ID = classID;
                        _clsPayment.PAID_YEAR = paidYear;
                        _clsPayment.PAID_MONTH = PaidMonth;
                    }
                    else
                    {
                        _clsPayment.CLS_ID = int.Parse(cmbClsGroup.SelectedValue.ToString());
                        _clsPayment.PAID_YEAR = dtpPayMonth.SelectedDate.Value.Year;
                        _clsPayment.PAID_MONTH = dtpPayMonth.SelectedDate.Value.Month;
                    }

                    _clsPayment.MODIFY_USER = StaticProperty.LoginUserID;
                    _clsPayment.PAID_AMOUNT = double.Parse(txtPayment.Text.Trim());
                    _clsPayment.STD_REC_DATE = dtpDatePicke.SelectedDate.Value;
                    _clsPayment.CLASS_FEE = double.Parse(txtAmount.Text.Trim());
                    _clsPayment.CARD_ISSUED_FLG = chkCardIssue.IsChecked.Value ? 1 : 0;
                    _clsPayment.UpdatePayment();
                    BindPaymentHistoryGrid();
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private double getPaidAmount(DateTime selectDate)
        {
            double paidgetPaidAmount = 0;
            int rowCount = grdPayHistory.Items.Count;
            for (int i = 0; i < rowCount; i++)
            {
                var selectedRow = (System.Data.DataRowView)grdPayHistory.Items[i];
                if (txtPayment != null)
                {

                    double PaidFree = Double.Parse(selectedRow["PAID_AMOUNT"].ToString());
                    DateTime selectedDate = DateTime.Parse(selectedRow["STD_REC_DATE"].ToString());
                    if (selectedDate != selectDate)
                    {
                        paidgetPaidAmount += PaidFree;
                    }
                }
            }
            return paidgetPaidAmount;

        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dtpDatePicke.SelectedDate == null)
                {
                    MessageBox.Show("Date is required!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.No);
                    return;
                }

                double paidYearamt = getPaidAmount(dtpDatePicke.SelectedDate.Value);
                double balanceTobepaid = double.Parse(txtAmount.Text.Trim()) - paidYearamt;
                if (balanceTobepaid < 0) { balanceTobepaid = 0; }

                txtPayment.IsEnabled = false;
                txtPayment.Text = balanceTobepaid.ToString();
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void RadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                txtPayment.IsEnabled = true;
                txtPayment.Text = "0";
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void chkCardIssue_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                DoPaymentDA _clsPayment = new DoPaymentDA();
                _clsPayment.STD_ID = studentId;
                _clsPayment.CLS_ID = classID;
                _clsPayment.PAID_YEAR = paidYear;
                _clsPayment.PAID_MONTH = PaidMonth;
                _clsPayment.CARD_ISSUED_FLG = 1;
                _clsPayment.UpdateCardIssued();
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void chkCardIssue_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                DoPaymentDA _clsPayment = new DoPaymentDA();
                _clsPayment.STD_ID = studentId;
                _clsPayment.CLS_ID = classID;
                _clsPayment.PAID_YEAR = paidYear;
                _clsPayment.PAID_MONTH = PaidMonth;
                _clsPayment.CARD_ISSUED_FLG = 0;
                _clsPayment.UpdateCardIssued();
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void Label_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            try
            {
                lblChange.Visibility = System.Windows.Visibility.Hidden;
                lblUpdate.Visibility = System.Windows.Visibility.Visible;
                txtAmount.IsEnabled = true;
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void lblUpdate_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (txtAmount.Text.Trim() == "")
                {
                    MessageBox.Show("Class fee is required!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.No);
                    return;
                }
                if (double.Parse(txtAmount.Text.Trim()) < 0)
                {
                    MessageBox.Show("Please define a valid class fee!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.No);
                    return;
                }

                double paidYearamt = getPaidAmount(new DateTime(1, 1, 1));

                if ((double.Parse(txtAmount.Text.Trim()) < paidYearamt))
                {
                    MessageBox.Show("Define class fee is less than already paid amount!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.No);
                    return;
                }

                DoPaymentDA _clsPayment = new DoPaymentDA();
                _clsPayment.STD_ID = studentId;
                _clsPayment.CLS_ID = classID;
                _clsPayment.PAID_YEAR = paidYear;
                _clsPayment.PAID_MONTH = PaidMonth;
                _clsPayment.CLASS_FEE = double.Parse(txtAmount.Text.Trim());
                _clsPayment.UpdateClassFee();

                lblChange.Visibility = System.Windows.Visibility.Visible;
                lblUpdate.Visibility = System.Windows.Visibility.Hidden;
                txtAmount.IsEnabled = false;
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show("Do you want to delete this record?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    var selectedRow = grdPayHistory.SelectedItem as System.Data.DataRowView;
                    if (selectedRow != null)
                    {
                        DoPaymentDA _clsPayment = new DoPaymentDA();
                        _clsPayment.STD_ID = studentId;
                        _clsPayment.CLS_ID = classID;
                        _clsPayment.PAID_YEAR = paidYear;
                        _clsPayment.PAID_MONTH = PaidMonth;
                        _clsPayment.STD_REC_DATE = DateTime.Parse(selectedRow["STD_REC_DATE"].ToString()); ;
                        _clsPayment.DeletePayment();
                        BindPaymentHistoryGrid();
                    }
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void dtpDatePicke_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (rdoBtn != null)
                {
                    rdoBtn.IsChecked = false;
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ((FirstFloor.ModernUI.Windows.Controls.ModernWindow)this.Parent).Close();
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void cmbClsGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                List<ClassGroup> ClassGroups = new List<ClassGroup>();
                ClassGroups = (List<ClassGroup>)cmbClsGroup.ItemsSource;
                if (cmbClsGroup.SelectedIndex > -1 && cmbClsGroup.SelectedValue != null)
                {
                    Int32 classGroupID = Int32.Parse(cmbClsGroup.SelectedValue.ToString());
                    ClassGroup result = ClassGroups.Find(x => x.CLS_ID == classGroupID);
                    if (result != null)
                    {
                        txtAmount.Text = result.CLS_FEE.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void btnDeleteCalssfree_Click(object sender, RoutedEventArgs e)
        {
            try
            {                
                MessageBoxResult result = MessageBox.Show("Do you want to delete entire record with the payment?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    DoPaymentDA _clsPayment = new DoPaymentDA();
                    _clsPayment.STD_ID = studentId;
                    if (lblClass.Visibility == System.Windows.Visibility.Visible)
                    {
                        _clsPayment.CLS_ID = classID;
                        _clsPayment.PAID_YEAR = paidYear;
                        _clsPayment.PAID_MONTH = PaidMonth;
                        _clsPayment.DeleteEntirePaymentRecord();
                        ((FirstFloor.ModernUI.Windows.Controls.ModernWindow)this.Parent).Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void dtpDatePicke_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                dtpDatePicke.SelectedDate = DateTime.Now;
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
            }
        }
    }
}
