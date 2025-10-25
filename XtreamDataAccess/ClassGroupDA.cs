using System;
using System.Data;
using System.Data.SqlClient;

namespace XtreamDataAccess
{
    public class ClassGroupAttribute
    {
        public Int32 CLS_ID { get; set; }
        public string CLS_NAME { get; set; }
        public int CLS_ACTIVE_FLG { get; set; }
        public double CLS_FEE { get; set; }
        public double CLS_ADMITION_AMT { get; set; }
        public int CLS_DAY { get; set; }
        public double CLS_TIME { get; set; }
        public string CLS_COMMENT { get; set; }
        public DateTime CLS_START_DATE { get; set; }
        public double CLS_DURATION { get; set; }
        public int CAT_ID { get; set; }
        public int IS_CLASS_FLG { get; set; }
        public Int32 TOTAL_NUMBER_OF_WEEK { get; set; }
        public double COUSE_FEE { get; set; }
        public int SHOW_IN_WEB_FLG { get; set; }
        public int REGISTRATION_IN_WEB_FLG { get; set; }
    }

    public class ClassGroupDA : XtreamConectionString
    {
        public ClassGroupAttribute classFeilds;

        public ClassGroupDA()
        {
            classFeilds = new ClassGroupAttribute();
        }

        public DataSet SelectAllClass()
        {
            object[] parameterValues = null;
            return SqlHelper.ExecuteDataset(getConnetctionString, "SELECT_CLASSGROUP_ALL", parameterValues);
        }
        public DataSet InsertClass(Int32 UPDATE_STUDENT_FEE)
        {
            object[] parameterValues = { classFeilds.CLS_ID, classFeilds.CLS_NAME, classFeilds.CLS_ACTIVE_FLG, classFeilds.CLS_FEE, classFeilds.CLS_ADMITION_AMT,
                classFeilds.CLS_DAY, classFeilds.CLS_TIME, classFeilds.CLS_COMMENT, classFeilds.CLS_START_DATE, classFeilds.CLS_DURATION, classFeilds.CAT_ID, classFeilds.IS_CLASS_FLG,
                classFeilds.TOTAL_NUMBER_OF_WEEK, UPDATE_STUDENT_FEE, classFeilds.COUSE_FEE, classFeilds.SHOW_IN_WEB_FLG , classFeilds.REGISTRATION_IN_WEB_FLG};
            return SqlHelper.ExecuteDataset(getConnetctionString, "SAVE_CLASS_GROUP", parameterValues);
        }
        public Int32 DeleteClass()
        {
            try
            {
                object[] parameterValues = { classFeilds.CLS_ID };
                return SqlHelper.ExecuteNonQuery(getConnetctionString, "DELETE_CLASS_GROUP", parameterValues);
            }
            catch
            {
                throw;
            }
        }
        public DataSet TransferStudent(Int32 TRANSFER_CLS_ID)
        {
            object[] parameterValues = { classFeilds.CLS_ID, TRANSFER_CLS_ID };
            return SqlHelper.ExecuteDataset(getConnetctionString, "TRASFER_STUDENT", parameterValues);
        }
        public DataSet SelectSubClass()
        {
            object[] parameterValues =  { classFeilds.CLS_ID };
            return SqlHelper.ExecuteDataset(getConnetctionString, "SELECT_INTAKE_CLASSES", parameterValues);
        }

        public void InsertSubClass(DataTable SubClassList)
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
                        cmdBulk.CommandText = "UPDATE_SUB_CLASS";
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@CLS_ID", classFeilds.CLS_ID));
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@SClassList", SubClassList));
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

        public DataSet SelectAllIntake()
        {
            object[] parameterValues = null;
            return SqlHelper.ExecuteDataset(getConnetctionString, "SELECT_CLASSGROUP_INTAKE", parameterValues);
        }

        public DataSet SelectAllSubClass()
        {
            object[] parameterValues = {classFeilds.CLS_ID};
            return SqlHelper.ExecuteDataset(getConnetctionString, "SELECT_SUB_CLASS", parameterValues);
        }

    }
}
