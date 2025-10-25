using System;
using System.Windows;
using System.Windows.Controls;
using XtreamDataAccess;

namespace CMSXtream.Pages.View
{
    /// <summary>
    /// Interaction logic for Incentive.xaml
    /// </summary>
    public partial class Incentive : UserControl
    {      
        public Incentive()
        {
            InitializeComponent();
            Loaded += Incentive_Loaded;

        }


        void Incentive_Loaded(object sender, RoutedEventArgs e)
        {
            LoadInt();
            LoadInfo();
        }

        private void LoadInt()
        {
            try
            {
                cmbSearchUser.IsEnabled = false;
                if (StaticProperty.LoginisAdmin == "1")
                {
                    cmbSearchUser.IsEnabled = true;
                }
                LoadActiveusers();
                dtpPayMonth.SelectedDate = DateTime.Now;
                dtpPayMonth.DisplayDateStart = new DateTime(2021,1, 1);
                dtpPayMonth.DisplayDateEnd = DateTime.Now;
                pgrsBar.Value = 0;                
            }
            catch (Exception ex)
            {
                pgrsBar.Value = 0;
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void dtpPayMonth_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadInfo();
        }

        private void LoadInfo()
        {
            try
            {
                if (dtpPayMonth.SelectedDate != null & cmbSearchUser.SelectedValue!=null)
                {
                    BackUPDB _clsBackUp = new BackUPDB();
                    DateTime selectedDate = new DateTime(dtpPayMonth.SelectedDate.Value.Year, dtpPayMonth.SelectedDate.Value.Month, 1);
                    System.Data.DataSet ds = _clsBackUp.GetIncentiveInfo(selectedDate, cmbSearchUser.SelectedValue.ToString());
                    System.Data.DataTable table = ds.Tables[0];
                    System.Data.DataTable table2 = ds.Tables[1];

                    pgrsBar.Value = 0;
                    lblInfo.Content = string.Empty;
                    lblIncentiveAmt.Content = string.Empty;

                    if (table.Rows.Count > 0)
                    {
                        pgrsBar.Value = double.Parse(table.Rows[0][0].ToString());
                        lblInfo.Content = table.Rows[0][1].ToString();
                        lblIncentiveAmt.Content = table.Rows[0][2].ToString();

                        if (table2.Rows.Count > 0)
                        {
                            grdInsentiveInfo.ItemsSource = table2.DefaultView;
                        }
                        else
                        {
                            grdInsentiveInfo.ItemsSource = null;
                        }
                    }
                    else
                    {
                        grdInsentiveInfo.ItemsSource = null;
                    }
                }
            }
            catch (Exception ex)
            {
                pgrsBar.Value = 0;
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void LoadActiveusers()
        {
            try
            {
                BackUPDB _clsBackUp = new BackUPDB(); 
                System.Data.DataSet ds = _clsBackUp.GetIncentiveAvailableUser( );
                System.Data.DataTable table = ds.Tables[0];
                cmbSearchUser.ClearValue(ItemsControl.ItemsSourceProperty);
                if (table.Rows.Count > 0)
                {
                    cmbSearchUser.ItemsSource = table.DefaultView;
                    cmbSearchUser.DisplayMemberPath = "CLS_USER_ID";
                    cmbSearchUser.SelectedValuePath = "CLS_USER_ID";
                    cmbSearchUser.SelectedValue = StaticProperty.LoginUserID;
                    if (cmbSearchUser.SelectedValue == null)
                    {
                        cmbSearchUser.SelectedIndex = 0;
                    }
                } 
            }
            catch (Exception ex)
            {
                pgrsBar.Value = 0;
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void cmbSearchUser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                pgrsBar.Value = 0;
                lblInfo.Content = string.Empty;
                lblIncentiveAmt.Content = string.Empty;
                LoadInfo();
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
