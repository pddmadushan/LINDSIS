using System;
using System.Windows;
using System.Windows.Controls;
using XtreamDataAccess;

namespace CMSXtream.Pages.View
{
    /// <summary>
    /// Interaction logic for StudentAttendanceHistory.xaml
    /// </summary>
    public partial class StudentAttendanceHistory : UserControl
    {
        public Int32 classID { get; set; }
        public Int32 studentID { get; set; }
        public Int32 classYear { get; set; }
        public Int32 classMonth { get; set; }

        public StudentAttendanceHistory()
        {
            InitializeComponent();
        }
        public void LoadFormContaint()
        {
            StudentDA _clsPayment = new StudentDA();
            _clsPayment.STD_ID = studentID;
            _clsPayment.CLS_ID = classID;
            _clsPayment.CLASS_YEAR = classYear;
            _clsPayment.CLASS_MONTH = classMonth;
            System.Data.DataTable table = _clsPayment.StudentClassAttendance().Tables[0];
            if (table.Rows.Count > 0)
            {
                grdAttHistory.ItemsSource = table.DefaultView;
            }
            else
            {
                grdAttHistory.ItemsSource = null;
            }
        }

        private void chkMark_Checked(object sender, RoutedEventArgs e)
        {
            var selectedRow = grdAttHistory.SelectedItem as System.Data.DataRowView;
            if (selectedRow != null)
            {
                if (selectedRow["CLS_REC_DATE"].ToString() != "")
                {
                    StudentDA _clsPayment = new StudentDA();
                    _clsPayment.STD_ID = studentID;
                    _clsPayment.CLS_ID = classID;
                    _clsPayment.STD_REC_DATE = DateTime.Parse(selectedRow["CLS_REC_DATE"].ToString());
                    _clsPayment.MODIFY_USER = StaticProperty.LoginUserID;
                    _clsPayment.CLS_REC_ATT_FLG = 1;
                    _clsPayment.UpdateStudentAttendence();
                    LoadFormContaint();
                }
            }
        }        
        private void chkMark_Unchecked(object sender, RoutedEventArgs e)
        {
            var selectedRow = grdAttHistory.SelectedItem as System.Data.DataRowView;
            if (selectedRow != null)
            {
                if (selectedRow["CLS_REC_DATE"].ToString() != "")
                {
                    StudentDA _clsPayment = new StudentDA();
                    _clsPayment.STD_ID = studentID;
                    _clsPayment.CLS_ID = classID;
                    _clsPayment.STD_REC_DATE = DateTime.Parse(selectedRow["CLS_REC_DATE"].ToString());
                    _clsPayment.MODIFY_USER = StaticProperty.LoginUserID;
                    _clsPayment.CLS_REC_ATT_FLG = 0;
                    _clsPayment.UpdateStudentAttendence();
                    LoadFormContaint();
                }
            }
        }
    }
}
