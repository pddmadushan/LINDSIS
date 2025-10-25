using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using XtreamDataAccess;
using DataTable = System.Data.DataTable;

namespace CMSXtream.Pages.DataEntry
{
    /// <summary>
    /// Interaction logic for StudentFiltersAdvance.xaml
    /// </summary>
    public partial class LeadsSummary : UserControl
    {
        public DispatcherTimer timer;
        private bool isTransferRunning = false;

        Boolean isCheckAllGroup = false;
        int ChkBoxSlctnMode = 2;
        DateTime FromDate;
        DateTime ToDate;

        System.Data.DataTable _cmptable = new System.Data.DataTable();
        System.Data.DataTable dtLable = new System.Data.DataTable();
        System.Data.DataTable CommisionBreackDouwn ;
        public LeadsSummary()
        {
            InitializeComponent();
            BindCampaignGrid();
            BindLabelGrid();
            LoadCommition();
            _cmptable.Columns.Add("CMP_ID", typeof(Int32));
            for (int i = 0; i < grdCampaign.Items.Count; i++)
            {
                var selectedRow = (System.Data.DataRowView)grdCampaign.Items[i];
                int cmpId = Int32.Parse(selectedRow["CMP_ID"].ToString());
                _cmptable.Rows.Add(cmpId);
            }

            dtLable.Columns.Add("LBL_ID", typeof(Int32));

            DateTime dt = DateTime.Now;
            DateTime dtFrom = dt.AddDays(-2);
            dtpFromDate.SelectedDate = dtFrom;

            FromDate = dtpFromDate.SelectedDate.Value;
            ToDate = dtpToDate.SelectedDate.Value;

            LoadGrid();
            LoadClassGroup();
            lblAttLstRf.Content = DateTime.Now.ToString("hh:mm:ss tt");

            initializeTimer();

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

        private void chkLblMark_Checked(object sender, RoutedEventArgs e)
        {
            UpdateLblInCount();
        }

        private void chkLblMark_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateLblInCount();
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
        private void lableTextSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchSting = lableTextSearch.Text.Trim();
            if (searchSting != string.Empty)
            {
                Helper.searchGridByKey(grdLable, "LBL_DES", searchSting);
            }
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
        private void BindCampaignGrid()
        {
            try
            {
                FilterLeadDA _campaigns = new FilterLeadDA();
                System.Data.DataTable CampaignsAll = _campaigns.GetCampaigsToChkBox().Tables[0];
                if (CampaignsAll.Rows.Count > 0)
                {
                    grdCampaign.ItemsSource = CampaignsAll.DefaultView;
                }
                else
                {
                    grdCampaign.ItemsSource = null;
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
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

        void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
            HighlightLastRow();
        }


        /// <summary>
        /// //////////////////////////////checkboxes
        /// </summary>

        private void chkCmpMark_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                isCheckAllGroup = true;
                FristCellCheckUncheck(grdCampaign, true);
                txtGroupCount.Content = grdCampaign.Items.Count.ToString();
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
        private void chkCmpMark_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                isCheckAllGroup = true;
                FristCellCheckUncheck(grdCampaign, false);
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

        private void UpdateCampaingCount()
        {
            try
            {
                if (!isCheckAllGroup)
                {
                    int rowCount = grdCampaign.Items.Count;
                    string selectedList = "Selected Group List:";
                    int selectedCount = 0;
                    if (rowCount > 0)
                    {
                        for (int i = 0; i < rowCount; i++)
                        {
                            var selectedRow = (System.Data.DataRowView)grdCampaign.Items[i];
                            string displayName = selectedRow["CMP_DES"].ToString();
                            DataGridCell cell = GetCell(i, 0, grdCampaign);
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
        private void groupTextSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchSting = groupTextSearch.Text.Trim();
            if (searchSting != string.Empty)
            {
                Helper.searchGridByKey(grdCampaign, "CMP_DES", searchSting);
            }
        }

        private void chkCmpMarkAll_Checked(object sender, RoutedEventArgs e)
        {
            UpdateCampaingCount();
        }
        private void chkCmpMarkAll_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateCampaingCount();
        }

        private void btnSearchStudent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _cmptable.Clear();
                Int32 rowCount = grdCampaign.Items.Count;
                if (rowCount > 0)
                {
                    for (int i = 0; i < rowCount; i++)
                    {
                        var selectedRow = (System.Data.DataRowView)grdCampaign.Items[i];
                        int cmpId = Int32.Parse(selectedRow["CMP_ID"].ToString());
                        DataGridCell cell = GetCell(i, 0, grdCampaign);
                        System.Windows.Controls.CheckBox chk = GetVisualChild<System.Windows.Controls.CheckBox>(cell);
                        if (chk.IsChecked.Value)
                        {
                            _cmptable.Rows.Add(cmpId);
                        }
                    }
                }

                dtLable.Clear();
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
                FromDate = dtpFromDate.SelectedDate.Value;
                ToDate = dtpToDate.SelectedDate.Value;
                LoadGrid();
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }            
        }
        private void loadMyLeads(int showCondition, int userFromLogging = 0)
        {
            timer.Stop();
            try
            {
                int _user = userFromLogging;
                var selectedRowUser = grdLeadList.SelectedItem as System.Data.DataRowView;
                if (selectedRowUser != null && _user == 0)
                {
                    _user = int.Parse(selectedRowUser["USER_ID"].ToString());
                }

                if (_user == 0) return;

                CMSXtream.Pages.DataEntry.MyLeads MyLeadOpen = new CMSXtream.Pages.DataEntry.MyLeads();
                if (grdLeadList.Items.Count - 1 == grdLeadList.SelectedIndex)
                {
                    CMSXtream.Pages.DataEntry.MyLeads.TotalClick = true;
                }
                else
                {
                    CMSXtream.Pages.DataEntry.MyLeads.TotalClick = false;
                }
                PopupHelper dialog = new PopupHelper
                {
                    Title = LoginDA.getUserName(_user) + "'s Leads ",
                    Content = MyLeadOpen,
                    ResizeMode = ResizeMode.CanResize,
                    Width = this.Width,
                    Height = this.Height,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    WindowState = WindowState.Maximized,

                };
                MyLeadOpen.LoadMyLeads(_user, _cmptable, dtLable, FromDate, ToDate, _user != 0, showCondition);
                dialog.ShowDialog();
                LoadGrid();
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
            finally
            {
                timer.Start();
            }
        }
        private void LoadGrid()
        {
            try
            {
                DataTable TblLoadLead = LoadLeadSummeryDate(_cmptable, dtLable, FromDate, ToDate);
                if (TblLoadLead.Rows.Count > 0)
                {
                    grdLeadList.ItemsSource = TblLoadLead.DefaultView;
                }
                else
                {
                    grdLeadList.ItemsSource = null;
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
                lblLoadTime.Content = "Last refreshed time " + DateTime.Now.ToString();
                SetTransferCount(GetTransferCountfromDB());
            }
        }

        private DataTable LoadLeadSummery()
        {
            try
            {
                DataTable dtCampaign = new DataTable();
                dtCampaign.Columns.Add("CMP_ID", typeof(Int32));

                for (int i = 0; i < grdCampaign.Items.Count; i++)
                {
                    var selectedRow = (System.Data.DataRowView)grdCampaign.Items[i];
                    int cmpId = Int32.Parse(selectedRow["CMP_ID"].ToString());
                    DataGridCell cell = GetCell(i, 0, grdCampaign);
                    System.Windows.Controls.CheckBox chk = GetVisualChild<System.Windows.Controls.CheckBox>(cell);
                    if (chk.IsChecked.Value)
                    {
                        dtCampaign.Rows.Add(cmpId);
                    }
                }

                System.Data.DataTable dtLable = new System.Data.DataTable();
                dtLable.Columns.Add("LBL_ID", typeof(Int32));
                int rowCount = grdLable.Items.Count;
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

                FromDate = dtpFromDate.SelectedDate.Value;
                ToDate = dtpToDate.SelectedDate.Value;

                DataTable TblLoadLead = LoadLeadSummeryDate(dtCampaign, dtLable, FromDate, ToDate);
                return TblLoadLead;

            }
            catch (Exception ex)
            {
                System.Data.DataTable TblLoadLead = new System.Data.DataTable();
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
                return TblLoadLead;
            }
        }

        private DataTable LoadLeadSummeryDate(DataTable dtCampaign, DataTable dtLable, DateTime FromDate, DateTime ToDate)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                string _user = StaticProperty.LoginUserID.ToString();
                if (StaticProperty.LoginisAdmin == "1")
                {
                    _user = "SYSADMIN";
                }
                FilterLeadDA _filterStd = new FilterLeadDA();
                DataTable TblLoadLead = _filterStd.GetSummary(dtCampaign, dtLable, _user, FromDate, ToDate);
                return TblLoadLead;

            }
            catch (Exception ex)
            {
                System.Data.DataTable TblLoadLead = new System.Data.DataTable();
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
                return TblLoadLead;
            }
            finally 
            {
                Mouse.OverrideCursor = null;
            }
        }
        private void btnShowLead_Click(object sender, RoutedEventArgs e)
        {

            loadMyLeads(2);
        }
        private void btnViewPendin_Click(object sender, RoutedEventArgs e)
        {
            loadMyLeads(0);
        }

        private void btnViewAttended_Click(object sender, RoutedEventArgs e)
        { 
            loadMyLeads(4); 
        }

        private void btnViewdeDeleted_Click(object sender, RoutedEventArgs e)
        {
            loadMyLeads(3);
        }

        private void btnViewCompleted_Click(object sender, RoutedEventArgs e)
        {
            loadMyLeads(1);
        }

        private void btnComBreakDown_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CommisionBreackDouwn.Rows.Count > 0)
                {
                    CMSXtream.Pages.View.CommisionBreakDown MyCom = new CMSXtream.Pages.View.CommisionBreakDown();
                    PopupHelper dialog = new PopupHelper
                    {
                        Title = "My Commision Break Down",
                        Content = MyCom,
                        ResizeMode = ResizeMode.NoResize,
                        Width = 750,
                        Height = 600,
                        WindowStartupLocation = WindowStartupLocation.CenterScreen

                    };
                    MyCom.CommisionBreakDownTable = CommisionBreackDouwn;
                    MyCom.LoadFormContaint();
                    dialog.ShowDialog();
                }
                else
                {
                MessageBox.Show("You should work hard, No commision claculated!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }

        }

        private void btnComRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadCommition();
        }

        private void LoadCommition()
        {
            try
            {
                ClassAttendanceDA _clsPayment = new ClassAttendanceDA();
                _clsPayment.COM_VIEW_DATE = dtpComEffective.SelectedDate.Value;
                _clsPayment.CLS_USER_ID = StaticProperty.LoginUserID;

                System.Data.DataSet ComData= _clsPayment.CommisionBreakDown();
                txtCommisionMessage.Text = ComData.Tables[0].Rows[0][0].ToString();
                CommisionBreackDouwn = ComData.Tables[1];                
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        //private int CheckBoxSelectionMode()
        //{
        //    try
        //    {
        //        if (chkAll != null & chkComplete != null & chkPendig != null)
        //        {
        //            if (chkAll.IsChecked == true)
        //            {
        //                ChkBoxSlctnMode = 2;//all
        //            }
        //            if (chkPendig.IsChecked == true & chkComplete.IsChecked == false)
        //            {
        //                ChkBoxSlctnMode = 0;//pending records
        //            }
        //            if (chkPendig.IsChecked == false & chkComplete.IsChecked == true)
        //            {
        //                ChkBoxSlctnMode = 1;//complete records
        //            }
        //            if (chkPendig.IsChecked == false & chkComplete.IsChecked == false)
        //            {
        //                ChkBoxSlctnMode = 2;//all records
        //                chkAll.IsChecked = true;
        //            }
        //            return ChkBoxSlctnMode;
        //        }
        //        return 2;
        //    }
        //    catch (Exception ex)
        //    {
        //        LogFile logger = new LogFile();
        //        logger.MyLogFile(ex);
        //        MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
        //        return -1;
        //    }
        //}

        private void btnAddLeads_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                    CMSXtream.Pages.DataEntry.MannualAddLeads MannualAddLeads = new CMSXtream.Pages.DataEntry.MannualAddLeads();
                    PopupHelper dialog = new PopupHelper
                    {
                        Title = "Add Leads",
                        Content = MannualAddLeads,
                        ResizeMode = ResizeMode.CanResize,
                        Width = 1200,
                        Height = MannualAddLeads.Height,
                        WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    };
                    dialog.ShowDialog();
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void HighlightLastRow()
        {
            if (grdLeadList.Items.Count > 1)
            {
                var lastItem = grdLeadList.Items[grdLeadList.Items.Count - 1];
                DataGridRow lastRow = (DataGridRow)grdLeadList.ItemContainerGenerator.ContainerFromItem(lastItem);
                if (lastRow != null)
                {
                    lastRow.Background = Brushes.LightGreen;
                    lastRow.Header = "Total";
                    if (StaticProperty.LoginisAdmin != "1")
                    {
                        lastRow.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }
        private void LoadClassGroup()
        {
            try
            {
                ClassAttendanceDA classAttendanceDA = new ClassAttendanceDA();
                DataTable dt = new DataTable();
                dt = classAttendanceDA.ClassAttSummery().Tables[0];
                grdGroups.ItemsSource = dt.DefaultView;

                if (dt.Rows.Count==0)
                {
                    txtblckNoData.Visibility = Visibility.Visible;
                    grdGroups.Visibility= Visibility.Collapsed;
                }
                else
                {
                    grdGroups.Visibility = Visibility.Visible;
                    txtblckNoData.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex) 
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }
        private void btnClsAttList_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedRow = grdGroups.SelectedItem as System.Data.DataRowView;
                if (selectedRow != null)
                {
                    Int32 clasID = Int32.Parse(selectedRow["CLS_ID"].ToString());
                    string clsName = selectedRow["CLS_NAME"].ToString();
                    CMSXtream.Pages.View.StudentSummery form = new CMSXtream.Pages.View.StudentSummery(clasID, clsName);
                    PopupHelper dialog = new PopupHelper
                    {
                        Title = "Veiw Student",
                        Content = form,
                        ResizeMode = ResizeMode.NoResize,
                        Width = 1000
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
        private void btnRefreshAtt_Click(object sender, EventArgs e)
        {
            try
            {
                lblAttLstRf.Content = DateTime.Now.ToString("hh:mm:ss tt");
                LoadClassGroup();
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void btnLaodTransfer_Click(object sender, RoutedEventArgs e)
        {
            if (txtTransferCount.Content.ToString() == "0")
            {
                MessageBox.Show("No Transfer Lead Found. Refresh the grid to load", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                return;
            }
            try
            {
                ToDate = DateTime.Now;
                loadMyLeads(5, StaticProperty.LoginID);
            }
            finally
            {
                ToDate = dtpToDate.SelectedDate.Value;
            }
        }
        private void initializeTimer()
        {
            try
            {
                LoadClassGroup(); 
                Int32 TimeInterval = 10;
                timer = new DispatcherTimer(new TimeSpan(0, 0, TimeInterval), DispatcherPriority.Normal, async delegate
                {
                    if (isTransferRunning)
                    {
                        return;
                    }

                    string transferCount = "";
                    isTransferRunning = true;
                    try
                    {
                        await Task.Run(() =>
                        {
                            transferCount = GetTransferCountfromDB();
                        });
                    }
                    finally
                    {
                        isTransferRunning = false;
                    }
                    SetTransferCount(transferCount);


                }, this.Dispatcher);
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void SetTransferCount(string transferCount)
        {
            switch (transferCount)
            {
                case "-1":
                    txtTransferCount.Foreground = Brushes.Red;
                    txtTransferCount.Content = "Error";
                    break;
                case "0":
                    txtTransferCount.Foreground = Brushes.Green;
                    txtTransferCount.Content = transferCount;
                    break;
                default:
                    txtTransferCount.Foreground = Brushes.Orange;
                    txtTransferCount.Content = transferCount;
                    break;
            }
        }

        private string GetTransferCountfromDB()
        {
            try
            {
                ClassAttendanceDA classAttendanceDA = new ClassAttendanceDA();
                return classAttendanceDA.GetTransferCount(StaticProperty.LoginUserID, FromDate, DateTime.Now);
               
            }
            catch (Exception ex)
            {
               return "-1";
            }
        }
    }
}

