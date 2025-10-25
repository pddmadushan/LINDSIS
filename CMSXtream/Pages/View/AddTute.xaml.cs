using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using XtreamDataAccess;

namespace CMSXtream.Pages.View
{
    /// <summary>
    /// Interaction logic for AddTute.xaml
    /// </summary>
    public partial class AddTute : UserControl
    {
        public Int32 classID { get; set; }
        public DateTime classDate { get; set; }
        public AddTute()
        {
            InitializeComponent();
        }

        public void LoadFormContaint()
        {
            LoadAssosory();
            BindAssosoryGrid();
        }

        private void LoadAssosory()
        {
            try
            {
                AccessoryDA _Assosory = new AccessoryDA();
                System.Data.DataTable table = _Assosory.SelectAllAccessory().Tables[0];
                List<AccessoryAttribute> assosoryClass = new List<AccessoryAttribute>();
                foreach (System.Data.DataRow row in table.Rows)
                {
                    assosoryClass.Add(new AccessoryAttribute(Int32.Parse(row["ACC_ID"].ToString()), row["ACC_NAME"].ToString(), double.Parse(row["ACC_AMOUNT"].ToString()), Int32.Parse(row["ACC_PAYBLE_FLG"].ToString()), row["ACC_COMMENT"].ToString(), Int32.Parse(row["ACC_DELIVERABLE_FLG"].ToString())));
                }
                cmbAccesory.ClearValue(ItemsControl.ItemsSourceProperty);
                cmbAccesory.ItemsSource = assosoryClass;
                cmbAccesory.DisplayMemberPath = "ACC_NAME";
                cmbAccesory.SelectedValuePath = "ACC_ID";

            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }

        }

        private void BindAssosoryGrid()
        {
            try
            {
                AddTuteDA _clsPayment = new AddTuteDA();
                _clsPayment.CLS_ID = classID;
                System.Data.DataTable table = _clsPayment.SelectClassAccessory().Tables[0];
                if (table.Rows.Count > 0)
                {
                    grdClassAccessory.ItemsSource = table.DefaultView;
                }
                else
                {
                    grdClassAccessory.ItemsSource = null;
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
                if (cmbAccesory.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select a Accessory!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                    return;
                }

                AddTuteDA _clsPayment = new AddTuteDA();
                _clsPayment.CLS_ID = classID;
                _clsPayment.CLS_REC_DATE = classDate;
                _clsPayment.ACC_ID = Int32.Parse(cmbAccesory.SelectedValue.ToString());
                _clsPayment.SaveClassAccessory();
                BindAssosoryGrid();
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
                  MessageBoxResult result = MessageBox.Show("Do you want to delete this record?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                  if (result == MessageBoxResult.Yes)
                  {
                      var selectedRow = grdClassAccessory.SelectedItem as System.Data.DataRowView;
                      if (selectedRow != null)
                      {
                          AddTuteDA _clsPayment = new AddTuteDA();
                          _clsPayment.CLS_ID = classID;
                          _clsPayment.CLS_REC_DATE = classDate;
                          _clsPayment.ACC_ID = int.Parse(selectedRow["ACC_ID"].ToString());
                          _clsPayment.DeleteClassAccessory();
                          BindAssosoryGrid();
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

        private void btnViewStudent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedRow = grdClassAccessory.SelectedItem as System.Data.DataRowView;
                if (selectedRow != null)
                {
                CMSXtream.Pages.View.TuteDistributedStudent form = new CMSXtream.Pages.View.TuteDistributedStudent();
                PopupHelper dialog = new PopupHelper
                {
                    Title = selectedRow["ACC_NAME"].ToString() + " - Distributed List",
                    Content = form,
                    ResizeMode = ResizeMode.NoResize,
                    Width = 300
                };
                
                form.AccossoryID = Int32.Parse(selectedRow["ACC_ID"].ToString());
                form.classID = classID;
                form.classDate = DateTime.Parse(selectedRow["CLS_ACC_REC_DATE"].ToString());
                form.AccossoryAmount = Double.Parse(selectedRow["ACC_AMOUNT"].ToString());
                form.LoadFormContaint();
                dialog.ShowDialog();
                BindAssosoryGrid();
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
