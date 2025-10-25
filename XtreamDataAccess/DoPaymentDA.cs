using System;
using System.Data;

namespace XtreamDataAccess
{
    public class DoPaymentDA : XtreamConectionString
    {
        public Int32 STD_ID { get; set; }
        public Int32 CLS_ID { get; set; }
        public int PAID_YEAR { get; set; }
        public int PAID_MONTH { get; set; }
        public string MODIFY_USER { get; set; }
        public double PAID_AMOUNT { get; set; }
        public DateTime STD_REC_DATE { get; set; }
        public int CARD_ISSUED_FLG { get; set; }
        public double CLASS_FEE { get; set; } 

        public Int32 ACC_ID { get; set; }
        public DateTime ASS_REC_DATE { get; set; }
        public DateTime PMT_REC_DATE { get; set; }
        public double ASS_REC_AMOUNT { get; set; }

        public DateTime RELEASE_DATE { get; set; }
        public double RELEASE_AMOUNT { get; set; }
        public string RELEASE_COMMENT { get; set; }

        public Int32 COU_PARTNER_ID { get; set; }
        public DateTime COU_DATE { get; set; }
        public String COU_TRACKING_NO { get; set; }
        public String COU_COMMENT { get; set; }
        public Int32 ASS_PACK_READY { get; set; }
        
        public DataSet SelectPayment()
        {
            try
            {
                object[] parameterValues = { STD_ID, CLS_ID, PAID_YEAR, PAID_MONTH };
                return SqlHelper.ExecuteDataset(getConnetctionString, "SELECT_STUDENT_PAYMENT", parameterValues);
            }
            catch
            {
                throw;
            }
        }

        public Int32 UpdatePayment()
        {
            try
            {
                object[] parameterValues = { STD_ID, CLS_ID, PAID_YEAR, PAID_MONTH, MODIFY_USER, PAID_AMOUNT, STD_REC_DATE,CLASS_FEE, CARD_ISSUED_FLG };
                return SqlHelper.ExecuteNonQuery(getConnetctionString, "UPDATE_PAYMENT", parameterValues);
            }
            catch
            {
                throw;
            }
        }

        public Int32 DeleteEntirePaymentRecord()
        {
            try
            {
                object[] parameterValues = { STD_ID, CLS_ID, PAID_YEAR, PAID_MONTH};
                return SqlHelper.ExecuteNonQuery(getConnetctionString, "DETATE_ENTIRE_PAYMENT", parameterValues);
            }
            catch
            {
                throw;
            }
        }

        public Int32 UpdateCardIssued()
        {
            try
            {
                object[] parameterValues = { STD_ID, CLS_ID, PAID_YEAR, PAID_MONTH, CARD_ISSUED_FLG };
                return SqlHelper.ExecuteNonQuery(getConnetctionString, "UPDATE_CARD_ISSUED", parameterValues);
            }
            catch
            {
                throw;
            }
        }

        public Int32 UpdateClassFee()
        {
            try
            {
                object[] parameterValues = { STD_ID, CLS_ID, PAID_YEAR, PAID_MONTH, CLASS_FEE };
                return SqlHelper.ExecuteNonQuery(getConnetctionString, "UPDATE_CLASS_FEE", parameterValues);
            }
            catch
            {
                throw;
            }
        }



        public DataSet SelectPaymentAccessory()
        {
            try
            {
                object[] parameterValues = { STD_ID, ACC_ID, ASS_REC_DATE };
                return SqlHelper.ExecuteDataset(getConnetctionString, "SELECT_STUDENT_ACCESSORY_PAYMENT", parameterValues);
            }
            catch
            {
                throw;
            }
        }

        public DataSet SelectCuoreierInfo()
        {
            try
            {
                object[] parameterValues = { STD_ID, ACC_ID, ASS_REC_DATE };
                return SqlHelper.ExecuteDataset(getConnetctionString, "SELECT_STUDENT_COUREIR_INFO", parameterValues);
            }
            catch
            {
                throw;
            }
        }

        public Int32 UpdatePaymentAccessory()
        {
            try
            {
                object[] parameterValues = { STD_ID, ACC_ID, ASS_REC_DATE, PMT_REC_DATE, MODIFY_USER, PAID_AMOUNT, ASS_REC_AMOUNT };
                return SqlHelper.ExecuteNonQuery(getConnetctionString, "UPDATE_PAYMENT_ACCESSORY", parameterValues);
            }
            catch
            {
                throw;
            }
        }
        public Int32 UpdateStudentAccessory()
        {
            try
            {
                object[] parameterValues = { STD_ID, ACC_ID, ASS_REC_DATE, ASS_REC_AMOUNT, COU_PARTNER_ID, COU_DATE, COU_TRACKING_NO, COU_COMMENT, ASS_PACK_READY };
                return SqlHelper.ExecuteNonQuery(getConnetctionString, "SAVE_STUDENT_ACCESSORY", parameterValues);
            }
            catch
            {
                throw;
            }
        }
     
        public Int32 UpdateAccessoryAmount()
        {
            try
            {
                object[] parameterValues = { STD_ID, ACC_ID, ASS_REC_DATE, ASS_REC_AMOUNT };
                return SqlHelper.ExecuteNonQuery(getConnetctionString, "UPDATE_STUDENT_ACCESSORY", parameterValues);
            }
            catch
            {
                throw;
            }
        }
        public Int32 DeletePayment()
        {
            try
            {
                object[] parameterValues = { STD_ID, CLS_ID, PAID_YEAR, PAID_MONTH,STD_REC_DATE };
                return SqlHelper.ExecuteNonQuery(getConnetctionString, "DETATE_PAYMENT", parameterValues);
            }
            catch
            {
                throw;
            }
        }
         public Int32 DeletePaymentAccessory()
        {
            try
            {
                object[] parameterValues = { STD_ID, ACC_ID, ASS_REC_DATE, PMT_REC_DATE};
                return SqlHelper.ExecuteNonQuery(getConnetctionString, "DELETE_PAYMENT_ACCESSORY", parameterValues);
            }
            catch
            {
                throw;
            }
        }
        public String InsertReleasePayment()
        {
            try
            {
                object[] parameterValues = { STD_ID, RELEASE_DATE, RELEASE_AMOUNT,MODIFY_USER, RELEASE_COMMENT };
                return SqlHelper.ExecuteDataset(getConnetctionString, "INSERT_RELEASE_PAYMENT", parameterValues).Tables[0].Rows[0]["RELEASE_STATUS"].ToString();

            }
            catch
            {
                throw;
            }
        }
    }
}
