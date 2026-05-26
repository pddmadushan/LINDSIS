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

        public DataSet Search1(DataTable dtLable, int USER, DateTime? FILTER_BY_DATE_FROM, DateTime? FILTER_BY_DATE_TO,
            int FILTER_BY_NAME_OR_TELEPHONE, int FILTER_BY_CMP_OR_ADD, int? CMP_ID, string ADD_NAME, string FILTER_TEXT, int isAdmin, 
            int isDatefiltered, DateTime? FILTER_BY_ATT_DATE_FROM, DateTime? FILTER_BY_ATT_DATE_TO, int isAttDatefiltered,int prorityLevel, int SelectStdOnly)
        {
            object[] parameterValues = { USER, FILTER_BY_DATE_FROM,FILTER_BY_DATE_TO,
             FILTER_BY_NAME_OR_TELEPHONE,  FILTER_BY_CMP_OR_ADD, CMP_ID, ADD_NAME, FILTER_TEXT,isAdmin ,isDatefiltered,FILTER_BY_ATT_DATE_FROM, FILTER_BY_ATT_DATE_TO,isAttDatefiltered,prorityLevel};

            return SqlHelper.ExecuteDataset(getConnetctionString, "SEARCH", parameterValues);
        }

        public DataSet Search(DataTable dtLable, int USER, DateTime? FILTER_BY_DATE_FROM, DateTime? FILTER_BY_DATE_TO,
        int FILTER_BY_NAME_OR_TELEPHONE, int FILTER_BY_CMP_OR_ADD, int? CMP_ID, string ADD_NAME, string FILTER_TEXT,
        int isAdmin, int isDatefiltered, DateTime? FILTER_BY_ATT_DATE_FROM, DateTime? FILTER_BY_ATT_DATE_TO,
        int isAttDatefiltered, int prorityLevel, int SelectStdOnly)
        {
            DataSet ds = new DataSet();

            using (SqlConnection cn = new SqlConnection(getConnetctionString))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "SEARCH";
                cmd.Parameters.Add(new SqlParameter("@SelectedLableList", dtLable));
                cmd.Parameters.Add(new SqlParameter("@USER", USER));
                cmd.Parameters.Add(new SqlParameter("@FILTER_BY_DATE_FROM", FILTER_BY_DATE_FROM));
                cmd.Parameters.Add(new SqlParameter("@FILTER_BY_DATE_TO", FILTER_BY_DATE_TO));
                cmd.Parameters.Add(new SqlParameter("@FILTER_BY_NAME_OR_TELEPHONE", FILTER_BY_NAME_OR_TELEPHONE));
                cmd.Parameters.Add(new SqlParameter("@FILTER_BY_CMP_OR_ADD", FILTER_BY_CMP_OR_ADD));
                cmd.Parameters.Add(new SqlParameter("@CMP_ID", CMP_ID));
                cmd.Parameters.Add(new SqlParameter("@ADD_NAME", ADD_NAME));
                cmd.Parameters.Add(new SqlParameter("@FILTER_TEXT", FILTER_TEXT));
                cmd.Parameters.Add(new SqlParameter("@isAdmin", isAdmin));
                cmd.Parameters.Add(new SqlParameter("@isDatefiltered", isDatefiltered));
                cmd.Parameters.Add(new SqlParameter("@FILTER_BY_ATT_DATE_FROM", FILTER_BY_ATT_DATE_FROM));
                cmd.Parameters.Add(new SqlParameter("@FILTER_BY_ATT_DATE_TO", FILTER_BY_ATT_DATE_TO));
                cmd.Parameters.Add(new SqlParameter("@isAttDatefiltered", isAttDatefiltered));
                cmd.Parameters.Add(new SqlParameter("@prorityLevel", prorityLevel));
                cmd.Parameters.Add(new SqlParameter("@SelectStdOnly", SelectStdOnly));
                cmd.Connection = cn;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                cmd.Dispose();
            }
            return ds;
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
