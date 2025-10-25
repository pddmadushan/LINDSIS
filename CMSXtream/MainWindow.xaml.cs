using FirstFloor.ModernUI.Windows.Controls;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows;

namespace CMSXtream
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ModernWindow
    {
        public MainWindow()
        { 
            InitializeComponent();

            SqlConnection dbConn = new SqlConnection(ConfigurationManager.ConnectionStrings["CMSXtreamConnection"].ToString());
            if (dbConn.Database.ToString() == "ENGLISHBUS")
            {
                this.Title = dbConn.Database.ToString()+ " [" + StaticProperty.LoginUserID + "]";
            }

            if (StaticProperty.LoginisAdmin != "1")
            {
                var window = App.Current.MainWindow as ModernWindow;
                //6 is Admin menu
                var toRemove = window.MenuLinkGroups.ElementAt(6);
                window.MenuLinkGroups.Remove(toRemove);

                //3 is Master menu
                var toRemoveMaster = window.MenuLinkGroups.ElementAt(3);
                window.MenuLinkGroups.Remove(toRemoveMaster);

                //remove assign lead page
                /* var Menu = window.MenuLinkGroups.ElementAt(2);
                 var link = Menu.Links.ElementAt(3);
                 Menu.Links.Remove(link);
                */
            }
        }

        private void ModernWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = System.Windows.MessageBox.Show("Do you want to exit from the system ?", "Confirmation"/*box tttle*/, MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }
    }

}
