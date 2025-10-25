using CMSXtream.Handlers;
using CMSXtream.Pages.View;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using XtreamDataAccess;

namespace CMSXtream.Pages.DataEntry
{
    /// <summary>
    /// Interaction logic for AssignStudentToAdmins.xaml
    /// </summary>
    public partial class AssignStudentToAdmins : UserControl
    {
        Boolean isCheckAll = false;
        Boolean RowCheckUncheck = false;
        Boolean GetUsersStd = false;
        Boolean isAssignCheked = true;
        Boolean CustomSelect = false;
        DateTime FromDate;
        DateTime ToDate;
        Boolean Filter;
        int ChkBoxMode;
        int selectedCount = 0;//selected check boxces
        DataTable LoadedStudentListAll_Tbl;
        DataTable CheckedStudentList_Tbl;
        DataTable UsersTable;
        string DataLoadedUser;
        int DataLoadedUserId;
        int PageMOde; //1-Admin, 0-user- for identify> edit button< refresh data ADMIN mode
        int UserType;
        string LgUser;
        public static int ToUserId { get; set; }
        public AssignStudentToAdmins()
        {
            InitializeComponent();

            GetCmpaingTochkBox();
            GetUserToCombo();
            CheckUserType();

            CheckedStudentList_Tbl = new DataTable();
            CheckedStudentList_Tbl.Columns.Add("CMP_STD_ID", typeof(Int32));
            pndDays.Visibility = Visibility.Hidden;
            btnTransferLeads.Visibility = Visibility.Collapsed;
        }


        public static T GetVisualChild<T>(Visual parent) where T : Visual      //GetVisualChild is variable
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

        private DataGridColumnHeader GetColumnHeader(DataGrid dataGrid, int columnIndex)
        {
            DataGridColumnHeader columnHeader = null;

            if (dataGrid != null && dataGrid.Columns.Count > columnIndex)
            {
                DataGridColumn column = dataGrid.Columns[columnIndex];

                // Find the DataGridColumnHeader by traversing the visual tree
                DataGridColumnHeadersPresenter presenter = GetVisualChild<DataGridColumnHeadersPresenter>(dataGrid);
                if (presenter != null)
                {
                    columnHeader = presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex) as DataGridColumnHeader;
                }
            }

