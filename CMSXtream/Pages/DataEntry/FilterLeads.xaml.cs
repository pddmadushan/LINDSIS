using Microsoft.Office.Interop.Excel;
using System;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using XtreamDataAccess;

namespace CMSXtream.Pages.DataEntry
    {
    /// <summary>
    /// Interaction logic for StudentFiltersAdvance.xaml
    /// </summary>
    public partial class FilterLeads : UserControl
    {
        Boolean isCheckAllGroup = false;
        System.Data.DataTable TblLoadLead;

        public FilterLeads()
        {
            InitializeComponent();
            BindCampaignGrid();
            BindLabelGrid();
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
                System.Data.DataTable table = _campaigns.GetCampaigsToChkBox().Tables[0];
                if (table.Rows.Count > 0)
                {
                    grdCampaign.ItemsSource = table.DefaultView;
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
        void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
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

        private void chkSMSMarkAll_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                FristCellCheckUncheck(grdLeadList, true);
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }
        private void chkSMSMarkAll_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                FristCellCheckUncheck(grdLeadList, false);
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }
        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                BindCampaignGrid();
                BindLabelGrid();
                if ((TblLoadLead != null))
                {
                    TblLoadLead.Clear();
                }
                lblCount.Content = null;
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

        private void lableTextSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchSting = lableTextSearch.Text.Trim();
            if (searchSting != string.Empty)
            {
                Helper.searchGridByKey(grdLable, "LBL_DES", searchSting);
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

        private void UpdateLblNotInCount()
        {
            try
            {
                if (!isCheckAllGroup)
                {
                    int rowCount = grdLable.Items.Count;
                    int selectedCount = 0;
                    string selectedList = "Selected Label List Not In:";
                    if (rowCount > 0)
                    {
                        for (int i = 0; i < rowCount; i++)
                        {
                            var selectedRow = (System.Data.DataRowView)grdLable.Items[i];
                            string displayName = selectedRow["LBL_DES"].ToString();
                            DataGridCell cell = GetCell(i, 6, grdLable);
                            System.Windows.Controls.CheckBox chk = GetVisualChild<System.Windows.Controls.CheckBox>(cell);
                            if (chk.IsChecked.Value)
                            {
                                selectedCount = selectedCount + 1;
                                selectedList = selectedList + "\n" + displayName;
                            }
                        }
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

        private void chkCmpMarkAll_Checked(object sender, RoutedEventArgs e)
        {
            UpdateCampaingCount();
        }
        private void chkCmpMarkAll_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateCampaingCount();
        }

        private void chkLblMark_Checked(object sender, RoutedEventArgs e)
        {
            UpdateLblInCount();
        }

        private void chkLblMark_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateLblInCount();
        }

        /// <summary>
        /// excell upload
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportCsv_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (grdLeadList.Items.Count > 0)
                {
                    int rowCount = grdLeadList.Items.Count;
                    if (rowCount > 0)
                    {
                        System.Data.DataTable ExportData = new System.Data.DataTable();
                        ExportData.Columns.Add("STD_TELEPHONE");
                        for (int i = 0; i < rowCount; i++)
                        {
                            var selectedRow = (System.Data.DataRowView)grdLeadList.Items[i];
                            string _mobileNo = selectedRow["STD_TELEPHONE"].ToString();
                            DataGridCell cell = GetCell(i, 0, grdLeadList);
                            System.Windows.Controls.CheckBox chk = GetVisualChild<System.Windows.Controls.CheckBox>(cell);
                            if (chk.IsChecked.Value)
                            {
                                ExportData.Rows.Add(_mobileNo);
                            }
                        }
                        if (ExportData.Rows.Count > 0)
                        {
                            DataSet _mobileNumbers = new DataSet();
                            _mobileNumbers.Tables.Add(ExportData);

                            string filePath, savingNameAndPath;
                            SelectSavePath(out savingNameAndPath, out filePath);

                            if (!(Directory.Exists(filePath)))
                            {
                                MessageBox.Show("Plaese select a folder", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                            }
                            else
                            {
                                ExportToExcel(_mobileNumbers, savingNameAndPath);
                            }
                        }
                        else
                        {
                            MessageBox.Show("No any Data selected");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("No any Data to Export");
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }
        public void ExportToExcel(DataSet dataSet, string outputPath)
        {
            try
            {
                // Create the Excel Application object
                //ApplicationClass excelApp = new ApplicationClass();
                Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
                // Create a new Excel Workbook
                Workbook excelWorkbook = xlApp.Workbooks.Add(Type.Missing);

                int sheetIndex = 0;
                int col = 0;
                int row = 0;
                Worksheet excelSheet = default(Worksheet);

                // Copy each DataTable as a new Sheet
                int totalCount = dataSet.Tables.Count;

                foreach (System.Data.DataTable dt in dataSet.Tables)
                {
                    sheetIndex += 1;
                    // Copy the DataTable to an object array
                    object[,] rawData = new object[dt.Rows.Count + 1, dt.Columns.Count];

                    // Copy the values to the object array
                    for (col = 0; col <= dt.Columns.Count - 1; col++)
                    {
                        for (row = 0; row <= dt.Rows.Count - 1; row++)
                        {
                            rawData[row, col] = dt.Rows[row].ItemArray[col];
                        }
                    }

                    // Calculate the final column letter
                    string finalColLetter = string.Empty;
                    string colCharset = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                    int colCharsetLen = colCharset.Length;

                    if (dt.Columns.Count > colCharsetLen)
                    {
                        finalColLetter = colCharset.Substring((dt.Columns.Count - 1) / colCharsetLen - 1, 1);
                    }

                    finalColLetter += colCharset.Substring((dt.Columns.Count - 1) % colCharsetLen, 1);

                    // Create a new Sheet
                    excelSheet = (Worksheet)excelWorkbook.Sheets.Add(excelWorkbook.Sheets[sheetIndex], Type.Missing, 1, XlSheetType.xlWorksheet);

                    excelSheet.Name = dt.TableName;

                    // Fast data export to Excel
                    string excelRange = string.Format("A1:{0}{1}", finalColLetter, dt.Rows.Count + 1);
                    excelSheet.Range[excelRange, Type.Missing].Value2 = rawData;

                    // Mark the first row as BOLD
                    // ((Range)excelSheet.Rows[1, Type.Missing]).Font.Bold = true;

                    excelSheet = null;
                }
                // Check if the output file already exists, and delete it
                if (File.Exists(outputPath))
                {
                    File.Delete(outputPath);
                }
                // Save and Close the Workbook
                excelWorkbook.SaveAs(outputPath, XlFileFormat.xlCSV,
                    Type.Missing, Type.Missing, Type.Missing, Type.Missing, XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing, Type.Missing);

                excelWorkbook.Close(true, Type.Missing, Type.Missing);

                excelWorkbook = null;

                // Release the Application object
                xlApp.Quit();
                xlApp = null;

                // Collect the unreferenced objects
                GC.Collect();
                GC.WaitForPendingFinalizers();
                MessageBox.Show("Export Complete");
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }
        public void SelectSavePath(out string fileNameAndPath, out string filePath)
        {
            fileNameAndPath = null;
            filePath = null;
            try
            {
                // Step 1: Show SaveFileDialog to enter a file name within the selected folder
                System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();

                string _userName = StaticProperty.LoginUserID.ToString();
                saveFileDialog.FileName = _userName + "-" + DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss");
                saveFileDialog.Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*";
                saveFileDialog.DefaultExt = "csv";

                // Show 2 the SaveFileDialog
                System.Windows.Forms.DialogResult fileResult = saveFileDialog.ShowDialog();

                if (fileResult == System.Windows.Forms.DialogResult.OK)
                {
                    // Step 3: Get the selected file path
                    fileNameAndPath = saveFileDialog.FileName;
                    filePath = Path.GetDirectoryName(fileNameAndPath);
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }
        //////////search button
        ///

        private void btnSearchStudent_Click(object sender, RoutedEventArgs e)
        {
            if (TblLoadLead != null)
            {
                TblLoadLead.Clear();
            }
            int rowCount = 0;
            lblCount.Content = "";
            try
            {
                System.Data.DataTable dtCampaign = new System.Data.DataTable();
                dtCampaign.Columns.Add("CMP_ID", typeof(Int32));
                rowCount = grdCampaign.Items.Count;
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
                            dtCampaign.Rows.Add(cmpId);
                        }
                    }
                }

                System.Data.DataTable dtLable = new System.Data.DataTable();
                dtLable.Columns.Add("LBL_ID", typeof(Int32));
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

                if (dtCampaign.Rows.Count == 0 && dtLable.Rows.Count == 0)
                {
                    MessageBoxResult result = MessageBox.Show("No filtration found. Do you want to load all active student?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.No)
                    {
                        return;
                    }
                }

                lblCount.Content = "Campaign : " + dtCampaign.Rows.Count.ToString() + " , Lable in : " + dtLable.Rows.Count.ToString();

                FilterLeadDA _filterStd = new FilterLeadDA();
                TblLoadLead = _filterStd.LeadFilter(dtCampaign, dtLable);

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
                MessageBox.Show("" + ex);
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }
    }
}

