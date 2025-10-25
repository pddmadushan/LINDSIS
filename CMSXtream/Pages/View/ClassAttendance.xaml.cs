using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using XtreamDataAccess;
using System.Windows.Interop;
using swf = System.Windows.Forms;

namespace CMSXtream.Pages.DataEntry
{
    /// <summary>
    /// Interaction logic for ClassAttendance.xaml
    /// </summary>
    public partial class ClassAttendance : UserControl
    {
        public Int32 isClassHeld = 0;
        public Int32 lastLoadClassGroup = -1;
        public ClassAttendance()
        {
            InitializeComponent();
            LoadClasess();
            LoadPart();
            if (CMSXtream.StaticProperty.LoginisAdmin != "1")
            {
                txtComment.IsEnabled = false;
            }
            ComponentDispatcher.ThreadPreprocessMessage += ComponentDispatcher_ThreadPreprocessMessage;
        }

        const int WM_KEYDOWN = 0x100;
        const int WM_SYSKEYDOWN = 0x0104;

        private void ComponentDispatcher_ThreadPreprocessMessage(ref MSG msg, ref bool handled)
        {
            if (txtComment.IsFocused)
            {
                if (msg.message == WM_KEYDOWN || msg.message == WM_SYSKEYDOWN)
                {
                    swf.Keys keyData = ((swf.Keys)((int)((long)msg.wParam))) | swf.Control.ModifierKeys;
                    String inputKeyStroke = String.Empty;

                    if (keyData.ToString() == "D, Control")
                    {
                        int curPos = txtComment.SelectionStart;
                        string replaceDateTime = DateTime.Now.ToString("yy/M/d");
                        txtComment.Text = txtComment.Text.Insert(curPos, replaceDateTime);
                        txtComment.CaretIndex = curPos + replaceDateTime.Length;
                    }
                }
            }
        }


        void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void TS_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            TS.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            ScrollViewer scrollviewer = sender as ScrollViewer;
            if (e.Delta > 0)
            {
                scrollviewer.LineUp();
            }
            else
            {
                scrollviewer.LineDown();
            }
            e.Handled = true;
        }

