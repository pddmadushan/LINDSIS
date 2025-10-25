using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Data;
using XtreamDataAccess;
using System.Text.RegularExpressions;
using System.Linq;
using System.Globalization;
using System.Windows.Data;

namespace CMSXtream.Pages.DataEntry
{
    /// <summary>
    /// Interaction logic for MannualAddLeads.xaml
    /// </summary>
    public partial class MannualAddLeads : UserControl
    {
        DataTable tblManualLead;
        public string OutResult { get; set; }
        public ObservableCollection<int> DataCollection { get; set; }
        public MannualAddLeads()
        {
            OutResult = "";
            InitializeComponent();
            BindCampaignCmb();
            BindUsersCmb();
            CheckUserType();
            GridDataBind();
            btnSave.Visibility = Visibility.Visible;
            btnClose.Visibility = Visibility.Hidden;
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            BindCampaignCmb();
            BindUsersCmb();
            CheckUserType();
        }

        private void grdManualLead_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            {
                e.Row.Header = (e.Row.GetIndex() + 1).ToString();
            }
        }
        private void GridDataBind()
        {
            try {
                tblManualLead = new DataTable();
                tblManualLead.Columns.Add("CMP_STD_ID", typeof(int));
                tblManualLead.Columns.Add("STD_FIRST_NAME", typeof(String));
                tblManualLead.Columns.Add("STD_LAST_NAME", typeof(String));
                tblManualLead.Columns.Add("STD_TELEPHONE", typeof(string));                
                tblManualLead.Columns.Add("USER_COMMENT", typeof(string));
                tblManualLead.Columns.Add("INV", typeof(int));
                tblManualLead.Columns["INV"].DefaultValue = 0;
                tblManualLead.Columns["CMP_STD_ID"].DefaultValue = 0;
                tblManualLead.Columns["STD_TELEPHONE"].DefaultValue = "";
                grdManualLead.ItemsSource = tblManualLead.DefaultView;

                //tblManualLead.Columns["STD_TELEPHONE"].AllowDBNull=true;
                tblManualLead.Rows.Add();//load 1st row
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
                if (EmtyName() == 0)
                {
                    if (ValidateMobileNumber() == 0)
                    {
                        if (tblManualLead.Rows.Count > 0)
                        {
                            int _user = int.Parse(cmbUsers.SelectedValue.ToString());

                            if (_user != -1)
                            {
                                if (cmbCampaigns.SelectedValue != null)
                                {
                                    DateTime? adDate = dtpAddDate.SelectedDate;
                                    string addName = txtbxAddName.Text;
                                    Boolean save = true;
                                    MessageBoxResult result=MessageBoxResult.Yes;
                                    if (adDate == null)
                                    {
                                        result = MessageBox.Show("Ad Date is not selected Do you want to Save?", "Confirmation"/*box tttle*/, MessageBoxButton.YesNo, MessageBoxImage.Question);
                                        if (result == MessageBoxResult.No)
                                        {
                                            save = false;
                                        }
                                    }
                                    //if((addName=="" || addName=="Ad Name") && result== MessageBoxResult.Yes)
                                    //{
                                    //    result = MessageBox.Show("Ad Name not Specified Do you want to Save?", "Confirmation"/*box tttle*/, MessageBoxButton.YesNo, MessageBoxImage.Question);
                                    //    if (result == MessageBoxResult.No)
                                    //    {
                                    //        save = false;
                                    //    }
                                    //    else
                                    //    {
                                    //        addName = null;
                                    //    }
                                    //}
                                    if (save)
                                    {
                                        if (tblManualLead.Columns["INV"] != null)
                                        {
                                            tblManualLead.Columns.Remove("INV");
                                        }
                                        int _cmpId = int.Parse(cmbCampaigns.SelectedValue.ToString());
                                        string _loggedUser = StaticProperty.LoginUserID;
                                        MannualAddLeadsDA Dupld = new MannualAddLeadsDA();

                                        DataTable retManualLead = new DataTable();
                                        retManualLead = Dupld.ManualLeadInsert(tblManualLead, _cmpId, _user, _loggedUser,adDate,addName);
                                        grdManualLead.ItemsSource = retManualLead.DefaultView; 

                                        btnSave.Visibility = Visibility.Hidden;
                                        btnClose.Visibility = Visibility.Visible;
                                        MessageBox.Show("Upload complete.Now you can add lables");
                                        OutResult = "OK";
                                        //((FirstFloor.ModernUI.Windows.Controls.ModernWindow)this.Parent).Close();
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Select a campaign");
                                }
                            }
                            else
                            {
                                MessageBox.Show("Select a User");
                            }
                        }
                        else
                        {
                            MessageBox.Show("No data to Upload");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Invalid Mobile Numbers are found!");
                    }
                }
                else
                {
                    MessageBox.Show("First name cannot be empty!");
                }
            }

            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }
        private void btnDeleteLead_Click(object sender, RoutedEventArgs e)
        {
            if (btnSave.Visibility == Visibility.Hidden) {
                MessageBox.Show("You can not delete already saved data from here", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.No);
                return;
            }

            try
            {
                var selectedRow = grdManualLead.SelectedItem as System.Data.DataRowView;
                DataRow row = selectedRow.Row;
                if (row != null)
                {
                    tblManualLead.Rows.Remove(row);
                }
            }
            catch(Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)

                {
                    int count = grdManualLead.Items.Count;
                    if (grdManualLead.SelectedIndex == (count - 1))
                    {
                        //e.Handled = true; // Prevent the default behavior (move to the next row)
                        tblManualLead.Rows.Add();
                        grdManualLead.CurrentCell = new DataGridCellInfo(grdManualLead.Items[count], grdManualLead.Columns[0]);//cursor set to 1st cell
                    }
                }
            }
            catch(Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void BindCampaignCmb()
        {
            try
            {
                DataTable tblCmpList = new DataTable();
                CampaingDataUploadDA cmbCampaign=new CampaingDataUploadDA();
                tblCmpList = cmbCampaign.SelectAllCampaing().Tables[0];
                cmbCampaigns.ItemsSource = tblCmpList.DefaultView;
                cmbCampaigns.SelectedValuePath = "CMP_ID";
                cmbCampaigns.DisplayMemberPath = "CMP_DES";
                if (cmbCampaigns.SelectedValue == null)
                {
                    cmbCampaigns.SelectedIndex = 0;
                    cmbCampaigns.SelectedValue = null;
                }

            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        public void BindUsersCmb()
        {
            try
            {
                AssignStudentToAdminsDA _users = new AssignStudentToAdminsDA();
                DataTable _userTable = new DataTable();
                _userTable = _users.GetAdminUsers().Tables[0];
                cmbUsers.ItemsSource = _userTable.DefaultView;
                cmbUsers.DisplayMemberPath = "USER_NAME";
                cmbUsers.SelectedValuePath = "ID";
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
            string _userName = StaticProperty.LoginUserID;
            AssignStudentToAdminsDA _userType = new AssignStudentToAdminsDA();
            int userType = _userType.CheckUserType(_userName);

            if(userType == 0)
            {
                cmbUsers.IsEnabled = false;
                cmbUsers.Text=_userName;
            }
            if(userType == 1)
            {
                cmbUsers.Text = _userName;
            }
            if (userType == -1)
            {
                cmbUsers.SelectedValue=-1;
            }
        }

        private int ValidateMobileNumber()
        {
            try
            {
                int count=-1;
                if (tblManualLead.Columns["INV"] != null)
                {

                    var regex = new Regex("[^0-9]");

                    var query = from r in tblManualLead.AsEnumerable()
                                where regex.IsMatch(r.Field<string>("STD_TELEPHONE")) || r.Field<string>("STD_TELEPHONE").Length > 10 || r.Field<string>("STD_TELEPHONE").Length < 9
                                select r;
                    count = query.Count();
                    foreach (var row in query)
                    {
                        row.SetField("INV", 1);
                    }
                    foreach (var row in tblManualLead.AsEnumerable().Except(query))
                    {
                        row.SetField("INV", 0);
                    }
                }
                    return count;
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
                return -1;
            }
        }

        private int EmtyName()
        {
            try
            {
                int count = -1;

                count = tblManualLead.AsEnumerable()
                    .Count(row => row.IsNull("STD_FIRST_NAME") || string.IsNullOrWhiteSpace(row["STD_FIRST_NAME"].ToString()));
                return count;
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
                return -1;
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            ((FirstFloor.ModernUI.Windows.Controls.ModernWindow)this.Parent).Close();
        }

        private void btnAddLable_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedRow = grdManualLead.SelectedItem as System.Data.DataRowView;
                if (selectedRow != null)
                {
                   
                    CMSXtream.Pages.DataEntry.AddLableToCmpStudent form = new CMSXtream.Pages.DataEntry.AddLableToCmpStudent();
                    int _cmpStdId = int.Parse(selectedRow["CMP_STD_ID"].ToString());

                    form.CmpStdId = _cmpStdId;
                    form.LoadLables();
                    form.GetClassesToCombo();
                    PopupHelper dialog = new PopupHelper
                    {
                        Title = "Define Leads Labels",
                        Content = form,
                        ResizeMode = ResizeMode.NoResize,
                        Width = form.Width,
                        Height = form.Height,
                    };
                    form.initComment = selectedRow["USER_COMMENT"].ToString();

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
