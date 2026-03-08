using CMSXtream.Handlers;
using CMSXtream.Pages.View;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using XtreamDataAccess;

namespace CMSXtream.Pages.DataEntry
{
    /// <summary>
    /// Interaction logic for AddLableToCmpStudent.xaml
    /// </summary>
    public partial class AddLableToCmpStudent : UserControl
    {
        public int CmpStdId { get; set; }
        public string CmpStdPhoneNumber { get; set; }
        Boolean singleCheck = false;
        public int UserId { get; set; }
        public string AssingUserCD { get; set; }

        Boolean CustomSelect = false;
        DataTable tblLoadLbl;
        DataTable tblClassList;
        DateTime? AttDate;
        public static int NoMoreAtt;
        public Boolean UserChange = false;
        public static int ProrityValue;
        public string initComment = string.Empty;
        public Boolean AddFromSearch = false;

        public AddLableToCmpStudent(Boolean CallFromTransfer = false)
        {
            if (!CallFromTransfer)
            {
                if (!ValidateTransfers())
                { 
                    return;
                }
            }
            InitializeComponent();            
        }

        private Boolean ValidateTransfers()
        {
            string transferCount = GetTransferCountfromDB();
            if (transferCount != "0")
            {
                MessageBox.Show("You have prioritised leads in my transfers. Go to My Lead and attend it", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                return false;
            }
            return true;
        }
        private string GetTransferCountfromDB()
        {
            try
            {
                ClassAttendanceDA classAttendanceDA = new ClassAttendanceDA();
                return classAttendanceDA.GetTransferCount(StaticProperty.LoginUserID, DateTime.Now.AddDays(-1), DateTime.Now);
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                return "-1";
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

        private void CheckboxCheckUncheck(DataGrid dtGrid, Boolean checkStatus)
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

                    chkHeader.IsChecked = false;
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
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
        private void txtLblDes_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchSting = txtLblDes.Text.Trim();
            if (searchSting != string.Empty)
            {
                Helper.searchGridByKey(grdLable, "LBL_DES", searchSting);
            }
        }

        private void chkCmpMarkAll_Checked(object sender, RoutedEventArgs e)
        {
            CheckboxCheckUncheck(grdLable, true);

        }
        private void chkCmpMarkAll_Unchecked(object sender, RoutedEventArgs e)
        {

            if (!singleCheck)
            {

                CheckboxCheckUncheck(grdLable, false);

            }
        }
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            singleCheck = true;
            UncheckHeaderCheckBox(grdLable, 0);
            singleCheck = false;
        }
        public void LoadUser()
        {
            if (grdLable != null)
            {
                AssignStudentToAdminsDA UserList = new AssignStudentToAdminsDA();
                DataTable tblUsers = UserList.GetAdminUsers().Tables[0];

                tblUsers.Rows.RemoveAt(0);

                cmbLeadTrnsUser.ItemsSource = tblUsers.DefaultView;
                cmbLeadTrnsUser.DisplayMemberPath = "USER_NAME";
                cmbLeadTrnsUser.SelectedValuePath = "ID";

                if (AssingUserCD != null)
                {
                    foreach (DataRowView row in tblUsers.DefaultView)
                    {
                        if (row["USER_NAME"].ToString() == AssingUserCD)
                        {
                            cmbLeadTrnsUser.SelectedItem = row;
                            UserId = int.Parse(row["ID"].ToString());
                            break;
                        }
                    }
                }
                else
                {
                    cmbLeadTrnsUser.SelectedValue = UserId;
                }
            }
        }
        public void LoadLables()
        {
            try
            {
                if (grdLable != null)
                {
                    LoadUser();

                    txtUserComment.Text = initComment;

                    AddLableToCmpStudentDA _loadLables = new AddLableToCmpStudentDA();
                    tblLoadLbl = _loadLables.SelectAllLablesAndStudentLable(CmpStdId).Tables[0];
                    if (tblLoadLbl.Rows.Count > 0)
                    {
                        grdLable.ItemsSource = tblLoadLbl.DefaultView;
                    }
                    else
                    {
                        grdLable.ItemsSource = null;
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

        private void PCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (sender != chkP1) chkP1.IsChecked = false;
            if (sender != chkP2) chkP2.IsChecked = false;
            if (sender != chkP3) chkP3.IsChecked = false;
            if (sender != chkNoMoreAttention) chkNoMoreAttention.IsChecked = false;

            if (chkNoMoreAttention.IsChecked == true)
            {
                if (dtpAttendantDate.SelectedDate != null)
                {
                    dtpAttendantDate.SelectedDate = null;
                    //MessageBoxResult result = MessageBox.Show("Attending date will be deleated, Do you want to continue?", "Confirmation"/*box tttle*/, MessageBoxButton.YesNo, MessageBoxImage.Question);
                    //if (result == MessageBoxResult.Yes)
                    //{
                    //    dtpAttendantDate.SelectedDate = null;
                    //}
                    //else
                    //{
                    //    chkNoMoreAttention.IsChecked = false;
                    //}
                }
                dtpAttendantDate.IsEnabled = false;
            }
        }
        private void chkNoMoreAttention_Unchecked(object sender, RoutedEventArgs e)
        {
            dtpAttendantDate.IsEnabled = true;
        }
        private void btnSaveLbl_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SaveValidation())
                {
                    int rowCount = 0;
                    DataTable _selectedLblTbl = new DataTable();
                    _selectedLblTbl.Columns.Add("LBL_ID", typeof(Int32));
                    rowCount = grdLable.Items.Count;
                    //if (rowCount > 0)
                    //{
                    for (int i = 0; i < rowCount; i++)
                    {
                        var selectedRow = (System.Data.DataRowView)grdLable.Items[i];
                        int lblId = Int32.Parse(selectedRow["LBL_ID"].ToString());
                        DataGridCell cell = GetCell(i, 0, grdLable);
                        CheckBox chk = GetVisualChild<CheckBox>(cell);
                        if (chk.IsChecked.Value)
                        {
                            _selectedLblTbl.Rows.Add(lblId);
                        }
                    }
                    try
                    {
                        ProrityValue = 0;

                        if (chkP3.IsChecked.Value) ProrityValue = 3;
                        if (chkP2.IsChecked.Value) ProrityValue = 2;
                        if (chkP1.IsChecked.Value) ProrityValue = 1;

                        if (chkNoMoreAttention.IsChecked == true)
                        {
                            NoMoreAtt = 1;
                        }
                        else
                        {
                            NoMoreAtt = 0;
                        }
                        string _loggedUser = StaticProperty.LoginUserID;
                        DateTime? attDate = dtpAttendantDate.SelectedDate;
                        int clsId = Int32.Parse(cmbClass.SelectedValue.ToString());
                        int userId = -1;
                        if (cmbLeadTrnsUser.SelectedValue != null)
                        {
                            userId = Int32.Parse(cmbLeadTrnsUser.SelectedValue.ToString());
                        }
                        AddLableToCmpStudentDA _saveLbl = new AddLableToCmpStudentDA();
                        _saveLbl.SaveLeadsLable(_selectedLblTbl, CmpStdId, _loggedUser, clsId, attDate, NoMoreAtt, ProrityValue, userId);

                        string curentComment = txtUserComment.Text.Trim();
                        if (curentComment != string.Empty || (curentComment == string.Empty && initComment != string.Empty))
                        {
                            MyLeadsDA _saveComment = new MyLeadsDA();
                            _saveComment.SaveUserComment(CmpStdId, curentComment, _loggedUser, clsId, attDate);
                            initComment = curentComment;
                            UserChange = true;
                        }

                        //reload lable changes
                        MyLeads.Reload = true;

                        var _selectedLbl = from r1 in tblLoadLbl.AsEnumerable()
                                           join r2 in _selectedLblTbl.AsEnumerable()
                                           on Convert.ToInt32(r1["LBL_ID"]) equals Convert.ToInt32(r2["LBL_ID"])
                                           select r1.Field<string>("LBL_DES");

                        MyLeads.SavedLableList = string.Join(", ", _selectedLbl);
                        MyLeads.tobeInfoText = "";
                        if (dtpAttendantDate.SelectedDate != null)
                        {
                            MyLeads.tobeInfoText = dtpAttendantDate.SelectedDate.Value.ToString("yyyy-MM-dd");
                        }
                        if (clsId != -1)
                        {
                            MyLeads.tobeInfoText = MyLeads.tobeInfoText + " " + cmbClass.Text;
                        }

                        if (userId != -1 && userId != UserId)
                        {
                            UserChange = true;
                        }
                        //reload lable changes

                        if (chkFstSMS.IsChecked == true)
                        {
                            if (clsId == -1)
                            {
                                MessageBox.Show("Please select a class for send SMS!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                            }
                            else
                            {
                                ((FirstFloor.ModernUI.Windows.Controls.ModernWindow)this.Parent).Close();
                                btnSendSMS();
                            }
                        }
                        else
                        {
                            if (AddFromSearch)
                            {
                                MessageBoxResult result = MessageBox.Show("Do you wan to send initial SMS?", "Confirmation"/*box tttle*/, MessageBoxButton.YesNo, MessageBoxImage.Question);
                                if (result == MessageBoxResult.Yes)
                                {
                                    if (clsId == -1)
                                    {
                                        MessageBox.Show("Please select a class for send SMS!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                                    }
                                    else
                                    {
                                        ((FirstFloor.ModernUI.Windows.Controls.ModernWindow)this.Parent).Close();
                                        btnSendSMS();
                                    }
                                }
                                else
                                {
                                    ((FirstFloor.ModernUI.Windows.Controls.ModernWindow)this.Parent).Close();
                                }
                            }
                            else
                            {
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
            }
            //}
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void btnSendSMS()
        {
            try
            {
                if (CmpStdPhoneNumber == String.Empty || CmpStdPhoneNumber == "(0)-")
                {
                    MessageBox.Show("Telephone number is required!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                    return;
                }

                string Telephone = CmpStdPhoneNumber.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("-", "");
                Telephone = PhoneNumberFormatter.AddLeadingZero(Telephone);
                if (Telephone.Length != 10)
                {
                    MessageBox.Show("Incorrect Telephone number!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                    return;
                }

                CMSXtream.Pages.DataEntry.SendSMS form = new CMSXtream.Pages.DataEntry.SendSMS();
                PopupHelper dialog = new PopupHelper
                {
                    Title = "Sent SMS to Student " + Telephone,
                    Content = form,
                    ResizeMode = ResizeMode.NoResize,
                    Width = 1100,
                    Height = 680
                };
                form.btnAddNew.Visibility = System.Windows.Visibility.Hidden;
                form.callingForm = true;
                form.filteredTable = null;

                form.txtRichBox.Text = GetMessageByTelephone(Telephone);
                form.txtEmpNumber.Text = Telephone;                
                form.txtTelephoneNumber.Text = Telephone;
                form.LoadSMSHistory();
                dialog.ShowDialog();
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        public string GetMessageByTelephone(string telephone)
        {
            string msg = "";
            AddLableToCmpStudentDA _saveLbl = new AddLableToCmpStudentDA();
            DataTable dt = _saveLbl.GetFristMessage(telephone).Tables[0];
            if (dt.Rows.Count > 0)
            {
                msg = dt.Rows[0][0].ToString();
            }
            return msg;
        }

        private Boolean SaveValidation()
        {
            if (chkNoMoreAttention.IsChecked != true && dtpAttendantDate.SelectedDate == null)
            {
                MessageBox.Show("You can not save the record without Attending Date!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                return false;
            }
            return true;
        }
        public void GetClassesToCombo()
        {
            if (grdLable != null)
            {
                try
                {
                    AddLableToCmpStudentDA _getClasses = new AddLableToCmpStudentDA();
                    DataSet _spReturn = _getClasses.GetClassesToComboAndAttDate(CmpStdId);
                    AttDate = (_spReturn.Tables[1].Rows[0][0] != DBNull.Value) ? (DateTime?)Convert.ToDateTime(_spReturn.Tables[1].Rows[0][0]) : null;
                    tblClassList = _spReturn.Tables[0];
                    NoMoreAtt = Convert.ToInt32(_spReturn.Tables[2].Rows[0][0]);
                    ProrityValue = Convert.ToInt32(_spReturn.Tables[2].Rows[0][1]);

                    dtpAttendantDate.SelectedDate = AttDate;
                    cmbClass.ItemsSource = tblClassList.DefaultView;
                    cmbClass.DisplayMemberPath = "CLS_NAME";
                    cmbClass.SelectedValuePath = "CLS_ID";

                    DisplayClassCmbBox(tblClassList);
                    if (NoMoreAtt == 1)
                    {
                        chkNoMoreAttention.IsChecked = true;
                    }

                    chkP1.IsChecked = ProrityValue == 1;
                    chkP2.IsChecked = ProrityValue == 2;
                    chkP3.IsChecked = ProrityValue == 3;
                }
                catch (Exception ex)
                {
                    LogFile logger = new LogFile();
                    logger.MyLogFile(ex);
                    MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);

                }
            }
        }
        void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)//grid row num
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void btnResetLbl_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (tblLoadLbl.Rows.Count > 0)
                {
                    grdLable.ItemsSource = null;
                    grdLable.ItemsSource = tblLoadLbl.DefaultView;
                }
                else
                {
                    grdLable.ItemsSource = null;
                }
                dtpAttendantDate.SelectedDate = AttDate;
                DisplayClassCmbBox(tblClassList);
                if (NoMoreAtt == 1)
                {
                    chkNoMoreAttention.IsChecked = true;
                }
                else
                {
                    chkNoMoreAttention.IsChecked = false;
                }
                chkP1.IsChecked = ProrityValue == 1;
                chkP2.IsChecked = ProrityValue == 2;
                chkP3.IsChecked = ProrityValue == 3;
                cmbLeadTrnsUser.SelectedValue = UserId;
                txtUserComment.Text = initComment;
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }
        private void DisplayClassCmbBox(DataTable clsList)
        {
            try
            {
                var quary = clsList.AsEnumerable().Count(r => Convert.ToInt32(r["CLS_ID"]) == 0);
                if (quary == 0)
                {
                    cmbClass.SelectedValue = -1;
                }
                else//if already entitle std to a cls -to load selected clss cmb
                {
                    cmbClass.SelectedValue = 0;//sp out entitled cls value=0
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }
        private void userComment_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            string comment = txtUserComment.Text.ToString();
            String inputKeyStroke = String.Empty;
            if (comment.Contains("@D"))
            {
                int curPos = txtUserComment.SelectionStart;
                string replaceDateTime = DateTime.Now.ToString("yy/MM/dd");
                txtUserComment.Text = comment.Replace("@D", replaceDateTime);
                txtUserComment.CaretIndex = curPos + replaceDateTime.Length;
            }
        }

        private void btnHistory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CmpStdId > 0 && CmpStdPhoneNumber != "")
                {
                    int cmpStdId = CmpStdId;
                    string _mobileNo = CmpStdPhoneNumber;

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
    }
}
