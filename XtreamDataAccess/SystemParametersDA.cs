using System;
using System.Data;

namespace XtreamDataAccess
{
    public class SystemParametersDA:XtreamConectionString
    {
        public DataSet GetParametersToGrid()
        {
            object[] parameterValues = { };
            return SqlHelper.ExecuteDataset(getConnetctionString, "LOAD_PARAMETERS", parameterValues);
        }

        public void UpadateParaValues(string paraId,string paraString,string paraStringUnicode)
        {
            object[] parameterValues = { paraId, paraString , paraStringUnicode};
            SqlHelper.ExecuteNonQuery(getConnetctionString, "SAVE_PARA_VALUES", parameterValues);
        }

        public DataSet FinalizeIncentive()
        {
            object[] parameterValues = { };
            return SqlHelper.ExecuteDataset(getConnetctionString, "FINALIZED_INCENTIVE", parameterValues);
        }
    }
}
