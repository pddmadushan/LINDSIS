using System.Data;


namespace XtreamDataAccess
{
    public class Excel_upload: XtreamConectionString
    {
        

        public DataSet Uploadtemptable(string STD_FULL_NAME, string PHONE_NO)
        {
            object[] parameterValues = { STD_FULL_NAME, PHONE_NO };
            return SqlHelper.ExecuteDataset(getConnetctionString, "SP_UPLOAD_TEMP_TABLE", parameterValues);
        }

        public DataSet Upload_STDtable()
        {
            try
            {
                return SqlHelper.ExecuteDataset(getConnetctionString, "EXCEL_UPLOAD");
               

            }
            catch 
            {
                 throw;
            }
        }
    }
}
