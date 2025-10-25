using System.Data;
using System.Data.SqlClient;

namespace XtreamDataAccess
{
    public class LeadTranferUserSelectDA : XtreamConectionString
    {
        public DataTable ViewLeadTranferHistory(int? CMP_STD_ID, DataTable tblLeds)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(getConnetctionString))
                {
                    cn.Open();
                    try
                    {
                        SqlCommand cmdBulk = new SqlCommand("VIEW_LEAD_CHANGED_HISTORY", cn);
                        cmdBulk.CommandType = CommandType.StoredProcedure;
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@CMP_STD_ID", CMP_STD_ID));
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@studentList", tblLeds));
                        SqlDataAdapter adp = new SqlDataAdapter(cmdBulk);
                        DataSet ds = new DataSet();
                        adp.Fill(ds);
                        return ds.Tables[0];
                    }
                    catch
                    {
                        throw;
                    }
                    finally
                    {
                        cn.Close();
                        cn.Dispose();
                    }
                }
            }
            catch
            {
                throw;
            }
        }

    }
}
