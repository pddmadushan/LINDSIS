using System;
using System.Data;
using System.Data.SqlClient;


namespace XtreamDataAccess
{
    public class SMSDA : XtreamConectionString
    {
        public DataSet Option01Student(Int32 CLS_FILTER_ID, string CLS_FILTER_TEXT)
        {
            object[] parameterValues = { CLS_FILTER_ID, CLS_FILTER_TEXT };
            return SqlHelper.ExecuteDataset(getConnetctionString, "SEARCH_OPT_01_SMS", parameterValues);
        }

        public DataSet Option02Student(DateTime CLS_FROM, DateTime CLS_TO)
        {
            object[] parameterValues = { CLS_FROM, CLS_TO };
            return SqlHelper.ExecuteDataset(getConnetctionString, "SEARCH_OPT_02_SMS", parameterValues);
        }

        public DataSet Option03Student(Int32 CAT_ID, Int32 CLS_WEEK, Int32 STD_ACTIVE_FLG)
        {
            object[] parameterValues = { CAT_ID, CLS_WEEK, STD_ACTIVE_FLG };
            return SqlHelper.ExecuteDataset(getConnetctionString, "SEARCH_OPT_03_SMS", parameterValues);
        }

        public DataSet Option04Student(Int32 CLS_ID, DateTime CLS_REC_DATE, Int32 CLS_STATUS)
        {
            object[] parameterValues = { CLS_ID, CLS_REC_DATE, CLS_STATUS };
            return SqlHelper.ExecuteDataset(getConnetctionString, "SEARCH_OPT_04_SMS", parameterValues);
        }

        public DataSet Option05Student(Int32 RSN_ID, Int32 STD_INACTIVE_FLG)
        {
            object[] parameterValues = { RSN_ID, STD_INACTIVE_FLG };
            return SqlHelper.ExecuteDataset(getConnetctionString, "SEARCH_OPT_05_SMS", parameterValues);
        }

        public DataSet Option06Student(Int32 CLS_ID, Int32 SUB_CLS_ID)
        {
            object[] parameterValues = { CLS_ID, SUB_CLS_ID };
            return SqlHelper.ExecuteDataset(getConnetctionString, "SEARCH_OPT_06_SMS", parameterValues);
        }

        public DataSet Option07Student(Int32 CLS_ID, DateTime CLS_REC_DATE)
        {
            object[] parameterValues = { CLS_ID, CLS_REC_DATE };
            return SqlHelper.ExecuteDataset(getConnetctionString, "SEARCH_OPT_07_SMS", parameterValues);
        }

        public DataSet Option08Student()
        {
            object[] parameterValues = null;
            return SqlHelper.ExecuteDataset(getConnetctionString, "SEARCH_OPT_08_SMS", parameterValues);
        }

        public DataSet Option09Student(Int32 CLS_ID, Int32 ACC_ID,Int32 MINIMUM_PAY)
        {
            object[] parameterValues = { CLS_ID, ACC_ID, MINIMUM_PAY };
            return SqlHelper.ExecuteDataset(getConnetctionString, "SEARCH_OPT_09_SMS", parameterValues);
        }

        public DataSet Option10Student(Int32 MINIMUM_PAY)
        {
            object[] parameterValues = { MINIMUM_PAY };
            return SqlHelper.ExecuteDataset(getConnetctionString, "SEARCH_OPT_10_SMS", parameterValues);
        }


        public DataSet SelectClassFromAttendance(DateTime CLS_REC_DATE)
        {
            object[] parameterValues = { CLS_REC_DATE };
            return SqlHelper.ExecuteDataset(getConnetctionString, "SELECT_CLASSES_FROM_ATTENDANCE", parameterValues);
        }

        public DataTable SMSRefresh(DataTable SMSStudentList)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(getConnetctionString))
                {
                    cn.Open();
                    try
                    {
                        SqlCommand cmdBulk = new SqlCommand("SMS_STUDENT_REFRESH",cn);
                        cmdBulk.CommandType = CommandType.StoredProcedure;
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@SMSSTDList", SMSStudentList));
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
        public DataTable SMSRefreshbulk(DataTable SMSStudentList)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(getConnetctionString))
                {
                    cn.Open();
                    try
                    {
                        SqlCommand cmdBulk = new SqlCommand("SMS_STUDENT_REFRESH_BULK", cn);
                        cmdBulk.CommandType = CommandType.StoredProcedure;
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@SMSSTDListBulk", SMSStudentList));
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

        public void SMSList(Int32 isGatway, DataTable SMSContactList)
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
                        cmdBulk.CommandText = "INSERT_SMS_STUDENT";
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@IsSMSGateway", isGatway));
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@SMSContactList", SMSContactList));
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

        public Int32 DeletePendingSMS()
        {
            try
            {
                object[] parameterValues = null;
                return SqlHelper.ExecuteNonQuery(getConnetctionString, "DELETE_PENDINGSMS", parameterValues);
            }
            catch
            {
                throw;
            }
        }
        public Int32 DeletePendingStudentSMS(Int32 INBOX_ID, Int32 STD_ID)
        {
            try
            {
                object[] parameterValues = {INBOX_ID, STD_ID};
                return SqlHelper.ExecuteNonQuery(getConnetctionString, "DELETE_STUDENTPENDINGSMS", parameterValues);
            }
            catch
            {
                throw;
            }
        }

        public DataTable SMSAdvanceFilter(DataTable SMSGROUPList, DataTable SMSLABELList, DataTable SMSSTDList , DataTable SMSLABELListN)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(getConnetctionString))
                {
                    cn.Open();
                    try
                    {
                        SqlCommand cmdBulk = new SqlCommand("SMS_STUDENT_ADVANCE", cn);
                        cmdBulk.CommandType = CommandType.StoredProcedure;
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@SMSGROUPList", SMSGROUPList));
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@SMSLABELList", SMSLABELList));
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@SMSSTDList", SMSSTDList));
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@SMSLABELListN", SMSLABELListN));
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

        public void BulkLableOparation(Int32 lableId,DataTable SMSStudentList,Boolean isdelete)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(getConnetctionString))
                {
                    cn.Open();
                    try
                    {
                        SqlCommand cmdBulk ;
                        if (isdelete)
                        {
                            cmdBulk = new SqlCommand("DELETE_BULK_LABLE", cn);
                        }
                        else
                        {
                            cmdBulk = new SqlCommand("INSERT_BULK_LABLE", cn);
                        }
                        cmdBulk.CommandType = CommandType.StoredProcedure;
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@LBL_ID", lableId));
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@SMSSTDList", SMSStudentList));
                        cmdBulk.ExecuteNonQuery();
                        cmdBulk.Dispose();
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
