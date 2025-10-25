using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using XtreamDataAccess;

namespace CMSXtream.Pages.DataEntry
{
    /// <summary>
    /// Interaction logic for CampaingName.xaml
    /// </summary>
    public partial class CampaingName : UserControl
    {
        public CampaingName()
        {
            InitializeComponent();

            foreach (DataGridColumn col in cmpdatagrid.Columns)//hide column
            {
                if (col.Header.ToString() == "cmp_id")
                {
                    col.Visibility = Visibility.Hidden;
                }
            }
            cmpdatagrid.Height = 500;
            GetDataFromDBtoDatagrid();
        }
        public void GetDataFromDBtoDatagrid()
        {
            try
            {
                CampaingDA _cmpNames = new CampaingDA();
                System.Data.DataTable table = _cmpNames.SelectAllCampaing().Tables[0];
                if (table.Rows.Count > 0)
                {
                    cmpdatagrid.ItemsSource = table.DefaultView;
                }
                else
                {
                    cmpdatagrid.ItemsSource = null;
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (StaticProperty.LoginisAdmin != "1")
            {
                MessageBox.Show("Sorry, you are not authorized to delete Campaign, please contact administrator,", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.No);
                return;
            }

            MessageBoxResult result = MessageBox.Show("Do you want to delete this Campaign?", "Confirmation"/*box tttle*/, MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var selectedRow = cmpdatagrid.SelectedItem as System.Data.DataRowView;
                if (selectedRow != null)
                {
                    try
                    {
                        CampaingDA _campaingDel = new CampaingDA();
                        Int32 CMPId = Int32.Parse(selectedRow["CMP_ID"].ToString());
                        _campaingDel.CMP_ID = CMPId;
                        _campaingDel.DeleteCMP();
                        TextAddcmp.Text = "";
                        cmplbl.Content = -1;
                        GetDataFromDBtoDatagrid();
                    }
                    catch (SqlException ex) { 
                        MessageBox.Show("The record Can't delete! recored is used elsewhere in the system.");
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
        private void cmpsave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TextAddcmp.Text.Trim() != string.Empty)
                {
                    CampaingDA _campaingSave = new CampaingDA();
                    _campaingSave.CMP_ID = Int32.Parse(cmplbl.Content.ToString());//-1 to identify new data to sp
                    _campaingSave.CMP_DES = TextAddcmp.Text.Trim();
                    if (cmpActiveChk.IsChecked == true) {
                        _campaingSave.CMP_ACTIVE = 1;
                    }
                    else {
                        _campaingSave.CMP_ACTIVE = 0;
                    }

                    if (_campaingSave.Savedata() > 0)
                    {
                        if (Convert.ToInt32(cmplbl.Content) != -1)
                        {
                            MessageBox.Show("Data is Succesfully Updated");
                            GetDataFromDBtoDatagrid();
                            TextAddcmp.Text = "";
                            cmplbl.Content = -1;
                          
                        }
                        else
                        {
                            MessageBox.Show("Recored is Successfully saved");
                            GetDataFromDBtoDatagrid();
                            TextAddcmp.Text = "";
                            cmplbl.Content = -1;
                        }
                    }
                    else {
                        MessageBox.Show("Campaing name Already  Exist !!");
                    }
                }
                cmpActiveChk.IsChecked = false;


            }
            catch (Exception ex) {

                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }
        private void cmpdatagrid_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedRow = cmpdatagrid.SelectedItem as System.Data.DataRowView;  //????AS ystem.Data.DataRowView
            if (StaticProperty.LoginisAdmin != "1")
            {
                MessageBox.Show("Sorry, you are not authorized to Edit campaign details, please contact administrator,", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.No);
                return;
            }
            if ( selectedRow != null )
            {
                Int32 CMP_ID = Int32.Parse(selectedRow["CMP_ID"].ToString());
                TextAddcmp.Text= selectedRow["CMP_DES"].ToString();
                cmplbl.Content=CMP_ID.ToString();

               // Int32 _chkValue = Int32.Parse(selectedRow["IS_ACTIVE"].ToString());
                bool _chkValue = bool.Parse(selectedRow["IS_ACTIVE"].ToString());

                if (_chkValue == true)
                {
                   cmpActiveChk.IsChecked= true;
                }
                else
                {
                    cmpActiveChk.IsChecked = false;
                }

            }
        }

        private void btnAddNewCampaign_Click(object sender, RoutedEventArgs e)
        {
            cmplbl.Content = -1;
            TextAddcmp.Text = "";
            cmpActiveChk.IsChecked=false;
        }
    }
}
