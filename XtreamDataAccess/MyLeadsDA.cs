using System;
using System.Data;
using System.Data.SqlClient;


namespace XtreamDataAccess
{
    public class MyLeadsDA : XtreamConectionString
    {
      public static  int? CMP_STD_ID;
        public DataSet GetMyLeads(int clsUserId, DataTable campaings, DataTable dtLable, DateTime FromDate, DateTime ToDate, Boolean filter, int chkBoxMode, int viewDeleteRecAdminPg)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(getConnetctionString))
                {
                    cn.Open();
                    try
                    {
                        SqlCommand cmdBulk = new SqlCommand("LOAD_MY_LEADS", cn);
                        cmdBulk.CommandType = CommandType.StoredProcedure;
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@SelectedCampaing", campaings));
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@SelectedLableList", dtLable));
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@CLS_USER_ID", clsUserId));
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@FROM_DATE", FromDate));
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@TO_DATE", ToDate));
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@FILTER", filter));
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@CHK_BOX_MODE", chkBoxMode));
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@CMP_STD_ID", CMP_STD_ID));//new line for search load lead
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@viewDeleteRecAdminPg", viewDeleteRecAdminPg));
                        cmdBulk.CommandTimeout = 300;
                        SqlDataAdapter adp = new SqlDataAdapter(cmdBulk);
                        DataSet ds = new DataSet();
                        adp.Fill(ds);
                        //return ds.Tables[0];
                        return ds;
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

        public int AddMyLeadAsStudent(int cmpStdId, string stdConvertedUserId)
        {
            object[] parameterValues = { cmpStdId, stdConvertedUserId };
            object RESULT = SqlHelper.ExecuteScalar(getConnetctionString, "ADD_MY_ASSIGNEE_AS_STUDENT", parameterValues);

            return Convert.ToInt32(RESULT);
        }

        public int SaveUserComment(int CMP_STD_ID, string COMMENT)
        {
            object[] parameterValues = { CMP_STD_ID, COMMENT };
            return SqlHelper.ExecuteNonQuery(getConnetctionString, "SAVE_USER_COMMENT", parameterValues);

        }
        public int SaveUserComment(int CMP_STD_ID, string COMMENT, string EDITED_USER , int CLS_ID ,DateTime? ATT_DATE )
        {
            object[] parameterValues = { CMP_STD_ID, COMMENT, EDITED_USER, CLS_ID, ATT_DATE };
            return SqlHelper.ExecuteNonQuery(getConnetctionString, "SAVE_USER_COMMENT_AUDIT", parameterValues);

        }

        public void SaveUNsavedComment(DataTable UnsavedData, string loggedUser)
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
                        cmdBulk.CommandText = "SAVE_UNSAVED_COMMENTS";
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@UNSAVED_COMMENT_TBL", UnsavedData));
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@EDITED_USER", loggedUser));
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

        public void DeleteLead(int cmpStdId)
        {
            object[] parameterValues = { cmpStdId };
            SqlHelper.ExecuteNonQuery(getConnetctionString, "DELETE_LEAD_FROM_MYLEAD", parameterValues);
        }

        public void RestoreDeletedLead(int cmpStdId)
        {
            object[] parameterValues = { cmpStdId };
            SqlHelper.ExecuteNonQuery(getConnetctionString, "RESTORE_DELETED_LEAD_FROM_MYLEAD", parameterValues);
        }
        public DataSet GetUsers()
        {
            object[] parameterValues = { };
            return SqlHelper.ExecuteDataset(getConnetctionString, "LOAD_USERS_DROPDOWN", parameterValues);
        }
    }
}
