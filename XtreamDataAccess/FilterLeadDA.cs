using System.Data.SqlClient;
using System.Data;
using System;

namespace XtreamDataAccess
{
    public class FilterLeadDA:XtreamConectionString
    {
        public DataSet GetCampaigsToChkBox()
        {
            try
            {
                object[] parameterValues = { };
                return SqlHelper.ExecuteDataset(getConnetctionString, "LOAD_CAMPAIGN_TO_ASSIGN_STD", parameterValues);
            }
            catch
            {
                throw;
            }
        }

        public DataTable LeadFilter(DataTable CmpLIst, DataTable LblList)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(getConnetctionString))
                {
                    cn.Open();
                    try
                    {
                        SqlCommand cmdBulk = new SqlCommand("LOAD_FILTER_LEADS_DATA", cn);
                        cmdBulk.CommandType = CommandType.StoredProcedure;
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@SelectedCampaing", CmpLIst));
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@SelectedLableList", LblList));
                        cmdBulk.CommandTimeout = 300;
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

        public DataTable GetSummary(DataTable CmpLIst, DataTable dtLable, string User,DateTime FromDate,DateTime ToDate)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(getConnetctionString))
                {
                    cn.Open();
                    try
                    {
                        SqlCommand cmdBulk = new SqlCommand("LOAD_LEADS_SUMMARY", cn);
                        cmdBulk.CommandType = CommandType.StoredProcedure;
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@SelectedCampaing", CmpLIst));
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@SelectedLableList", dtLable));
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@UserId", User));
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@FromDate", FromDate));
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@ToDate", ToDate));
                        cmdBulk.CommandTimeout = 300;                        
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
