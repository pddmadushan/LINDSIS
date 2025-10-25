using System.Data.SqlClient;
using System.Data;
using System;

namespace XtreamDataAccess
{
    public class MannualAddLeadsDA: XtreamConectionString
    {
        public DataTable ManualLeadInsert(DataTable MANUAL_LEAD_TABLE,int cmpId,int userId,string loggedUser,DateTime? adDate,string adName)
        {
            DataTable dataTable = new DataTable();
            try
            {
                using (SqlConnection cn = new SqlConnection(getConnetctionString))
                {
                    cn.Open();

                    SqlTransaction trn = cn.BeginTransaction(IsolationLevel.Snapshot);
                    try
                    {
                        SqlCommand cmdBulk = new SqlCommand();
                        cmdBulk.CommandType = CommandType.StoredProcedure;
                        cmdBulk.CommandText = "MANUAL_LEAD_ENTER";
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@MANUAL_LEAD_TABLE", MANUAL_LEAD_TABLE));
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@CMP_ID", cmpId));
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@USER_ID", userId));
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@LOGGED_USER", loggedUser));
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@AD_DATE", adDate));
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@AD_NAME", adName));
                        cmdBulk.Connection = trn.Connection;
                        cmdBulk.Transaction = trn;

                        using (SqlDataAdapter dataAdapter = new SqlDataAdapter(cmdBulk))
                        {
                            dataAdapter.Fill(dataTable);
                        }
                        cmdBulk.Dispose();
                        trn.Commit();
                        trn.Dispose();
                    }
                    catch
                    {
                        trn.Rollback();
                        throw;
                    }
                    finally
                    {
                        trn.Dispose();                        
                    }
                }
                return dataTable;
            }
            catch
            {
                throw;
            }
        }
    }
}
