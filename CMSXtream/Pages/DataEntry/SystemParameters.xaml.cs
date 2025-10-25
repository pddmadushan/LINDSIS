using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using XtreamDataAccess;

namespace CMSXtream.Pages.DataEntry
{
    /// <summary>
    /// Interaction logic for SystemParameters.xaml
    /// </summary>
    public partial class SystemParameters : UserControl
    {
        DataTable tblParameters;
        public SystemParameters()
        {
            InitializeComponent();
            LoadParameters();
        }
        private void grdParameters_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            {
                e.Row.Header = (e.Row.GetIndex() + 1).ToString();
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

        private int GetColumnIndex(string XName)
        {
            try
            {
                var column = grdParameters.FindName(XName) as DataGridTemplateColumn;
                if (column != null)
                {
                    int columnIndex = grdParameters.Columns.IndexOf(column);
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

        private void LoadParameters()
        {
            try
            {
                tblParameters = new DataTable();
                SystemParametersDA loadPara = new SystemParametersDA();
                tblParameters = loadPara.GetParametersToGrid().Tables[0];
                tblParameters.Columns.Add("IsRdOnly", typeof(Boolean));

                foreach (DataRow row in tblParameters.Rows)
                {
                    row["IsRdOnly"] = true;
                }

                grdParameters.ItemsSource = tblParameters.DefaultView;
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private TextBox ValueTextBox(string txbxColName,Boolean status)
        {
            var selectedRow = grdParameters.SelectedItem as System.Data.DataRowView;
            int row_id = grdParameters.SelectedIndex;
            int _txtColIndex = GetColumnIndex(txbxColName);
             DataGridCell cell = GetCell(row_id, _txtColIndex, grdParameters);
            TextBox txt = GetVisualChild<TextBox>(cell);

            DataRow row = selectedRow.Row;
            row["IsRdOnly"] = status;
            txt.IsReadOnly = status;

            return txt;
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ValueTextBox("txtbxParaString", false);
                //ValueTextBox("txtbxParaStringUnicode", false);
                grdParameters.ItemsSource = tblParameters.DefaultView;
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }
        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedRow = grdParameters.SelectedItem as System.Data.DataRowView;
                DataRow row = selectedRow.Row;
                string _txtbx1Defaulvalue = row["PARA_STRING"].ToString();
                string _txtbx2Defaulvalue = row["PARA_STRING_UNICODE"].ToString();

                ValueTextBox("txtbxParaString", true).Text = _txtbx1Defaulvalue;
                //ValueTextBox("txtbxParaStringUnicode", true).Text = _txtbx2Defaulvalue;
                grdParameters.ItemsSource = tblParameters.DefaultView;
            }
            catch(Exception ex) 
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
                var selectedRow = grdParameters.SelectedItem as System.Data.DataRowView;
                DataRow row = selectedRow.Row;
                string _paraId = row["PARA_ID"].ToString();
                string _paraString = ValueTextBox("txtbxParaString", true).Text;
                if (_paraString == string.Empty)
                {
                    _paraString = null;
                }
                //string _paraStringUnicode = ValueTextBox("txtbxParaStringUnicode", true).Text;
                //if (_paraStringUnicode == string.Empty)
                //{
                string _paraStringUnicode = null;
                //}
                try
                {
                    SystemParametersDA savePara = new SystemParametersDA();
                    savePara.UpadateParaValues(_paraId, _paraString, _paraStringUnicode);
                    LoadParameters();
                }
                catch(Exception ex)
                {

                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var originalSource = e.OriginalSource as UIElement;
                if (originalSource is TextBox textBox && !textBox.AcceptsReturn)
                {
                    textBox.AcceptsReturn = true;
                }
            }
        }

        private void btnRecalculate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataView dtView = new DataView();
                dtView = grdParameters.ItemsSource as DataView;
                if (dtView != null)
                {
                    dtView.RowFilter = "PARA_ID = 'CUR_YEAR'";
                    string year = dtView.ToTable().Rows[0]["PARA_STRING"].ToString();
                    dtView.RowFilter = null;
                    dtView.RowFilter = "PARA_ID = 'CUR_MONTH'";
                    string Month = dtView.ToTable().Rows[0]["PARA_STRING"].ToString();
                    dtView.RowFilter = null;

                    MessageBoxResult result = MessageBox.Show("Do you want to finalize incentive for " + Month + "/" + year + "?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        SystemParametersDA savePara = new SystemParametersDA();
                        savePara.FinalizeIncentive();
                        MessageBox.Show("Incentive has been finalized!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
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
