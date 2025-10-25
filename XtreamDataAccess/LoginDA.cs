using System;
using System.Data;
using System.Text;


namespace XtreamDataAccess
{
    public class LoginDA : XtreamConectionString
    {
        public Int32 ID { get; set; }
        public string CLS_USER_ID { get; set; }

        private string UserPassword; // This is the backing field
        public string CLS_USER_PASSWORD
        {
            get { return GenarateXtreamHash(UserPassword); }
            set { UserPassword = value; }
        }
        public Int32 CLS_USER_ACTIVE { get; set; }
        public Int32 CLS_USER_ADMIN { get; set; }
        public Double INCENTIVE_PTG { get; set; }

        public DataSet GetUserDetails()
        {
            try
            {
                object[] parameterValues = { CLS_USER_ID };
                return SqlHelper.ExecuteDataset(getConnetctionString, "SYS_LOGIN_AUTONTYCATION", parameterValues);
            }
            catch
            {
                throw;
            }
        }

        public static string getUserName(int UserID)
        {
            string userName = "";
            try
            {
                LoginDA _clsLogin = new LoginDA();
                _clsLogin.ID = UserID;
                System.Data.DataTable table = _clsLogin.GetUserDetailsByID().Tables[0];

                if (table.Rows.Count > 0)
                {
                    userName = table.Rows[0]["CLS_USER_ID"].ToString();
                }
            }
            catch (Exception ex)
            {
            }
            return userName;
        }

        private DataSet GetUserDetailsByID()
        {
            try
            {
                object[] parameterValues = { ID };
                return SqlHelper.ExecuteDataset(getConnetctionString, "SELECT_USER_FROM_ID", parameterValues);
            }
            catch
            {
                throw;
            }
        }

        public DataSet SelectUserAccount()
        {
            try
            {
                object[] parameterValues = null;
                return SqlHelper.ExecuteDataset(getConnetctionString, "SELECT_USER_ACCOUNT", parameterValues);
            }
            catch
            {
                throw;
            }
        }

        public DataSet SaveUser()
        {
            try
            {
                object[] parameterValues = { CLS_USER_ID, CLS_USER_PASSWORD, INCENTIVE_PTG };
                return SqlHelper.ExecuteDataset(getConnetctionString, "SAVE_USER_ACCOUNT", parameterValues);
            }
            catch
            {
                throw;
            }
        }

        public void UpdateActiveStatus()
        {
            try
            {
                object[] parameterValues = { CLS_USER_ID, CLS_USER_ACTIVE };
                SqlHelper.ExecuteNonQuery(getConnetctionString, "UPDATE_USER_ACTIVE_LOCK", parameterValues);
            }
            catch
            {
                throw;
            }
        }

        public void UpdatePassword()
        {
            try
            {
                object[] parameterValues = { CLS_USER_ID, CLS_USER_PASSWORD, INCENTIVE_PTG };
                SqlHelper.ExecuteNonQuery(getConnetctionString, "UPDATE_USER_PASSWORD", parameterValues);
            }
            catch
            {
                throw;
            }
        }

        public void UpdatePasswordSelf()
        {
            try
            {
                object[] parameterValues = { CLS_USER_ID, CLS_USER_PASSWORD };
                SqlHelper.ExecuteNonQuery(getConnetctionString, "UPDATE_USER_PASSWORD_SELF", parameterValues);
            }
            catch
            {
                throw;
            }
        }

        public string GenarateXtreamHash(string password)
        {
            string md5data = Hash(password);
            md5data = Hash(string.Format("$LINDSIS$V1${0}@XTreamSoft", md5data));
            return md5data;
        }

        private string Hash(string password)
        {
            var bytes = new UTF8Encoding().GetBytes(password);
            var hashBytes = System.Security.Cryptography.MD5.Create().ComputeHash(bytes);
            return Convert.ToBase64String(hashBytes);
        }

        public void UpdateAdminStatus()
        {
            try
            {
                object[] parameterValues = { CLS_USER_ID, CLS_USER_ADMIN };
                SqlHelper.ExecuteNonQuery(getConnetctionString, "UPDATE_USER_ACTIVE_ADMIN", parameterValues);
            }
            catch
            {
                throw;
            }
        }

    }
}
