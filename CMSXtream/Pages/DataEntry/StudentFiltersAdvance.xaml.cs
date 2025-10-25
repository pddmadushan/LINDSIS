using Microsoft.Office.Interop.Excel;
using System;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using XtreamDataAccess;

namespace CMSXtream.Pages.DataEntry
{
    /// <summary>
    /// Interaction logic for StudentFiltersAdvance.xaml
    /// </summary>
    public partial class StudentFiltersAdvance : UserControl
    {
        Boolean isCheckAllGroup = false;
        System.Data.DataTable ExportDataTbl;

        public StudentFiltersAdvance()
        {
            InitializeComponent();
            BindClassGrid();
            BindLabelGrid();
        }

        private void BindClassGrid()
        {
            try
            {
                ClassGroupDA _clsAccessory = new ClassGroupDA();
                System.Data.DataTable table = _clsAccessory.SelectAllClass().Tables[0];
                if (table.Rows.Count > 0)
                {
                    grdGroup.ItemsSource = table.DefaultView;
                }
                else
                {
                    grdGroup.ItemsSource = null;
                }

            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void BindLabelGrid()
        {
            try
            {
                LableDA _clsReason = new LableDA();
                System.Data.DataTable table = _clsReason.SelectLable().Tables[0];
                if (table.Rows.Count > 0)
                {
                    grdLable.ItemsSource = table.DefaultView;
                }
                else
                {
                    grdLable.ItemsSource = null;
                }

            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void btnAddStudentBySearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CMSXtream.Pages.DataEntry.StudentFilters form = new CMSXtream.Pages.DataEntry.StudentFilters();
                PopupHelper dialog = new PopupHelper
                {
                    Title = "Filter Students",
                    Content = form,
                    ResizeMode = ResizeMode.NoResize,
                    Width = 800
                };

                SendSMS _objSMS = new SendSMS();
                form.stdSum = _objSMS;
                //search by reason should be desable
                form.grpBoxStudent5.IsEnabled = false;
                dialog.ShowDialog();
                if (form.doRefresh == 1)
                {
                    BindSubStudentGrid(_objSMS.filteredTable);
                    filterString.Content = _objSMS.filteredString;
                    filterString.ToolTip = _objSMS.filteredString;
                    FristCellCheckUncheck(grdStudent, true);
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        public void BindSubStudentGrid(System.Data.DataTable table)
        {
            try
            {
                if (table != null)
                {
                    if (table.Rows.Count > 0)
                    {
                        grdStudent.ItemsSource = table.DefaultView;
                    }
                    else
                    {
                        grdStudent.ItemsSource = null;
                    }
                }
                else
                {
                    grdStudent.ItemsSource = null;
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void btnStdntView_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CMSXtream.Pages.DataEntry.Student form = new CMSXtream.Pages.DataEntry.Student();
                PopupHelper dialog = new PopupHelper
                {
                    Title = "View Student",
                    Content = form,
                    ResizeMode = ResizeMode.NoResize,
                    Width = 1000
                };

                StudentAttribute stAttPass = new StudentAttribute();
                var selectedRow = grdStudent.SelectedItem as System.Data.DataRowView;
                if (selectedRow != null)
                {
                    stAttPass.STD_ID = int.Parse(selectedRow["STD_ID"].ToString());

                    StudentDA _clsStudent = new StudentDA();
                    _clsStudent.STD_ID = stAttPass.STD_ID;

                    System.Data.DataTable table = _clsStudent.SelectStudentDetails().Tables[0];
                    if (table.Rows.Count > 0)
                    {
                        stAttPass.CLS_ID = int.Parse(table.Rows[0]["CLS_ID"].ToString());
                        stAttPass.CLS_NAME = table.Rows[0]["CLS_NAME"].ToString();
                        stAttPass.STD_INITIALS = table.Rows[0]["STD_INITIALS"].ToString();
                        stAttPass.STD_SURNAME = table.Rows[0]["STD_SURNAME"].ToString();
                        stAttPass.STD_FULL_NAME = table.Rows[0]["STD_FULL_NAME"].ToString();
                        stAttPass.STD_GENDER = int.Parse(table.Rows[0]["STD_GENDER"].ToString());
                        stAttPass.STD_DATEOFBIRTH = DateTime.Parse(table.Rows[0]["STD_DATEOFBIRTH"].ToString());
                        stAttPass.STD_JOIN_DATE = DateTime.Parse(table.Rows[0]["STD_JOIN_DATE"].ToString());
                        stAttPass.STD_EMAIL = table.Rows[0]["STD_EMAIL"].ToString();
                        stAttPass.STD_NIC = table.Rows[0]["STD_NIC"].ToString();
                        stAttPass.STD_ADDRESS = table.Rows[0]["STD_ADDRESS"].ToString();
                        stAttPass.STD_CLASS_FEE = Double.Parse(table.Rows[0]["STD_CLASS_FEE"].ToString());
                        stAttPass.STD_TELEPHONE = table.Rows[0]["STD_TELEPHONE"].ToString();
                        stAttPass.STD_ACTIVE_FLG = table.Rows[0]["STD_ACTIVE_FLG"].ToString() == "" ? 2 : int.Parse(table.Rows[0]["STD_ACTIVE_FLG"].ToString());
                        stAttPass.STD_COMMENT = table.Rows[0]["STD_COMMENT"].ToString();
                        stAttPass.STD_TEMP_NOTE = table.Rows[0]["STD_TEMP_NOTE"].ToString();
                        stAttPass.RSN_ID = int.Parse(table.Rows[0]["RSN_ID"].ToString());
                    }
                    else
                    {
                        return;
                    }
                }
                form.IsAddNew = false;
                form.stAtt = stAttPass;
                form.LoadFormContaint();
                dialog.ShowDialog();
                string ReturnMessage = form.OutResult;
                if (ReturnMessage != string.Empty && ReturnMessage != null)
                {
                    MessageBox.Show(ReturnMessage, StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void btnSearchStudent_Click(object sender, RoutedEventArgs e)
        {
            int rowCount = 0;
            lblCount.Content = "";
            try
            {
                System.Data.DataTable dtClassGrou = new System.Data.DataTable();
                dtClassGrou.Columns.Add("CLS_ID", typeof(Int32));
                rowCount = grdGroup.Items.Count;
                if (rowCount > 0)
                {
                    for (int i = 0; i < rowCount; i++)
                    {
                        var selectedRow = (System.Data.DataRowView)grdGroup.Items[i];
                        int classId = Int32.Parse(selectedRow["CLS_ID"].ToString());
                        DataGridCell cell = GetCell(i, 0, grdGroup);
                        System.Windows.Controls.CheckBox chk = GetVisualChild<System.Windows.Controls.CheckBox>(cell);
                        if (chk.IsChecked.Value)
                        {
                            dtClassGrou.Rows.Add(classId);
                        }
                    }
                }

                System.Data.DataTable dtLable = new System.Data.DataTable();
                dtLable.Columns.Add("LBL_ID", typeof(Int32));
                rowCount = grdLable.Items.Count;
                if (rowCount > 0)
                {
                    for (int i = 0; i < rowCount; i++)
                    {
                        var selectedRow = (System.Data.DataRowView)grdLable.Items[i];
                        int labelId = Int32.Parse(selectedRow["LBL_ID"].ToString());
                        DataGridCell cell = GetCell(i, 0, grdLable);
                        System.Windows.Controls.CheckBox chk = GetVisualChild<System.Windows.Controls.CheckBox>(cell);
                        if (chk.IsChecked.Value)
                        {
                            dtLable.Rows.Add(labelId);
                        }
                    }
                }

                System.Data.DataTable dtLableN = new System.Data.DataTable();
                dtLableN.Columns.Add("LBL_ID", typeof(Int32));
                if (rowCount > 0)
                {
                    for (int i = 0; i < rowCount; i++)
                    {
                        var selectedRow = (System.Data.DataRowView)grdLable.Items[i];
                        int labelId = Int32.Parse(selectedRow["LBL_ID"].ToString());
                        DataGridCell cell = GetCell(i, 6, grdLable);
                        System.Windows.Controls.CheckBox chk = GetVisualChild<System.Windows.Controls.CheckBox>(cell);
                        if (chk.IsChecked.Value)
                        {
                            dtLableN.Rows.Add(labelId);
                        }
                    }
                }

                System.Data.DataTable dtStudent = new System.Data.DataTable();
                dtStudent.Columns.Add("STD_ID", typeof(Int32));
                rowCount = grdStudent.Items.Count;
                if (rowCount > 0)
                {
                    for (int i = 0; i < rowCount; i++)
                    {
                        var selectedRow = (System.Data.DataRowView)grdStudent.Items[i];
                        int studentId = Int32.Parse(selectedRow["STD_ID"].ToString());
                        DataGridCell cell = GetCell(i, 0, grdStudent);
                        System.Windows.Controls.CheckBox chk = GetVisualChild<System.Windows.Controls.CheckBox>(cell);
                        if (chk.IsChecked.Value)
                        {
                            dtStudent.Rows.Add(studentId);
                        }
                    }
                }

                if (dtClassGrou.Rows.Count == 0 && dtLable.Rows.Count == 0 && dtStudent.Rows.Count == 0 && dtLableN.Rows.Count == 0)
                {
                    MessageBoxResult result = MessageBox.Show("No filtration found. Do you want to load all active student?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.No)
                    {
                        return;
                    }
                }

                lblCount.Content = "Class : " + dtClassGrou.Rows.Count.ToString() + " , Lable in : " + dtLable.Rows.Count.ToString() + " , Lable not in : " + dtLableN.Rows.Count.ToString();

                SMSDA _objSMS = new SMSDA();
                System.Data.DataTable table = _objSMS.SMSAdvanceFilter(dtClassGrou, dtLable, dtStudent, dtLableN);
                if (table.Rows.Count > 0)
                {
                    grdSMSList.ItemsSource = table.DefaultView;
                    ExportDataTbl = table;
                }
                else
                {
                    grdSMSList.ItemsSource = null;
                }

            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        public DataGridCell GetCell(int row, int column, DataGrid dtGrid)
        {
            DataGridRow rowContainer = GetRow(row, dtGrid);
            if (rowContainer != null)
            {
                DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(rowContainer);
                if (presenter == null)
                {
                    dtGrid.ScrollIntoView(rowContainer, dtGrid.Columns[column]);
                    presenter = GetVisualChild<DataGridCellsPresenter>(rowContainer);
                }
                DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                return cell;
            }
            return null;
        }
        public DataGridRow GetRow(int index, DataGrid dtGrid)
        {
            DataGridRow row = (DataGridRow)dtGrid.ItemContainerGenerator.ContainerFromIndex(index);
            if (row == null)
            {
                dtGrid.UpdateLayout();
                dtGrid.ScrollIntoView(dtGrid.Items[index]);
                row = (DataGridRow)dtGrid.ItemContainerGenerator.ContainerFromIndex(index);
            }
            return row;
        }
        public static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>
                    (v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }

        private void FristCellCheckUncheck(DataGrid dtGrid, Boolean checkStatus)
        {
            int rowCount = 0;
            try
            {
                rowCount = dtGrid.Items.Count;
                if (rowCount > 0)
                {
                    for (int i = 0; i < rowCount; i++)
                    {
                        DataGridCell cell = GetCell(i, 0, dtGrid);
                        System.Windows.Controls.CheckBox chk = GetVisualChild<System.Windows.Controls.CheckBox>(cell);
                        chk.IsChecked = checkStatus;
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
        private void FristCellCheckUncheckN(DataGrid dtGrid, Boolean checkStatus)
        {
            int rowCount = 0;
            try
            {
                rowCount = dtGrid.Items.Count;
                if (rowCount > 0)
                {
                    for (int i = 0; i < rowCount; i++)
                    {
                        DataGridCell cell = GetCell(i, 6, dtGrid);
                        System.Windows.Controls.CheckBox chk = GetVisualChild<System.Windows.Controls.CheckBox>(cell);
                        chk.IsChecked = checkStatus;
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

        private void chkGrpMark_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                isCheckAllGroup = true;
                FristCellCheckUncheck(grdGroup, true);
                txtGroupCount.Content = grdGroup.Items.Count.ToString();
                txtGroupCount.ToolTip = "";
                isCheckAllGroup = false;
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void chkGrpMark_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                isCheckAllGroup = true;
                FristCellCheckUncheck(grdGroup, false);
                txtGroupCount.Content = "0";
                txtGroupCount.ToolTip = "";
                isCheckAllGroup = false;
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }

        }

        private void chkLblMarkAll_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                isCheckAllGroup = true;
                FristCellCheckUncheck(grdLable, true);
                lblIncount.Content = grdLable.Items.Count.ToString();
                isCheckAllGroup = false;
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void chkLblMarkAll_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                isCheckAllGroup = true;
                FristCellCheckUncheck(grdLable, false);
                lblIncount.Content = "0";
                isCheckAllGroup = false;
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }
        private void chkLblMarkAllN_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                isCheckAllGroup = true;
                FristCellCheckUncheckN(grdLable, true);
                lblOntIncount.Content = grdLable.Items.Count.ToString();
                isCheckAllGroup = false;
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void chkLblMarkAllN_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                isCheckAllGroup = true;
                FristCellCheckUncheckN(grdLable, false);
                lblOntIncount.Content = "0";
                isCheckAllGroup = false;
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void chkStdntMarkAll_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                FristCellCheckUncheck(grdStudent, false);
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }

        }

        private void chkStdntMarkAll_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                FristCellCheckUncheck(grdStudent, true);
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void chkSMSMarkAll_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                FristCellCheckUncheck(grdSMSList, true);
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void chkSMSMarkAll_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                FristCellCheckUncheck(grdSMSList, false);
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CMSXtream.Pages.DataEntry.Student form = new CMSXtream.Pages.DataEntry.Student();
                PopupHelper dialog = new PopupHelper
                {
                    Title = "View Student",
                    Content = form,
                    ResizeMode = ResizeMode.NoResize,
                    Width = 1000
                };

                StudentAttribute stAttPass = new StudentAttribute();
                var selectedRow = grdSMSList.SelectedItem as System.Data.DataRowView;
                if (selectedRow != null)
                {
                    stAttPass.STD_ID = int.Parse(selectedRow["STD_ID"].ToString());

                    StudentDA _clsStudent = new StudentDA();
                    _clsStudent.STD_ID = stAttPass.STD_ID;

                    System.Data.DataTable table = _clsStudent.SelectStudentDetails().Tables[0];
                    if (table.Rows.Count > 0)
                    {
                        stAttPass.CLS_ID = int.Parse(table.Rows[0]["CLS_ID"].ToString());
                        stAttPass.CLS_NAME = table.Rows[0]["CLS_NAME"].ToString();
                        stAttPass.STD_INITIALS = table.Rows[0]["STD_INITIALS"].ToString();
                        stAttPass.STD_SURNAME = table.Rows[0]["STD_SURNAME"].ToString();
                        stAttPass.STD_FULL_NAME = table.Rows[0]["STD_FULL_NAME"].ToString();
                        stAttPass.STD_GENDER = int.Parse(table.Rows[0]["STD_GENDER"].ToString());
                        stAttPass.STD_DATEOFBIRTH = DateTime.Parse(table.Rows[0]["STD_DATEOFBIRTH"].ToString());
                        stAttPass.STD_JOIN_DATE = DateTime.Parse(table.Rows[0]["STD_JOIN_DATE"].ToString());
                        stAttPass.STD_EMAIL = table.Rows[0]["STD_EMAIL"].ToString();
                        stAttPass.STD_NIC = table.Rows[0]["STD_NIC"].ToString();
                        stAttPass.STD_ADDRESS = table.Rows[0]["STD_ADDRESS"].ToString();
                        stAttPass.STD_CLASS_FEE = Double.Parse(table.Rows[0]["STD_CLASS_FEE"].ToString());
                        stAttPass.STD_TELEPHONE = table.Rows[0]["STD_TELEPHONE"].ToString();
                        stAttPass.STD_ACTIVE_FLG = table.Rows[0]["STD_ACTIVE_FLG"].ToString() == "" ? 2 : int.Parse(table.Rows[0]["STD_ACTIVE_FLG"].ToString());
                        stAttPass.STD_COMMENT = table.Rows[0]["STD_COMMENT"].ToString();
                        stAttPass.STD_TEMP_NOTE = table.Rows[0]["STD_TEMP_NOTE"].ToString();
                        stAttPass.RSN_ID = int.Parse(table.Rows[0]["RSN_ID"].ToString());
                    }
                    else
                    {
                        return;
                    }
                }
                form.IsAddNew = false;
                form.stAtt = stAttPass;
                form.LoadFormContaint();
                dialog.ShowDialog();
                string ReturnMessage = form.OutResult;
                if (ReturnMessage != string.Empty && ReturnMessage != null)
                {
                    MessageBox.Show(ReturnMessage, StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }
        private void btnComments_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedRow = grdSMSList.SelectedItem as System.Data.DataRowView;
                if (selectedRow != null)
                {
                    System.Data.DataTable table = new System.Data.DataTable();
                    string note = selectedRow["STD_TEMP_NOTE"].ToString();
                    if (note != "")
                    {
                        string StudentName = selectedRow["STD_INITIALS"].ToString();
                        MessageBoxResult resultMessageBox = MessageBox.Show(" Note : " + note + "\n Do you want to delete special note?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (resultMessageBox == MessageBoxResult.Yes)
                        {
                            Int32 studentID = int.Parse(selectedRow["STD_ID"].ToString());
                            ClassAttendanceDA objClss = new ClassAttendanceDA();
                            objClss.UpdateStudentNote(studentID);
                            selectedRow["STD_TEMP_NOTE"] = null;

                        }
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

        private void btnAddLable_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedRow = grdSMSList.SelectedItem as System.Data.DataRowView;
                if (selectedRow != null)
                {

                    CMSXtream.Pages.DataEntry.AddLable form = new CMSXtream.Pages.DataEntry.AddLable();
                    form.STD_ID = int.Parse(selectedRow["STD_ID"].ToString());
                    form.BindReasonGridFromForm();
                    PopupHelper dialog = new PopupHelper
                    {
                        Title = "View Student Labels",
                        Content = form,
                        ResizeMode = ResizeMode.NoResize,
                        Width = 400,
                        Height = 400
                    };
                    dialog.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }

        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (grdSMSList.ItemsSource != null)
                {
                    Int32 rowCount = grdSMSList.Items.Count;

                    System.Data.DataTable dtStudent = new System.Data.DataTable();
                    dtStudent.Columns.Add("STD_ID", typeof(Int32));
                    dtStudent.Columns.Add("IS_SELECT", typeof(Int32));

                    if (rowCount > 0)
                    {
                        for (int i = 0; i < rowCount; i++)
                        {
                            var selectedRow = (System.Data.DataRowView)grdSMSList.Items[i];
                            int studentId = Int32.Parse(selectedRow["STD_ID"].ToString());
                            DataGridCell cell = GetCell(i, 0, grdSMSList);
                            System.Windows.Controls.CheckBox chk = GetVisualChild<System.Windows.Controls.CheckBox>(cell);
                            if (chk.IsChecked.Value)
                            {
                                dtStudent.Rows.Add(studentId, 1);
                            }
                            else
                            {
                                dtStudent.Rows.Add(studentId, 0);
                            }
                        }
                    }

                    if (dtStudent.Rows.Count > 0)
                    {
                        SMSDA _clsSMS = new SMSDA();
                        System.Data.DataTable table = _clsSMS.SMSRefreshbulk(dtStudent);
                        if (table.Rows.Count > 0)
                        {
                            grdSMSList.ItemsSource = table.DefaultView;
                        }
                        else
                        {
                            grdSMSList.ItemsSource = null;
                        }
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

        private void btnSendSMSALL_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (grdSMSList.ItemsSource != null)
                {
                    Int32 rowCount = grdSMSList.Items.Count;

                    System.Data.DataTable dtStudent = new System.Data.DataTable();
                    dtStudent.Columns.Add("STD_ID", typeof(Int32));

                    if (rowCount > 0)
                    {
                        for (int i = 0; i < rowCount; i++)
                        {
                            var selectedRow = (System.Data.DataRowView)grdSMSList.Items[i];
                            int studentId = Int32.Parse(selectedRow["STD_ID"].ToString());
                            DataGridCell cell = GetCell(i, 0, grdSMSList);
                            System.Windows.Controls.CheckBox chk = GetVisualChild<System.Windows.Controls.CheckBox>(cell);
                            if (chk.IsChecked.Value)
                            {
                                dtStudent.Rows.Add(studentId);
                            }
                        }
                    }

                    if (dtStudent.Rows.Count > 0)
                    {
                        CMSXtream.Pages.DataEntry.SendSMS form = new CMSXtream.Pages.DataEntry.SendSMS();
                        PopupHelper dialog = new PopupHelper
                        {
                            Title = "Sent SMS to Student ",
                            Content = form,
                            ResizeMode = ResizeMode.NoResize,
                            Width = 1000,
                            Height = 680
                        };
                        form.btnAddNew.Visibility = System.Windows.Visibility.Hidden;
                        form.callingForm = true;
                        form.LoadFromStudentList(dtStudent);
                        form.SearchText.Content = "Load from advance search";
                        dialog.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("Unable to find any recipient!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
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

        private void btnLblAddtoAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                var selectedRowLbl = grdLable.SelectedItem as System.Data.DataRowView;
                if (selectedRowLbl != null)
                {
                    int lableId = Int32.Parse(selectedRowLbl["LBL_ID"].ToString());
                    if (grdSMSList.ItemsSource != null)
                    {
                        Int32 rowCount = grdSMSList.Items.Count;

                        System.Data.DataTable dtStudent = new System.Data.DataTable();
                        dtStudent.Columns.Add("STD_ID", typeof(Int32));

                        if (rowCount > 0)
                        {
                            for (int i = 0; i < rowCount; i++)
                            {
                                var selectedRow = (System.Data.DataRowView)grdSMSList.Items[i];
                                int studentId = Int32.Parse(selectedRow["STD_ID"].ToString());
                                DataGridCell cell = GetCell(i, 0, grdSMSList);
                                System.Windows.Controls.CheckBox chk = GetVisualChild<System.Windows.Controls.CheckBox>(cell);
                                if (chk.IsChecked.Value)
                                {
                                    dtStudent.Rows.Add(studentId);
                                }
                            }
                        }

                        if (dtStudent.Rows.Count > 0)
                        {
                            string LableName = selectedRowLbl["LBL_DES"].ToString();
                            MessageBoxResult resultMessageBox = MessageBox.Show("Do you want to add [" + LableName + "] to selected students?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                            if (resultMessageBox == MessageBoxResult.Yes)
                            {
                                SMSDA _clsSMS = new SMSDA();
                                _clsSMS.BulkLableOparation(lableId, dtStudent, false);
                                btnRefresh_Click(null, null);
                            }
                        }
                        else
                        {
                            MessageBox.Show("No student found!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                        }
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

        private void btnLblDeletefromAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedRowLbl = grdLable.SelectedItem as System.Data.DataRowView;
                if (selectedRowLbl != null)
                {
                    int lableId = Int32.Parse(selectedRowLbl["LBL_ID"].ToString());

                    if (grdSMSList.ItemsSource != null)
                    {
                        Int32 rowCount = grdSMSList.Items.Count;

                        System.Data.DataTable dtStudent = new System.Data.DataTable();
                        dtStudent.Columns.Add("STD_ID", typeof(Int32));

                        if (rowCount > 0)
                        {
                            for (int i = 0; i < rowCount; i++)
                            {
                                var selectedRow = (System.Data.DataRowView)grdSMSList.Items[i];
                                int studentId = Int32.Parse(selectedRow["STD_ID"].ToString());
                                DataGridCell cell = GetCell(i, 0, grdSMSList);
                                System.Windows.Controls.CheckBox chk = GetVisualChild<System.Windows.Controls.CheckBox>(cell);
                                if (chk.IsChecked.Value)
                                {
                                    dtStudent.Rows.Add(studentId);
                                }
                            }
                        }

                        if (dtStudent.Rows.Count > 0)
                        {
                            string LableName = selectedRowLbl["LBL_DES"].ToString();
                            MessageBoxResult resultMessageBox = MessageBox.Show("Do you want to delete [" + LableName + "] from selected students?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                            if (resultMessageBox == MessageBoxResult.Yes)
                            {
                                SMSDA _clsSMS = new SMSDA();
                                _clsSMS.BulkLableOparation(lableId, dtStudent, true);
                                btnRefresh_Click(null, null);
                            }
                        }
                        else
                        {
                            MessageBox.Show("No student found!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                        }
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

        private void groupTextSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchSting = groupTextSearch.Text.Trim();
            if (searchSting != string.Empty)
            {
                Helper.searchGridByKey(grdGroup, "CLS_NAME", searchSting);
            }
        }

        private void lableTextSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchSting = lableTextSearch.Text.Trim();
            if (searchSting != string.Empty)
            {
                Helper.searchGridByKey(grdLable, "LBL_DES", searchSting);
            }
        }

        private void UpdateGroupCount()
        {
            try
            {
                if (!isCheckAllGroup)
                {
                    int rowCount = grdGroup.Items.Count;
                    string selectedList = "Selected Group List:";
                    int selectedCount = 0;
                    if (rowCount > 0)
                    {
                        for (int i = 0; i < rowCount; i++)
                        {
                            var selectedRow = (System.Data.DataRowView)grdGroup.Items[i];
                            string displayName = selectedRow["CLS_NAME"].ToString();
                            DataGridCell cell = GetCell(i, 0, grdGroup);
                            System.Windows.Controls.CheckBox chk = GetVisualChild<System.Windows.Controls.CheckBox>(cell);
                            if (chk.IsChecked.Value)
                            {
                                selectedCount = selectedCount + 1;
                                selectedList = selectedList + "\n" + displayName;
                            }
                        }
                    }
                    txtGroupCount.Content = selectedCount.ToString();
                    txtGroupCount.ToolTip = selectedList;
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void UpdateLblInCount()
        {
            try
            {
                if (!isCheckAllGroup)
                {
                    int rowCount = grdLable.Items.Count;
                    int selectedCount = 0;
                    string selectedList = "Selected Label List In:";
                    if (rowCount > 0)
                    {
                        for (int i = 0; i < rowCount; i++)
                        {
                            var selectedRow = (System.Data.DataRowView)grdLable.Items[i];
                            string displayName = selectedRow["LBL_DES"].ToString();
                            DataGridCell cell = GetCell(i, 0, grdLable);
                            System.Windows.Controls.CheckBox chk = GetVisualChild<System.Windows.Controls.CheckBox>(cell);
                            if (chk.IsChecked.Value)
                            {
                                selectedCount = selectedCount + 1;
                                selectedList = selectedList + "\n" + displayName;
                            }
                        }
                    }
                    lblIncount.Content = selectedCount.ToString();
                    lblIncount.ToolTip = selectedList;
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void UpdateLblNotInCount()
        {
            try
            {
                if (!isCheckAllGroup)
                {
                    int rowCount = grdLable.Items.Count;
                    int selectedCount = 0;
                    string selectedList = "Selected Label List Not In:";
                    if (rowCount > 0)
                    {
                        for (int i = 0; i < rowCount; i++)
                        {
                            var selectedRow = (System.Data.DataRowView)grdLable.Items[i];
                            string displayName = selectedRow["LBL_DES"].ToString();
                            DataGridCell cell = GetCell(i, 6, grdLable);
                            System.Windows.Controls.CheckBox chk = GetVisualChild<System.Windows.Controls.CheckBox>(cell);
                            if (chk.IsChecked.Value)
                            {
                                selectedCount = selectedCount + 1;
                                selectedList = selectedList + "\n" + displayName;
                            }
                        }
                    }
                    lblOntIncount.Content = selectedCount.ToString();
                    lblOntIncount.ToolTip = selectedList;
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void chkGrpMarkAll_Checked(object sender, RoutedEventArgs e)
        {
            UpdateGroupCount();
        }

        private void chkGrpMarkAll_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateGroupCount();
        }

        private void chkLblMark_Checked(object sender, RoutedEventArgs e)
        {
            UpdateLblInCount();
        }

        private void chkLblMark_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateLblInCount();
        }

        private void chkLblMarkN_Checked(object sender, RoutedEventArgs e)
        {
            UpdateLblNotInCount();
        }

        private void chkLblMarkN_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateLblNotInCount();
        }
        private void ExportCsv_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (grdSMSList.Items.Count > 0)
                {
                    int rowCount = grdSMSList.Items.Count;
                    if (rowCount > 0)
                    {
                        System.Data.DataTable ExportData = new System.Data.DataTable();
                        ExportData.Columns.Add("STD_TELEPHONE");
                        for (int i = 0; i < rowCount; i++)
                        {
                            var selectedRow = (System.Data.DataRowView)grdSMSList.Items[i];
                            string _mobileNo = selectedRow["STD_TELEPHONE"].ToString();
                            DataGridCell cell = GetCell(i, 0, grdSMSList);
                            System.Windows.Controls.CheckBox chk = GetVisualChild<System.Windows.Controls.CheckBox>(cell);
                            if (chk.IsChecked.Value)
                            {
                                ExportData.Rows.Add(_mobileNo);
                            }
                        }
                        if (ExportData.Rows.Count > 0)
                        {
                            DataSet _mobileNumbers = new DataSet();
                            _mobileNumbers.Tables.Add(ExportData);

                            string filePath, savingNameAndPath;
                            SelectSavePath(out savingNameAndPath, out filePath);
                          
                            if (!(Directory.Exists(filePath)))
                            {
                                MessageBox.Show("Plaese select a folder", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                            }
                            else
                            {
                                ExportToExcel(_mobileNumbers, savingNameAndPath);
                            }
                        }
                        else
                        {
                            MessageBox.Show("No any Data selected");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("No any Data to Export");
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }
        public void ExportToExcel(DataSet dataSet, string outputPath)
        {
            try
            {
                // Create the Excel Application object
                //ApplicationClass excelApp = new ApplicationClass();
                Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
                // Create a new Excel Workbook
                Workbook excelWorkbook = xlApp.Workbooks.Add(Type.Missing);

                int sheetIndex = 0;
                int col = 0;
                int row = 0;
                Worksheet excelSheet = default(Worksheet);

                // Copy each DataTable as a new Sheet
                int totalCount = dataSet.Tables.Count;

                foreach (System.Data.DataTable dt in dataSet.Tables)
                {
                    sheetIndex += 1;
                    // Copy the DataTable to an object array
                    object[,] rawData = new object[dt.Rows.Count + 1, dt.Columns.Count];

                    // Copy the values to the object array
                    for (col = 0; col <= dt.Columns.Count - 1; col++)
                    {
                        for (row = 0; row <= dt.Rows.Count - 1; row++)
                        {
                            rawData[row, col] = dt.Rows[row].ItemArray[col];
                        }
                    }

                    // Calculate the final column letter
                    string finalColLetter = string.Empty;
                    string colCharset = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                    int colCharsetLen = colCharset.Length;

                    if (dt.Columns.Count > colCharsetLen)
                    {
                        finalColLetter = colCharset.Substring((dt.Columns.Count - 1) / colCharsetLen - 1, 1);
                    }

                    finalColLetter += colCharset.Substring((dt.Columns.Count - 1) % colCharsetLen, 1);

                    // Create a new Sheet
                    excelSheet = (Worksheet)excelWorkbook.Sheets.Add(excelWorkbook.Sheets[sheetIndex], Type.Missing, 1, XlSheetType.xlWorksheet);

                    excelSheet.Name = dt.TableName;

                    // Fast data export to Excel
                    string excelRange = string.Format("A1:{0}{1}", finalColLetter, dt.Rows.Count + 1);
                    excelSheet.Range[excelRange, Type.Missing].Value2 = rawData;

                    // Mark the first row as BOLD
                    // ((Range)excelSheet.Rows[1, Type.Missing]).Font.Bold = true;

                    excelSheet = null;
                }
                // Check if the output file already exists, and delete it
                if (File.Exists(outputPath))
                {
                    File.Delete(outputPath);
                }
                // Save and Close the Workbook
                excelWorkbook.SaveAs(outputPath, XlFileFormat.xlCSV,
                    Type.Missing, Type.Missing, Type.Missing, Type.Missing, XlSaveAsAccessMode.xlExclusive,Type.Missing,Type.Missing,
                    Type.Missing, Type.Missing, Type.Missing);

                excelWorkbook.Close(true, Type.Missing, Type.Missing);

                excelWorkbook = null;

                // Release the Application object
                xlApp.Quit();
                xlApp = null;

                // Collect the unreferenced objects
                GC.Collect();
                GC.WaitForPendingFinalizers();
                MessageBox.Show("Export Complete");
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }
        public void SelectSavePath(out string fileNameAndPath,out string filePath)
        {
            fileNameAndPath = null;
            filePath = null;
            try
            {
                // Step 1: Show SaveFileDialog to enter a file name within the selected folder
                System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();

                string _userName = StaticProperty.LoginUserID.ToString();
                saveFileDialog.FileName = _userName + "-" + DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss");
                saveFileDialog.Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*";
                saveFileDialog.DefaultExt = "csv";

                // Show 2 the SaveFileDialog
                System.Windows.Forms.DialogResult fileResult = saveFileDialog.ShowDialog();

                if (fileResult == System.Windows.Forms.DialogResult.OK)
                {
                    // Step 3: Get the selected file path
                    fileNameAndPath = saveFileDialog.FileName;
                    filePath = Path.GetDirectoryName(fileNameAndPath);
                }
            }
            catch(Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }
    }
}
