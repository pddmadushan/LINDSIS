using System;
using System.Data;
using System.Windows;
using XtreamDataAccess;
using CMSXtream.Pages.DataEntry;
using DataTable = System.Data.DataTable;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Office.Interop.Excel;

namespace CMSXtream.Pages.View
{
    /// <summary>
    /// Interaction logic for CampaignDataUploadx.xaml
    /// </summary>
    public partial class CampaignDataUploadx : System.Windows.Controls.UserControl
    {
        private string _selectedFilePath;
        private int _recordCount = 0;
        DataTable TblCmpHistory;
        DataTable table = new DataTable();
        MessageBoxResult ResultFullNameConfirmation;
        public CampaignDataUploadx()
        {
            InitializeComponent();
            LoadDataToCombo_FromSQL();
            CAMP_COMBO.SelectedValue = null;//set default value
        }
        private void LoadDataToCombo_FromSQL()
        {
            try
            {
                CampaingDataUploadDA CampaingTable = new CampaingDataUploadDA();
                DataTable Combo_dataload_table = new DataTable();
                Combo_dataload_table = CampaingTable.SelectAllCampaing().Tables[0];//The .Tables[0] part of the code is used to access the first table in a DataSet object or a result set. It's a common practice when dealing with ADO.NET data access.
                CAMP_COMBO.ItemsSource = Combo_dataload_table.DefaultView;///DefaultView is a property of a DataTable that returns a DataView object. A DataView represents a customized view of a table and allows for filtering, sorting, and other operations on the data.
                CAMP_COMBO.DisplayMemberPath = "CMP_DES";
                CAMP_COMBO.SelectedValuePath = "CMP_ID";

                /////new
                TblCmpHistory= Combo_dataload_table.Copy();
                var query = TblCmpHistory.AsEnumerable().Where(r => r.Field<int?>("CMP_ID") == null);
                if (query.Any())
                {
                    DataRow row = query.First();
                    if(row != null)
                    {
                        row["CMP_ID"] = -1;
                        row["CMP_DES"] = "ALL (Selected Campaigns)";
                    }
                }
                /////new
                if (CAMP_COMBO.SelectedValue == null)
                {
                    CAMP_COMBO.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }
        private void BrowseBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
                openFileDialog.Filter = "All Files|*.*"; // Set the file filter as needed
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                bool? result = openFileDialog.ShowDialog();
                if (result == true)
                {
                    _selectedFilePath = openFileDialog.FileName;

                    path_lbl.Content = _selectedFilePath;
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }
        public DataTable ReadExcel()
        {
            try
            {
                ResultFullNameConfirmation = MessageBoxResult.Yes;
                //Create COM Objects.
                Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
                Microsoft.Office.Interop.Excel.Workbook excelBook = excelApp.Workbooks.Open(_selectedFilePath);
                Microsoft.Office.Interop.Excel._Worksheet excelSheet = excelBook.Sheets[1];
                Microsoft.Office.Interop.Excel.Range excelRange = excelSheet.UsedRange;

                int rows = excelRange.Rows.Count;
                DataTable _tblExlData = new DataTable();

                // Specify the column names  to get data from excell
                string PhoneNo = "phone_number";
                string Full_Name = "full_name";
                string bandscore = "bandscore";
                string exam_type = "exam_type";
                string when_do_you_want = "when_do_you_want";
                string your_english_knowledge = "your_english_knowledge";
                string your_ielts_knowledge = "your_ielts_knowledge";

                //string phnNoColumns = @"phone|phone_number";
                // Regex regex = new Regex(phnNoColumns, RegexOptions.IgnoreCase);

                _tblExlData.Columns.Add(PhoneNo, typeof(string));
                _tblExlData.Columns.Add(Full_Name, typeof(string));
                _tblExlData.Columns.Add(bandscore, typeof(string));
                _tblExlData.Columns.Add(exam_type, typeof(string));
                _tblExlData.Columns.Add(when_do_you_want, typeof(string));
                _tblExlData.Columns.Add(your_english_knowledge, typeof(string));
                _tblExlData.Columns.Add(your_ielts_knowledge, typeof(string));

                // Find the positions of the target columns
                int column_PhoneNo = -1;
                int column_Full_Name = -1;
                int column_bandscore = -1;
                int column_exam_type = -1;
                int column_when_do_you_want = -1;
                int column_your_english_knowledge = -1;
                int column_your_ielts_knowledge = -1;
                for (int col = 1; col <= excelRange.Columns.Count; col++)
                {
                    string header = excelRange.Cells[1, col].Value2 != null ? excelRange.Cells[1, col].Value2.ToString() : "";

                    // if (header.Equals(PhoneNo, StringComparison.OrdinalIgnoreCase))
                    // if (regex.IsMatch(header))
                    if (header.Equals(PhoneNo, StringComparison.OrdinalIgnoreCase) || header.Equals("phone", StringComparison.OrdinalIgnoreCase))
                    {
                        column_PhoneNo = col;
                    }
                    else if (header.Equals(Full_Name, StringComparison.OrdinalIgnoreCase))
                    {
                        column_Full_Name = col;
                    }
                    else if (header.StartsWith(bandscore, StringComparison.OrdinalIgnoreCase))
                    {
                        column_bandscore = col;
                    }
                    else if (header.StartsWith(exam_type, StringComparison.OrdinalIgnoreCase))
                    {
                        column_exam_type = col;
                    }
                    else if (header.StartsWith(when_do_you_want, StringComparison.OrdinalIgnoreCase))
                    {
                        column_when_do_you_want = col;
                    }
                    else if (header.StartsWith(your_english_knowledge, StringComparison.OrdinalIgnoreCase))
                    {
                        column_your_english_knowledge = col;
                    }
                    else if (header.StartsWith(your_ielts_knowledge, StringComparison.OrdinalIgnoreCase))
                    {
                        column_your_ielts_knowledge = col;
                    }

                    // Break out of the loop if both columns are found
                    if (column_PhoneNo != -1 && column_Full_Name != -1)
                    {
                        break;
                    }
                }
                if (column_PhoneNo == -1)
                {
                    MessageBox.Show("Cannot Find 'Phone' or 'Phone_number' Column in Selected File.\nPlease Check File again!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
                    CloseExcelSheet(excelApp, excelBook, excelSheet, excelRange);
                    return null;
                }
                else {
                    if (column_Full_Name == -1)
                    {
                        ResultFullNameConfirmation = MessageBoxResult.No;
                        MessageBoxResult result = System.Windows.MessageBox.Show("Cannot Find 'Full_name' Column in Selected File.\nDo you want to continue without name?", "Confirmation"/*box tttle*/, MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                        {
                            ResultFullNameConfirmation = MessageBoxResult.Yes;
                        }
                        else
                        {
                            CloseExcelSheet(excelApp, excelBook, excelSheet, excelRange);
                        }
                    }
                    if (ResultFullNameConfirmation== MessageBoxResult.Yes) {
                        // Read the entire range of data at once
                        object[,] data = excelRange.Value2 as object[,];

                        // Populate the DataTable with the data from the Excel range
                        for (int row = 2; row <= rows; row++)
                        {
                            //DataRow dataRow = table.NewRow();
                            DataRow dataRow = _tblExlData.NewRow();
                            dataRow[PhoneNo] = data[row, column_PhoneNo];
                            if (column_Full_Name!=-1) {
                                dataRow[Full_Name] = data[row, column_Full_Name]?.ToString().Substring(0, Math.Min(100, data[row, column_Full_Name].ToString().Length));//100 chara. from fullname
                            }

                            if (column_bandscore != -1)
                            {
                                dataRow[bandscore] = data[row, column_bandscore];
                            }
                            if (column_exam_type != -1)
                            {
                                dataRow[exam_type] = data[row, column_exam_type];
                            }
                            if (column_when_do_you_want != -1)
                            {
                                dataRow[when_do_you_want] = data[row, column_when_do_you_want];
                            }
                            if (column_your_english_knowledge != -1)
                            {
                                dataRow[your_english_knowledge] = data[row, column_your_english_knowledge];
                            }
                            if (column_your_ielts_knowledge != -1)
                            {
                                dataRow[your_ielts_knowledge] = data[row, column_your_ielts_knowledge];
                            }

                            _tblExlData.Rows.Add(dataRow);
                        }
                        // Close Excel objects to release resources

                        /*excelBook.Close(false);
                        excelApp.Quit();
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(excelRange);
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(excelSheet);
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(excelBook);
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
                        */
                        CloseExcelSheet(excelApp, excelBook, excelSheet, excelRange);
                        return _tblExlData;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (IndexOutOfRangeException ex) {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("Select a Valid file format!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
                return null;
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
                return null;
            }
        }
        private void excelReadbtn_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedFilePath == null)
            {
                System.Windows.Forms.MessageBox.Show("Please Select a File");
            }
            else
            {
                //ReadExcel();
                GetDataFromTempTbltodatagrid();
                _recordCount = Int32.Parse(grid.Items.Count.ToString());
                lblRecordCount.Content = _recordCount.ToString();
            }
        }
        public void GetDataFromTempTbltodatagrid()
        {
            try
            {
                // CampaingDataUploadDA _cmpdata = new CampaingDataUploadDA();
                // table = _cmpdata.GetCampaingTempData().Tables[0];

                CampaingDataUploadDA _cmpdata = new CampaingDataUploadDA();//new
                _cmpdata.Cmp_id = Convert.ToInt32(CAMP_COMBO.SelectedValue);
                table = _cmpdata.READ_PROCESS_SHOW_EXCEL_DATA(ReadExcel()).Tables[0];//new
                _selectedFilePath = null;
                path_lbl.Content = string.Empty;

                if (table.Rows.Count > 0)
                {
                    grid.ItemsSource = table.DefaultView;
                }
                else
                {
                    grid.ItemsSource = null;
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }
        public void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = System.Windows.MessageBox.Show("Do you want to delete this lable?", "Confirmation"/*box tttle*/, MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var selectedRow = grid.SelectedItem as System.Data.DataRowView;
                    if (selectedRow != null)
                    {
                        DataRow row = selectedRow.Row;
                        table.Rows.Remove(row);
                        grid.ItemsSource = table.DefaultView;
                        _recordCount = _recordCount - 1;
                        lblRecordCount.Content = _recordCount.ToString();
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
        public void btnedit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //CMSXtream.Pages.DataEntry.ExcelDataEdit ExcelEdit = new CMSXtream.Pages.DataEntry.ExcelDataEdit();
                ExcelDataEdit ExcelEdit = new ExcelDataEdit();

                PopupHelper dialog = new PopupHelper
                {
                    Title = "Edit Student Data",
                    Content = ExcelEdit,
                    ResizeMode = ResizeMode.NoResize,
                    Width = 600,
                    Height = 350
                };
                CampaingDataUploadDA StdFormPass = new CampaingDataUploadDA();
                var selectedRow = grid.SelectedItem as System.Data.DataRowView;

                if (selectedRow != null)
                {
                    StdFormPass.Row_id = int.Parse(selectedRow["Row_id"].ToString());
                    StdFormPass.Full_name = selectedRow["FULL_NAME"].ToString();
                    StdFormPass.First_name = selectedRow["name_1"].ToString();
                    StdFormPass.Last_name = selectedRow["name_2"].ToString();
                    StdFormPass.Mobile = selectedRow["phone"].ToString();

                }
                ExcelEdit.griddta = StdFormPass;//pass row values through constructor
                ExcelEdit.LoadGridRowToEditForm();
                dialog.ShowDialog();
                //if (ExcelEdit.FormOutResult == 1)
                // {
                //     GetDataFromTempTbltodatagrid();
                //  }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }
        private void DbUploadbtn_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateMobileNumber(table)<1) //validate invalid mobile before upload
            {
                if (CAMP_COMBO.SelectedValue != null)//deafaul value null sp return nuull="select campagn"
                {

                    //if (txtAddName.Text.Trim() == "")
                    //{
                    //    System.Windows.Forms.MessageBox.Show("Please enter ad name");
                    //    return;
                    //}

                    try
                    {
                        DataTable FinalDbUploadTable = new DataTable();
                        FinalDbUploadTable.Columns.Add("Full_Name", typeof(string));
                        FinalDbUploadTable.Columns.Add("Mobile", typeof(string));
                        FinalDbUploadTable.Columns.Add("fstName", typeof(string));
                        FinalDbUploadTable.Columns.Add("LastName", typeof(string));
                        FinalDbUploadTable.Columns.Add("MOBILE_NO_DUP_SEQ", typeof(int));
                        FinalDbUploadTable.Columns.Add("bandscore", typeof(string));
                        FinalDbUploadTable.Columns.Add("exam_type", typeof(string));
                        FinalDbUploadTable.Columns.Add("when_do_you_want", typeof(string));
                        FinalDbUploadTable.Columns.Add("your_english_knowledge", typeof(string));
                        FinalDbUploadTable.Columns.Add("your_ielts_knowledge", typeof(string));
                        // FinalDbUploadTable.Columns.Add("CMP_ID", typeof(int));

                        string full_name;
                        string mobil_no;
                        string fst_name;
                        string lst_name;
                        int dupSeq;
                        string bandscore;
                        string exam_type;
                        string when_do_you_want;
                        string your_english_knowledge;
                        string your_ielts_knowledge;


                        if (grid.ItemsSource == null || grid.Items.Count == 0)
                        {
                            System.Windows.Forms.MessageBox.Show("No data to Upload");
                        }
                        else
                        {
                            int rowCount = grid.Items.Count;

                            for (int i = 0; i < rowCount; i++)
                            {
                                var selectedRow = (System.Data.DataRowView)grid.Items[i];
                                full_name = selectedRow["FULL_NAME"].ToString();
                                mobil_no = selectedRow["phone"].ToString();
                                fst_name = selectedRow["name_1"].ToString();
                                lst_name = selectedRow["name_2"].ToString();
                                dupSeq =  Convert.ToInt32( selectedRow["MOBILE_NO_DUP_SEQ"]);
                                bandscore = selectedRow["bandscore"].ToString();
                                exam_type = selectedRow["exam_type"].ToString();
                                when_do_you_want = selectedRow["when_do_you_want"].ToString();
                                your_english_knowledge = selectedRow["your_english_knowledge"].ToString();
                                your_ielts_knowledge = selectedRow["your_ielts_knowledge"].ToString();

                                // CMP_ID = Convert.ToInt32(CAMP_COMBO.SelectedValue);
                                //  FinalDbUploadTable.Rows.Add(full_name, mobil_no, fst_name, lst_name, CMP_ID);
                                FinalDbUploadTable.Rows.Add(full_name, mobil_no, fst_name, lst_name, dupSeq, bandscore, exam_type, when_do_you_want, your_english_knowledge, your_ielts_knowledge);
                            }

                            CampaingDataUploadDA final_data = new CampaingDataUploadDA();

                            // var regex = new Regex("[^a-zA-Z\\s.]");

                            //  var query = from r in FinalDbUploadTable.AsEnumerable()
                            //              where regex.IsMatch(r.Field<string>("FULL_NAME")) ||
                            //             regex.IsMatch(r.Field<string>("fstName")) ||
                            //             regex.IsMatch(r.Field<string>("LastName"))
                            //             select r;
                            // if (query.Count() > 0)
                            //  {
                            //     System.Windows.Forms.MessageBox.Show("Invalid Charactors are found(?/><!@#$%......). Please check again! ");
                            //  }
                            //  else
                            //   {
                            //if ((dtpAddDate.Text == "Ad Date" || dtpAddDate.Text == string.Empty))
                            // {
                            //    MessageBox.Show("Please Select Ad Date)");
                            //}
                            //else
                            //{
                            if (txtAddName.Text == "Ad Name")
                            {
                                txtAddName.Text = "";
                            }

                            try
                            {
                                int CMP_ID = Convert.ToInt32(CAMP_COMBO.SelectedValue);
                                string _addName = txtAddName.Text.ToString();
                                DateTime _adddate = dtpAddDate.SelectedDate.Value;
                                string _user = StaticProperty.LoginUserID;

                                int _affectedRows = 0;
                                final_data.FinalUploadDataToTemp(FinalDbUploadTable, CMP_ID, _addName, _adddate, _user, out _affectedRows);
                                System.Windows.MessageBox.Show("Data Upload Complete.\n" + _affectedRows + " New Record(s) are inserted from " + _recordCount + " Record(s) !");

                                grid.ItemsSource = null;
                                CAMP_COMBO.SelectedValue = -1;
                                _recordCount = 0;
                                lblRecordCount.Content = "0";
                                txtAddName.Text = "";
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
                else
                {
                    System.Windows.Forms.MessageBox.Show("Select a Campaing");
                }
            } 
            else 
            {
                MessageBox.Show("Invalid Phone numbers are found!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }
        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadDataToCombo_FromSQL();
                CAMP_COMBO.SelectedValue = null;
                //_selectedFilePath = null;
                // path_lbl.Content = "File path";
                // table.Rows.Clear();
                // _recordCount = 0;
                // lblRecordCount.Content = 0;
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void btnViewHistory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                    CMSXtream.Pages.View.DataUploadedHistory uploadedHistory = new CMSXtream.Pages.View.DataUploadedHistory();
                    PopupHelper dialog = new PopupHelper
                    {
                        Title = "Data Uploaded History",
                        Content = uploadedHistory,
                        ResizeMode = ResizeMode.CanResize,
                        Width = 1350,
                        Height = uploadedHistory.Height,
                        WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    };
                uploadedHistory.LoadCmPList(TblCmpHistory);
                uploadedHistory.LoadSummery(-1, DateTime.Now, DateTime.Now);
                dialog.ShowDialog();
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }
        private int ValidateMobileNumber(DataTable tblRdToUpl)
        {
            try
            {
                int count = -1;

                var regex = new Regex("[^0-9]");

                var query = from r in tblRdToUpl.AsEnumerable()
                            where regex.IsMatch(r.Field<string>("phone")) || r.Field<string>("phone").Length > 10 || r.Field<string>("phone").Length < 9
                            select r;
                    count = query.Count();
                foreach (var row in query)
                {
                    row.SetField("INVALID_REC", 9);
                }
                foreach (var row in tblRdToUpl.AsEnumerable().Except(query))
                {
                    var query2 = from r in tblRdToUpl.AsEnumerable()
                                 where r.Field<int?>("INVALID_REC") == 9 // Replace "INV" with the actual column name
                                 select r;
                    foreach (var row2 in query2)
                    {
                        row.SetField("INVALID_REC", 0);
                    }
                }
                 if(count > 0)
                {
                    grid.ItemsSource = tblRdToUpl.DefaultView;
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
        private void CloseExcelSheet(Microsoft.Office.Interop.Excel.Application excelApp, Workbook excelBook, Microsoft.Office.Interop.Excel._Worksheet excelSheet, Microsoft.Office.Interop.Excel.Range excelRange)
        {
            try
            {
                excelBook.Close(false);
                excelApp.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(excelRange);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(excelSheet);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(excelBook);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
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
                var selectedRow = grid.SelectedItem as System.Data.DataRowView;

                if (selectedRow != null)
                {
                    string _mobileNo = selectedRow["phone"].ToString();

                    DBSearch form = new DBSearch();
                    PopupHelper dialog = new PopupHelper
                    {
                        Title = "Student History",
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
    }
}