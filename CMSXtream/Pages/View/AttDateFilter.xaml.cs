using CMSXtream.Handlers;
using System;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using XtreamDataAccess;

namespace CMSXtream.Pages.View
{
    /// <summary>
    /// Interaction logic for DBSearch.xaml
    /// </summary>
    public partial class AttDateFilter : UserControl
    {
        int IsAdmin;
        DataTable tblUsers;
        public static int ToUserId = -1;
        public static Boolean IsLeadTransfered;
        DateTime FromDate;
        DateTime ToDate;
        int UserId;
        DataTable TblHistory;

        public AttDateFilter()
        {
            InitializeComponent();
            GetUsersAddsCampaignsFromDB();
            CheckUserType();

            dtpFollowingFromDate.SelectedDate = DateTime.Now;
            dtpFollowingToDate.SelectedDate = DateTime.Now;
        }
        void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void GetUsersAddsCampaignsFromDB()
        {
            try
            {
                DBSearchDA UserAndcmpAndAddList = new DBSearchDA();
                tblUsers = UserAndcmpAndAddList.LoadCampaignsAndAddNamesAndUsers().Tables[2];

                //tblUsers.Rows.RemoveAt(0);

                cmbUser.ItemsSource = tblUsers.DefaultView;
                cmbUser.DisplayMemberPath = "USER_NAME";
                cmbUser.SelectedValuePath = "ID";
                cmbUser.SelectedIndex = 0;

                foreach (DataRowView row in tblUsers.DefaultView)
                {
                    if (row["USER_NAME"].ToString() == StaticProperty.LoginUserID)
                    {
                        cmbUser.SelectedItem = row;
                        break;
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

        private void Search (){
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                if (cmbUser.SelectedValue == null)
                {
                    MessageBox.Show("Select a User");
                    return;
                }
                FromDate = dtpFollowingFromDate.SelectedDate.Value;
                ToDate = dtpFollowingToDate.SelectedDate.Value;
                UserId = int.Parse(cmbUser.SelectedValue.ToString());

                FollowingDateHistoryDA getHistory = new FollowingDateHistoryDA();
                TblHistory = new DataTable();
                TblHistory = getHistory.SearchFollowingDateHis(UserId, FromDate, ToDate).Tables[0];
                grdLeadData.ItemsSource = TblHistory.DefaultView;
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

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            Search();
        }
        private void CheckUserType()
        {
            try
            {
                IsAdmin = int.Parse(StaticProperty.LoginisAdmin);
                if (IsAdmin != 1)
                {
                    cmbUser.Text = StaticProperty.LoginUserID;
                    cmbUser.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
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
        private int GetColumnIndex(string XName)
        {
            try
            {
                var column = grdLeadData.FindName(XName) as DataGridTemplateColumn;
                if (column != null)
                {
                    int columnIndex = grdLeadData.Columns.IndexOf(column);
                    return columnIndex;
                }
                return -1;
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
                return -1;
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
        private void btnTranferLead_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Boolean loadPopup = false;
                DataTable _tblusers;
                string assgnedUser = null;
                int cmpStdId;
                Boolean isNewLead = false;
                var selectedLead = grdLeadData.SelectedItem as System.Data.DataRowView;
                Mouse.OverrideCursor = Cursors.Wait;
                if (selectedLead != null)
                {
                    DataRow dr = selectedLead.Row;
                    if (dr["CMP_STD_ID"] != DBNull.Value)
                    {
                        cmpStdId = int.Parse(dr["CMP_STD_ID"].ToString());
                        _tblusers = tblUsers.Copy();
                        var selectUser = _tblusers.AsEnumerable().Where(r => Convert.ToInt32(r["ID"]) == -1);
                        if (selectUser.Any())
                        {
                            DataRow dataRow = selectUser.First();

                            if (dataRow != null)
                            {
                                dataRow["USER_NAME"] = "Select a User";
                            }
                        }
                        if ((dr["CLS_USER_ID"] != DBNull.Value))
                        {
                            assgnedUser = dr["CLS_USER_ID"].ToString();
                            var selectedUser = _tblusers.AsEnumerable().Where(r => Convert.ToString(r["USER_NAME"]) == assgnedUser);

                            if (selectedUser.Any())
                            {
                                DataRow dataRow = selectedUser.First();

                                if (dataRow != null)
                                {
                                    _tblusers.Rows.Remove(dataRow);
                                }
                            }
                            loadPopup = true;
                        }
                        else
                        {
                            MessageBoxResult result = MessageBox.Show("Selected student has not assign to a user!.Thefore Can't complete this operation! \n Do you Want to Assign lead to User ?", "Confirmation"/*box tttle*/, MessageBoxButton.YesNo, MessageBoxImage.Question);

                            if (result == MessageBoxResult.Yes)
                            {
                                //////asign selected student to user
                                loadPopup = true;
                                isNewLead = true;
                            }
                        }
                        if (loadPopup)
                        {
                            CMSXtream.Pages.DataEntry.LeadTranferUserSelect form = new CMSXtream.Pages.DataEntry.LeadTranferUserSelect();
                            PopupHelper dialog = new PopupHelper
                            {
                                Title = "Select User",
                                Content = form,
                                ResizeMode = ResizeMode.NoResize,
                                MaxWidth = 800,
                                MaxHeight = 500,
                                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                            };
                            form.BindUserList(_tblusers, assgnedUser, isNewLead, true, false, cmpStdId, null);
                            dialog.ShowDialog();

                            try
                            {
                                if (IsLeadTransfered)
                                {
                                    DBSearchDA chngUser = new DBSearchDA();
                                    chngUser.CHANGE_LEADS_USER_SelRow(cmpStdId, ToUserId, assgnedUser, StaticProperty.LoginUserID);

                                    IsLeadTransfered = false;
                                    Search();
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
                        MessageBox.Show("Selected student is not a Campaign student(Old Record)!.Thefore Can't complete this operation!");
                    }
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
            finally { Mouse.OverrideCursor = null; }
        }
        private void btnViewDetail_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedLead = grdLeadData.SelectedItem as System.Data.DataRowView;

                if (selectedLead != null)
                {
                    DataRow dr = selectedLead.Row;
                    if (dr["CMP_STD_ID"] != DBNull.Value)
                    {
                        int CMP_STD_ID = int.Parse(dr["CMP_STD_ID"].ToString());
                        MyLeadsDA.CMP_STD_ID = CMP_STD_ID;

                        DataTable _tblCmp = new DataTable();
                        DataTable _tblLbl = new DataTable();
                        _tblCmp.Columns.Add("CMP_ID", typeof(int));
                        _tblLbl.Columns.Add("LBL_ID", typeof(int));

                        CMSXtream.Pages.DataEntry.MyLeads MyLeadOpen = new CMSXtream.Pages.DataEntry.MyLeads();
                        PopupHelper dialog = new PopupHelper
                        {
                            Title = "My Leads ",
                            Content = MyLeadOpen,
                            ResizeMode = ResizeMode.CanResize,
                            Width = this.Width,
                            Height = this.Height,
                            WindowStartupLocation = WindowStartupLocation.CenterOwner,
                            WindowState = WindowState.Maximized

                        };
                        MyLeadOpen.LoadMyLeads(-2, _tblCmp, _tblLbl, DateTime.Now, DateTime.Now, false, -2);
                        dialog.ShowDialog();
                        Search();
                        MyLeadsDA.CMP_STD_ID = null;
                    }
                    else
                    {
                        MessageBox.Show("Selected student is not a Campaign student(Old Record)!.Thefore Cant open this Window!");
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
        private void btnRestoreStudent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedRow = grdLeadData.SelectedItem as System.Data.DataRowView;
                DataRow row = selectedRow.Row;
                if (row != null)
                {
                    int _delCmpStdId = int.Parse(row["CMP_STD_ID"].ToString());
                    MessageBoxResult result = MessageBox.Show("Do you want to Restore selected students ?", "Confirmation"/*box tttle*/, MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        MyLeadsDA _delcmpstd = new MyLeadsDA();
                        _delcmpstd.RestoreDeletedLead(_delCmpStdId);
                        var record = TblHistory.AsEnumerable()
                            .Where(r => Convert.ToInt32(r["CMP_STD_ID"]) == _delCmpStdId).ToList();
                        record.ForEach(r => r["IS_DELETED"] = 0);
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
        private void btnDeleteStudent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedRow = grdLeadData.SelectedItem as System.Data.DataRowView;
                DataRow row = selectedRow.Row;
                if (row != null)
                {
                    int _delCmpStdId = int.Parse(row["CMP_STD_ID"].ToString());
                    MessageBoxResult result = MessageBox.Show("Do you want to delete selected students ?", "Confirmation"/*box tttle*/, MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        MyLeadsDA _delcmpstd = new MyLeadsDA();
                        _delcmpstd.DeleteLead(_delCmpStdId);
                        var record = TblHistory.AsEnumerable()
                            .Where(r => Convert.ToInt32(r["CMP_STD_ID"]) == _delCmpStdId).ToList();
                        record.ForEach(r => r["IS_DELETED"] = 1);
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
                Mouse.OverrideCursor = Cursors.Wait;
                var selectedRow = grdLeadData.SelectedItem as System.Data.DataRowView;
                if (selectedRow != null)
                {
                    CMSXtream.Pages.DataEntry.AddLableToCmpStudent form = new CMSXtream.Pages.DataEntry.AddLableToCmpStudent();
                    int _cmpStdId = int.Parse(selectedRow["CMP_STD_ID"].ToString());
                    form.CmpStdId = _cmpStdId;
                    form.CmpStdPhoneNumber = selectedRow["STD_TELEPHONE"].ToString();
                    form.AssingUserCD = selectedRow["CLS_USER_ID"].ToString();
                    form.initComment = selectedRow["COMMENT_OLD"].ToString();
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

                    if (form.UserChange == true)
                    {
                        Mouse.OverrideCursor = Cursors.Wait;
                        Search();
                    }

                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
            finally { Mouse.OverrideCursor = null; }
        }

        private void btnHistory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedRow = grdLeadData.SelectedItem as System.Data.DataRowView;

                if (selectedRow != null)
                {
                    int cmpStdId = int.Parse(selectedRow["CMP_STD_ID"].ToString());
                    string _mobileNo = selectedRow["STD_TELEPHONE"].ToString();

                    DBSearch form = new DBSearch();
                    PopupHelper dialog = new PopupHelper
                    {
                        Title = "History",
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

        private void btnAddStudent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show("Do you want to add this Lead as Student ?", "Confirmation"/*box tttle*/, MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    var selectedRow = grdLeadData.SelectedItem as System.Data.DataRowView;
                    if (selectedRow != null)
                    {
                        String _stdConvertedUserId = StaticProperty.LoginUserID;
                        int cmpStdId = int.Parse(selectedRow["CMP_STD_ID"].ToString());
                        string _mobileNo = selectedRow["STD_TELEPHONE"].ToString();
                        MyLeadsDA _addMyLeadAsStd = new MyLeadsDA();
                        Int32 STD = _addMyLeadAsStd.AddMyLeadAsStudent(cmpStdId, _stdConvertedUserId);

                        if (STD > 0)
                        {
                            MessageBox.Show("Please manually refresh", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
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
    }
}