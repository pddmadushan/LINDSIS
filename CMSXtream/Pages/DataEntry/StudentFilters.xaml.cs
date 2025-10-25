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
    /// Interaction logic for StudentFilters.xaml
    /// </summary>
    public partial class StudentFilters : UserControl
    {
        public CMSXtream.Pages.DataEntry.SendSMS stdSum { get; set; }
        public int doRefresh { get; set; }
        public StudentFilters()
        {
            InitializeComponent();

            txtSearchText.Text = "";

            cmbSearchType.DisplayMemberPath = "Value";
            cmbSearchType.SelectedValuePath = "Key";
            cmbSearchType.Items.Add(new KeyValuePair<string, string>("0", "--All--"));
            cmbSearchType.Items.Add(new KeyValuePair<string, string>("1", "ID"));
            cmbSearchType.Items.Add(new KeyValuePair<string, string>("2", "Name"));
            cmbSearchType.Items.Add(new KeyValuePair<string, string>("3", "NIC"));
            cmbSearchType.Items.Add(new KeyValuePair<string, string>("4", "Telephone"));
            cmbSearchType.Items.Add(new KeyValuePair<string, string>("5", "Class Group"));

            cmbSearchType.SelectedIndex = 5;
            cmbClassGroup.SelectedIndex = -1;

            cmbStatus.DisplayMemberPath = "Value";
            cmbStatus.SelectedValuePath = "Key";
            cmbStatus.Items.Add(new KeyValuePair<string, string>("2", "--All--"));
            cmbStatus.Items.Add(new KeyValuePair<string, string>("1", "Attended"));
            cmbStatus.Items.Add(new KeyValuePair<string, string>("0", "Not Attended"));
            cmbStatus.SelectedIndex = 0;

            LoadClassCombo();
            LoadClassCatogory();
            LoadReason();
            LoadIntake();
            MandotoryAccesory();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void LoadClassCatogory()
        {
            ClassCategoryDA _clsClassGroup = new ClassCategoryDA();
            System.Data.DataTable table = _clsClassGroup.SelectAllCategory().Tables[0];
            cmbClsCatogory.DisplayMemberPath = "CAT_NAME";
            cmbClsCatogory.SelectedValuePath = "CAT_ID";
            cmbClsCatogory.ItemsSource = table.DefaultView;

            if (table.Rows.Count > 1)
            {
                cmbClsCatogory.SelectedIndex = 1;
            }
        }
        
        private void LoadIntake()
        {
            ClassGroupDA _clsReason = new ClassGroupDA();
            System.Data.DataTable table = _clsReason.SelectAllIntake().Tables[0];
            cmbIntake.DisplayMemberPath = "CLS_NAME";
            cmbIntake.SelectedValuePath = "CLS_ID";            

            if (table.Rows.Count > 0)
            {
                cmbIntake.ItemsSource = table.DefaultView;
            }
            else
            {
                cmbIntake.ItemsSource = null;
            }
        }

        private void LoadSubClass()
        {
            ClassGroupDA _clsReason = new ClassGroupDA();
            int calssId = 0;
            if (cmbIntake.SelectedIndex != -1)
            {
                calssId = Int32.Parse(cmbIntake.SelectedValue.ToString());
            }


            _clsReason.classFeilds.CLS_ID = calssId;
            System.Data.DataTable table = _clsReason.SelectAllSubClass().Tables[0];
            cmbSubClass.DisplayMemberPath = "CLS_NAME";
            cmbSubClass.SelectedValuePath = "CLS_ID";

            if (table.Rows.Count > 0)
            {
                cmbSubClass.SelectedIndex = 0;
                System.Data.DataRow toInsert = table.NewRow();
                toInsert["CLS_ID"] = 0;
                toInsert["CLS_NAME"] = "None";
                table.Rows.InsertAt(toInsert, 0);
                toInsert = table.NewRow();
                toInsert["CLS_ID"] = -1;
                toInsert["CLS_NAME"] = "All";
                table.Rows.InsertAt(toInsert, 0);
                cmbSubClass.ItemsSource = table.DefaultView;
            }
            else
            {
                cmbSubClass.ItemsSource = null;
            }
        }

        //private void LoadLable()
        //{
        //    LableDA _clsReason = new LableDA();
        //    System.Data.DataTable table = _clsReason.SelectLable().Tables[0];
        //    cmbLabel.DisplayMemberPath = "LBL_DES";
        //    cmbLabel.SelectedValuePath = "LBL_ID";

        //    if (table.Rows.Count > 0)
        //    {
        //        cmbLabel.ItemsSource = table.DefaultView;
        //    }
        //    else
        //    {
        //        cmbLabel.ItemsSource = null;
        //    }
        //}

        private void LoadReason()
        {
            ResonDA _clsReason = new ResonDA();
            System.Data.DataTable table = _clsReason.SelectReason().Tables[0];
            cmbInactiveReason.DisplayMemberPath = "RSN_DES";
            cmbInactiveReason.SelectedValuePath = "RSN_ID";

            if (table.Rows.Count > 0)
            {
                cmbInactiveReason.SelectedIndex = 0;

                System.Data.DataRow toInsert = table.NewRow();
                toInsert["RSN_ID"] = -1;
                toInsert["RSN_DES"] = "Inactive but Reason not assigned";
                table.Rows.InsertAt(toInsert, 0);

                cmbInactiveReason.ItemsSource = table.DefaultView;
            }
            else
            {
                cmbInactiveReason.ItemsSource = null;
            }
        }

        private void LoadClassCombo()
        {
            try
            {
                StudentDA _clsStudent = new StudentDA();
                System.Data.DataTable table = _clsStudent.SelectAllClassGroup(1).Tables[0];
                cmbClassGroup.ClearValue(ItemsControl.ItemsSourceProperty);
                cmbClassGroup.ItemsSource = table.DefaultView;
                cmbClassGroup.DisplayMemberPath = "CLS_NAME";
                cmbClassGroup.SelectedValuePath = "CLS_ID";

                cmbClassGroup1.ClearValue(ItemsControl.ItemsSourceProperty);
                cmbClassGroup1.ItemsSource = table.DefaultView;
                cmbClassGroup1.DisplayMemberPath = "CLS_NAME";
                cmbClassGroup1.SelectedValuePath = "CLS_ID";

            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void MandotoryAccesory()
        {
            try
            {
                StudentDA _clsStudent = new StudentDA();
                System.Data.DataTable table = _clsStudent.SelectMendotoryAccesory().Tables[0];
                cmbSearchAccesory.ClearValue(ItemsControl.ItemsSourceProperty);
                cmbSearchAccesory.ItemsSource = table.DefaultView;
                cmbSearchAccesory.DisplayMemberPath = "ACC_NAME";
                cmbSearchAccesory.SelectedValuePath = "ACC_ID";

            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void cmbSearchType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cmbSearchType.SelectedValue.ToString() == "0")
                {
                    txtSearchText.Text = string.Empty;
                    txtSearchText.IsEnabled = false;
                }
                else
                {
                    txtSearchText.IsEnabled = true;
                }
                if (cmbSearchType.SelectedValue.ToString() == "5")
                {
                    txtSearchText.Visibility = System.Windows.Visibility.Hidden;
                    cmbClassGroup.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    txtSearchText.Visibility = System.Windows.Visibility.Visible;
                    cmbClassGroup.Visibility = System.Windows.Visibility.Hidden;
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void btnAddList_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SMSDA _clsStudent = new SMSDA();
                int filterID = int.Parse(cmbSearchType.SelectedValue.ToString());
                string fiterString = txtSearchText.Text.Trim();
                string fiterDisString = txtSearchText.Text.Trim();
                if (filterID == 5)
                {
                    if (cmbClassGroup.SelectedIndex != -1)
                    {
                        fiterString = cmbClassGroup.SelectedValue.ToString();
                        fiterDisString = cmbClassGroup.Text.ToString();
                    }
                }
                System.Data.DataTable table = _clsStudent.Option01Student(filterID, fiterString).Tables[0];
                stdSum.filteredTable = table;
                stdSum.filteredString = "Searched by " + cmbSearchType.Text.ToString() + "[" + fiterDisString.ToString() + "]";
                doRefresh = 1;
                ((FirstFloor.ModernUI.Windows.Controls.ModernWindow)this.Parent).Close();

            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void btnAddList2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DateTime fromDate = DateTime.Parse(FromDate.SelectedDate.Value.ToShortDateString());
                DateTime toDate = DateTime.Parse(ToDate.SelectedDate.Value.ToShortDateString());
                SMSDA _clsStudent = new SMSDA();
                System.Data.DataTable table = _clsStudent.Option02Student(fromDate, toDate).Tables[0];
                stdSum.filteredTable = table;    
                stdSum.filteredString = "Searched by Start Date" + "[" + FromDate.SelectedDate.Value.ToShortDateString().ToString() + " To " + ToDate.SelectedDate.Value.ToShortDateString().ToString() + "]";
                doRefresh = 1;
                ((FirstFloor.ModernUI.Windows.Controls.ModernWindow)this.Parent).Close();
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void btnAddList3_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Int32 classCatId = Int32.Parse( cmbClsCatogory.SelectedValue.ToString());
                if (txtWeekNumber.Text.Trim() == string.Empty) { txtWeekNumber.Text = "0"; } 
                Int32 ClasWeek = Int32.Parse(txtWeekNumber.Text);
                Int32 chkActiveFlg = chkActive.IsChecked.Value ? 1 : 0 ;
                SMSDA _clsStudent = new SMSDA();
                System.Data.DataTable table = _clsStudent.Option03Student(classCatId, ClasWeek, chkActiveFlg).Tables[0];
                stdSum.filteredTable = table;
                stdSum.filteredString = "Searched by Absent Week" + "[Class: " + cmbClsCatogory.Text.ToString() + "][Week: " + ClasWeek.ToString() + "][Active: " + chkActiveFlg.ToString() + "]";
                doRefresh = 1;
                ((FirstFloor.ModernUI.Windows.Controls.ModernWindow)this.Parent).Close();
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void btnAddList4_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dtpClassDate.SelectedDate == null)
                {
                    MessageBox.Show("Attendance Date is required!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.No);
                    return;
                }

                if (cmbClsFromAttendance.SelectedIndex == -1)
                {
                    MessageBox.Show("Select a Class!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.No);
                    return;
                }

                Int32 classId = Int32.Parse(cmbClsFromAttendance.SelectedValue.ToString());
                Int32 classStatus = Int32.Parse(cmbStatus.SelectedValue.ToString());
                DateTime classDate = DateTime.Parse(dtpClassDate.SelectedDate.Value.ToShortDateString());
                SMSDA _clsStudent = new SMSDA();
                System.Data.DataTable table = _clsStudent.Option04Student(classId, classDate, classStatus).Tables[0];
                stdSum.filteredTable = table;
                stdSum.filteredString = "Searched by Attendance" + "[Date: " + dtpClassDate.SelectedDate.Value.ToShortDateString() + "][Class: " + cmbClsFromAttendance.Text.ToString() + "][Status: " + cmbStatus.Text.ToString() + "]";
                doRefresh = 1;
                ((FirstFloor.ModernUI.Windows.Controls.ModernWindow)this.Parent).Close();
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }     

        private void dtpClassDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cmbClsFromAttendance != null)
                {
                    cmbClsFromAttendance.SelectedIndex = -1;

                    SMSDA _clsStudent = new SMSDA();
                    DateTime classDate = DateTime.Parse(dtpClassDate.SelectedDate.Value.ToShortDateString());
                    System.Data.DataTable table = _clsStudent.SelectClassFromAttendance(classDate).Tables[0];
                    cmbClsFromAttendance.ItemsSource = table.DefaultView;
                    cmbClsFromAttendance.DisplayMemberPath = "CLS_NAME";
                    cmbClsFromAttendance.SelectedValuePath = "CLS_ID";

                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void btnAddList5_Click(object sender, RoutedEventArgs e)
        {
            if (cmbInactiveReason.SelectedIndex == -1)
            {
                MessageBox.Show("Select a Reason!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.No);
                return;
            }
            Int32 ReasonId = Int32.Parse(cmbInactiveReason.SelectedValue.ToString());
            Int32 InactiveFlg = chkAllInactive.IsChecked.Value ? 1 : (chkInactive.IsChecked.Value ? 2 : 0);
            String InactiveString = chkAllInactive.IsChecked.Value ? "Any Inactive" : (chkInactive.IsChecked.Value ? "Temporarily Inactive" : "Permanently Inactive");
            SMSDA _clsStudent = new SMSDA();
            System.Data.DataTable table = _clsStudent.Option05Student(ReasonId, InactiveFlg).Tables[0];
            stdSum.filteredTable = table;
            stdSum.filteredString = "Searched by Reason" + "[ " + cmbInactiveReason.Text + "][ " + InactiveString.ToString() + "]";
            doRefresh = 1;
            ((FirstFloor.ModernUI.Windows.Controls.ModernWindow)this.Parent).Close();
        }

        private void btnAddList6_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cmbIntake.SelectedIndex == -1)
                {
                    MessageBox.Show("Select a Intake!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.No);
                    return;
                }
                Int32 ClassId = Int32.Parse(cmbIntake.SelectedValue.ToString());
                Int32 SubClassId = Int32.Parse(cmbSubClass.SelectedValue.ToString());
                SMSDA _clsStudent = new SMSDA();
                System.Data.DataTable table = _clsStudent.Option06Student(ClassId, SubClassId).Tables[0];
                stdSum.filteredTable = table;
                stdSum.filteredString = "Searched by Intake" + "[ " + cmbIntake.Text + "][ " + cmbSubClass.Text + "]";
                doRefresh = 1;
                ((FirstFloor.ModernUI.Windows.Controls.ModernWindow)this.Parent).Close();
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void btnAddList7_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cmbClassAbsant.SelectedIndex == -1)
                {
                    MessageBox.Show("Select a class!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.No);
                    return;
                }
                Int32 classId = Int32.Parse(cmbClassAbsant.SelectedValue.ToString());
                DateTime classDate = DateTime.Parse(dtpClassDateAbsant.SelectedDate.Value.ToShortDateString());
                SMSDA _clsStudent = new SMSDA();
                System.Data.DataTable table = _clsStudent.Option07Student(classId, classDate).Tables[0];
                stdSum.filteredTable = table;
                stdSum.filteredString = "Searched by present and absent after" + "[ " + classDate.ToShortDateString() + "," + cmbClassAbsant.Text + "]";
                doRefresh = 1;
                ((FirstFloor.ModernUI.Windows.Controls.ModernWindow)this.Parent).Close();
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void btnAddList8_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SMSDA _clsStudent = new SMSDA();
                System.Data.DataTable table = _clsStudent.Option08Student().Tables[0];
                stdSum.filteredTable = table;
                stdSum.filteredString = "Searched by never attended  ";
                doRefresh = 1;
                ((FirstFloor.ModernUI.Windows.Controls.ModernWindow)this.Parent).Close();
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void cmbIntake_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                LoadSubClass();
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void dtpClassDateAbsant_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                SMSDA _clsStudent = new SMSDA();
                DateTime classDate = DateTime.Parse(dtpClassDateAbsant.SelectedDate.Value.ToShortDateString());
                System.Data.DataTable table = _clsStudent.SelectClassFromAttendance(classDate).Tables[0];
                cmbClassAbsant.ItemsSource = table.DefaultView;
                cmbClassAbsant.DisplayMemberPath = "CLS_NAME";
                cmbClassAbsant.SelectedValuePath = "CLS_ID";
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void btnAddList09_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (cmbClassGroup1.SelectedIndex == -1)
                {
                    MessageBox.Show("Select a class!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.No);
                    return;
                }

                if (cmbSearchAccesory.SelectedIndex == -1)
                {
                    MessageBox.Show("Select a accessory!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.No);
                    return;
                }

                Int32 minPayment = 0; 
                if (txtPayamount.Text.Trim() != "")
                {
                    minPayment = Int32.Parse(txtPayamount.Text.Trim());
                }               

                Int32 classId = Int32.Parse(cmbClassGroup1.SelectedValue.ToString());
                Int32 accossryId = Int32.Parse(cmbSearchAccesory.SelectedValue.ToString());

                SMSDA _clsStudent = new SMSDA();
                System.Data.DataTable table = _clsStudent.Option09Student(classId, accossryId, minPayment).Tables[0];
                stdSum.filteredTable = table;
                stdSum.filteredString = "Searched by pending shipment" + "[" + cmbClassGroup1.Text + ", " + cmbSearchAccesory.Text + ", " + minPayment.ToString() + "]";;
                doRefresh = 1;
                ((FirstFloor.ModernUI.Windows.Controls.ModernWindow)this.Parent).Close();
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }
        private void btnAddList10_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SMSDA _clsStudent = new SMSDA();
                Int32 ClasWeek = Int32.Parse(txtMinPayamount.Text);
                System.Data.DataTable table = _clsStudent.Option10Student(ClasWeek).Tables[0];
                stdSum.filteredTable = table;
                stdSum.filteredString = "Searched by pending payments for course fee";
                doRefresh = 1;
                ((FirstFloor.ModernUI.Windows.Controls.ModernWindow)this.Parent).Close();
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
