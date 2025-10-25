using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using XtreamDataAccess;

namespace CMSXtream.Pages.View
{
    /// <summary>
    /// Interaction logic for CommisionBreakDown.xaml
    /// </summary>
    public partial class CommisionBreakDown : UserControl
    {
        public System.Data.DataTable CommisionBreakDownTable;
        public CommisionBreakDown()
        {
            InitializeComponent();
        }
        public void LoadFormContaint()
        {
            if (CommisionBreakDownTable.Rows.Count > 0)
            {
                grdComBreakDowun.ItemsSource = CommisionBreakDownTable.DefaultView;
            }
            else
            {
                grdComBreakDowun.ItemsSource = null;
            }
        }
    }
}
