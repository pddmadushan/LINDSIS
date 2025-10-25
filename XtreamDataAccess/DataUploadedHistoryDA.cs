using System;
using System.Data;
using System.Data.SqlClient;

namespace XtreamDataAccess
{
    public class DataUploadedHistoryDA:XtreamConectionString
    {
        public DataSet GetHistorySummery(int cmpId,DateTime? fromDate,DateTime? toDate)
        {
            object[] parameterValues = { cmpId , fromDate , toDate };
            return SqlHelper.ExecuteDataset(getConnetctionString, "UPLOADED_HISTORY_SUMMERY", parameterValues);
        }

        public void DeleteUloadedHistory(int addId, string user)
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
                        cmdBulk.CommandText = "DELETE_UPLOAD_DATA";
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@ADD_ID", addId));
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@USER", user));
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

        public DataSet SelectAllCampaing()
        {
            object[] parameterValues = null;
            return SqlHelper.ExecuteDataset(getConnetctionString, "LOAD_CAMPAIGN_TO_HISTORY_PAGE", parameterValues);
        }
    }
}
