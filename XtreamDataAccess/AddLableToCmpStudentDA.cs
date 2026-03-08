using System;
using System.Data;
using System.Data.SqlClient;

namespace XtreamDataAccess
{
    public class AddLableToCmpStudentDA:XtreamConectionString
    {
        public int LBL_ID {  get; set; }
        public string LBL_DES {  get; set; }    
        public DataSet SelectAllLablesAndStudentLable(int CMP_STD_ID)
        {
            object[] parameterValues = { CMP_STD_ID };
            return SqlHelper.ExecuteDataset(getConnetctionString, "SelectAllLablesAndStudentLable", parameterValues);
        }

        public void SaveLeadsLable(DataTable selectedLable, int cmpStd_id,string loggedUser,int clsId, DateTime? attDate,int noMoreAtt,int ProrityValue, int UserID)
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
                        cmdBulk.CommandText = "SAVE_CMP_STUDENT_LBL";
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@CMP_STD_ID", cmpStd_id));
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@CHECKED_LBL_TBL", selectedLable));
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@EDITED_USER", loggedUser));
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@ATT_DATE", attDate));
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@CLS_ID", clsId));
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@noMoreAtt", noMoreAtt));
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@prorityValue", ProrityValue));
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@userID", UserID));
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

        public Int32 DeleteLable()
        {
            try
            {
                object[] parameterValues = { LBL_ID };
                return SqlHelper.ExecuteNonQuery(getConnetctionString, "DELETE_LABLE_NW", parameterValues);
            }
            catch
            {
                throw;
            }
        }

        public Int32 SaveLable()
        {
            try
            {
                object[] parameterValues = { LBL_ID, LBL_DES };
                return SqlHelper.ExecuteNonQuery(getConnetctionString, "SAVE_LABLE", parameterValues);
            }
            catch
            {
                throw;
            }
        }

        public DataSet GetClassesToComboAndAttDate(int cmpStdId)
        {
            try
            {
                object[] parameterValues = { cmpStdId };
                return SqlHelper.ExecuteDataset(getConnetctionString, "LOAD_CLASSES_AND_ATTENTAND_DATE", parameterValues);
            }
            catch
            {
                throw;
            }
        }

        public DataSet GetFristMessage(String Telephone)
        {
            try
            {
                object[] parameterValues = { Telephone };
                return SqlHelper.ExecuteDataset(getConnetctionString, "GetMessageByTelephone", parameterValues);
            }
            catch
            {
                throw;
            }
        }

    }
}
