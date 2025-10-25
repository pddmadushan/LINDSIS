using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using XtreamDataAccess;

namespace CMSXtream.Pages.DataEntry
{
    /// <summary>
    /// Interaction logic for ChangePassword.xaml
    /// </summary>
    public partial class ChangePasswordSelf : UserControl
    {

        public Boolean IsAddNew { get; set; }
        
        public String UserAccount { get; set; }
        public Boolean IsActive { get; set; }
        public Boolean IsAdmin { get; set; }
        public double incentivePtg { get; set; }
        public string OutResult { get; set; }

        public ChangePasswordSelf()
        {
            InitializeComponent();
            txtUserId.Text = StaticProperty.LoginUserID; 
        }


        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoginDA _clsLogin = new LoginDA();

                    if (txtUserId.Text.Trim() == String.Empty)
                    {
                        MessageBox.Show("Please define a user name!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                        txtUserId.Focus();
                        return;
                    }

                    if (txtPassWord.Password.Trim() == String.Empty)
                    {
                        MessageBox.Show("Please define a password!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                        txtPassWord.Focus();
                        return;
                    }

                    if (txtPassWord.Password.Trim() != txtConfirmPassWord.Password.Trim())
                    {
                        MessageBox.Show("Password confirmation is wrong!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                        txtConfirmPassWord.Focus();
                        return;
                    }

                    _clsLogin.CLS_USER_ID = txtUserId.Text.Trim();
                    _clsLogin.CLS_USER_PASSWORD = txtPassWord.Password.Trim();
                    _clsLogin.UpdatePasswordSelf();
                    MessageBox.Show("User password has been updated!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No); 

            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has occurred.Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

    }
}
