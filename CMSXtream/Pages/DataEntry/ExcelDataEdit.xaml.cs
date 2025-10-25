using System;
using System.Windows;
using System.Windows.Controls;
using XtreamDataAccess;

namespace CMSXtream.Pages.DataEntry
{
    /// <summary>
    /// Interaction logic for ExcelDataEdit.xaml
    /// </summary>
    public partial class ExcelDataEdit : UserControl
    {
        public int FormOutResult {  get; set; }
        public CampaingDataUploadDA griddta;

        public ExcelDataEdit()
        {
            InitializeComponent();
            this.DataContext = this;




        }
        public void LoadGridRowToEditForm()
        {
            //CampaingDataUploadDA ExcelData = new CampaingDataUploadDA();
            Fullname.Text = griddta.Full_name;
            FirstName.Text = griddta.First_name;
            LastName.Text = griddta.Last_name;
            Mobile.Text = griddta.Mobile;



        }


        private void Cancel_ButtonClick(object sender, RoutedEventArgs e)
        {
            try {

                MessageBoxResult result = MessageBox.Show("Do you want to Cancel this Without Saving?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    Window.GetWindow(this).Close();
                }
            }catch {
                throw;
            }
         }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            CampaingDataUploadDA PassUserEditDta = new CampaingDataUploadDA();
            string Edit_Full_name = Fullname.Text.ToString();
            string Edit_First_name = FirstName.Text.ToString();
           string Edit_Last_name = LastName.Text.ToString();
            string Edit_Mobile = Mobile.Text.ToString();
            Int32 Row_id=griddta.Row_id;


            FormOutResult = 1;

            CampaingDataUploadDA SaveUserEditData = new CampaingDataUploadDA();
            SaveUserEditData.SaveUserUpadateData( Row_id,Edit_Full_name, Edit_First_name, Edit_Last_name, Edit_Mobile);
            ((FirstFloor.ModernUI.Windows.Controls.ModernWindow)this.Parent).Close();
            // Window.GetWindow(this).Close();



        }

      
    }
}
