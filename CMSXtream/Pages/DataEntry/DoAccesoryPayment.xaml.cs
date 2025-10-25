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
    /// Interaction logic for DoAccesoryPayment.xaml
    /// </summary>
    public partial class DoAccesoryPayment : UserControl
    {
        public Int32 accID { get; set; }
        public Double accAmount { get; set; }
        public DateTime accRecDate { get; set; }
        public Int32 studentId { get; set; }
        public Int32 isCouseFee { get; set; }
        public Int32 isPackReady { get; set; }

        public DoAccesoryPayment()
        {
            InitializeComponent();
            isCouseFee = 0;
            if (StaticProperty.LoginisAdmin != "1")
            {
                dtpDatePicke.SelectedDate = DateTime.Now;
                dtpDatePicke.IsEnabled = false;
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        public void LoadFormContaint()
        {
            BindAccesoryGrid();
            BindCoureirPartners();
            if (accID != -1)
            {
                cmbAccesory.SelectedValue = accID;
                cmbAccesory_SelectionChanged(null, null);

                txtAmount.Text = accAmount.ToString();
                cmbAccesory.IsEnabled = false;
            }
            else
            {
                cmbAccesory.IsEnabled = true;
            }

            //dtpDatePicke.SelectedDate = accRecDate;
            if (accAmount > 0)
            {
                grdAssPayHistory.Visibility = System.Windows.Visibility.Visible;
                grpPaymentBox.Visibility = System.Windows.Visibility.Visible;
                BindPaymentHistoryGrid();
            }
            else
            {
                grdAssPayHistory.Visibility = System.Windows.Visibility.Hidden;
                grpPaymentBox.Visibility = System.Windows.Visibility.Hidden;
                txtPayment.Text = "0";
            }

            chkReadyForCourier.IsChecked = (isPackReady==1);
            SetQuoreirInfo();

        }

        private void BindPaymentHistoryGrid()
        {
            try
            {
                DoPaymentDA _clsPayment = new DoPaymentDA();
                _clsPayment.STD_ID = studentId;
                if (accID == -1)
                {
                    _clsPayment.ACC_ID = Int32.Parse(cmbAccesory.SelectedValue.ToString());
                }
                else
                {
                    _clsPayment.ACC_ID = accID;
                }
                _clsPayment.ASS_REC_DATE = accRecDate;
                System.Data.DataTable table = _clsPayment.SelectPaymentAccessory().Tables[0];
                if (table.Rows.Count > 0)
                {
                    grdAssPayHistory.ItemsSource = table.DefaultView;
                }
                else
                {
                    grdAssPayHistory.ItemsSource = null;
                }

            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void SetQuoreirInfo()
        {
            try
            {
                DoPaymentDA _clsPayment = new DoPaymentDA();
                _clsPayment.STD_ID = studentId;
                _clsPayment.ACC_ID = accID;
                _clsPayment.ASS_REC_DATE = accRecDate;

                System.Data.DataTable table = _clsPayment.SelectCuoreierInfo().Tables[0];
                if (table.Rows.Count > 0)
                {
                    cmbCourierPartner.SelectedValue = table.Rows[0]["COU_PARTNER_ID"].ToString();
                    txtTrackingNo.Text = table.Rows[0]["COU_TRACKING_NO"].ToString();
                    txtCourierComment.Text = table.Rows[0]["COU_COMMENT"].ToString();
                    dtpCourierDatePicke.SelectedDate = DateTime.Parse(table.Rows[0]["COU_DATE"].ToString());
                }
                else
                {
                    cmbCourierPartner.SelectedValue = 0;
                }

            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void BindAccesoryGrid()
        {
            try
            {

                AccessoryDA _Assosory = new AccessoryDA();
                System.Data.DataTable table = _Assosory.SelectAllAccessory().Tables[0];
                List<AccessoryAttribute> assosoryClass = new List<AccessoryAttribute>();
                foreach (System.Data.DataRow row in table.Rows)
                {
                    assosoryClass.Add(new AccessoryAttribute(Int32.Parse(row["ACC_ID"].ToString()), row["ACC_NAME"].ToString(), double.Parse(row["ACC_AMOUNT"].ToString()), Int32.Parse(row["ACC_PAYBLE_FLG"].ToString()), row["ACC_COMMENT"].ToString(), Int32.Parse(row["ACC_DELIVERABLE_FLG"].ToString())));
                }
                cmbAccesory.ClearValue(ItemsControl.ItemsSourceProperty);
                cmbAccesory.ItemsSource = assosoryClass;
                cmbAccesory.DisplayMemberPath = "ACC_NAME";
                cmbAccesory.SelectedValuePath = "ACC_ID";

            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }


        private void BindCoureirPartners()
        {
            try
            {

                AccessoryDA _Assosory = new AccessoryDA();
                System.Data.DataTable table = _Assosory.SelectAllCourierPtners().Tables[0];
                List<CureirPatner> assosoryClass = new List<CureirPatner>();
                foreach (System.Data.DataRow row in table.Rows)
                {
                    assosoryClass.Add(new CureirPatner(Int32.Parse(row["COU_PARTNER_ID"].ToString()), row["COU_PARTNER_NAME"].ToString()));
                }
                cmbCourierPartner.ClearValue(ItemsControl.ItemsSourceProperty);
                cmbCourierPartner.ItemsSource = assosoryClass;
                cmbCourierPartner.DisplayMemberPath = "COU_PARTNER_NAME";
                cmbCourierPartner.SelectedValuePath = "COU_PARTNER_ID";

            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void cmbAccesory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                List<AccessoryAttribute> AccessoryAtt = new List<AccessoryAttribute>();
                AccessoryAtt = (List<AccessoryAttribute>)cmbAccesory.ItemsSource;
                if (cmbAccesory.SelectedIndex >= 0 && cmbAccesory.SelectedValue != null)
                {
                    AccessoryAttribute result = AccessoryAtt.Find(x => x.ACC_ID == Int32.Parse(cmbAccesory.SelectedValue.ToString()));
                    if (result != null)
                    {
                        if (isCouseFee == 0)
                        {
                            txtAmount.Text = result.ACC_AMOUNT.ToString();
                        }
                    }
                    if (isCouseFee == 0)
                    {
                        if (result.ACC_AMOUNT > 0)
                        {
                            grdAssPayHistory.Visibility = System.Windows.Visibility.Visible;
                            grpPaymentBox.Visibility = System.Windows.Visibility.Visible;
                        }
                        else
                        {
                            grdAssPayHistory.Visibility = System.Windows.Visibility.Hidden;
                            grpPaymentBox.Visibility = System.Windows.Visibility.Hidden;
                            txtPayment.Text = "0";
                        }
                    }
                    else
                    {
                        grdAssPayHistory.Visibility = System.Windows.Visibility.Visible;
                        grpPaymentBox.Visibility = System.Windows.Visibility.Visible;
                        txtAmount.Text = "0";
                    }

                    if (result.ACC_DELIVERABLE_FLG == 1)
                    {
                        grpCoureirBox.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        grpCoureirBox.Visibility = Visibility.Hidden;
                    }

                }
                else{
                    grpCoureirBox.Visibility = Visibility.Hidden;
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

                if (cmbAccesory.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select a accessory!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.No);
                    return;
                }

                double paidYearamt = getPaidAmount(dtpDatePicke.SelectedDate.Value);

                if (double.Parse(txtAmount.Text.Trim()) > 0 & (double.Parse(txtAmount.Text.Trim()) <= paidYearamt))
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

                    if (double.Parse(txtAmount.Text.Trim()) > 0 & double.Parse(txtPayment.Text.Trim()) < 0)
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

                DoPaymentDA _clsPayment = new DoPaymentDA();
                _clsPayment.STD_ID = studentId;
                if (accID == -1)
                {
                    _clsPayment.ACC_ID = Int32.Parse(cmbAccesory.SelectedValue.ToString());
                }
                else
                {
                    _clsPayment.ACC_ID = accID;
                }
                _clsPayment.ASS_REC_DATE = accRecDate;
                _clsPayment.PMT_REC_DATE = dtpDatePicke.SelectedDate.Value;
                _clsPayment.MODIFY_USER = StaticProperty.LoginUserID;
                _clsPayment.PAID_AMOUNT = double.Parse(txtPayment.Text.Trim());
                _clsPayment.ASS_REC_AMOUNT = double.Parse(txtAmount.Text.Trim());
                _clsPayment.UpdatePaymentAccessory();
                BindPaymentHistoryGrid();
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
            int rowCount = grdAssPayHistory.Items.Count;
            for (int i = 0; i < rowCount; i++)
            {
                var selectedRow = (System.Data.DataRowView)grdAssPayHistory.Items[i];
                if (txtPayment != null)
                {

                    double PaidFree = Double.Parse(selectedRow["PAID_AMOUNT"].ToString());
                    DateTime selectedDate = DateTime.Parse(selectedRow["PMT_REC_DATE"].ToString());
                    if (selectedDate != selectDate)
                    {
                        paidgetPaidAmount += PaidFree;
                    }
                }
            }
            return paidgetPaidAmount;

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cmbAccesory.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select a accessory!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.No);
                    return;
                }

                //if (grdAssPayHistory.Visibility == System.Windows.Visibility.Visible)
                //{
                //    double paidYearamt = getPaidAmount(dtpDatePicke.SelectedDate.Value);
                //    if ((double.Parse(txtAmount.Text.Trim()) <= paidYearamt))
                //    {
                //        MessageBox.Show("Full payment already has been done!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.No);
                //        return;
                //    }

                //    if (rdoBtn.IsChecked.Value == false)
                //    {
                //        if (txtPayment.Text.Trim() == "")
                //        {
                //            MessageBox.Show("Payment amount is required!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.No);
                //            return;
                //        }
                //        if (double.Parse(txtPayment.Text.Trim()) <= 0)
                //        {
                //            MessageBox.Show("Please define a valid amount!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.No);
                //            return;
                //        }

                //        if ((double.Parse(txtAmount.Text.Trim()) - paidYearamt) < double.Parse(txtPayment.Text.Trim()))
                //        {
                //            MessageBox.Show("Payment should not be greater than balance to paid!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.No);
                //            return;
                //        }
                //    }
                //}

                DoPaymentDA _clsPayment = new DoPaymentDA();
                _clsPayment.STD_ID = studentId;
                if (accID == -1)
                {
                    _clsPayment.ACC_ID = Int32.Parse(cmbAccesory.SelectedValue.ToString());
                }
                else
                {
                    _clsPayment.ACC_ID = accID;
                }
                _clsPayment.ASS_REC_DATE = accRecDate;
                _clsPayment.ASS_REC_AMOUNT = double.Parse(txtAmount.Text.Trim());

                if (dtpCourierDatePicke.Text != string.Empty)
                {
                    _clsPayment.COU_DATE = dtpCourierDatePicke.SelectedDate.Value;
                }
                else
                {
                    _clsPayment.COU_DATE = DateTime.Now;
                }

                if (grpCoureirBox.Visibility == Visibility.Visible)
                {
                    _clsPayment.COU_PARTNER_ID = Int32.Parse(cmbCourierPartner.SelectedValue.ToString());
                }
                else
                {
                    _clsPayment.COU_PARTNER_ID = 0;
                }

                _clsPayment.COU_TRACKING_NO = txtTrackingNo.Text.Trim().ToString();
                _clsPayment.COU_COMMENT = txtCourierComment.Text.Trim().ToString();
                _clsPayment.ASS_PACK_READY = 0;
                if (chkReadyForCourier.IsChecked.Value)
                {
                    _clsPayment.ASS_PACK_READY = 1;
                }
                _clsPayment.UpdateStudentAccessory();
                ((FirstFloor.ModernUI.Windows.Controls.ModernWindow)this.Parent).Close();
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
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

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show("Do you want to delete this record?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    var selectedRow = grdAssPayHistory.SelectedItem as System.Data.DataRowView;
                    if (selectedRow != null)
                    {
                        DoPaymentDA _clsPayment = new DoPaymentDA();
                        _clsPayment.STD_ID = studentId;
                        if (accID == -1)
                        {
                            _clsPayment.ACC_ID = Int32.Parse(cmbAccesory.SelectedValue.ToString());
                        }
                        else
                        {
                            _clsPayment.ACC_ID = accID;
                        }
                        _clsPayment.ASS_REC_DATE = accRecDate;
                        _clsPayment.PMT_REC_DATE = DateTime.Parse(selectedRow["PMT_REC_DATE"].ToString());
                        _clsPayment.DeletePaymentAccessory();
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

        private void Label_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (cmbAccesory.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select a accessory!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.No);
                    return;
                }

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
                    MessageBox.Show("Amount is required!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.No);
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
                    MessageBox.Show("Define amount is less than already paid amount!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.No);
                    return;
                }

                DoPaymentDA _clsPayment = new DoPaymentDA();
                _clsPayment.STD_ID = studentId;
                if (accID == -1)
                {
                    _clsPayment.ACC_ID = Int32.Parse(cmbAccesory.SelectedValue.ToString());
                }
                else
                {
                    _clsPayment.ACC_ID = accID;
                }
                _clsPayment.ASS_REC_DATE = accRecDate;
                _clsPayment.ASS_REC_AMOUNT = double.Parse(txtAmount.Text.Trim());
                _clsPayment.UpdateAccessoryAmount();

                grpPaymentBox.Visibility = Visibility.Visible;
                grdAssPayHistory.Visibility = Visibility.Visible;

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

        private void cmbCourierPartner_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cmbCourierPartner.SelectedIndex != 0)
                {
                    dtpCourierDatePicke.IsEnabled = true;
                    txtTrackingNo.IsEnabled = true;
                    txtCourierComment.IsEnabled = true;
                    dtpCourierDatePicke.SelectedDate = DateTime.Now;
                }
                else
                {

                    txtTrackingNo.Text = string.Empty;
                    txtCourierComment.Text = string.Empty;
                    dtpCourierDatePicke.Text = string.Empty;

                    dtpCourierDatePicke.IsEnabled = false;
                    txtTrackingNo.IsEnabled = false;
                    txtCourierComment.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void chkReadyForCourier_Click(object sender, RoutedEventArgs e)
        {
            if (!chkReadyForCourier.IsChecked.Value)
            {
                cmbCourierPartner.SelectedIndex = 0; 
            }
        }
    }
}
