using System;
using System.Data;

namespace XtreamDataAccess
{
    public class ClassCategoryAttribute
    {
        public int CAT_ID { get; set; }
        public string CAT_NAME { get; set; }
        public Double CAT_COM_USER_PTG { get; set; }
        public Double CAT_COM_COMN_PTG { get; set; }
    }
    public class CategoryAssosoryAttribute
    {
        public int CAT_ID { get; set; }
        public int ACC_ID { get; set; }
        public int SAVE_DELETE_FLG { get; set; }
    }
    public class ClassCategoryDA : XtreamConectionString
    {
        public ClassCategoryAttribute classFeilds ;
        public CategoryAssosoryAttribute classCategoryFeilds;

        public ClassCategoryDA()
        {
            classFeilds = new ClassCategoryAttribute();
            classCategoryFeilds = new CategoryAssosoryAttribute();
        }

        public DataSet SelectAllCategory()
        {
            object[] parameterValues = null;
            return SqlHelper.ExecuteDataset(getConnetctionString, "SELECT_CLASS_CATEGORY", parameterValues);
        }

        public DataSet InsertCategory()
        {
            try
            {
                object[] parameterValues = { classFeilds.CAT_ID, classFeilds.CAT_NAME, classFeilds.CAT_COM_USER_PTG, classFeilds.CAT_COM_COMN_PTG };
                return SqlHelper.ExecuteDataset(getConnetctionString, "SAVE_CLASS_CATEGORY", parameterValues);
            }
            catch
            {
                throw;
            }
        }
        public Int32 DeleteCategory()
        {
            try
            {
                object[] parameterValues = { classFeilds.CAT_ID };
                return SqlHelper.ExecuteNonQuery(getConnetctionString, "DELETE_CLASS_CATEGORY", parameterValues);
            }
            catch
            {
                throw;
            }
        }
        public Int32 SaveDeleteCategoryAssosory()
        {
            try
            {
                 object[] parameterValues = { classCategoryFeilds.ACC_ID, classCategoryFeilds.CAT_ID, classCategoryFeilds.SAVE_DELETE_FLG };
                return SqlHelper.ExecuteNonQuery(getConnetctionString, "SAVE_DELETE_CATEGORY_ACCESSORY", parameterValues);
            }
            catch
            {
                throw;
            }
        }
        public DataSet SelectCategoryAssosory()
        {
            try
            {
                object[] parameterValues = { classCategoryFeilds.CAT_ID };
                return SqlHelper.ExecuteDataset(getConnetctionString, "SELECT_CATEGORY_ACCESSORY", parameterValues);
            }
            catch
            {
                throw;
            }
        }
    }
}
