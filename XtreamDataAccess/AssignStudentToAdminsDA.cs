using System;
using System.Data;
using System.Data.SqlClient;


namespace XtreamDataAccess
{
    public class AssignStudentToAdminsDA : XtreamConectionString
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
        public DataSet GetAdminUsers()
        {
            object[] parameterValues = { };
            return SqlHelper.ExecuteDataset(getConnetctionString, "LOAD_USERS_DROPDOWN", parameterValues);
        }

        public DataSet GetStdDataToGrid()
        {
            object[] parameterValues = { };
            return SqlHelper.ExecuteDataset(getConnetctionString, "LOAD_STUDENT_DATA", parameterValues);
        }

        public DataTable GetSelectedCampainStudentData(DataTable cmptable)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(getConnetctionString))
                {
                    cn.Open();
                    try
                    {
                        SqlCommand cmdBulk = new SqlCommand("LOAD_STUDENT_DATA", cn);
                        cmdBulk.CommandType = CommandType.StoredProcedure;
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@SelectedCampaing", cmptable));
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

        public void InserDataToStdUserTable(DataTable selectedStd, int ID, string LgUser)
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
                        cmdBulk.CommandText = "ASIGN_STD_TO_USER";
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@UnassignStudents", selectedStd));
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@USER_ID", ID));
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@LG_USER", LgUser));
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
        public DataTable GetUsersStudent(DataTable cmptable, int user_id,DateTime? fromDate,DateTime? toDate)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(getConnetctionString))
                {
                    cn.Open();
                    try
                    {
                        SqlCommand cmdBulk = new SqlCommand("GetUsersStudent", cn);
                        cmdBulk.CommandType = CommandType.StoredProcedure;
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@SelectedCampaing", cmptable));
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@CLS_USER_ID", user_id));
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@FROMDATE", fromDate));
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@TODATE", toDate));
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

        public void UnassignStudentFromStdUserTable(DataTable uncheckesStd,string dataLoadedUser,string loggedUser)
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
                        cmdBulk.CommandText = "UnAssignStudentFromUsers";
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@studentList", uncheckesStd));
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@USER", dataLoadedUser));
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@LOGGED_USER", loggedUser));
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

        public void SingleStudentAssignToUser(int SELECTED_STD, int USER_ID,string LgUser)
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
                        cmdBulk.CommandText = "SINGLE_STUDENT_ASSIGN_TO_USER";
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@SELECTED_STD", SELECTED_STD));
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@USER_ID", USER_ID));
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@LG_USER", LgUser));
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
        public void SingleStudentUnAssignFromUser(int SELECTED_STD, int USER_ID, string LoggedUser)
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
                        cmdBulk.CommandText = "SINGLE_STUDENT_UNASSIGN_FROM_USER";
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@SELECTED_STD", SELECTED_STD));
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@USER_ID", USER_ID));
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@LOGGED_USER", LoggedUser));
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

        public int CheckUserType(string CLS_USER_ID)
        {
            try
            {
                object[] parameterValues = { CLS_USER_ID };
                object result = SqlHelper.ExecuteScalar(getConnetctionString, "GET_USER_TYPE", parameterValues);
                return Convert.ToInt32(result);
            }
            catch
            {
                throw;
            }
        }

        public DataTable GetMyStudents(DataTable cmptable, string user_name)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(getConnetctionString))
                {
                    cn.Open();
                    try
                    {
                        SqlCommand cmdBulk = new SqlCommand("GetMyStudents", cn);
                        cmdBulk.CommandType = CommandType.StoredProcedure;
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@SelectedCampaing", cmptable));
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@Username", user_name));
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

        public int AddMyLeadAsStudent(int cmpStdId, string stdConvertedUserId)
        {
            object[] parameterValues = { cmpStdId, stdConvertedUserId };
            object RESULT = SqlHelper.ExecuteScalar(getConnetctionString, "ADD_MY_ASSIGNEE_AS_STUDENT", parameterValues);

            return Convert.ToInt32(RESULT);
        }
        
        public void CHANGE_LEADS_USER(DataTable stdList, int ToUserId, string LoggedUser, string DataLoadedUer)
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
                        cmdBulk.CommandText = "CHANGE_LEADS_USER";
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@studentList", stdList));
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@ToUser", ToUserId));
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@LoggedUser", LoggedUser));
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@DataLoadedUer", DataLoadedUer));
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
        public void DeleteBulkLeads(DataTable tblCmpStdId)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(getConnetctionString))
                {
                    cn.Open();
                    try
                    {
                        SqlCommand cmdBulk = new SqlCommand("SP_DELETE_BULK_LEAD", cn);
                        cmdBulk.CommandType = CommandType.StoredProcedure;
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@DELETED_LEAD_BULK", tblCmpStdId));
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
