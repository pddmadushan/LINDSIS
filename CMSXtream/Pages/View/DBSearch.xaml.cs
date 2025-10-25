using CMSXtream.Handlers;
using CMSXtream.Pages.DataEntry;
using System;
using System.Collections.Generic;
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
    public partial class DBSearch : UserControl
    {
        int IsAdmin;
        DataTable tblAddList;
        DataTable tblCmpList;
        DataTable tblUsers;
        DataSet dsCmPAndAddaAndUser;
        int IsdateFiltered;
        public static int ToUserId=-1;
        public static Boolean IsLeadTransfered;
        int IsAttdateFiltered;
        int IsPrirityFiltered;
        int DeletedCount;
        public DBSearch()
        {
            InitializeComponent();
            LoadcmbTpOrName();           
            GetUsersAddsCampaignsFromDB();
            LoadcmbAddOrCmp();
            CheckUserType();
            LoadP();

            dtpFromDate.SelectedDate = DateTime.Now;
            dtpToDate.SelectedDate = DateTime.Now;
            dtpAttFromDate.SelectedDate = DateTime.Now;
            dtpAttToDate.SelectedDate = DateTime.Now;
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
        void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void LoadcmbTpOrName()
        {
            try
            {
                cmbTpOrName.DisplayMemberPath = "Value";
                cmbTpOrName.SelectedValuePath = "Key";
                cmbTpOrName.Items.Add(new KeyValuePair<string, string>("2", "Name"));
                cmbTpOrName.Items.Add(new KeyValuePair<string, string>("1", "Telephone"));

                cmbTpOrName.SelectedValue = 1;
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }
        private void LoadcmbAddOrCmp()
        {
            try
            {
                cmbAddOrCmp.DisplayMemberPath = "Value";
                cmbAddOrCmp.SelectedValuePath = "Key";
                //cmbAddOrCmp.Items.Add(new KeyValuePair<string, string>("-1", "--All--(Campaigns & Adds)"));
                cmbAddOrCmp.Items.Add(new KeyValuePair<string, string>("1", "Campaign"));
                //cmbAddOrCmp.Items.Add(new KeyValuePair<string, string>("2", "Add Name"));

                cmbAddOrCmp.SelectedValue = 1;
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void LoadP()
        {
            try
            {
                cmbP.DisplayMemberPath = "Value";
                cmbP.SelectedValuePath = "Key";
                cmbP.Items.Add(new KeyValuePair<string, string>("1", "P1"));
                cmbP.Items.Add(new KeyValuePair<string, string>("2", "P2"));
                cmbP.Items.Add(new KeyValuePair<string, string>("3", "P3"));
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }


        private void GetUsersAddsCampaignsFromDB()
        {
            try
            {
                dsCmPAndAddaAndUser = new DataSet();
                DBSearchDA UserAndcmpAndAddList = new DBSearchDA();
                dsCmPAndAddaAndUser = UserAndcmpAndAddList.LoadCampaignsAndAddNamesAndUsers();

                tblUsers = new DataTable();
                tblUsers = dsCmPAndAddaAndUser.Tables[2];
                cmbUser.ItemsSource = tblUsers.DefaultView;
                cmbUser.DisplayMemberPath = "USER_NAME";
                cmbUser.SelectedValuePath = "ID";
                cmbUser.SelectedValue = -1;

                tblAddList = dsCmPAndAddaAndUser.Tables[0];
                tblCmpList = dsCmPAndAddaAndUser.Tables[1];
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void cmbAddOrCmp_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cmbAddOrCmp != null)
                {
                    int cmbSelVal = int.Parse(cmbAddOrCmp.SelectedValue.ToString());
                    if (cmbSelVal == 1)
                    {
                        cmbLoadedAddOrCmp.ItemsSource = tblCmpList.DefaultView;
                        cmbLoadedAddOrCmp.DisplayMemberPath = "CMP_DES";
                        cmbLoadedAddOrCmp.SelectedValuePath = "CMP_ID";
                        cmbLoadedAddOrCmp.SelectedValue = -1;
                        cmbLoadedAddOrCmp.IsEnabled = true;
                    }
                    if (cmbSelVal == 2)
                    {
                        cmbLoadedAddOrCmp.ItemsSource = tblAddList.DefaultView;
                        cmbLoadedAddOrCmp.DisplayMemberPath = "ADD_DES";
                        cmbLoadedAddOrCmp.SelectedValuePath = "ID";
                        cmbLoadedAddOrCmp.SelectedValue = -1;
                        cmbLoadedAddOrCmp.IsEnabled = true;
                    }
                    if (cmbSelVal == -1)
                    {
                        cmbLoadedAddOrCmp.IsEnabled = false;
                        cmbLoadedAddOrCmp.ItemsSource = null;
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
        public void btnSearchClick()
        {
            btnSearch_Click(null, null);
        }
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result=MessageBoxResult.Yes;

                if (txtbox.Text == string.Empty && int.Parse(cmbLoadedAddOrCmp.SelectedValue.ToString()) == -1
                    && IsAttdateFiltered == 0 && IsdateFiltered == 0 && IsPrirityFiltered == 0)
                {
                    if (IsAdmin != 1 || (IsAdmin == 1 && int.Parse(cmbUser.SelectedValue.ToString()) != -1))
                    {
                        result = MessageBox.Show("Are you sure load All leads data without any filteration?", "Confirmation"/*box tttle*/, MessageBoxButton.YesNo, MessageBoxImage.Question);
                    }
                    else
                    {
                        if (int.Parse(cmbUser.SelectedValue.ToString()) == -1)
                        {
                            result = MessageBox.Show("Are you sure load All leads and Student data in entire Database without any filteration?", "Confirmation"/*box tttle*/, MessageBoxButton.YesNo, MessageBoxImage.Question);
                        }
                    }
                }
                if (result == MessageBoxResult.Yes)
                {
                    LoadSearchData();
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }
        private void LoadSearchData()
        {
            try
            {
                int user = int.Parse(cmbUser.SelectedValue.ToString());
                DateTime? fromDate = dtpFromDate.SelectedDate;
                DateTime? toDate = dtpToDate.SelectedDate;
                DateTime? fromAttDate = dtpAttFromDate.SelectedDate;
                DateTime? toAttDate = dtpAttToDate.SelectedDate;
                int filterByNameOrPhone = int.Parse(cmbTpOrName.SelectedValue.ToString());
                int filterByCmpOrAdd = int.Parse(cmbAddOrCmp.SelectedValue.ToString());
                int prorityLevel = -1;

                int? cmpId = null;
                string addName = null;
                string filteText;
                if (filterByCmpOrAdd == 1)
                {
                    cmpId = int.Parse(cmbLoadedAddOrCmp.SelectedValue.ToString());
                }
                if (filterByCmpOrAdd == 2)
                {
                    addName = cmbLoadedAddOrCmp.Text;
                }
                if (txtbox.Text == string.Empty)
                {
                    filteText = null;
                }
                else
                {
                    user = -1;
                    filteText = txtbox.Text;
                }

                if (IsPrirityFiltered == 1) 
                {
                    prorityLevel = int.Parse(cmbP.SelectedValue.ToString());
                }
                DataTable dt = new DataTable();
                DBSearchDA search = new DBSearchDA();
                dt = search.Search(user, fromDate, toDate, filterByNameOrPhone,
                    filterByCmpOrAdd, cmpId, addName, filteText, IsAdmin, IsdateFiltered, fromAttDate, toAttDate, IsAttdateFiltered, prorityLevel).Tables[0];
                grdLeadData.ItemsSource = dt.DefaultView;

                var quarry = dt.AsEnumerable().Where(r => r.Field<string>("IS_REGISTERED") == "Yes");
                var quarry2 = dt.AsEnumerable().Where(r => r.Field<int>("IS_DELETED") == 1);
                var quarry3 = dt.AsEnumerable().Where(r => r.Field<int>("NO_MORE_ATTENTION") == 1);
                int regCount = quarry.Count();
                int total = dt.Rows.Count;
                int unRegCount = total - regCount;
                DeletedCount= quarry2.Count();

                lblTotal.Content = total.ToString();
                lblReg.Content = regCount.ToString();
                lblUnReg.Content = unRegCount.ToString();
                lblDelRec.Content = DeletedCount.ToString();
                lblNoMrAttRec.Content = quarry3.Count().ToString();
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
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
                    grdLeadData.Columns.RemoveAt(GetColumnIndex("clmTransferLead"));//remove lead tranfer button
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
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

        private void chkEnableDateFilter_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                dtpFromDate.IsEnabled = true;
                dtpToDate.IsEnabled = true;
                IsdateFiltered = 1;
                txtbox.Text = "";
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void chkEnableDateFilter_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                dtpFromDate.IsEnabled = false;
                dtpToDate.IsEnabled = false;
                IsdateFiltered = 0;
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void btnViewDetail_Click(object sender, EventArgs e)
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
                            Title = "My Leads",
                            Content = MyLeadOpen,
                            ResizeMode = ResizeMode.CanResize,
                            Width = this.Width,
                            Height = this.Height,
                            WindowStartupLocation = WindowStartupLocation.CenterOwner,
                            WindowState = WindowState.Maximized

                        };
                        MyLeadOpen.LoadMyLeads(-2, _tblCmp, _tblLbl, DateTime.Now, DateTime.Now, false, -2);
                        dialog.ShowDialog();
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
        private void btnTranferLead_Click(object sender, EventArgs e)
        {
            try
            {
                Boolean loadPopup = false;
                DataTable _tblusers;
                string assgnedUser=null;
                int cmpStdId;
                Boolean isNewLead=false;
                var selectedLead = grdLeadData.SelectedItem as System.Data.DataRowView;

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
                        if ((dr["ASSIGNED_USER"] != DBNull.Value))
                        {
                            assgnedUser = dr["ASSIGNED_USER"].ToString();
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
                                isNewLead=true;
                            }
                        }
                        if (loadPopup)
                        {
                            int _columnIndex = GetColumnIndex("clmTransferLead");
                            int row_id = grdLeadData.SelectedIndex;
                            DataGridCell cell = GetCell(row_id, _columnIndex, grdLeadData);
                            Button btntransfer = GetVisualChild<Button>(cell);

                            Point point = btntransfer.PointToScreen(new Point(0, 0));
                            CMSXtream.Pages.DataEntry.LeadTranferUserSelect form = new CMSXtream.Pages.DataEntry.LeadTranferUserSelect();
                            PopupHelper dialog = new PopupHelper
                            {
                                Title = "Select User",
                                Content = form,
                                ResizeMode = ResizeMode.NoResize,
                                //MaxWidth = form.ActualWidth,
                                //MaxHeight = form.ActualHeight,
                                //WindowStartupLocation = 0,
                                // Left = point.X - 225,
                                // Top = point.Y - 225
                                MaxWidth = 800,
                                MaxHeight = 500,
                                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                            };
                            form.BindUserList(_tblusers, assgnedUser, isNewLead,true,false, cmpStdId,null);
                            dialog.ShowDialog();

                            try
                            {
                                if (IsLeadTransfered)
                                {
                                    DBSearchDA chngUser = new DBSearchDA();
                                chngUser.CHANGE_LEADS_USER_SelRow(cmpStdId, ToUserId, assgnedUser, StaticProperty.LoginUserID);
                                
                                    IsLeadTransfered=false;
                                    var assignUser = _tblusers.AsEnumerable().FirstOrDefault(r => Convert.ToInt32(r["ID"]) == ToUserId);
                                    if (assignUser != null)
                                    {
                                        if (int.Parse(cmbUser.SelectedValue.ToString())!=-1)
                                        {
                                            dr.Delete();
                                        }
                                        else
                                        {
                                            dr["ASSIGNED_USER"] = assignUser["USER_NAME"].ToString();
                                            dr["ASSIGNED_DATE"] = DateTime.Now.ToString("yyyy-MM-dd");
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

        }

        private void chkEnableAttDateFilter_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                dtpAttFromDate.IsEnabled = true;
                dtpAttToDate.IsEnabled = true;
                IsAttdateFiltered = 1;
                txtbox.Text = "";
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void chkEnableAttDateFilter_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                dtpAttFromDate.IsEnabled = false;
                dtpAttToDate.IsEnabled = false;
                IsAttdateFiltered = 0;
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void chkP_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                cmbP.IsEnabled = true;
                cmbP.SelectedIndex = 0;
                IsPrirityFiltered = 1;
                txtbox.Text = "";
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void chkP_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                cmbP.IsEnabled = false;
                cmbP.SelectedIndex = -1;
                IsPrirityFiltered = 0;
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
                        row["IS_DELETED"] = 1;
                        DeletedCount = DeletedCount + 1;
                        lblDelRec.Content = DeletedCount.ToString();
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
                        row["IS_DELETED"] = 0;
                        DeletedCount = DeletedCount - 1;
                        lblDelRec.Content = DeletedCount.ToString();
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

        private void cmbTpOrName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            txtbox.Text = "";
        }

        private void DataGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Prevent the event from bubbling up
            e.Handled = true;

            // Determine the current vertical offset
            var scrollViewer = GetScrollViewer(grdLeadData);
            if (scrollViewer != null)
            {
                // Calculate the new vertical offset
                double newOffset = scrollViewer.VerticalOffset - (e.Delta > 0 ? 1 : -1) * scrollViewer.ViewportHeight;
                scrollViewer.ScrollToVerticalOffset(newOffset);
            }
        }

        private ScrollViewer GetScrollViewer(DependencyObject parent)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is ScrollViewer scrollViewer)
                    return scrollViewer;
                else
                {
                    var result = GetScrollViewer(child);
                    if (result != null)
                        return result;
                }
            }
            return null;
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
                    form.AssingUserCD = selectedRow["ASSIGNED_USER"].ToString();
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

                    if (form.UserChange == true)
                    {
                        Mouse.OverrideCursor = Cursors.Wait;
                        LoadSearchData();
                    }
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
