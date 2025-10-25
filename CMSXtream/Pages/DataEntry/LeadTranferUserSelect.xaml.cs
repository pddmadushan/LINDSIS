using CMSXtream.Pages.View;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using XtreamDataAccess;


namespace CMSXtream.Pages.DataEntry
{
    /// <summary>
    /// Interaction logic for LeadTranferUserSelect.xaml
    /// </summary>
    public partial class LeadTranferUserSelect : UserControl
    {
        string OldUser;
        Boolean IsAssignLead= false;
        Boolean IsSearchPG= false;
        Boolean IsFromMyLeadpg = false;
        public LeadTranferUserSelect()
        {
            InitializeComponent();
        }
        public void BindUserList(DataTable users, string _oldUser, Boolean isassLead = false,Boolean isSearchpg=false, Boolean isFromMyLeadpg=false, int? cmpStdId=null,DataTable tblLeads=null)
        {
            cmbLeadTrnsUser.ItemsSource = users.DefaultView;
            cmbLeadTrnsUser.DisplayMemberPath = "USER_NAME";
            cmbLeadTrnsUser.SelectedValuePath = "ID";
            cmbLeadTrnsUser.SelectedValue = -1;
            OldUser = _oldUser;

            IsAssignLead= isassLead;
            IsSearchPG = isSearchpg;
            IsFromMyLeadpg = isFromMyLeadpg;

            ViewLeadHistory(cmpStdId,tblLeads);
        }

        private void btnTransferLeads_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int _userId = int.Parse(cmbLeadTrnsUser.SelectedValue.ToString());
                if (_userId != -1)
                {
                    string _toUserName = cmbLeadTrnsUser.Text;
                    MessageBoxResult result ;
                    if (!IsAssignLead) {
                        result = MessageBox.Show("Are you sure change " + OldUser + " 's Leads to " + _toUserName + " ?", "Confirmation"/*box tttle*/, MessageBoxButton.YesNo, MessageBoxImage.Question);
                    }
                    else
                    {
                        result = MessageBox.Show("Are you sure assign Selected Leads to " + _toUserName + " ?", "Confirmation"/*box tttle*/, MessageBoxButton.YesNo, MessageBoxImage.Question);
                    }
                    if (result == MessageBoxResult.Yes)
                    {
                        if (!IsSearchPG && !IsFromMyLeadpg) {
                            AssignStudentToAdmins.ToUserId = _userId; 
                        }
                        else if(!IsSearchPG && IsFromMyLeadpg)
                        {
                            MyLeads.ToUserId = _userId;
                            MyLeads.IsLeadTransfered = true;
                        }
                        else
                        {
                            DBSearch.ToUserId = _userId;
                            DBSearch.IsLeadTransfered = true;
                            AttDateFilter.IsLeadTransfered=true;
                            AttDateFilter.ToUserId = _userId;
                        }
                        ((FirstFloor.ModernUI.Windows.Controls.ModernWindow)this.Parent).Close();
                    }
                }
                else
                {
                    MessageBox.Show("Select a User!");
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

        private void ViewLeadHistory(int? CmpUserId,DataTable leads)
        {
            try
            {
                LeadTranferUserSelectDA LdHis = new LeadTranferUserSelectDA();
                DataTable tblHistory = LdHis.ViewLeadTranferHistory(CmpUserId, leads);
                grdHistory.ItemsSource = tblHistory.DefaultView;
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