            return columnHeader;
        }
        private void FirstColmnBoxCheckUncheck(DataGrid dtGrid, Boolean checkStatus)
        {
            int rowCount = 0;
            try
            {
                rowCount = dtGrid.Items.Count;
                if (rowCount > 0)
                {
                    for (int i = 0; i < rowCount; i++)
                    {
                        DataGridCell cell = GetCell(i, 0, dtGrid);////0- check bx are in 1ST column 
                        CheckBox chk = GetVisualChild<CheckBox>(cell);
                        chk.IsChecked = checkStatus;
                    }
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!" + ex, StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }
        public void GetCmpaingTochkBox()
        {
            try
            {
                AssignStudentToAdminsDA _cmpNames = new AssignStudentToAdminsDA();
                System.Data.DataTable table = _cmpNames.GetCampaigsToChkBox().Tables[0];
                if (table.Rows.Count > 0)
                {
                    gridCmpNames.ItemsSource = table.DefaultView;
                }
                else
                {
                    gridCmpNames.ItemsSource = null;
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }


        private void chkCmpMarkAll_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                isCheckAll = true;
                FirstColmnBoxCheckUncheck(gridCmpNames, true);
                lblCampaingCount.Content = gridCmpNames.Items.Count.ToString();
                lblCampaingCount.ToolTip = "";
                isCheckAll = false;
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void chkCmpMarkAll_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (RowCheckUncheck == false)
                {
                    FirstColmnBoxCheckUncheck(gridCmpNames, false);
                }
                lblCampaingCount.Content = 0;
                lblCampaingCount.ToolTip = "";
                isCheckAll = false;
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }
        private void chkCmpMark_Checked(object sender, RoutedEventArgs e)
        {
            if (gridCmpNames.SelectedItems.Count > 1)//identify multiple row select or not 
            {
                CustomSelect = true;
            }
            if (CustomSelect)
            {
                MultipleRowSelect(gridCmpNames, true);
            }

            UpdateCampaingCount();
            if (gridCmpNames.Items.Count == selectedCount)
            {
                GetUsersStd = true;/////NEW
                UncheckHeaderCheckBox(gridCmpNames, 0);
                GetUsersStd = false;/////NEW
            }

        }

        private void chkCmpMark_Unchecked(object sender, RoutedEventArgs e)
        {
            if (gridCmpNames.SelectedItems.Count > 1)//identify multiple row select or not 
            {
                CustomSelect = true;
            }
            if (CustomSelect)
            {
                MultipleRowSelect(gridCmpNames, false);
            }
            RowCheckUncheck = true;
            UncheckHeaderCheckBox(gridCmpNames, 0);
            UpdateCampaingCount();
            RowCheckUncheck = false;
        }
        private void UpdateCampaingCount()
        {
            try
            {
                if (!isCheckAll)
                {
                    int rowCount = gridCmpNames.Items.Count;
                    string selectedList = "Selected Campaing List:";
                    // int selectedCount = 0;
                    selectedCount = 0;
                    if (rowCount > 0)
                    {
                        for (int i = 0; i < rowCount; i++)
                        {
                            var selectedRow = (System.Data.DataRowView)gridCmpNames.Items[i];
                            string displayName = selectedRow["CMP_DES"].ToString();
                            DataGridCell cell = GetCell(i, 0, gridCmpNames);//1 CHECK BOX 2ND COLUMN
                            CheckBox chk = GetVisualChild<CheckBox>(cell);
                            if (chk.IsChecked.Value)
                            {
                                selectedCount = selectedCount + 1;
                                selectedList = selectedList + "\n" + displayName;
                            }
                        }


                    }
                    lblCampaingCount.Content = selectedCount.ToString();
                    lblCampaingCount.ToolTip = selectedList;

                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }
        private void UncheckHeaderCheckBox(DataGrid dtGrid, int columnIndex)
        {
            try
            {
                // Get the header checkbox
                DataGridColumnHeader header = GetColumnHeader(dtGrid, columnIndex);
                CheckBox chkHeader = GetVisualChild<CheckBox>(header);

                if (chkHeader != null)
                {
                    // Uncheck only the header checkbox
                    if (GetUsersStd == false)
                    {
                        chkHeader.IsChecked = false;
                    }
                    else
                    {
                        chkHeader.IsChecked = true;
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

        public void GetUserToCombo()
        {
            try
            {
                AssignStudentToAdminsDA _users = new AssignStudentToAdminsDA();
                // DataTable _userTable = new DataTable();
                // _userTable = _users.GetAdminUsers().Tables[0];
                // cmbSelectUser.ItemsSource = _userTable.DefaultView;
                UsersTable = _users.GetAdminUsers().Tables[0];
                cmbSelectUser.ItemsSource = UsersTable.DefaultView;
                cmbSelectUser.DisplayMemberPath = "USER_NAME";
                cmbSelectUser.SelectedValuePath = "ID";

                dtpTodate.SelectedDate=DateTime.Now;
                dtpFromdate.SelectedDate= dtpTodate.SelectedDate.Value.AddDays(-1);

                if (cmbSelectUser.SelectedValue == null)
                {
                    cmbSelectUser.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }
        private void getALLUnassignStudent()
        {
            int rowCount = 0;
            try
            {
                DataTable _cmptable = new DataTable();
                _cmptable.Columns.Add("CMP_ID", typeof(Int32));
                rowCount = gridCmpNames.Items.Count;
                if (rowCount > 0)
                {
                    for (int i = 0; i < rowCount; i++)
                    {
                        var selectedRow = (System.Data.DataRowView)gridCmpNames.Items[i];
                        int cmpId = Int32.Parse(selectedRow["CMP_ID"].ToString());
                        DataGridCell cell = GetCell(i, 0, gridCmpNames);
                        CheckBox chk = GetVisualChild<CheckBox>(cell);
                        if (chk.IsChecked.Value)
                        {
                            _cmptable.Rows.Add(cmpId);
                        }
                    }
                }
                if (_cmptable.Rows.Count == 0)
                {
                    MessageBox.Show("No filtration Data");
                }
                else
                {
                    AssignStudentToAdminsDA _returnData = new AssignStudentToAdminsDA();
                    if (LoadedStudentListAll_Tbl != null)
                    {
                        LoadedStudentListAll_Tbl.Clear();
                    }
                    LoadedStudentListAll_Tbl = _returnData.GetSelectedCampainStudentData(_cmptable);
                    grdStdData.ItemsSource = LoadedStudentListAll_Tbl.DefaultView;
                    btnAssignStudent.Visibility = Visibility.Visible;
                    btnUnAssignStudent.Visibility = Visibility.Collapsed;
                   
                    PageMOde = 1;
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }
        private void btnSearchUnassignStudent_Click(object sender, RoutedEventArgs e)
        {
            getALLUnassignStudent();
            lblDataCount.Content = "";
            lblDataCount.ToolTip = "";
            pndDays.Visibility = Visibility.Collapsed;
            btnTransferLeads.Visibility = Visibility.Collapsed;
        }
        private void chkStdMark_Checked(object sender, RoutedEventArgs e)
        {
            if (grdStdData.SelectedItems.Count > 1)//identify multiple row select or not 
            {
                CustomSelect = true;
            }
            if (CustomSelect)
            {
                MultipleRowSelect(grdStdData, true);
            }
            UpdateStudentCount();
            if (grdStdData.Items.Count == selectedCount)
            {
                GetUsersStd = true;
                UncheckHeaderCheckBox(grdStdData, 0);
                GetUsersStd = false;
            }
        }
        private void chkStdMark_Unchecked(object sender, RoutedEventArgs e)
        {
            if (grdStdData.SelectedItems.Count > 1)//identify multiple row select or not 
            {
                CustomSelect = true;
            }
            if (CustomSelect)
            {
                MultipleRowSelect(grdStdData, false);
            }
            RowCheckUncheck = true;
            UncheckHeaderCheckBox(grdStdData, 0);
            RowCheckUncheck = false;
            UpdateStudentCount();
        }
        private void chkStdMarkAll_Checked(object sender, RoutedEventArgs e)
        {
            try
            {

                isCheckAll = true;
                FirstColmnBoxCheckUncheck(grdStdData, true);
                lblDataCount.Content = grdStdData.Items.Count.ToString();
                lblDataCount.ToolTip = "";
                isCheckAll = false;
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }
        private void chkStdMarkAll_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (RowCheckUncheck == false)//avoid calling mehod when uncheck row checkboxes
                {
                    FirstColmnBoxCheckUncheck(grdStdData, false);
                }
                isCheckAll = false;
                lblDataCount.Content = 0;
                lblDataCount.ToolTip = "";
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }
        private void btnAssignStudent_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                int _user_id;
                _user_id = Int32.Parse(cmbSelectUser.SelectedValue.ToString());
                ////new 
                if (CheckedStudentList_Tbl != null)
                {
                    CheckedStudentList_Tbl.Clear();
                }
                CheckedStudentList_Tbl = SelectedStdDataTable();
                /////////new
                if (CheckedStudentList_Tbl.Rows.Count == 0)//
                {
                    MessageBox.Show("No Any Selected Data");
                }
                else if (_user_id == -1)
                {
                    MessageBox.Show("Select a user");
                }
                else
                {
                    try
                    {
                        string _userName = cmbSelectUser.Text.ToString();
                        MessageBoxResult result = MessageBox.Show("Do you want assign selected students to User: " + _userName + " ?", "Confirmation"/*box tttle*/, MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                        {
                            AssignStudentToAdminsDA _inserStdUser = new AssignStudentToAdminsDA();
                            _inserStdUser.InserDataToStdUserTable(CheckedStudentList_Tbl, _user_id, LgUser);
                            RefreshDatagrid(CheckedStudentList_Tbl);
                            lblDataCount.Content = "";
                            lblDataCount.ToolTip = "";
                        }
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show("Error" + ex);
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

        private void GetAssignStudentToUser()
        {
            int rowCount = 0;
            if (cmbSelectUser.SelectedValue != null)
            {
                int _user_id = Int32.Parse(cmbSelectUser.SelectedValue.ToString());

                //lblCount.Content = "";
                try
                {
                    DataTable _cmptable = new DataTable();
                    _cmptable.Columns.Add("CMP_ID", typeof(Int32));
                    rowCount = gridCmpNames.Items.Count;
                    if (rowCount > 0)
                    {
                        for (int i = 0; i < rowCount; i++)
                        {
                            var selectedRow = (System.Data.DataRowView)gridCmpNames.Items[i];
                            int cmpId = Int32.Parse(selectedRow["CMP_ID"].ToString());
                            DataGridCell cell = GetCell(i, 0, gridCmpNames);
                            CheckBox chk = GetVisualChild<CheckBox>(cell);
                            if (chk.IsChecked.Value)
                            {
                                _cmptable.Rows.Add(cmpId);
                            }
                        }
                    }
                    if (_cmptable.Rows.Count == 0)
                    {
                        MessageBox.Show("No filtration Data");
                    }
                    else if (_user_id == -1)
                    {
                        MessageBox.Show("Select a user");
                    }
                    else
                    {
                        DataLoadedUser = cmbSelectUser.Text.ToString();
                        DataLoadedUserId = Int32.Parse(cmbSelectUser.SelectedValue.ToString());
                        DateTime? _fromDate = dtpFromdate.SelectedDate;
                        DateTime? _todate=dtpTodate.SelectedDate;
                        if (_fromDate == null || _todate == null)
                        {
                            MessageBox.Show("Please Select valid Date!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
                        }
                        else
                        {
                            AssignStudentToAdminsDA _returnData = new AssignStudentToAdminsDA();
                            // DataTable _returnStdUserDatatable = _returnData.GetUsersStudent(_cmptable, _user_id);
                            if (LoadedStudentListAll_Tbl != null)
                            {
                                LoadedStudentListAll_Tbl.Clear();//
                            }
                            LoadedStudentListAll_Tbl = _returnData.GetUsersStudent(_cmptable, _user_id,_fromDate,_todate);
                            grdStdData.ItemsSource = LoadedStudentListAll_Tbl.DefaultView;

                            //grdStdData.ItemsSource = _returnStdUserDatatable.DefaultView;
                            // GetUsersStd = true;//check all checkbox checked
                            // UncheckHeaderCheckBox(grdStdData, 0);
                            //  GetUsersStd = false;

                            btnAssignStudent.Visibility = Visibility.Collapsed;//hide assign std btn
                            btnUnAssignStudent.Visibility = Visibility.Visible;//unhide unassign std btn

                            pndDays.Visibility = Visibility.Visible;

                            PageMOde = 2;

                            btnTransferLeads.Visibility = Visibility.Visible;
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
        }
        private void btnGetAssignStd_Click(object sender, RoutedEventArgs e)
        {
            GetAssignStudentToUser();
            
        }
        public void UnassignStudends()
        {
            int rowCount = 0;
            //lblCount.Content = "";
            try
            {
                if (CheckedStudentList_Tbl != null)
                {
                    CheckedStudentList_Tbl.Clear();//
                }
                rowCount = grdStdData.Items.Count;
                if (rowCount > 0)
                {
                    for (int i = 0; i < rowCount; i++)
                    {
                        var selectedRow = (System.Data.DataRowView)grdStdData.Items[i];
                        int _cmpStdId = Int32.Parse(selectedRow["CMP_STD_ID"].ToString());
                        DataGridCell cell = GetCell(i, 0, grdStdData);
                        CheckBox chk = GetVisualChild<CheckBox>(cell);
                        if (chk.IsChecked.Value)
                        {
                            CheckedStudentList_Tbl.Rows.Add(_cmpStdId);//
                        }
                    }
                }
                if (CheckedStudentList_Tbl.Rows.Count == 0)//
                {
                    MessageBox.Show("No selected Data");
                }
                else
                {
                    try
                    {
                        AssignStudentToAdminsDA _unassignstd = new AssignStudentToAdminsDA();
                        _unassignstd.UnassignStudentFromStdUserTable(CheckedStudentList_Tbl,DataLoadedUser,StaticProperty.LoginUserID);
                        MessageBox.Show("Changes have been done.");
                        RefreshDatagrid(CheckedStudentList_Tbl);
                    }
                    catch (SqlException ex)
                    {
                        LogFile logger = new LogFile();
                        logger.MyLogFile(ex);
                        MessageBox.Show("System error has occurred.Please check log file!" + ex, StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
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

        private void UpdateStudentCount()
        {
            try
            {
                if (!isCheckAll)
                {
                    int rowCount = grdStdData.Items.Count;
                    string selectedList = "Selected Student List:";
                    if (rowCount > 0)
                    {
                        selectedCount = 0;
                        for (int i = 0; i < rowCount; i++)
                        {
                            var selectedRow = (System.Data.DataRowView)grdStdData.Items[i];
                            string displayName = selectedRow["STD_FULL_NAME"].ToString();
                            DataGridCell cell = GetCell(i, 0, grdStdData);//1 CHECK BOX 2ND COLUMN
                            CheckBox chk = GetVisualChild<CheckBox>(cell);
                            if (chk.IsChecked.Value)
                            {
                                selectedCount = selectedCount + 1;
                                selectedList = selectedList + "\n" + displayName;
                            }
                        }
                    }
                    lblDataCount.Content = selectedCount.ToString();
                    lblDataCount.ToolTip = selectedList;
                }
                else
                {
                    lblDataCount.Content = grdStdData.Items.Count.ToString();
                    lblDataCount.ToolTip = "";
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void campainTextSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchString = campainTextSearch.Text.Trim();
            if (searchString != string.Empty)
            {
                Helper.searchGridByKey(gridCmpNames, "CMP_DES", searchString);
            }
        }

        private void studentTextSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchString = studentTextSearch.Text.Trim();
            if (searchString != string.Empty)
            {
                Helper.searchGridByKey(grdStdData, "STD_FULL_NAME", searchString);
            }
        }
        private void btnAssignRowStudent_Click(object sender, RoutedEventArgs e)//row wise assign button
        {
            try
            {
                var selectedRow = grdStdData.SelectedItem as System.Data.DataRowView;
                if (selectedRow != null)
                {
                    int _CMP_STD_ID = Int32.Parse(selectedRow["CMP_STD_ID"].ToString());
                    int _userId = Int32.Parse(cmbSelectUser.SelectedValue.ToString());
                    string _studentName = selectedRow["STD_FULL_NAME"].ToString();
                    string _userName = cmbSelectUser.Text.ToString();
                    if (_userId != -1)
                    {
                        MessageBoxResult result = MessageBox.Show("Do you want assign this students (" + _studentName + ") to User: " + _userName + " ?", "Confirmation"/*box tttle*/, MessageBoxButton.YesNo, MessageBoxImage.Question);

                        if (result == MessageBoxResult.Yes)
                        {
                            try
                            {
                                AssignStudentToAdminsDA _assignSingleStd = new AssignStudentToAdminsDA();
                                _assignSingleStd.SingleStudentAssignToUser(_CMP_STD_ID, _userId,LgUser);
                                DataRow row = selectedRow.Row;
                                LoadedStudentListAll_Tbl.Rows.Remove(row);
                                grdStdData.ItemsSource = null;
                                grdStdData.ItemsSource = LoadedStudentListAll_Tbl.DefaultView;
                                
                            }
                            catch (SqlException ex)
                            {
                                if (ex.Number == 2627 || ex.Number == 2601)
                                {
                                    MessageBox.Show("The record already added. Please Refresh Page");
                                }
                                else
                                {
                                    LogFile logger = new LogFile();
                                    logger.MyLogFile(ex);
                                    MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
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
                    else
                    {
                        MessageBox.Show("Please Select a User");
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

        private void btnUnassignRowStudent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedRow = grdStdData.SelectedItem as System.Data.DataRowView;
                int _cmpStdId = Int32.Parse(selectedRow["CMP_STD_ID"].ToString());
                string _studentName = selectedRow["STD_FULL_NAME"].ToString();

                MessageBoxResult result = MessageBox.Show("Do you want Unassign this students (" + _studentName + ") from User: " + DataLoadedUser + " ?", "Confirmation"/*box tttle*/, MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        AssignStudentToAdminsDA _unassignSingleStd = new AssignStudentToAdminsDA();
                        _unassignSingleStd.SingleStudentUnAssignFromUser(_cmpStdId, DataLoadedUserId,StaticProperty.LoginUserID);
                        DataRow row = selectedRow.Row;
                        LoadedStudentListAll_Tbl.Rows.Remove(row);
                        grdStdData.ItemsSource = null;
                        grdStdData.ItemsSource = LoadedStudentListAll_Tbl.DefaultView;
                        if (lblDataCount.Content != "")
                        {
                            lblDataCount.Content = Int32.Parse(lblDataCount.Content.ToString()) - 1;

                            if (Int32.Parse(lblDataCount.Content.ToString()) - 1 < 0)//prevent - values
                            {
                                lblDataCount.Content = 0;
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        LogFile logger = new LogFile();
                        logger.MyLogFile(ex);
                        MessageBox.Show("System error has occurred.Please check log file!" + ex, StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
                    }
                    catch (Exception ex)
                    {
                        LogFile logger = new LogFile();
                        logger.MyLogFile(ex);
                        MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
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
        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            GetCmpaingTochkBox();
            CheckUserType();
            GetUserToCombo();
            pndDays.Visibility = Visibility.Collapsed;
            btnTransferLeads.Visibility = Visibility.Collapsed;

            lblCampaingCount.Content = "";
            lblDataCount.Content = "";
            if (gridCmpNames.Items.Count > 0)
            {
                UncheckHeaderCheckBox(gridCmpNames, 0);
            }

            if (LoadedStudentListAll_Tbl != null)
            {
                LoadedStudentListAll_Tbl.Clear();
                grdStdData.ItemsSource = LoadedStudentListAll_Tbl.DefaultView;
            }

        }
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CMSXtream.Pages.DataEntry.Student form = new CMSXtream.Pages.DataEntry.Student();
                PopupHelper dialog = new PopupHelper
                {
                    Title = "Edit Student",
                    Content = form,
                    ResizeMode = ResizeMode.NoResize,
                    Width = 1000
                };
                StudentAttribute stAttPass = new StudentAttribute();
                var selectedRow = grdStdData.SelectedItem as System.Data.DataRowView;
                if (selectedRow != null)
                {
                    stAttPass.STD_ID = int.Parse(selectedRow["STD_ID"].ToString());
                    stAttPass.CLS_ID = int.Parse(selectedRow["CLS_ID"].ToString());
                    stAttPass.CLS_NAME = selectedRow["CLS_NAME"].ToString();
                    stAttPass.STD_INITIALS = selectedRow["STD_INITIALS"].ToString();
                    stAttPass.STD_SURNAME = selectedRow["STD_SURNAME"].ToString();
                    stAttPass.STD_FULL_NAME = selectedRow["STD_FULL_NAME"].ToString();
                    stAttPass.STD_GENDER = Int32.Parse(selectedRow["STD_GENDER"].ToString());
                    stAttPass.STD_DATEOFBIRTH = DateTime.Parse(selectedRow["STD_DATEOFBIRTH"].ToString());
                    stAttPass.STD_JOIN_DATE = DateTime.Parse(selectedRow["STD_JOIN_DATE"].ToString());
                    stAttPass.STD_EMAIL = selectedRow["STD_EMAIL"].ToString();
                    stAttPass.STD_NIC = selectedRow["STD_NIC"].ToString();
                    stAttPass.STD_ADDRESS = selectedRow["STD_ADDRESS"].ToString();
                    stAttPass.STD_CLASS_FEE = double.Parse(selectedRow["STD_CLASS_FEE"].ToString());
                    stAttPass.STD_TELEPHONE = selectedRow["STD_TELEPHONE"].ToString();
                    stAttPass.STD_ACTIVE_FLG = selectedRow["STD_ACTIVE_FLG"].ToString() == "" ? 2 : int.Parse(selectedRow["STD_ACTIVE_FLG"].ToString());
                    stAttPass.STD_COMMENT = selectedRow["STD_COMMENT"].ToString();
                    stAttPass.STD_TEMP_NOTE = selectedRow["STD_TEMP_NOTE"].ToString();
                    stAttPass.RSN_ID = Int32.Parse(selectedRow["RSN_ID"].ToString());

                }
                form.IsAddNew = false;
                form.stAtt = stAttPass;
                form.LoadFormContaint();
                dialog.ShowDialog();
                string ReturnMessage = form.OutResult;
                if (ReturnMessage != string.Empty && ReturnMessage != null)
                {
                    MessageBox.Show(ReturnMessage, StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                    if (PageMOde == 1)
                    {
                        getALLUnassignStudent();
                    }
                    if (PageMOde == 2)
                    {
                        GetAssignStudentToUser();
                    }
                    if (UserType == 0)
                    {
                        GetMystudent();
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
        public void CheckUserType()
        {
            try
            {
                // string _userName = StaticProperty.LoginUserID;
                LgUser = StaticProperty.LoginUserID;
                AssignStudentToAdminsDA _userType = new AssignStudentToAdminsDA();
                //UserType = _userType.CheckUserType(_userName);
                UserType = _userType.CheckUserType(LgUser);


                btnSearchUnassignStudent.Visibility = Visibility.Visible;
                btnAssignStudent.Visibility = Visibility.Visible;
                cmbSelectUser.IsEnabled = true;
                cmbSelectUser.SelectedValue = -1;
                CheckBox.Visibility = Visibility.Visible;
                btnGetAssignStd.Visibility = Visibility.Visible;
                btnUnAssignStudent.Visibility = Visibility.Collapsed;
                lblLeadSearch.Visibility = Visibility.Visible;
                studentTextSearch.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }
        /* private void GetMyStudentbtn_Click(object sender, RoutedEventArgs e)
         {
             GetMystudent();
         }*/
        private void GetMystudent()
        {
            if (cmbSelectUser.SelectedValue != null)
            {
                try
                {
                    int rowCount = 0;
                    int _user_id = Int32.Parse(cmbSelectUser.SelectedValue.ToString());

                    DataTable _cmptable = new DataTable();
                    _cmptable.Columns.Add("CMP_ID", typeof(Int32));
                    rowCount = gridCmpNames.Items.Count;
                    if (rowCount > 0)
                    {
                        for (int i = 0; i < rowCount; i++)
                        {
                            var selectedRow = (System.Data.DataRowView)gridCmpNames.Items[i];
                            int cmpId = Int32.Parse(selectedRow["CMP_ID"].ToString());
                            DataGridCell cell = GetCell(i, 0, gridCmpNames);
                            CheckBox chk = GetVisualChild<CheckBox>(cell);
                            if (chk.IsChecked.Value)
                            {
                                _cmptable.Rows.Add(cmpId);
                            }
                        }
                    }
                    if (_cmptable.Rows.Count == 0)
                    {
                        MessageBox.Show("No filtration Data");
                    }
                    else
                    {
                        AssignStudentToAdminsDA _getMyStudent = new AssignStudentToAdminsDA();
                        if (LoadedStudentListAll_Tbl != null)
                        {
                            LoadedStudentListAll_Tbl.Clear();
                        }
                        try
                        {
                            LoadedStudentListAll_Tbl = _getMyStudent.GetMyStudents(_cmptable, StaticProperty.LoginUserID);
                            grdStdData.ItemsSource = LoadedStudentListAll_Tbl.DefaultView;
                            lblDataCount.Width = 120;
                            lblDataCount.Content = "Std Count : " + grdStdData.Items.Count.ToString();
                        }
                        catch (SqlException ex)
                        {
                            LogFile logger = new LogFile();
                            logger.MyLogFile(ex);
                            MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
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
        }
        public void RefreshDatagrid(DataTable SelectedList)
        {
            try
            {
                var query = from r1 in LoadedStudentListAll_Tbl.AsEnumerable()
                            join r2 in SelectedList.AsEnumerable()
                            on (Convert.ToInt32(r1["CMP_STD_ID"])) equals (Convert.ToInt32(r2["CMP_STD_ID"]))
                            select r1;
                foreach (DataRow row in query.ToList())
                {
                    row.Delete();
                }
                LoadedStudentListAll_Tbl.AcceptChanges();
                grdStdData.ItemsSource = LoadedStudentListAll_Tbl.DefaultView;
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void btnGetMyLeads_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int rowCount = 0;
                DataTable _cmptable = new DataTable();
                _cmptable.Columns.Add("CMP_ID", typeof(Int32));
                rowCount = gridCmpNames.Items.Count;
                if (rowCount > 0)
                {
                    for (int i = 0; i < rowCount; i++)
                    {
                        var selectedRow = (System.Data.DataRowView)gridCmpNames.Items[i];
                        int cmpId = Int32.Parse(selectedRow["CMP_ID"].ToString());
                        DataGridCell cell = GetCell(i, 0, gridCmpNames);
                        CheckBox chk = GetVisualChild<CheckBox>(cell);
                        if (chk.IsChecked.Value)
                        {
                            _cmptable.Rows.Add(cmpId);
                        }
                    }
                }
                if (cmbSelectUser.SelectedValue != null)
                {
                    int _user_id = Int32.Parse(cmbSelectUser.SelectedValue.ToString());
                    if (_user_id != -1)
                    {

                        if (_cmptable.Rows.Count > 0)
                        {
                            if (dtpFromdate.SelectedDate == null || dtpTodate.SelectedDate == null)
                            {
                                MessageBox.Show("Please select Valid Date!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
                            }
                            else
                            {

                                DataLoadedUserId = Int32.Parse(cmbSelectUser.SelectedValue.ToString());
                                CMSXtream.Pages.DataEntry.MyLeads MyLeadOpen = new CMSXtream.Pages.DataEntry.MyLeads();
                                PopupHelper dialog = new PopupHelper
                                {
                                    Title = LoginDA.getUserName(DataLoadedUserId) + "'s Leads ",
                                    Content = MyLeadOpen,
                                    ResizeMode = ResizeMode.CanResize,
                                    Width = this.Width,
                                    Height = this.Height,
                                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                                    WindowState = WindowState.Maximized
                                };

                                // FromDate = DateTime.Now.AddYears(-1);
                                // ToDate = DateTime.Now;
                                FromDate = dtpFromdate.SelectedDate.Value;
                                ToDate = dtpTodate.SelectedDate.Value;

                                System.Data.DataTable dtLable = new System.Data.DataTable();
                                dtLable.Columns.Add("LBL_ID", typeof(Int32));

                                Filter = true;
                                ChkBoxMode = 2;
                                MyLeadOpen.LoadMyLeads(DataLoadedUserId, _cmptable, dtLable, FromDate, ToDate, Filter, ChkBoxMode);
                                dialog.ShowDialog();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Please select campagns");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please select user");
                    }
                }
                else
                {
                    MessageBox.Show("Please select user");
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void btnAddStudent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show("Do you want to add this Lead ?", "Confirmation"/*box tttle*/, MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    var selectedRow = grdStdData.SelectedItem as System.Data.DataRowView;
                    if (selectedRow != null)
                    {
                        String _stdConvertedUserId = StaticProperty.LoginUserID;
                        int cmpStdId = int.Parse(selectedRow["CMP_STD_ID"].ToString());
                        AssignStudentToAdminsDA _addMyLeadAsStd = new AssignStudentToAdminsDA();
                        Int32 STD = _addMyLeadAsStd.AddMyLeadAsStudent(cmpStdId, _stdConvertedUserId);
                        if (STD > 0)
                        {
                            DataRow row = selectedRow.Row;//REFRESH
                            row["IS_CLS_STD"] = 1;
                            row["ISNOT_CLS_STD"] = 0;
                            row["STD_ID"] = STD.ToString();
                            row["INCOMPLETED_STD_DATA"] = 1;
                            row["PENDING_DAYS"] = 0;
                            grdStdData.ItemsSource = LoadedStudentListAll_Tbl.DefaultView;
                        }
                        else
                        {
                            MessageBox.Show("Cannot add this Lead as Student because the telephone number already exists with an \b\b Active\b\b or \b\bTemporarily Inactive student\b\b. \n Mobile number is associated with Student ID : " + STD, StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
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
        private void btnUnAssignStudent_Click(object sender, EventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Do you want to Unassign selected students from User: " + DataLoadedUser + " ?", "Confirmation"/*box tttle*/, MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                UnassignStudends();
                lblDataCount.Content = "";
                lblDataCount.ToolTip = "";
            }
        }
        public void MultipleRowSelect(DataGrid dtgrd, Boolean status)
        {
            try
            {
                foreach (var selectedItem in dtgrd.SelectedItems)
                {
                    DataGridRow row = dtgrd.ItemContainerGenerator.ContainerFromItem(selectedItem) as DataGridRow;
                    if (row != null)
                    {
                        int rowIndex = row.GetIndex();

                        DataGridCell cell = GetCell(rowIndex, 0, dtgrd);
                        CheckBox ch = GetVisualChild<CheckBox>(cell);
                        ch.IsChecked = status;
                    }
                }
                CustomSelect = false;
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }
        void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)//grid row num
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private DataTable SelectedStdDataTable()
        {
            try
            {
                DataTable _tbleSelectedStd = new DataTable();
                _tbleSelectedStd.Columns.Add("CMP_STD_ID", typeof(Int32));
                int rowCount = 0;
                rowCount = grdStdData.Items.Count;
                if (rowCount > 0)
                {
                    for (int i = 0; i < rowCount; i++)
                    {
                        var selectedRow = (System.Data.DataRowView)grdStdData.Items[i];
                        int CMP_STD_ID = Int32.Parse(selectedRow["CMP_STD_ID"].ToString());
                        DataGridCell cell = GetCell(i, 0, grdStdData);
                        CheckBox chk = GetVisualChild<CheckBox>(cell);
                        if (chk.IsChecked.Value)
                        {
                            _tbleSelectedStd.Rows.Add(CMP_STD_ID);
                        }
                    }
                }
                return _tbleSelectedStd;
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
                return null;
            }
        }

        private void btnTransferLeads_Click(object sender, EventArgs e)
        {

            try
            {
                ToUserId = -1;
                DataTable _tblTransferStd = new DataTable();
                _tblTransferStd = SelectedStdDataTable();
                if (_tblTransferStd.Rows.Count > 0)
                {
                    /////popup window
                    DataTable _tblUsers = new DataTable();
                    _tblUsers = UsersTable.Copy(); ;
                    var selectedUser = _tblUsers.AsEnumerable()
                     .Where(r => Convert.ToInt32(r["ID"]) == DataLoadedUserId);

                    if (selectedUser.Any())
                    {
                        DataRow dataRow = selectedUser.First();

                        if (dataRow != null)
                        {
                            _tblUsers.Rows.Remove(dataRow);
                        }

                        Point point = btnTransferLeads.PointToScreen(new Point(0, 0));
                        CMSXtream.Pages.DataEntry.LeadTranferUserSelect form = new CMSXtream.Pages.DataEntry.LeadTranferUserSelect();
                        PopupHelper dialog = new PopupHelper
                        {
                            Title = "Select User",
                            Content = form,
                            ResizeMode = ResizeMode.NoResize,
                            /*MaxWidth = form.ActualWidth,
                            MaxHeight = form.ActualHeight,
                            WindowStartupLocation = 0,
                            Left = point.X - 225,
                            Top = point.Y*/
                            MaxWidth = 800,
                            MaxHeight = 500,
                            WindowStartupLocation = WindowStartupLocation.CenterScreen,
                        };
                        form.BindUserList(_tblUsers, DataLoadedUser,false,false,false,null, _tblTransferStd);
                        dialog.ShowDialog();
                        if (ToUserId != -1)//refresh page and db changes
                        {
                            try
                            {
                                AssignStudentToAdminsDA _chgUser = new AssignStudentToAdminsDA();
                                _chgUser.CHANGE_LEADS_USER(_tblTransferStd, ToUserId,StaticProperty.LoginUserID,DataLoadedUser);

                                //referh datgrid
                                RefreshDatagrid(_tblTransferStd);
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
                else
                {
                    MessageBox.Show("No selected Data");
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }
        private void btnDeleteLead_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedRow = grdStdData.SelectedItem as System.Data.DataRowView;
                int _cmpStdId = Int32.Parse(selectedRow["CMP_STD_ID"].ToString());
                string _studentName = selectedRow["STD_FULL_NAME"].ToString();

                MessageBoxResult result = MessageBox.Show("Do you want to Delete this Lead (" + _studentName + ")" + " ?", "Confirmation"/*box tttle*/, MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        AssignStudentToAdminsDA delLead = new AssignStudentToAdminsDA();
                        delLead.DeleteLead(_cmpStdId);
                        DataRow row = selectedRow.Row;
                        LoadedStudentListAll_Tbl.Rows.Remove(row);
                        grdStdData.ItemsSource = null;
                        grdStdData.ItemsSource = LoadedStudentListAll_Tbl.DefaultView;
                        if (lblDataCount.Content != "")
                        {
                            lblDataCount.Content = Int32.Parse(lblDataCount.Content.ToString()) - 1;

                            if (Int32.Parse(lblDataCount.Content.ToString()) - 1 < 0)//avoid - values
                            {
                                lblDataCount.Content = 0;
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
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void btnBulkDeleteLead_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CheckedStudentList_Tbl != null)
                {
                    CheckedStudentList_Tbl.Clear();
                }
                CheckedStudentList_Tbl = SelectedStdDataTable();
                if (CheckedStudentList_Tbl.Rows.Count == 0)//
                {
                    MessageBox.Show("No Any Selected Data");
                }
                else
                {
                    try
                    {
                        MessageBoxResult result = MessageBox.Show("Do you want to Delete selected Leads ?", "Confirmation"/*box tttle*/, MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                        {
                            AssignStudentToAdminsDA bulkLeadDelete = new AssignStudentToAdminsDA();
                            bulkLeadDelete.DeleteBulkLeads(CheckedStudentList_Tbl);
                            RefreshDatagrid(CheckedStudentList_Tbl);
                            lblDataCount.Content = "";
                            lblDataCount.ToolTip = "";
                        }
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show("Error" + ex);
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
        private void btnViewDeletedLeads_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int rowCount = 0;
                if (cmbSelectUser.SelectedValue != null)
                {
                    int _user_id = Int32.Parse(cmbSelectUser.SelectedValue.ToString());

                    DataTable _cmptable = new DataTable();
                    _cmptable.Columns.Add("CMP_ID", typeof(Int32));
                    rowCount = gridCmpNames.Items.Count;
                    if (rowCount > 0)
                    {
                        for (int i = 0; i < rowCount; i++)
                        {
                            var selectedRow = (System.Data.DataRowView)gridCmpNames.Items[i];
                            int cmpId = Int32.Parse(selectedRow["CMP_ID"].ToString());
                            DataGridCell cell = GetCell(i, 0, gridCmpNames);
                            CheckBox chk = GetVisualChild<CheckBox>(cell);
                            if (chk.IsChecked.Value)
                            {
                                _cmptable.Rows.Add(cmpId);
                            }
                        }
                    }
                    if (_cmptable.Rows.Count == 0)
                    {
                        MessageBox.Show("No filtration Data");
                    }
                    else
                    {
                        CMSXtream.Pages.DataEntry.MyLeads MyLeadOpen = new CMSXtream.Pages.DataEntry.MyLeads();

                        PopupHelper dialog = new PopupHelper
                        {
                            Title = LoginDA.getUserName(_user_id) + "'s Leads ",
                            Content = MyLeadOpen,
                            ResizeMode = ResizeMode.CanResize,
                            Width = this.Width,
                            Height = this.Height,
                            WindowStartupLocation = WindowStartupLocation.CenterOwner,
                            WindowState = WindowState.Maximized
                        };
                        
                        MyLeadOpen.LoadMyLeads(_user_id, _cmptable, null, DateTime.Now.Date, DateTime.Now.Date, _user_id != 0, 1, 1);
                        if (_user_id == -1)
                        {
                            MyLeads.TotalClick = true;//avoid deleting history when tranfer lead
                        }
                        dialog.ShowDialog();
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

        private void btnHistory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedRow = grdStdData.SelectedItem as System.Data.DataRowView;

                if (selectedRow != null)
                {
                    string _mobileNo = selectedRow["STD_TELEPHONE"].ToString();

                    DBSearch form = new DBSearch();
                    PopupHelper dialog = new PopupHelper
                    {
                        Title = "Class Payment",
                        Content = form,
                        ResizeMode = ResizeMode.NoResize,
                        Width = 1200,
                        Height = 700
                    };
                    form.txtbox.Text = _mobileNo;
                    form.btnSearchClick();
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

        private void btnAddLable_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                var selectedRow = grdStdData.SelectedItem as System.Data.DataRowView;
                if (selectedRow != null)
                {
                    CMSXtream.Pages.DataEntry.AddLableToCmpStudent form = new CMSXtream.Pages.DataEntry.AddLableToCmpStudent();
                    int _cmpStdId = int.Parse(selectedRow["CMP_STD_ID"].ToString());
                    form.CmpStdId = _cmpStdId;
                    form.CmpStdPhoneNumber = selectedRow["STD_TELEPHONE"].ToString();
                    form.UserId = Int32.Parse(cmbSelectUser.SelectedValue.ToString());
                    form.initComment = selectedRow["USER_COMMENT"].ToString();
                    form.LoadLables();
                    form.GetClassesToCombo();
                    PopupHelper dialog = new PopupHelper
                    {
                        Title = PhoneNumberFormatter.Format(selectedRow["STD_TELEPHONE"].ToString()) + " - " + selectedRow["STD_FULL_NAME"].ToString(),
                        Content = form,
                        ResizeMode = ResizeMode.NoResize,
                        Width = form.Width,
                        Height = form.Height,
                    };
                    form.UserChange = false;
                    Mouse.OverrideCursor = null;
                    dialog.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

    }
}
