using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using XtreamDataAccess;

namespace CMSXtream.Pages.DataEntry
{
    /// <summary>
    /// Interaction logic for AddLable.xaml
    /// </summary>
    public partial class AddLable : UserControl
    {
        public Int32 STD_ID { get; set; }
        public AddLable()
        {
            InitializeComponent();
            STD_ID = -1;
            foreach (DataGridColumn col in grdReason.Columns)
            {
                if (col.Header.ToString() == "CK")
                {
                    col.Visibility = Visibility.Hidden;
                }
            }

            grdReason.Height = 600;
            BindReasonGrid();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtReason.Text.Trim() != string.Empty)
                {
                    LableDA _clsReason = new LableDA();
                    _clsReason.LBL_DES = txtReason.Text.Trim();
                    _clsReason.LBL_ID = Int32.Parse(lblId.Content.ToString());
                    if (_clsReason.SaveLable() > 0)
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
        
        public void BindReasonGridFromForm()
        {
            foreach (DataGridColumn col in grdReason.Columns)
            {
                if (col.Header.ToString() == "CK")
                {
                    col.Visibility = Visibility.Visible;
                }
            }
            grdReason.Height = 300;
            BindReasonGrid();
        }

        public void BindReasonGrid()
        {
            try
            {
                LableDA _clsReason = new LableDA();
                System.Data.DataTable table = _clsReason.SelectStudentLableAll(STD_ID).Tables[0];
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


        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            try
            {

                LableDA _clsStudent = new LableDA();
                var selectedRow = grdReason.SelectedItem as System.Data.DataRowView;
                if (selectedRow != null)
                {
                    _clsStudent.LBL_ID = int.Parse(selectedRow["LBL_ID"].ToString());
                    _clsStudent.UpdateLable(STD_ID, 1);
                }

            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {

                LableDA _clsStudent = new LableDA();
                var selectedRow = grdReason.SelectedItem as System.Data.DataRowView;
                if (selectedRow != null)
                {
                    _clsStudent.LBL_ID = int.Parse(selectedRow["LBL_ID"].ToString());
                    _clsStudent.UpdateLable(STD_ID, 0);
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
                Int32 resonId = Int32.Parse(selectedRow["LBL_ID"].ToString());
                lblId.Content = resonId.ToString();
                txtReason.Text = selectedRow["LBL_DES"].ToString();
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (StaticProperty.LoginisAdmin != "1")
            {
                MessageBox.Show("Sorry, you are not authorized to delete label, please contact administrator,", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.No);
                return;
            }

            MessageBoxResult result = MessageBox.Show("Do you want to delete this lable?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                var selectedRow = grdReason.SelectedItem as System.Data.DataRowView;
                if (selectedRow != null)
                {
                    LableDA _clsReason = new LableDA();
                    Int32 resonId = Int32.Parse(selectedRow["LBL_ID"].ToString());
                    _clsReason.LBL_ID = resonId;
                    _clsReason.DeleteLable();
                    BindReasonGrid();
                }
            }

        }

        private void txtReason_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchSting = txtReason.Text.Trim();
            if (searchSting != string.Empty)
            {
                Helper.searchGridByKey(grdReason, "LBL_DES", searchSting);
            }
        }
    }
}
