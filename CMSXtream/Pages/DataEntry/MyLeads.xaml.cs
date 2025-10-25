using CMSXtream.Handlers;
using CMSXtream.Pages.View;
using Microsoft.Office.Interop.Excel;
using System;
using System.Data;
using System.Data.SqlTypes;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

using XtreamDataAccess;


namespace CMSXtream.Pages.DataEntry
{
    /// <summary>
    /// Interaction logic for MyLeads.xaml
    /// </summary>
    public partial class MyLeads : UserControl
    {

        System.Data.DataTable TblMyLeads;
        System.Data.DataTable TblCampaigns;
        System.Data.DataTable TblLables;
        System.Data.DataTable TblUsers;

        int UserId;
        public int ChkBoxMode { get; set; }//All records =2

        DateTime FromDate;
        DateTime ToDate;
        Boolean Filter;
        Boolean FirtPopup = true;
        MessageBoxResult result;
        ComboBox cmbCls;
        public static Boolean Reload { get; set; }
        public static string SavedLableList { get; set; }
        public static string tobeInfoText { get; set; }
        public static Boolean IsLeadTransfered;
        public static int ToUserId = -1;
        public static Boolean TotalClick = false;

        public Boolean callFromTransfer = false;

        public MyLeads()
        {
            InitializeComponent();
            CheckUserType();
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
            try
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
            catch (Exception ex)
            {
                return null;
            }
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

        private void btnAddStudent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show("Do you want to add this Lead as Student ?", "Confirmation"/*box tttle*/, MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    var selectedRow = grdMyCmpStd.SelectedItem as System.Data.DataRowView;
                    if (selectedRow != null)
                    {
                        String _stdConvertedUserId = StaticProperty.LoginUserID;
                        int cmpStdId = int.Parse(selectedRow["CMP_STD_ID"].ToString());
                        MyLeadsDA _addMyLeadAsStd = new MyLeadsDA();
                        Int32 STD = _addMyLeadAsStd.AddMyLeadAsStudent(cmpStdId, _stdConvertedUserId);

                        if (STD > 0)
                        {
                            DataRow row = selectedRow.Row;//REFRESH
                            row["IS_CLS_STD"] = 1;
                            row["ISNOT_CLS_STD"] = 0;
                            row["STD_ID"] = STD.ToString();
                            row["INCOMPLETED_STD_DATA"] = 1;
                            row["PENDING_DAYS"] = 0;
                            grdMyCmpStd.ItemsSource = TblMyLeads.DefaultView;
                        }
                        else
                        {
                            MessageBox.Show("Cannot add this Lead as Student because the telephone number already exists with an \b\b Active\b\b or \b\bTemporarily Inactive student\b\b. \n Mobile number is associated with Student ID : " + STD, StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
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

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var unSavedComment = TblMyLeads.AsEnumerable().Count(a => a.Field<int>("EDIT_COMMENT") == 0 && TextBoxString(a) != null && TextBoxString(a) != string.Empty);
                if (unSavedComment > 0)
                {
                    result = MessageBox.Show("Unsaved Comment are found! are you sure to Continue?", "Confirmation"/*box tttle*/, MessageBoxButton.YesNo, MessageBoxImage.Question);
                }
                else
                {
                    result = MessageBoxResult.Yes;//popup edit window, if no any unsaved comments
                }
                if (result == MessageBoxResult.Yes)
                {

                    CMSXtream.Pages.DataEntry.Student form = new CMSXtream.Pages.DataEntry.Student();
                    PopupHelper dialog = new PopupHelper
                    {
                        Title = "Edit Student",
                        Content = form,
                        ResizeMode = ResizeMode.NoResize,
                        Width = 1000
                    };
                    StudentAttribute stAttPass = new StudentAttribute();
                    var selectedRow = grdMyCmpStd.SelectedItem as System.Data.DataRowView;
                    if (selectedRow != null)
                    {
                        stAttPass.STD_ID = int.Parse(selectedRow["STD_ID"].ToString());
                        stAttPass.CLS_ID = int.Parse(selectedRow["CLS_ID"].ToString());
                        stAttPass.CLS_NAME = selectedRow["CLS_NAME"].ToString();
                        stAttPass.STD_INITIALS = selectedRow["STD_INITIALS"].ToString();
                        stAttPass.STD_SURNAME = selectedRow["STD_SURNAME"].ToString();
                        stAttPass.STD_FULL_NAME = selectedRow["STD_FULL_NAME"].ToString();
                        stAttPass.STD_GENDER = Int32.Parse(selectedRow["STD_GENDER"].ToString());
                        stAttPass.STD_DATEOFBIRTH = DateTime.Parse(selectedRow["STD_DATEOFBIRTH"].ToString());
                        stAttPass.STD_JOIN_DATE = DateTime.Parse(selectedRow["STD_JOIN_DATE"].ToString());
                        stAttPass.STD_EMAIL = selectedRow["STD_EMAIL"].ToString();
                        stAttPass.STD_NIC = selectedRow["STD_NIC"].ToString();
                        stAttPass.STD_ADDRESS = selectedRow["STD_ADDRESS"].ToString();
                        stAttPass.STD_CLASS_FEE = double.Parse(selectedRow["STD_CLASS_FEE"].ToString());
                        stAttPass.STD_TELEPHONE = selectedRow["STD_TELEPHONE"].ToString();
                        stAttPass.STD_ACTIVE_FLG = selectedRow["STD_ACTIVE_FLG"].ToString() == "" ? 2 : int.Parse(selectedRow["STD_ACTIVE_FLG"].ToString());
                        stAttPass.STD_COMMENT = selectedRow["STD_COMMENT"].ToString();
                        stAttPass.STD_TEMP_NOTE = selectedRow["STD_TEMP_NOTE"].ToString();
                        stAttPass.RSN_ID = Int32.Parse(selectedRow["RSN_ID"].ToString());

                    }
                    form.IsAddNew = false;
                    form.stAtt = stAttPass;
                    form.LoadFormContaint();
                    dialog.ShowDialog();
                    string ReturnMessage = form.OutResult;
                    if (ReturnMessage != string.Empty && ReturnMessage != null)
                    {

                        LoadMyLeads(UserId, TblCampaigns, TblLables, FromDate, ToDate, Filter, ChkBoxMode);//refresh grid
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

        public void LoadMyLeads(int ClsUserId, System.Data.DataTable tblCampaign, System.Data.DataTable tblLables, DateTime fromDate, DateTime toDate, Boolean filter, int chkBoxMode,int viewDeleteRecAdminPg = 0)
        {
            try
            {

                if (chkBoxMode == 5)
                {
                    callFromTransfer = true;
                }

                if (FirtPopup)
                {
                    UserId = ClsUserId;
                    TblCampaigns = tblCampaign;
                    TblLables = tblLables;
                    FirtPopup = false;
                    FromDate = fromDate;
                    ToDate = toDate;
                    Filter = filter;
                    ChkBoxMode = chkBoxMode;
                }

                MyLeadsDA _myLeadsDA = new MyLeadsDA();
                DataSet _dsLoadTable = new DataSet();
                _dsLoadTable = _myLeadsDA.GetMyLeads(ClsUserId, tblCampaign, tblLables, fromDate, toDate, filter, chkBoxMode, viewDeleteRecAdminPg);
                TblMyLeads = _dsLoadTable.Tables[0];
                grdMyCmpStd.ItemsSource = TblMyLeads.DefaultView;

                UpdateLabledStdcount();
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void btnComments_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedRow = grdMyCmpStd.SelectedItem as System.Data.DataRowView;
                DataRow row = selectedRow.Row;
                row["EDIT_COMMENT"] = 0;
                grdMyCmpStd.ItemsSource = TblMyLeads.DefaultView;

                int row_id = grdMyCmpStd.SelectedIndex;//MAKE TEXT EDITABLE
                int _columnIndex = GetColumnIndex("userCommentCol");
                DataGridCell cell = GetCell(row_id, _columnIndex, grdMyCmpStd);
                System.Windows.Controls.TextBox txt = GetVisualChild<System.Windows.Controls.TextBox>(cell);
                txt.IsReadOnly = false;
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void btnCommentsSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedRow = grdMyCmpStd.SelectedItem as System.Data.DataRowView;
                int cmpStdId = int.Parse(selectedRow["CMP_STD_ID"].ToString());
                int _columnIndex = GetColumnIndex("userCommentCol");
                int row_id = grdMyCmpStd.SelectedIndex;//get text box text
                DataGridCell cell = GetCell(row_id, _columnIndex, grdMyCmpStd);
                System.Windows.Controls.TextBox txt = GetVisualChild<System.Windows.Controls.TextBox>(cell);
                string comment = txt.Text.ToString();
                txt.IsReadOnly = true;

                DataRow row = selectedRow.Row;
                string _loggedUser = StaticProperty.LoginUserID;
                MyLeadsDA _saveComment = new MyLeadsDA();//save comment

                _saveComment.SaveUserComment(cmpStdId, comment);

                //refresh
                row["EDIT_COMMENT"] = 1;
                row["USER_COMMENT"] = comment;
                grdMyCmpStd.ItemsSource = TblMyLeads.DefaultView;
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void btnAddLable_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                var selectedRow = grdMyCmpStd.SelectedItem as System.Data.DataRowView;
                if (selectedRow != null)
                {
                    CMSXtream.Pages.DataEntry.AddLableToCmpStudent form = new CMSXtream.Pages.DataEntry.AddLableToCmpStudent(callFromTransfer);
                    int _cmpStdId = int.Parse(selectedRow["CMP_STD_ID"].ToString());
                    form.CmpStdId = _cmpStdId;
                    form.CmpStdPhoneNumber = selectedRow["STD_TELEPHONE"].ToString();
                    form.UserId = UserId;
                    form.initComment = selectedRow["USER_COMMENT"].ToString();
                    form.LoadLables();
                    form.GetClassesToCombo();
                    PopupHelper dialog = new PopupHelper
                    {
                        Title = PhoneNumberFormatter.Format(selectedRow["STD_TELEPHONE"].ToString()) + " - " + selectedRow["STD_FULL_NAME"].ToString(),
                        Content = form,
                        ResizeMode = ResizeMode.NoResize,
                        Width = form.Width,
                        Height = form.Height,
                    };                    
                    Reload = false;
                    form.UserChange = false;                    
                    Mouse.OverrideCursor = null;
                    dialog.ShowDialog();

                    if (Reload)//refresh changes
                    {
                        Mouse.OverrideCursor = Cursors.Wait;
                        LoadMyLeads(UserId, TblCampaigns, TblLables, FromDate, ToDate, Filter, ChkBoxMode);//refresh grid
                        /* 
                        DataRow row = selectedRow.Row;
                        row["CMP_STD_LBL"] = SavedLableList;
                        row["TOBE_INFO"] = tobeInfoText;
                        row["PRORITY_VALUE"] = AddLableToCmpStudent.ProrityValue;

                        if (form.UserChange == true || (ChkBoxMode ==0 && AddLableToCmpStudent.NoMoreAtt==1))
                        {
                            row.Delete();
                            grdMyCmpStd.ItemsSource = null;
                            grdMyCmpStd.ItemsSource = TblMyLeads.DefaultView;
                        }
                        */                     
                    }
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string searchSting = txtSearch.Text.Trim();
                if (searchSting != string.Empty)
                {
                    if (int.TryParse(searchSting, out int intValue))
                    {
                        Helper.searchGridByKey(grdMyCmpStd, "STD_TELEPHONE", searchSting);
                    }
                    else
                    {
                        Helper.searchGridByKey(grdMyCmpStd, "STD_FULL_NAME", searchSting);
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

        //private void grdMyCmpStd_Unloaded(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        System.Data.DataTable tblUnSavedCommnt = new System.Data.DataTable();
        //        tblUnSavedCommnt.Columns.Add("CMP_STD_ID", typeof(int));
        //        tblUnSavedCommnt.Columns.Add("USER_COMMENT", typeof(string));

        //        foreach (DataRow row in TblMyLeads.AsEnumerable())
        //        {
        //            if (row.Field<int>("EDIT_COMMENT") == 0 && (TextBoxString(row) != null && TextBoxString(row) != string.Empty))
        //            {
        //                DataRow newRow = tblUnSavedCommnt.NewRow();
        //                newRow["CMP_STD_ID"] = Convert.ToInt32(row["CMP_STD_ID"]);
        //                newRow["USER_COMMENT"] = TextBoxString(row);
        //                tblUnSavedCommnt.Rows.Add(newRow);
        //            }
        //        }
        //        if (tblUnSavedCommnt.Rows.Count > 0)
        //        {
        //            MessageBoxResult result = MessageBox.Show("Unsaved Comment fields are found! Do you want to Save ?", "Confirmation"/*box tttle*/, MessageBoxButton.YesNo, MessageBoxImage.Question);
        //            if (result == MessageBoxResult.Yes)
        //            {
        //                string _loggedUser = StaticProperty.LoginUserID;
        //                MyLeadsDA _saveComment = new MyLeadsDA();
        //                _saveComment.SaveUNsavedComment(tblUnSavedCommnt, _loggedUser);
        //            }
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        LogFile logger = new LogFile();
        //        logger.MyLogFile(ex);
        //        MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
        //    }
        //}
        private string TextBoxString(DataRow a)
        {
            try
            {
                int rowIndex = a.Table.Rows.IndexOf(a);
                int columnIndex = GetColumnIndex("userCommentCol");
                DataGridCell cell = GetCell(rowIndex, columnIndex, grdMyCmpStd);
                string comment = "";
                if (cell != null)
                {
                    System.Windows.Controls.TextBox txt = GetVisualChild<System.Windows.Controls.TextBox>(cell);
                    comment = txt.Text.ToString().Trim();
                }
                return comment;
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);

                return string.Empty;
            }
        }
        void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)//grid row num
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();

            var selectedRow = (System.Data.DataRowView)e.Row.Item;
            string numberSq = selectedRow["MOBILE_NO_DUP_SEQ"].ToString();

            if (numberSq != null)
            { 
                if (numberSq!="0")
                {
                    //int _columnIndex = GetColumnIndex("clmHistory");
                    //int row_id = grdMyCmpStd.SelectedIndex;
                    //DataGridCell cell = GetCell(row_id, _columnIndex, grdMyCmpStd);
                    //System.Windows.Controls.Button btntransfer = GetVisualChild<System.Windows.Controls.Button>(cell);
                    //btntransfer.Visibility = Visibility.Hidden;
                }               

            }
        }

        private void ExportCsv_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (grdMyCmpStd.Items.Count > 0)
                {
                    int rowCount = grdMyCmpStd.Items.Count;
                    if (rowCount > 0)
                    {
                        System.Data.DataTable ExportData = new System.Data.DataTable();
                        ExportData.Columns.Add("STD_TELEPHONE");
                        for (int i = 0; i < rowCount; i++)
                        {
                            var selectedRow = (System.Data.DataRowView)grdMyCmpStd.Items[i];
                            string _mobileNo = selectedRow["STD_TELEPHONE"].ToString();
                            ExportData.Rows.Add(_mobileNo);
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

        /*
        private void cmbClass_Loaded(object sender, RoutedEventArgs e)
        {
            
            
            int _rowCount=grdMyCmpStd.Items.Count;

            for (int i = 0; i < _rowCount; i++)
            {

                DataGridCell cell = GetCell(i, 6, grdMyCmpStd);
                if (cell != null)
                {
                    ComboBox txt = GetVisualChild<ComboBox>(cell);
                    txt.ItemsSource = TblClassList.DefaultView;
                    txt.DisplayMemberPath = "CLS_NAME";
                    txt.SelectedValuePath = "CLS_ID";

                    txt.SelectedValue = 2;
                }
            }
    }

    private void cmbClass_DropDownClosed(object sender, EventArgs e)
    {
        try
        {
            cmbCls.ItemsSource = null;
        }
        catch (Exception ex)
        {
            LogFile logger = new LogFile();
            logger.MyLogFile(ex);
            MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
        }
    }
    */
        private void btnDeleteStudent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedRow = grdMyCmpStd.SelectedItem as System.Data.DataRowView;
                DataRow row = selectedRow.Row;
                if (row != null)
                {
                    int _delCmpStdId = int.Parse(row["CMP_STD_ID"].ToString());
                    MessageBoxResult result = MessageBox.Show("Do you want to delete selected students ?", "Confirmation"/*box tttle*/, MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        MyLeadsDA _delcmpstd = new MyLeadsDA();
                        _delcmpstd.DeleteLead(_delCmpStdId);
                        if (ChkBoxMode != 2)
                        {
                            TblMyLeads.Rows.Remove(row);
                            UpdateLabledStdcount();
                        }
                        else//prvent rmving of grid rows- All lead mode
                        {
                            row["IS_DELETED"] = 1;
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

        private void UpdateLabledStdcount()
        {
            try
            {
                var query = TblMyLeads.AsEnumerable().Count(a => a.Field<string>("CMP_STD_LBL") != null);

                lblLabledStdCount.Content = query.ToString() + "/" + grdMyCmpStd.Items.Count.ToString();
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void btnRestoreStudent_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedRow = grdMyCmpStd.SelectedItem as System.Data.DataRowView;
                DataRow row = selectedRow.Row;
                if (row != null)
                {
                    int _delCmpStdId = int.Parse(row["CMP_STD_ID"].ToString());
                    MessageBoxResult result = MessageBox.Show("Do you want to Restore selected students ?", "Confirmation"/*box tttle*/, MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        MyLeadsDA _delcmpstd = new MyLeadsDA();
                        _delcmpstd.RestoreDeletedLead(_delCmpStdId);
                        if (ChkBoxMode != 2)
                        {
                            TblMyLeads.Rows.Remove(row);
                            grdMyCmpStd.ItemsSource = null;
                            grdMyCmpStd.ItemsSource = TblMyLeads.DefaultView;
                            UpdateLabledStdcount();
                        }
                        else//prvent rmving of grid rows- All lead mode
                        {
                            row["IS_DELETED"] = 0;
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
        private int GetColumnIndex(string XName)
        {
            try
            {
                var column = grdMyCmpStd.FindName(XName) as DataGridTemplateColumn;
                if (column != null)
                {
                    int columnIndex = grdMyCmpStd.Columns.IndexOf(column);
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

        private void btnTranferLead_Click(object sender, EventArgs e)
        {
            try
            {
                System.Data.DataTable _tblusers;
                string assgnedUser;
                int cmpStdId;
                var selectedLead = grdMyCmpStd.SelectedItem as System.Data.DataRowView;

                if (selectedLead != null)
                {
                    DataRow dr = selectedLead.Row;
                    cmpStdId = int.Parse(dr["CMP_STD_ID"].ToString());
                    assgnedUser = dr["CLS_USER_ID"].ToString();

                    if (TblUsers == null)
                    {
                        MyLeadsDA _loadUsers = new MyLeadsDA();
                        TblUsers = _loadUsers.GetUsers().Tables[0];
                    }
                    _tblusers = TblUsers.Copy();
                    assgnedUser = dr["CLS_USER_ID"].ToString();
                    var selectedUser = _tblusers.AsEnumerable().Where(r => Convert.ToString(r["USER_NAME"]) == assgnedUser);

                    if (selectedUser.Any())
                    {
                        DataRow dataRow = selectedUser.First();

                        if (dataRow != null)
                        {
                            _tblusers.Rows.Remove(dataRow);
                        }
                    }
                    int _columnIndex = GetColumnIndex("clmTransferLead");
                    int row_id = grdMyCmpStd.SelectedIndex;
                    DataGridCell cell = GetCell(row_id, _columnIndex, grdMyCmpStd);
                    System.Windows.Controls.Button btntransfer = GetVisualChild<System.Windows.Controls.Button>(cell);
                    System.Windows.Point point = btntransfer.PointToScreen(new System.Windows.Point(0, 0));
                    CMSXtream.Pages.DataEntry.LeadTranferUserSelect form = new CMSXtream.Pages.DataEntry.LeadTranferUserSelect();
                    PopupHelper dialog = new PopupHelper
                    {
                        Title = "Select User",
                        Content = form,
                        ResizeMode = ResizeMode.NoResize,
                        MaxWidth = 800,
                        MaxHeight = 500,
                        WindowStartupLocation = WindowStartupLocation.CenterScreen,
                      
                    };
                    form.BindUserList(_tblusers, assgnedUser, false, false, true, cmpStdId,null);
                    dialog.ShowDialog();

                    try
                    {
                        if (IsLeadTransfered)
                        {
                            DBSearchDA chngUser = new DBSearchDA();
                            chngUser.CHANGE_LEADS_USER_SelRow(cmpStdId, ToUserId, assgnedUser, StaticProperty.LoginUserID);

                            IsLeadTransfered = false;
                            var assignUser = _tblusers.AsEnumerable().FirstOrDefault(r => Convert.ToInt32(r["ID"]) == ToUserId);
                            if (assignUser != null)
                            {
                                if (!TotalClick)
                                {
                                    dr.Delete();
                                    grdMyCmpStd.ItemsSource = null;
                                    grdMyCmpStd.ItemsSource = TblMyLeads.DefaultView;
                                }
                                else
                                {
                                    dr["CLS_USER_ID"] = assignUser["USER_NAME"].ToString();
                                    dr["PENDING_DAYS"] = 0;
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
            try
            {
                int IsAdmin = int.Parse(StaticProperty.LoginisAdmin);
                if (IsAdmin != 1)
                {
                    grdMyCmpStd.Columns.RemoveAt(GetColumnIndex("clmTransferLead"));//remove lead tranfer button
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void grdMyCmpStd_PreviewMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var selectedRow = grdMyCmpStd.SelectedItem as System.Data.DataRowView;
            DataRow row = selectedRow.Row;
            string mobileNo = row["STD_TELEPHONE"].ToString();
            Clipboard.SetText(mobileNo);
        }

        private void btnAddLead_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MannualAddLeads form = new MannualAddLeads();
                PopupHelper dialog = new PopupHelper
                {
                    Title = "Class Payment",
                    Content = form,
                    ResizeMode = ResizeMode.NoResize,
                    Width = 1000,
                    Height = 500
                };
                dialog.ShowDialog();

                string ReturnMessage = form.OutResult;
                if (ReturnMessage != string.Empty && ReturnMessage != null)
                {
                    LoadMyLeads(UserId, TblCampaigns, TblLables, FromDate, ToDate, Filter, ChkBoxMode);//refresh grid
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void userComment_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedRow = grdMyCmpStd.SelectedItem as System.Data.DataRowView;

                if (selectedRow != null)
                {
                    int cmpStdId = int.Parse(selectedRow["CMP_STD_ID"].ToString());
                    int _columnIndex = GetColumnIndex("userCommentCol");
                    int row_id = grdMyCmpStd.SelectedIndex;//get text box text
                    DataGridCell cell = GetCell(row_id, _columnIndex, grdMyCmpStd);
                    System.Windows.Controls.TextBox txt = GetVisualChild<System.Windows.Controls.TextBox>(cell);
                    string comment = txt.Text.ToString();

                    DataRow row = selectedRow.Row;
                    if (comment.Trim().Length > 0 || row["EDIT_COMMENT"].ToString() != "1")
                    {
                        txt.IsReadOnly = true;
                       
                        string _loggedUser = StaticProperty.LoginUserID;
                        MyLeadsDA _saveComment = new MyLeadsDA();//save comment

                        _saveComment.SaveUserComment(cmpStdId, comment);

                        //refresh
                        if (comment.Trim().Length == 0)
                        {
                            row["EDIT_COMMENT"] = 0;
                        }
                        else
                        {
                            row["EDIT_COMMENT"] = 1;
                        }

                        row["USER_COMMENT"] = comment;
                        grdMyCmpStd.ItemsSource = TblMyLeads.DefaultView;
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

        private void btnHistory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedRow = grdMyCmpStd.SelectedItem as System.Data.DataRowView;

                if (selectedRow != null)
                {
                    int cmpStdId = int.Parse(selectedRow["CMP_STD_ID"].ToString());
                    string _mobileNo = selectedRow["STD_TELEPHONE"].ToString();

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

        private void userComment_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var selectedRow = grdMyCmpStd.SelectedItem as System.Data.DataRowView;
            DataRow row = selectedRow.Row;
            row["EDIT_COMMENT"] = 0;
            grdMyCmpStd.ItemsSource = TblMyLeads.DefaultView;

            int row_id = grdMyCmpStd.SelectedIndex;
            int _columnIndex = GetColumnIndex("userCommentCol");
            DataGridCell cell = GetCell(row_id, _columnIndex, grdMyCmpStd);
            System.Windows.Controls.TextBox txt = GetVisualChild<System.Windows.Controls.TextBox>(cell);
            string comment = txt.Text.ToString();

            String inputKeyStroke = String.Empty;
            if (comment.Contains("@D"))
            {
                int curPos = txt.SelectionStart;
                string replaceDateTime = DateTime.Now.ToString("yy/MM/dd");
                txt.Text = comment.Replace("@D", replaceDateTime);
                txt.CaretIndex = curPos + replaceDateTime.Length;
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Mouse.OverrideCursor = Cursors.Wait;
                LoadMyLeads(UserId, TblCampaigns, TblLables, FromDate, ToDate, Filter, ChkBoxMode);
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
            finally
            {
                //Mouse.OverrideCursor = null;
            }
        }

    }
}


