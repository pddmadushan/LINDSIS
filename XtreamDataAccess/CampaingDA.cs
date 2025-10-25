using System;
using System.Data;


namespace XtreamDataAccess
{
    public class CampaingDA:XtreamConectionString
    {
        public int CMP_ID { get; set; }
        public string CMP_DES { get; set; }

        public int CMP_ACTIVE { get; set; }

        public DataSet SelectAllCampaing()
        {
            object[] parameterValues = null;
            return SqlHelper.ExecuteDataset(getConnetctionString, "SelectAllCampaing", parameterValues);
        }

        public Int32 DeleteCMP()
        {
            try
            {
                object[] parameterValues = { CMP_ID };
                return SqlHelper.ExecuteNonQuery(getConnetctionString, "DELETE_CAMPAING", parameterValues);
            }
            catch
            {
                throw;
            }
        }

        public Int32 Savedata() {
            try
            {
                object[] parameterValues = { CMP_ID, CMP_DES, CMP_ACTIVE };
                return SqlHelper.ExecuteNonQuery(getConnetctionString, "SAVE_CAMPAING", parameterValues);
            }
            catch
            {
                throw;
            }
        }
    }
}
