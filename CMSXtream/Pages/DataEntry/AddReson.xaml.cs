using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using XtreamDataAccess;

namespace CMSXtream.Pages.DataEntry
{
    /// <summary>
    /// Interaction logic for AddReson.xaml
    /// </summary>
    public partial class AddReson : UserControl
    {
        public AddReson()
        {
            InitializeComponent();
            BindReasonGrid();
            grdReason.Height = 600;
        }
        public void gridResize()
        {
            grdReason.Height = 300;
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtReason.Text.Trim() != string.Empty)
                {
                    ResonDA _clsReason = new ResonDA();
                    _clsReason.RSN_DES = txtReason.Text.Trim();
                    _clsReason.RSN_ID = Int32.Parse(lblId.Content.ToString());
                    if (_clsReason.SaveReason() > 0)
                    {
                        BindReasonGrid();
                        lblId.Content = "-1";
                        txtReason.Text = "";
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

        private void BindReasonGrid()
        {
            try
            {
                ResonDA _clsReason = new ResonDA();
                System.Data.DataTable table = _clsReason.SelectReason().Tables[0];
                if (table.Rows.Count > 0)
                {
                    grdReason.ItemsSource = table.DefaultView;
                }
                else
                {
                    grdReason.ItemsSource = null;
                }           

            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void grdReason_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedRow = grdReason.SelectedItem as System.Data.DataRowView;
            if (selectedRow != null)
            {
                Int32 resonId = Int32.Parse(selectedRow["RSN_ID"].ToString());
                lblId.Content = resonId.ToString();
                txtReason.Text = selectedRow["RSN_DES"].ToString();
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (StaticProperty.LoginisAdmin != "1")
            {
                MessageBox.Show("Sorry, you are not authorized to delete reason, please contact administrator,", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.No);
                return;
            }
            MessageBoxResult result = MessageBox.Show("Do you want to delete this reason?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                var selectedRow = grdReason.SelectedItem as System.Data.DataRowView;
                if (selectedRow != null)
                {
                    ResonDA _clsReason = new ResonDA();
                    Int32 resonId = Int32.Parse(selectedRow["RSN_ID"].ToString());
                    _clsReason.RSN_ID = resonId; 
                    _clsReason.DeleteReason();
                    BindReasonGrid();
                }
            }
        }

        private void txtReason_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchSting = txtReason.Text.Trim();
            if (searchSting != string.Empty)
            {
                Helper.searchGridByKey(grdReason, "RSN_DES", searchSting);
            }
        }

    }
}
