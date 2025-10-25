using System;
using System.Data;
using System.Data.SqlClient;

namespace XtreamDataAccess
{
    public class DBSearchDA : XtreamConectionString
    {
        public DataSet LoadCampaignsAndAddNamesAndUsers()
        {
            object[] parameterValues = null;
            return SqlHelper.ExecuteDataset(getConnetctionString, "LOAD_AddNames_AND_Campaings", parameterValues);
        }

        public DataSet Search( int USER, DateTime? FILTER_BY_DATE_FROM, DateTime? FILTER_BY_DATE_TO,
            int FILTER_BY_NAME_OR_TELEPHONE, int FILTER_BY_CMP_OR_ADD, int? CMP_ID, string ADD_NAME, string FILTER_TEXT, int isAdmin, int isDatefiltered, DateTime? FILTER_BY_ATT_DATE_FROM, DateTime? FILTER_BY_ATT_DATE_TO, int isAttDatefiltered,int prorityLevel)
        {
            object[] parameterValues = { USER, FILTER_BY_DATE_FROM,FILTER_BY_DATE_TO,
             FILTER_BY_NAME_OR_TELEPHONE,  FILTER_BY_CMP_OR_ADD, CMP_ID, ADD_NAME, FILTER_TEXT,isAdmin ,isDatefiltered,FILTER_BY_ATT_DATE_FROM, FILTER_BY_ATT_DATE_TO,isAttDatefiltered,prorityLevel};

            return SqlHelper.ExecuteDataset(getConnetctionString, "SEARCH", parameterValues);
        }

        public void CHANGE_LEADS_USER_SelRow(int cmpStdId, int ToUserId, string assignedUser, string loggedUser)
        {
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
                        cmdBulk.CommandText = "CHANGE_LEADS_USER_SelRow";
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@CMP_STD_ID", cmpStdId));
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@ToUser", ToUserId));
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@DataLoadedUer", assignedUser));
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@LoggedUser", loggedUser));
                        cmdBulk.Connection = trn.Connection;
                        cmdBulk.Transaction = trn;
                        cmdBulk.ExecuteNonQuery();
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
            }
            catch
            {
                throw;
            }
        }
    }
}
