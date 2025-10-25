using System;
using System.Data;

namespace XtreamDataAccess
{
    public class AccessoryAttribute
    {
        public Int32 ACC_ID { get; set; }
        public string ACC_NAME { get; set; }
        public double ACC_AMOUNT { get; set; }
        public int ACC_PAYBLE_FLG { get; set; }
        public string ACC_COMMENT { get; set; }
        public int ACC_MANDATORY { get; set; }
        public int ACC_DELIVERABLE_FLG { get; set; }
        public AccessoryAttribute() { }
        public AccessoryAttribute(Int32 id, string name, double amount, int payble, string comment, int diliverable)
        {
            this.ACC_ID = id;
            this.ACC_NAME = name;
            this.ACC_AMOUNT = amount;
            this.ACC_PAYBLE_FLG = payble;
            this.ACC_COMMENT = comment;
            this.ACC_DELIVERABLE_FLG = diliverable;
        }

    }

    public class CureirPatner
    {
        public Int32 COU_PARTNER_ID { get; set; }
        public string COU_PARTNER_NAME { get; set; }
        public CureirPatner(int id, string name)
        {
            this.COU_PARTNER_ID  = id;
            this.COU_PARTNER_NAME = name;
        }
    }

    public class AccessoryDA : XtreamConectionString
    {
        public AccessoryAttribute classFeilds;

        public AccessoryDA()
        {
            classFeilds = new AccessoryAttribute();
        }

        public DataSet SelectAllAccessory()
        {
            object[] parameterValues = null;
            return SqlHelper.ExecuteDataset(getConnetctionString, "SELECT_ACCESSORY", parameterValues);
        }

        public DataSet SelectAllCourierPtners()
        {
            object[] parameterValues = null;
            return SqlHelper.ExecuteDataset(getConnetctionString, "SELECT_COURIER_PARTNER", parameterValues);
        }

        public DataSet InsertAccessory()
        {
            try
            {
                object[] parameterValues = { classFeilds.ACC_ID, classFeilds.ACC_NAME, classFeilds.ACC_COMMENT, classFeilds.ACC_PAYBLE_FLG, classFeilds.ACC_AMOUNT, classFeilds.ACC_MANDATORY , classFeilds.ACC_DELIVERABLE_FLG };
                return SqlHelper.ExecuteDataset(getConnetctionString, "SAVE_ACCESSORY", parameterValues);
            }
            catch
            {
                throw;
            }
        }
        public Int32 DeleteAccessory()
        {
            try
            {
                object[] parameterValues = { classFeilds.ACC_ID };
                return SqlHelper.ExecuteNonQuery(getConnetctionString, "DELETE_ACCESSORY", parameterValues);
            }
            catch
            {
                throw;
            }
        }

    }
}
