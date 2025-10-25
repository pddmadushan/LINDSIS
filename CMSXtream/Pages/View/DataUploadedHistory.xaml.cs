using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using XtreamDataAccess;

namespace CMSXtream.Pages.View
{
    /// <summary>
    /// Interaction logic for DataUploadedHistory.xaml
    /// </summary>
    public partial class DataUploadedHistory : UserControl
    {
        DataTable tblHistory;
        int CmpId;
        DateTime? Fromdate;
        DateTime? Todate;
        Boolean DataLoaded = false;
        public DataUploadedHistory()
        {
            InitializeComponent();
        }
        private void grdManualLead_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            {
                e.Row.Header = (e.Row.GetIndex() + 1).ToString();
            }
        }
        public void LoadSummery(int cmpId,DateTime? fromDate ,DateTime? toDate)
        {
            try
            {
                if (!DataLoaded)
                {
                    DataLoaded = true;
                    CmpId = cmpId;
                    Fromdate = fromDate;
                    Todate = toDate;
                }
                tblHistory = new DataTable();
                DataUploadedHistoryDA Summery = new DataUploadedHistoryDA();
                tblHistory = Summery.GetHistorySummery(CmpId, Fromdate, Todate).Tables[0];
                grdUploadedHistory.ItemsSource = tblHistory.DefaultView;

                if (tblHistory.Rows.Count == 0)
                {
                    lblNorecod.Content = "No any records for selected Values!";
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }
        private void btnHistoryDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = System.Windows.MessageBox.Show("Warning: This action will delete all ##UNconverted leads to student## records from the entire system, including all places where these records are currently in use. Are you sure you want to proceed with deleting this uploaded data?", "Confirmation"/*box tttle*/, MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    var selectedRow = grdUploadedHistory.SelectedItem as System.Data.DataRowView;
                    if (selectedRow != null)
                    {
                        DataRow row = selectedRow.Row;
                        int addId= int.Parse(row["ADD_ID"].ToString());
                        string _user= StaticProperty.LoginUserID.ToString();
                        DataUploadedHistoryDA delete= new DataUploadedHistoryDA();
                        delete.DeleteUloadedHistory(addId, _user);

                        LoadSummery(-1,DateTime.Now,DateTime.Now);//refresh
                    }
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number==547)
                {
                    MessageBox.Show("Cannot delete the record! The record is used elsewhere in the system.", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
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
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                lblNorecod.Content = null;
                DataLoaded = false;
                int _cmpId = int.Parse(cmbCmp.SelectedValue.ToString());
                DateTime? _fromDate = dtpFrom.SelectedDate;
                DateTime? _toDate = dtpTo.SelectedDate;
                if (_fromDate == null || _toDate == null)
                {
                    MessageBox.Show("Please Select Valid Date", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
                }
                else
                {
                    LoadSummery(_cmpId, _fromDate, _toDate);
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }
        public void LoadCmPList(DataTable tblcmpList)
        {
            try
            {
                //DataUploadedHistoryDA Cmp = new DataUploadedHistoryDA();
                //DataTable _tblCmp = new DataTable();
                // _tblCmp = Cmp.SelectAllCampaing().Tables[0];
                // cmbCmp.ItemsSource = _tblCmp.DefaultView;

               // tblcmpList.DefaultView.Sort = "CMP_ID";
                cmbCmp.ItemsSource = tblcmpList.DefaultView;
                cmbCmp.SelectedValuePath = "CMP_ID";
                cmbCmp.DisplayMemberPath = "CMP_DES";
                cmbCmp.SelectedValue = -1;

                dtpFrom.SelectedDate = DateTime.Now;
                dtpTo.SelectedDate = DateTime.Now;
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
