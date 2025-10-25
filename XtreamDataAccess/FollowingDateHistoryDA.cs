using System;
using System.Data;

namespace XtreamDataAccess
{
    public class FollowingDateHistoryDA: XtreamConectionString
    {
        public DataSet SearchFollowingDateHis(int user,DateTime fromDate,DateTime toDate)
        {
            object[] parameterValues = {user, fromDate,toDate};
            return SqlHelper.ExecuteDataset(getConnetctionString, "SP_SEARCH_FOLLOWING_DATE_HISTORY", parameterValues);
        }
    }
}