        public void SetClassHeldFlag()
        {
            try
            {
                ClassAttendanceDA _clsClass = new ClassAttendanceDA();
                _clsClass.CLS_REC_DATE = DateTime.Parse(dtpHoldDate.SelectedDate.Value.ToString());
                _clsClass.CLS_ID = Int32.Parse(cmbClsGroup.SelectedValue.ToString());
                System.Data.DataTable table = _clsClass.CheckClassHeldFlag().Tables[0];
                isClassHeld = 0;
                if (table.Rows.Count > 0)
                {
                    isClassHeld = 1;
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        public void LoadClasess()
        {
            try
            {
                Int32 clsFilterID = 0;

                if (chkShowInactive.IsChecked.Value)
                {
                    clsFilterID = 1;
                }

                Int32 day = -1;
                if (!(chkSelectedDay.IsChecked.Value))
                {
                    day = (Int32)dtpHoldDate.SelectedDate.Value.DayOfWeek;
                    if (day == 0)
                    {
                        day = 7;
                    }
                }

                StudentDA _clsStudent = new StudentDA();
                System.Data.DataTable table = _clsStudent.SelectAllClassForStudent(clsFilterID, day).Tables[0];
                cmbClsGroup.ItemsSource = table.DefaultView;
                cmbClsGroup.DisplayMemberPath = "CLS_NAME";
                cmbClsGroup.SelectedValuePath = "CLS_ID";

                if (lastLoadClassGroup != -1)
                {
                    cmbClsGroup.SelectedValue = lastLoadClassGroup;
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }

        }
        public void LoadPart()
        {
            try
            {
                List<PartClass> PartClass = new List<PartClass>();
                PartClass.Add(new PartClass(1, "Part 01"));
                PartClass.Add(new PartClass(2, "Part 02"));
                PartClass.Add(new PartClass(3, "Part 03"));
                PartClass.Add(new PartClass(4, "Part 04"));
                PartClass.Add(new PartClass(5, "Part 05"));
                PartClass.Add(new PartClass(6, "Part 06"));
                PartClass.Add(new PartClass(7, "Part 07"));
                PartClass.Add(new PartClass(8, "Part 08"));
                PartClass.Add(new PartClass(9, "Part 09"));
                PartClass.Add(new PartClass(10, "Part 10"));
                cmbPart.ItemsSource = PartClass;
                cmbPart.DisplayMemberPath = "PART_NAME";
                cmbPart.SelectedValuePath = "PART_ID";
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }

        }
        public void LoadWeek()
        {
            try
            {
                if (cmbClsGroup.SelectedIndex != -1 & cmbClsGroup.SelectedValue != null)
                {
                    ClassAttendanceDA _clsClass = new ClassAttendanceDA();
                    _clsClass.CLS_REC_DATE = DateTime.Parse(dtpHoldDate.SelectedDate.Value.ToString());
                    _clsClass.CLS_ID = Int32.Parse(cmbClsGroup.SelectedValue.ToString());
                    System.Data.DataTable table = _clsClass.SelectNextWeek().Tables[0];
                    if (table.Rows.Count > 0)
                    {
                        txtWeekNumber.Text = table.Rows[0]["CLS_WEEK"].ToString();
                        cmbPart.SelectedValue = table.Rows[0]["CLS_PART"].ToString();
                        txtComment.Text = table.Rows[0]["CLS_COMMENT"].ToString();
                    }
                    else
                    {
                        txtWeekNumber.Text = "0";
                        cmbPart.SelectedValue = "1";
                        txtComment.Text = "";
                    }
                }
                else
                {
                    txtWeekNumber.Text = "";
                    cmbPart.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }

        }

        private void btnSaveClass_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cmbClsGroup.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select a class to mark attendance!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                    return;
                }
                if (txtWeekNumber.Text.Trim() == string.Empty)
                {
                    MessageBox.Show("Please define a week!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                    return;
                }

                StudentDA _clsStudent = new StudentDA();
                _clsStudent.CLS_COMMENT = txtComment.Text;
                Boolean iscancelCall = true;
                if (!(chkClassHoldFlg.IsChecked.Value))
                {
                    _clsStudent.CLS_WEEK = 0;
                    _clsStudent.CLS_PART = 0;
                    MessageBoxResult resultMessageBox = MessageBox.Show("Are you sure that class is not held?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (resultMessageBox != MessageBoxResult.Yes)
                    {
                        iscancelCall = false;
                    }
                }
                else
                {
                    _clsStudent.CLS_WEEK = Int32.Parse(txtWeekNumber.Text.Trim());
                }

                if (iscancelCall)
                {
                    _clsStudent.CLS_ID = Int32.Parse(cmbClsGroup.SelectedValue.ToString());
                    _clsStudent.CLS_REC_DATE = DateTime.Parse(dtpHoldDate.SelectedDate.Value.ToString());
                    _clsStudent.CLS_HOLD_FLG = chkClassHoldFlg.IsChecked.Value ? 1 : 0;
                    if (cmbPart.SelectedValue == null)
                    {
                        _clsStudent.CLS_PART = 1;
                    }
                    else
                    {
                        _clsStudent.CLS_PART = Int32.Parse(cmbPart.SelectedValue.ToString());
                    }
                    if (_clsStudent.InsertClassAttendance() > 0)
                    {
                        MessageBox.Show("Record has been successfully saved!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                        txtWeekNumber.IsEnabled = false;
                        cmbPart.IsEnabled = false;
                        btnAddStudent.IsEnabled = true;
                        btnAddTute.IsEnabled = true;
                        BindStudentGrid();

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

        private void BindStudentGrid()
        {
            try
            {
                if (cmbClsGroup.SelectedIndex > -1 && cmbClsGroup.SelectedValue != null)
                {
                    StudentDA _clsStudent = new StudentDA();
                    _clsStudent.CLS_ID = Int32.Parse(cmbClsGroup.SelectedValue.ToString());
                    _clsStudent.CLS_REC_DATE = DateTime.Parse(dtpHoldDate.SelectedDate.Value.ToString());
                    _clsStudent.CLS_HOLD_FLG = chkClassHoldFlg.IsChecked.Value ? 1 : 0;
                    System.Data.DataTable table = _clsStudent.SelectClassAttendance().Tables[0];
                    System.Data.DataTable PerviousTable = _clsStudent.SelectClassAttendance().Tables[1];
                    if (table.Rows.Count > 0)
                    {
                        grdStudentAttendance.ItemsSource = table.DefaultView;
                    }
                    else
                    {
                        grdStudentAttendance.ItemsSource = null;
                    }

                    if (PerviousTable.Rows.Count > 0)
                    {
                        grdStudentAttendance.Columns[1].Header = PerviousTable.Rows[0][0].ToString();
                        grdStudentAttendance.Columns[2].Header = PerviousTable.Rows[0][1].ToString();
                    }
                    else
                    {
                        grdStudentAttendance.Columns[1].Header = "LW";
                        grdStudentAttendance.Columns[2].Header = "WBL";
                    }

                }
                else
                {
                    if (grdStudentAttendance != null)
                        grdStudentAttendance.ItemsSource = null;
                }
                setAttendanceInfo();
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void BindHoldFlag()
        {
            try
            {
                if (cmbClsGroup.SelectedIndex > -1 && cmbClsGroup.SelectedValue != null)
                {
                    StudentDA _clsStudent = new StudentDA();
                    _clsStudent.CLS_ID = Int32.Parse(cmbClsGroup.SelectedValue.ToString());
                    _clsStudent.CLS_REC_DATE = DateTime.Parse(dtpHoldDate.SelectedDate.Value.ToString());

                    string clsHoldFlag = _clsStudent.SelectClassHoldFlg();
                    if (clsHoldFlag == "0")
                    {
                        chkClassHoldFlg.IsChecked = false;
                        btnAddStudent.IsEnabled = false;
                        btnAddTute.IsEnabled = false;
                    }
                    else
                    {
                        chkClassHoldFlg.IsChecked = true;
                        btnAddStudent.IsEnabled = true;
                        btnAddTute.IsEnabled = true;
                    }
                }
                else
                {
                    chkClassHoldFlg.IsChecked = false;
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

                var result = (e.OriginalSource as CheckBox).IsChecked;

                StudentDA _clsStudent = new StudentDA();
                //var result = (sender as CheckBox).IsChecked;
                var selectedRow = grdStudentAttendance.SelectedItem as System.Data.DataRowView;

                if (selectedRow != null)
                {

                    _clsStudent.STD_ID = int.Parse(selectedRow["STD_ID"].ToString());
                    _clsStudent.CLS_ID = Int32.Parse(cmbClsGroup.SelectedValue.ToString());
                    _clsStudent.STD_REC_DATE = DateTime.Parse(selectedRow["STD_REC_DATE"].ToString());
                    _clsStudent.MODIFY_USER = StaticProperty.LoginUserID;
                    _clsStudent.CLS_REC_ATT_FLG = 1;
                    _clsStudent.UpdateStudentAttendence();
                    setAttendanceInfo();

                    //New student 
                    if (selectedRow["CLASS_FEE"].ToString() == "")
                    {
                        BindStudentGrid();
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

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = (e.OriginalSource as CheckBox).IsChecked;
                MessageBoxResult resultMessageBox = MessageBox.Show("Do you want to mark this student as absent?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (resultMessageBox == MessageBoxResult.Yes)
                {
                    StudentDA _clsStudent = new StudentDA();
                    //var result = (sender as CheckBox).IsChecked;
                    var selectedRow = grdStudentAttendance.SelectedItem as System.Data.DataRowView;
                    if (selectedRow != null)
                    {
                        _clsStudent.STD_ID = int.Parse(selectedRow["STD_ID"].ToString());
                        _clsStudent.CLS_ID = Int32.Parse(cmbClsGroup.SelectedValue.ToString());
                        _clsStudent.STD_REC_DATE = DateTime.Parse(selectedRow["STD_REC_DATE"].ToString());
                        _clsStudent.MODIFY_USER = StaticProperty.LoginUserID;
                        _clsStudent.CLS_REC_ATT_FLG = 0;
                        _clsStudent.UpdateStudentAttendence();
                        setAttendanceInfo();
                    }
                }
                else
                {
                    //(sender as CheckBox).IsChecked = true;
                    (e.OriginalSource as CheckBox).IsChecked = true;
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void cmbClsGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cmbClsGroup.IsDropDownOpen)
                {
                    lastLoadClassGroup = Int32.Parse(cmbClsGroup.SelectedValue.ToString());
                    BindStudentGrid();
                    BindHoldFlag();
                    LoadWeek();
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void dtpHoldDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cmbClsGroup != null)
                {

                    string selectedValue = "";
                    if (cmbClsGroup.SelectedValue != null)
                    {
                        selectedValue = cmbClsGroup.SelectedValue.ToString();
                    }

                    //cmbClsGroup.SelectedIndex = -1;
                    if (!(chkSelectedDay.IsChecked.Value))
                    {
                        LoadClasess();
                        if (selectedValue != "")
                        {
                            cmbClsGroup.SelectedValue = selectedValue;
                        }
                    }
                    BindStudentGrid();
                    BindHoldFlag();
                    LoadWeek();
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void btnPayment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CMSXtream.Pages.DataEntry.Student form = new CMSXtream.Pages.DataEntry.Student();
                PopupHelper dialog = new PopupHelper
                {
                    Title = "Student Info",
                    Content = form,
                    ResizeMode = ResizeMode.NoResize,
                    Width = 1000
                };

                StudentAttribute stAttPass = new StudentAttribute();
                var selectedRow = grdStudentAttendance.SelectedItem as System.Data.DataRowView;
                if (selectedRow != null)
                {
                    stAttPass.STD_ID = int.Parse(selectedRow["STD_ID"].ToString());
                    stAttPass.CLS_ID = int.Parse(selectedRow["CLS_ID"].ToString());
                    stAttPass.CLS_NAME = selectedRow["CLS_NAME"].ToString();
                    stAttPass.STD_INITIALS = selectedRow["STD_INITIALS"].ToString();
                    stAttPass.STD_SURNAME = selectedRow["STD_SURNAME"].ToString();
                    stAttPass.STD_FULL_NAME = selectedRow["STD_FULL_NAME"].ToString();
                    stAttPass.STD_GENDER = int.Parse(selectedRow["STD_GENDER"].ToString());
                    stAttPass.STD_DATEOFBIRTH = DateTime.Parse(selectedRow["STD_DATEOFBIRTH"].ToString());
                    stAttPass.STD_JOIN_DATE = DateTime.Parse(selectedRow["STD_JOIN_DATE"].ToString());
                    stAttPass.STD_EMAIL = selectedRow["STD_EMAIL"].ToString();
                    stAttPass.STD_NIC = selectedRow["STD_NIC"].ToString();
                    stAttPass.STD_ADDRESS = selectedRow["STD_ADDRESS"].ToString();
                    stAttPass.STD_CLASS_FEE = Double.Parse(selectedRow["STD_CLASS_FEE"].ToString());
                    stAttPass.STD_TELEPHONE = selectedRow["STD_TELEPHONE"].ToString();
                    stAttPass.STD_ACTIVE_FLG = int.Parse(selectedRow["STD_ACTIVE_FLG"].ToString());
                    stAttPass.STD_COMMENT = selectedRow["STD_COMMENT"].ToString();
                    stAttPass.STD_TEMP_NOTE = selectedRow["STD_TEMP_NOTE"].ToString();
                    stAttPass.RSN_ID = int.Parse(selectedRow["RSN_ID"].ToString());
                }
                form.IsAddNew = false;
                //form.IsControlDisable = true;
                form.stAtt = stAttPass;
                form.attDate = dtpHoldDate.SelectedDate.Value;
                form.LoadFormContaint();
                form.btnSave.Visibility = System.Windows.Visibility.Visible;
                form.btnDelete.Visibility = System.Windows.Visibility.Hidden;
                dialog.ShowDialog();
                BindStudentGrid();

            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void chkShowInactive_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                lastLoadClassGroup = -1;
                LoadClasess();
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void chkAelectedDay_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                lastLoadClassGroup = -1;
                LoadClasess();
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void chkAelectedDay_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                lastLoadClassGroup = -1;
                LoadClasess();
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void chkShowInactive_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                lastLoadClassGroup = -1;
                LoadClasess();
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void chkClassHoldFlg_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cmbClsGroup.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select a class to mark attendance!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                    chkClassHoldFlg.IsChecked = !(chkClassHoldFlg.IsChecked.Value);
                    return;
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void btnAddStudent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cmbClsGroup.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select a class to mark attendance!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                    return;
                }

                CMSXtream.Pages.View.StudentSummery form = new CMSXtream.Pages.View.StudentSummery();
                PopupHelper dialog = new PopupHelper
                {
                    Title = "Add Student",
                    Content = form,
                    ResizeMode = ResizeMode.NoResize,
                    Width = 950
                };
                form.classDate = dtpHoldDate.SelectedDate.Value;
                form.selectedClass = Int32.Parse(cmbClsGroup.SelectedValue.ToString());
                form.btnAddNew.Visibility = System.Windows.Visibility.Hidden;
                form.ShowAddStudent();
                dialog.ShowDialog();
                BindStudentGrid();


                grdStudentAttendance.Focus();
                //then create a new cell info, with the item we wish to edit and the column number of the cell we want in edit mode
                DataGridCellInfo cellInfo = new DataGridCellInfo("1591", grdStudentAttendance.Columns[10]);
                //set the cell to be the active one
                grdStudentAttendance.SelectedItems.Add(cellInfo.Item);

            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void btnAddTute_Click(object sender, RoutedEventArgs e)
        {
            if (cmbClsGroup.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a class to distribute accessory !", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                return;
            }

            ClassAttendanceDA clsAtt = new ClassAttendanceDA();
            clsAtt.CLS_REC_DATE = dtpHoldDate.SelectedDate.Value;
            clsAtt.CLS_ID = Int32.Parse(cmbClsGroup.SelectedValue.ToString());
            if (clsAtt.GetClassAttendanceCount()[0] == 0)
            {
                MessageBox.Show("Nobody is in the class room. You cannot distribute accessory.!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                return;
            }

            CMSXtream.Pages.View.AddTute form = new CMSXtream.Pages.View.AddTute();
            PopupHelper dialog = new PopupHelper
            {
                Title = "Accessory Distribution",
                Content = form,
                ResizeMode = ResizeMode.NoResize,
                Width = 500
            };

            form.classID = Int32.Parse(cmbClsGroup.SelectedValue.ToString());
            form.classDate = dtpHoldDate.SelectedDate.Value;
            form.LoadFormContaint();
            dialog.ShowDialog();
            BindStudentGrid();
        }

        private void btnAddHistory_Click(object sender, RoutedEventArgs e)
        {
            if (cmbClsGroup.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a class to mark attendance!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                return;
            }

            CMSXtream.Pages.View.ClassHistory form = new CMSXtream.Pages.View.ClassHistory();
            PopupHelper dialog = new PopupHelper
            {
                Title = "Class History",
                Content = form,
                ResizeMode = ResizeMode.NoResize,
                Width = 500
            };

            form.classID = Int32.Parse(cmbClsGroup.SelectedValue.ToString());
            form.LoadFormContaint();
            dialog.ShowDialog();

        }

        private void Label_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            txtWeekNumber.IsEnabled = true;
            cmbPart.IsEnabled = true;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadClasess();
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Int32 inactiveFlg = 0;
                MessageBoxResult resultMessageBox = MessageBox.Show("Do you want to remove this student from attendance?", "Attendance Removal Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                inactiveFlg = 0;
                if (resultMessageBox == MessageBoxResult.Yes)
                {
                    ClassAttendanceDA _clsAttendance = new ClassAttendanceDA();
                    var selectedRow = grdStudentAttendance.SelectedItem as System.Data.DataRowView;
                    if (selectedRow != null)
                    {
                        _clsAttendance.STD_ID = int.Parse(selectedRow["STD_ID"].ToString());
                        _clsAttendance.CLS_ID = Int32.Parse(cmbClsGroup.SelectedValue.ToString());
                        _clsAttendance.CLS_REC_DATE = DateTime.Parse(selectedRow["STD_REC_DATE"].ToString());
                        _clsAttendance.IS_INACTIVE_STD = inactiveFlg;
                        _clsAttendance.RemoveStudentFromClass();
                        if (inactiveFlg == 1)
                        {
                            MessageBox.Show("Student has been removed from attendance and Inactivated!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                        }
                        BindStudentGrid();
                    }
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void setAttendanceInfo()
        {
            try
            {
                Int32[] attCount = { 0, 0, 0, 0 };
                if (cmbClsGroup.SelectedIndex > -1 && cmbClsGroup.SelectedValue != null)
                {
                    ClassAttendanceDA clsAtt = new ClassAttendanceDA();
                    clsAtt.CLS_REC_DATE = dtpHoldDate.SelectedDate.Value;
                    clsAtt.CLS_ID = Int32.Parse(cmbClsGroup.SelectedValue.ToString());
                    attCount = clsAtt.GetClassAttendanceCount();
                }
                lblAttendanceCount.Content = attCount[0].ToString() + " / " + grdStudentAttendance.Items.Count.ToString();
                lblCardCount.Content = attCount[1].ToString();
                string tooltip = attCount.ToString() + " student(s) came from " + grdStudentAttendance.Items.Count.ToString() + " student(s)";
                lblAttendanceCount.ToolTip = tooltip;
                lblAttendanceImage.ToolTip = tooltip;

                //lblTempInactive.Content = attCount[3].ToString();
                //lblPermInactive.Content = attCount[2].ToString();

                btnTempInactive.Content = attCount[3].ToString();
                btnPermInactive.Content = attCount[2].ToString();

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
                CMSXtream.Pages.DataEntry.Student form = new CMSXtream.Pages.DataEntry.Student();
                PopupHelper dialog = new PopupHelper
                {
                    Title = "Student Info",
                    Content = form,
                    ResizeMode = ResizeMode.NoResize,
                    Width = 1000
                };

                StudentAttribute stAttPass = new StudentAttribute();
                var selectedRow = grdStudentAttendance.SelectedItem as System.Data.DataRowView;
                if (selectedRow != null)
                {
                    stAttPass.STD_ID = int.Parse(selectedRow["STD_ID"].ToString());
                    stAttPass.CLS_ID = int.Parse(selectedRow["CLS_ID"].ToString());
                    stAttPass.CLS_NAME = selectedRow["CLS_NAME"].ToString();
                    stAttPass.STD_INITIALS = selectedRow["STD_INITIALS"].ToString();
                    stAttPass.STD_SURNAME = selectedRow["STD_SURNAME"].ToString();
                    stAttPass.STD_FULL_NAME = selectedRow["STD_FULL_NAME"].ToString();
                    stAttPass.STD_GENDER = int.Parse(selectedRow["STD_GENDER"].ToString());
                    stAttPass.STD_DATEOFBIRTH = DateTime.Parse(selectedRow["STD_DATEOFBIRTH"].ToString());
                    stAttPass.STD_JOIN_DATE = DateTime.Parse(selectedRow["STD_JOIN_DATE"].ToString());
                    stAttPass.STD_EMAIL = selectedRow["STD_EMAIL"].ToString();
                    stAttPass.STD_NIC = selectedRow["STD_NIC"].ToString();
                    stAttPass.STD_ADDRESS = selectedRow["STD_ADDRESS"].ToString();
                    stAttPass.STD_CLASS_FEE = Double.Parse(selectedRow["STD_CLASS_FEE"].ToString());
                    stAttPass.STD_TELEPHONE = selectedRow["STD_TELEPHONE"].ToString();
                    stAttPass.STD_ACTIVE_FLG = int.Parse(selectedRow["STD_ACTIVE_FLG"].ToString());
                    stAttPass.STD_COMMENT = selectedRow["STD_COMMENT"].ToString();
                    stAttPass.STD_TEMP_NOTE = selectedRow["STD_TEMP_NOTE"].ToString();
                    stAttPass.RSN_ID = int.Parse(selectedRow["RSN_ID"].ToString());
                }
                form.IsAddNew = false;
                form.stAtt = stAttPass;
                form.attDate = dtpHoldDate.SelectedDate.Value;
                form.LoadFormContaint();

                form.btnSave.Visibility = System.Windows.Visibility.Visible;
                form.btnDelete.Visibility = System.Windows.Visibility.Hidden;
                //form.btnNextMonthPayment.IsEnabled = false;
                //form.grdStudentClass.IsEnabled = false;
                //form.btnAddNewAssosory.IsEnabled = false;
                //form.grdAssosory.IsEnabled = false;
                //form.grdMovement.IsEnabled = false;

                dialog.ShowDialog();
                BindStudentGrid();

            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void btnSendSMS_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                int rowCount = grdStudentAttendance.Items.Count;
                if (rowCount > 0)
                {
                    System.Data.DataTable table = new System.Data.DataTable();
                    table.Columns.Add("STD_ID", typeof(Int32));
                    table.Columns.Add("CLS_ID", typeof(Int32));
                    table.Columns.Add("STD_INITIALS", typeof(string));
                    table.Columns.Add("CLS_NAME", typeof(string));
                    table.Columns.Add("STD_TELEPHONE", typeof(string));

                    for (int i = 0; i < rowCount; i++)
                    {
                        var selectedRow = (System.Data.DataRowView)grdStudentAttendance.Items[i];
                        Int32 studentID = int.Parse(selectedRow["STD_ID"].ToString());
                        string Telephone = selectedRow["STD_TELEPHONE"].ToString();
                        string StudentName = selectedRow["STD_INITIALS"].ToString();
                        string className = selectedRow["CLS_NAME"].ToString();
                        Int32 classID = int.Parse(selectedRow["CLS_ID"].ToString());
                        table.Rows.Add(studentID, classID, StudentName, className, Telephone);
                    }

                    CMSXtream.Pages.DataEntry.SendSMS form = new CMSXtream.Pages.DataEntry.SendSMS();
                    PopupHelper dialog = new PopupHelper
                    {
                        Title = "Sent SMS to Student ",
                        Content = form,
                        ResizeMode = ResizeMode.NoResize,
                        Width = 1000,
                        Height = 680
                    };
                    form.btnAddNew.Visibility = System.Windows.Visibility.Hidden;
                    form.callingForm = true;
                    form.filteredTable = table;
                    form.BindStudentGrid();
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

        private void btnSendSMS_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedRow = grdStudentAttendance.SelectedItem as System.Data.DataRowView;
                if (selectedRow != null)
                {
                    System.Data.DataTable table = new System.Data.DataTable();
                    table.Columns.Add("STD_ID", typeof(Int32));
                    table.Columns.Add("CLS_ID", typeof(Int32));
                    table.Columns.Add("STD_INITIALS", typeof(string));
                    table.Columns.Add("CLS_NAME", typeof(string));
                    table.Columns.Add("STD_TELEPHONE", typeof(string));

                    Int32 studentID = int.Parse(selectedRow["STD_ID"].ToString());
                    string Telephone = selectedRow["STD_TELEPHONE"].ToString();
                    string StudentName = selectedRow["STD_INITIALS"].ToString();
                    string className = selectedRow["CLS_NAME"].ToString();
                    Int32 classID = int.Parse(selectedRow["CLS_ID"].ToString());
                    table.Rows.Add(studentID, classID, StudentName, className, Telephone);

                    CMSXtream.Pages.DataEntry.SendSMS form = new CMSXtream.Pages.DataEntry.SendSMS();
                    PopupHelper dialog = new PopupHelper
                    {
                        Title = "Sent SMS to Student " + StudentName,
                        Content = form,
                        ResizeMode = ResizeMode.NoResize,
                        Width = 1000,
                        Height = 680
                    };
                    form.btnAddNew.Visibility = System.Windows.Visibility.Hidden;
                    form.callingForm = true;
                    form.filteredTable = table;
                    form.BindStudentGrid();

                    form.txtEmpNumber.Text = studentID.ToString();
                    form.chkAll.IsChecked = false;
                    form.LoadSMSHistory();

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

        private void btnComments_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedRow = grdStudentAttendance.SelectedItem as System.Data.DataRowView;
                if (selectedRow != null)
                {
                    System.Data.DataTable table = new System.Data.DataTable();
                    string note = selectedRow["STD_TEMP_NOTE"].ToString();
                    string StudentName = selectedRow["STD_INITIALS"].ToString();
                    MessageBoxResult resultMessageBox = MessageBox.Show(" Note : " + note + "\n Do you want to delete special note?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (resultMessageBox == MessageBoxResult.Yes)
                    {
                        Int32 studentID = int.Parse(selectedRow["STD_ID"].ToString());
                        ClassAttendanceDA objClss = new ClassAttendanceDA();
                        objClss.UpdateStudentNote(studentID);
                        BindStudentGrid();
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

        private void btnPrintrid_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (grdStudentAttendance.Items.Count > 0)
                {
                    PrintDialog pt = new PrintDialog();
                    if (pt.ShowDialog() == true)
                    {
                        pt.PrintVisual(PaymentParent, "Attendance Sheet");
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

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BindStudentGrid();
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
            try
            {
                if (cmbClsGroup.SelectedIndex > -1)
                {
                    MessageBoxResult resultMessageBox = MessageBox.Show("Do you want to delete all attendance?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (resultMessageBox == MessageBoxResult.Yes)
                    {
                        ClassAttendanceDA clsAtt = new ClassAttendanceDA();
                        clsAtt.CLS_REC_DATE = dtpHoldDate.SelectedDate.Value;
                        clsAtt.CLS_ID = Int32.Parse(cmbClsGroup.SelectedValue.ToString());
                        clsAtt.DeleteClassAttendance();
                        BindStudentGrid();
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

        private void lblTempInactive_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void btnTempInactive_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (cmbClsGroup.SelectedIndex > -1)
                {


                    CMSXtream.Pages.View.StudentSummery form = new CMSXtream.Pages.View.StudentSummery(Int32.Parse(cmbClsGroup.SelectedValue.ToString()), 2);
                    PopupHelper dialog = new PopupHelper
                    {
                        Title = "Veiw Student",
                        Content = form,
                        ResizeMode = ResizeMode.NoResize,
                        Width = 1000
                    };
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

        private void btnPermInactive_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (cmbClsGroup.SelectedIndex > -1)
                {
                    Int32 weekNumber;
                    if (txtWeekNumber.Text.Trim() == string.Empty)
                    {
                        weekNumber = 0;
                    }
                    else
                    {
                        weekNumber = Int32.Parse(txtWeekNumber.Text.Trim());
                    }

                    CMSXtream.Pages.View.StudentSummery form = new CMSXtream.Pages.View.StudentSummery(Int32.Parse(cmbClsGroup.SelectedValue.ToString()), 0, weekNumber);
                    
                    form.STD_LOAD_ID = 1;
                    form.STD_CURENTWEEK = weekNumber;
                    form.STD_INACTIVESTATE = 0;
                    form.selectedClass = Int32.Parse(cmbClsGroup.SelectedValue.ToString());

                    PopupHelper dialog = new PopupHelper
                    {
                        Title = "Veiw Future Arranged Student",
                        Content = form,
                        ResizeMode = ResizeMode.NoResize,
                        Width = 1000
                    };
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
