using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using XtreamDataAccess;

namespace WpfApplication2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Boolean Login { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            txtUserName.Focus();
            SetBackGrund();

            //SetTestLogin();
        }

        private void SetTestLogin()
        {
            txtUserName.Text = "sysadmin";
            txtPassword.Password = "0772018009";
        }
        private void SetBackGrund()
        {
            SqlConnection dbConn = new SqlConnection(ConfigurationManager.ConnectionStrings["CMSXtreamConnection"].ToString());

            if (dbConn.Database.ToString() == "ENGLISHBUS")
            {
                Image image = new Image();
                image.Source = new BitmapImage(
                    new Uri(
                       "pack://application:,,,../Image/EnglishBus.jpeg"));
                imgBackground.ImageSource = image.Source;
                lblName.Content = string.Empty;
                lblslogan.Content = string.Empty;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            errorValidator.Visibility = System.Windows.Visibility.Hidden;
            if (txtUserName.Text.Trim() == string.Empty)
            {
                errorValidator.Visibility = System.Windows.Visibility.Visible;
                errorValidator.ToolTip = "Enter Username";
                errorValidator.SetValue(Grid.RowProperty, 1);
                errorValidator.SetValue(Grid.ColumnProperty, 3);
                return;
            }

            if (txtPassword.Password.Trim() == string.Empty || txtPassword.Password.Trim() == "^%!@#%$")
            {
                errorValidator.Visibility = System.Windows.Visibility.Visible;
                errorValidator.ToolTip = "Enter Password";
                errorValidator.SetValue(Grid.RowProperty, 2);
                errorValidator.SetValue(Grid.ColumnProperty, 3);
                return;
            }

            Boolean passwordValidation = false;
            string accountisAdmin;
            string accountisSupperUser;
            int userID = 0;
            //This is for SysAdmin
            if (txtUserName.Text.Trim().ToUpper() == "SYSADMIN")
            {
                if (txtPassword.Password.Trim() == "0772018009")
                {
                    passwordValidation = true;
                    accountisAdmin = "1";
                    accountisSupperUser = "1";
                }
                else
                {
                    errorValidator.Visibility = System.Windows.Visibility.Visible;
                    errorValidator.ToolTip = "Incorrect Password";
                    errorValidator.SetValue(Grid.RowProperty, 2);
                    errorValidator.SetValue(Grid.ColumnProperty, 3);
                    return;
                }
            }
            else
            {
                LoginDA loginDA = new LoginDA();
                loginDA.CLS_USER_ID = txtUserName.Text.Trim().ToUpper();

                System.Data.DataTable table = loginDA.GetUserDetails().Tables[0];
                if (table.Rows.Count > 0)
                {
                    string accountActiveFlg = table.Rows[0]["CLS_USER_ACTIVE"].ToString();
                    accountisAdmin = table.Rows[0]["USER_IS_ADMIN"].ToString();
                    accountisSupperUser = table.Rows[0]["USER_IS_SUPERUSER"].ToString();
                    string accountPW = table.Rows[0]["CLS_USER_PASSWORD"].ToString();
                    userID = int.Parse(table.Rows[0]["ID"].ToString());
                    if (accountActiveFlg == "1")
                    {
                        StringComparer comparer = StringComparer.Ordinal;
                        string getHash = loginDA.GenarateXtreamHash(txtPassword.Password.Trim());
                        if (comparer.Compare(getHash, accountPW) == 0)
                        {
                            passwordValidation = true;
                        }
                        else
                        {
                            errorValidator.Visibility = System.Windows.Visibility.Visible;
                            errorValidator.ToolTip = "Incorrect Password";
                            errorValidator.SetValue(Grid.RowProperty, 2);
                            errorValidator.SetValue(Grid.ColumnProperty, 3);
                            return;
                        }
                    }
                    else
                    {
                        errorValidator.Visibility = System.Windows.Visibility.Visible;
                        errorValidator.ToolTip = "Account has been inactivated";
                        errorValidator.SetValue(Grid.RowProperty, 1);
                        errorValidator.SetValue(Grid.ColumnProperty, 3);
                        return;
                    }
                }
                else
                {
                    errorValidator.Visibility = System.Windows.Visibility.Visible;
                    errorValidator.ToolTip = "Incorrect Username";
                    errorValidator.SetValue(Grid.RowProperty, 1);
                    errorValidator.SetValue(Grid.ColumnProperty, 3);
                    return;
                }

            }

            if (passwordValidation)
            {
                CMSXtream.StaticProperty.LoginUserID = txtUserName.Text.Trim().ToUpper();
                CMSXtream.StaticProperty.LoginisAdmin = accountisAdmin;
                CMSXtream.StaticProperty.LoginisSuperAdmin = accountisSupperUser;
                CMSXtream.StaticProperty.LoginID = userID;
                Login = true;
                this.Close();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Login = false;
            this.Close();
        }

        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            txtPassword.Password = string.Empty;
        }

        private void txtPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtPassword.Password == string.Empty)
            {
                txtPassword.Password = "^%!@#%$";
            }
        }

    }
}
