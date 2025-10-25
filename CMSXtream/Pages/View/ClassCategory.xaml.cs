using System;
using System.Windows;
using System.Windows.Controls;
using XtreamDataAccess;

namespace CMSXtream.Pages.View
{
    /// <summary>
    /// Interaction logic for ClassCategory.xaml
    /// </summary>
    public partial class ClassCategory : UserControl
    {
        public ClassCategory()
        {
            InitializeComponent();
            BindStudentGrid();
        }
        private void BindStudentGrid()
        {
            try
            {
                ClassCategoryDA _ClsCategory = new ClassCategoryDA();
                System.Data.DataTable table = _ClsCategory.SelectAllCategory().Tables[0];
                if (table.Rows.Count > 0)
                {
                    grdClsCategory.ItemsSource = table.DefaultView;
                }
                else
                {
                    grdClsCategory.ItemsSource = null;
                }

            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void btnAddNew_Click(object sender, RoutedEventArgs e)
        {
            CMSXtream.Pages.DataEntry.CategoryForm form = new CMSXtream.Pages.DataEntry.CategoryForm();
            form.IsAddNew = true;
            PopupHelper dialog = new PopupHelper
            {
                Title = "Add Category",
                Content = form,
                ResizeMode = ResizeMode.NoResize
            };
            form.LoadFormContaint();
            dialog.ShowDialog();
            string ReturnMessage = form.OutResult;
            if (ReturnMessage != string.Empty && ReturnMessage != null)
            {
                MessageBox.Show(ReturnMessage, StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                BindStudentGrid();
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            CMSXtream.Pages.DataEntry.CategoryForm form = new CMSXtream.Pages.DataEntry.CategoryForm();
            PopupHelper dialog = new PopupHelper
            {
                Title = "Edit Category",
                Content = form,
                ResizeMode = ResizeMode.NoResize
            };

            ClassCategoryAttribute catAttPass = new ClassCategoryAttribute();
            var selectedRow = grdClsCategory.SelectedItem as System.Data.DataRowView;
            if (selectedRow != null)
            {
                catAttPass.CAT_ID = int.Parse(selectedRow["CAT_ID"].ToString());
                catAttPass.CAT_NAME = selectedRow["CAT_NAME"].ToString();
                catAttPass.CAT_COM_USER_PTG = selectedRow["CAT_COM_USER_PTG"].ToString().Trim() == "" ? 0 : Double.Parse(selectedRow["CAT_COM_USER_PTG"].ToString().Trim());
                catAttPass.CAT_COM_COMN_PTG = selectedRow["CAT_COM_COMN_PTG"].ToString().Trim() == "" ? 0 : Double.Parse(selectedRow["CAT_COM_COMN_PTG"].ToString().Trim());
            }
            form.IsAddNew = false;
            form.catAtt = catAttPass;
            form.LoadFormContaint();
            dialog.ShowDialog();
            string ReturnMessage = form.OutResult;
            if (ReturnMessage != string.Empty && ReturnMessage != null)
            {
                MessageBox.Show(ReturnMessage, StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                BindStudentGrid();
            }
        }

        private void groupTextSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchSting = groupTextSearch.Text.Trim();
            if (searchSting != string.Empty)
            {
                Helper.searchGridByKey(grdClsCategory, "CAT_NAME", searchSting);
            }
        }
    }
}
