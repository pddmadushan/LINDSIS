using System;
using System.Windows.Controls;
using XtreamDataAccess;

namespace CMSXtream.Pages.View
{
    /// <summary>
    /// Interaction logic for ClassHistory.xaml
    /// </summary>
    public partial class ClassHistory : UserControl
    {
        public Int32 classID { get; set; }
        public ClassHistory()
        {
            InitializeComponent();
        }
        public void LoadFormContaint()
        {
            ClassAttendanceDA _clsPayment = new ClassAttendanceDA();
            _clsPayment.CLS_ID = classID;
            System.Data.DataTable table = _clsPayment.ClassHistory().Tables[0];
            if (table.Rows.Count > 0)
            {
                grdClassHistory.ItemsSource = table.DefaultView;
            }
            else
            {
                grdClassHistory.ItemsSource = null;
            }
        }
    }
}
