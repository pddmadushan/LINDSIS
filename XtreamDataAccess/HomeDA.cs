using System.Data;

namespace XtreamDataAccess
{
    public class HomeDA : XtreamConectionString
    {
        public DataSet SelectClassSchedule()
        {
            object[] parameterValues = null;
            return SqlHelper.ExecuteDataset(getConnetctionString, "SELECT_CLASSSHEDULE", parameterValues);
        }
    }
}
