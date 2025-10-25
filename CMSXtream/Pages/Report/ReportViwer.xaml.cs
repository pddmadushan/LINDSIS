using System;
using System.Collections.Generic;
using System.IO;
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
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using CrystalDecisions.Shared;

namespace CMSXtream.Pages.Report
{
    /// <summary>
    /// Interaction logic for ReportViwer.xaml
    /// </summary>
    public partial class ReportViwer : UserControl
    {
        public ReportViwer()
        {
            InitializeComponent();
            LoadReportFile();
        }
        private void LoadReportFile()
        {
            try
            {
                string fileFullPath = string.Empty;
                string path = System.AppDomain.CurrentDomain.BaseDirectory;
                path = path + "\\ReportFile";
                string reportName = "CashFlow1.rpt";
                fileFullPath = path + "\\" + reportName ;
                if (File.Exists(fileFullPath))
                {
                    XtreamDataAccess.XtreamConectionString conInfo = new XtreamDataAccess.XtreamConectionString();
                    System.Data.SqlClient.SqlConnectionStringBuilder builder = new System.Data.SqlClient.SqlConnectionStringBuilder(conInfo.getConnetctionString);
                    // Retrieve the DataSource property.    
                    string serverName = builder.DataSource;
                    string databaseName = builder.InitialCatalog;
                    string userName = builder.UserID;
                    string password = builder.Password;

                    ReportDocument reportDoc = new ReportDocument();
                    reportDoc.Load(fileFullPath);

                    CrystalDecisions.CrystalReports.Engine.Database crDatabase;
                    CrystalDecisions.CrystalReports.Engine.Tables crTables;
                    TableLogOnInfo crTableLogOnInfo;
                    CrystalDecisions.Shared.ConnectionInfo crConnectionInfo;

                    crConnectionInfo = new CrystalDecisions.Shared.ConnectionInfo();
                    crConnectionInfo.ServerName = serverName;
                    crConnectionInfo.DatabaseName = databaseName;
                    crConnectionInfo.UserID = userName;
                    crConnectionInfo.Password = password;

                    crDatabase = reportDoc.Database;
                    crTables = crDatabase.Tables;
                    foreach (CrystalDecisions.CrystalReports.Engine.Table crTable in crTables)
                    {                    
                        crTableLogOnInfo = crTable.LogOnInfo;
                        crTableLogOnInfo.ConnectionInfo = crConnectionInfo;
                        crTable.ApplyLogOnInfo(crTableLogOnInfo);
                    }
                    reportViewer.ViewerCore.ReportSource = reportDoc;
                }
                else
                {
                    MessageBox.Show("File path or name has been changed");
                }  
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                MessageBox.Show("System error has been occurred .Please check log file!", StaticProperty.ClientName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }
        }
    }
}
