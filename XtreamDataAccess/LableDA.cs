using System;
using System.Data;

namespace XtreamDataAccess
{
    public class LableDA : XtreamConectionString
    {
        public int LBL_ID { get; set; }
        public string LBL_DES { get; set; }

        public DataSet SelectLable()
        {
            object[] parameterValues = null;
            return SqlHelper.ExecuteDataset(getConnetctionString, "SELECT_LABLE", parameterValues);
        }

        public DataSet SelectStudentLableAll(int STD_ID)
        {
            object[] parameterValues = { STD_ID };
            return SqlHelper.ExecuteDataset(getConnetctionString, "SELECT_STUDENT_LABLE_ALL", parameterValues);
        }

        public DataSet SelectStudentLable(int STD_ID)
        {
            object[] parameterValues = { STD_ID };
            return SqlHelper.ExecuteDataset(getConnetctionString, "SELECT_STUDENT_LABLE", parameterValues);
        }

        public Int32 UpdateLable(int STD_ID,int LBL_ENABLE)
        {
            try
            {
                object[] parameterValues = { STD_ID, LBL_ID, LBL_ENABLE };
                return SqlHelper.ExecuteNonQuery(getConnetctionString, "UPDATE_STUDENT_LABLE", parameterValues);
            }
            catch
            {
                throw;
            }
        }

        public Int32 ResetPassword(int STD_ID)
        {
            try
            {
                object[] parameterValues = { STD_ID };
                return SqlHelper.ExecuteNonQuery(getConnetctionString, "RESET_STUDENT_PASSWORD", parameterValues);
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

        public Int32 DeleteLable()
        {
            try
            {
                object[] parameterValues = { LBL_ID };
                return SqlHelper.ExecuteNonQuery(getConnetctionString, "DELETE_LABLE", parameterValues);
            }
            catch
            {
                throw;
            }
        }
    }
}
