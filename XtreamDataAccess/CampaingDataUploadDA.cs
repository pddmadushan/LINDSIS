using System;
using System.Data;
using System.Data.SqlClient;


namespace XtreamDataAccess

{
    public class CampaingDataUploadDA : XtreamConectionString
    {
        public Int32 Row_id { get; set; }
        
        public string Full_name { get; set; }

        public string First_name { get; set; }
        public string Last_name { get; set; }

        public int Cmp_id {  get; set; }

        public string Mobile { get; set; }

        public CampaingDataUploadDA() { }

        public DataSet SelectAllCampaing()
        {
            object[] parameterValues = null;
            return SqlHelper.ExecuteDataset(getConnetctionString, "LoadCampaingToCombo", parameterValues);
        }

        public void ExcelToTmpTable( DataTable excel_tbl)
        {

            try
            {
                using (SqlConnection cn = new SqlConnection(getConnetctionString))
                {
                    cn.Open();

                    SqlTransaction trn = cn.BeginTransaction(IsolationLevel.Snapshot);
                    try
                    {
                        SqlCommand cmdBulk = new SqlCommand();
                        cmdBulk.CommandType = CommandType.StoredProcedure;
                        cmdBulk.CommandText = "Excel_to_Temp_Table";
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@Excel_data_Table", excel_tbl));
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@CMP_ID", Cmp_id));
                        cmdBulk.Connection = trn.Connection;
                        cmdBulk.Transaction = trn;
                        cmdBulk.ExecuteNonQuery();
                        cmdBulk.Dispose();
                        trn.Commit();
                       trn.Dispose();                       
                    }
                    catch {
                        trn.Rollback();
                        throw;
                    }
                    finally
                    {
                        trn.Dispose();
                    }
                }
            }
            catch { 
                throw; 
            }
        }

        public DataSet ProcessTemporyDataToGridView()
        {
            object[] parameterValues = null;
            return SqlHelper.ExecuteDataset(getConnetctionString, "ShowReadExcelData", parameterValues);
        }

        public DataSet GetCampaingTempData()
        {
            object[] parameterValues = null;
            return SqlHelper.ExecuteDataset(getConnetctionString, "GetCampaingTempData", parameterValues);
        }

        public void FinalUploadDataToTemp(DataTable FinalTemp_tbl,int CMP_ID,string AddName,DateTime AddDate,string User ,out int AffecdRows)
        {

            try
            {
                using (SqlConnection cn = new SqlConnection(getConnetctionString))
                {
                    cn.Open();

                    SqlTransaction trn = cn.BeginTransaction(IsolationLevel.Snapshot);
                    try
                    {
                        SqlCommand cmdBulk = new SqlCommand();
                        cmdBulk.CommandType = CommandType.StoredProcedure;
                        cmdBulk.CommandText = "FINAL_DATA_UPLOAD";
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@FinalExcel_data_Table", FinalTemp_tbl));
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@CMP_ID", CMP_ID));
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@ADD_NAME", AddName));
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@ADD_DATE", AddDate));
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@USER", User));
                        cmdBulk.Connection = trn.Connection;
                        cmdBulk.Transaction = trn;
                        AffecdRows= cmdBulk.ExecuteNonQuery();
                        cmdBulk.Dispose();
                        trn.Commit();
                        trn.Dispose();
                    }
                    catch
                    {
                        trn.Rollback();
                        throw;
                    }
                    finally
                    {
                        trn.Dispose();
                    }

                }
            }
            catch { 
                throw; 
            }
        }

        public Int32 DeleteRowByUser()
        {
            try
            {
                object[] parameterValues = { Row_id };
                return SqlHelper.ExecuteNonQuery(getConnetctionString, "Delete_Excel_Row_BYuser", parameterValues);
            }
            catch
            {
                throw;
            }
        }

        public DataSet SaveUserUpadateData(Int32 Row_id,  string Edit_Full_name,string Edit_First_name,string Edit_Last_name,string Edit_Mobile)

        {
            try
            {
                object[] parameterValues = { Row_id, Edit_Full_name, Edit_Mobile, Edit_First_name, Edit_Last_name };
                return SqlHelper.ExecuteDataset(getConnetctionString, "Update_Excel_Row_BYuser", parameterValues);

            }
            catch
            {
                throw;
            }
        }

        public Int32 RefreshDatagrid()
        {
            object[] parameterValues = null;
            return SqlHelper.ExecuteNonQuery(getConnetctionString, "GetCampaingTempData", parameterValues);
        }

        public Int32 CheckInvalidRecords()
        {
            object[] parameterValues = null;
            return SqlHelper.ExecuteNonQuery(getConnetctionString, "INVALID_RECORDS_VALIDATE", parameterValues);
        }

        public DataSet READ_PROCESS_SHOW_EXCEL_DATA(DataTable tblexcelDta)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(getConnetctionString))
                {
                    cn.Open();

                    SqlTransaction trn = cn.BeginTransaction(IsolationLevel.Snapshot);
                    try
                    {
                        SqlCommand cmdBulk = new SqlCommand();
                        cmdBulk.CommandType = CommandType.StoredProcedure;
                        cmdBulk.CommandText = "READ_PROCESS_SHOW_EXCEL_DATA";
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@Excel_data_Table", tblexcelDta));
                        cmdBulk.Parameters.Add(new System.Data.SqlClient.SqlParameter("@CMP_ID", Cmp_id));
                        cmdBulk.Connection = trn.Connection;
                        cmdBulk.Transaction = trn;
                        SqlDataAdapter adp = new SqlDataAdapter(cmdBulk);
                        DataSet ds = new DataSet();
                        adp.Fill(ds);
                        cmdBulk.Dispose();
                        trn.Commit();
                        trn.Dispose();
                        return ds;
                    }
                    catch
                    {
                        trn.Rollback();
                        throw;
                    }
                    finally
                    {
                        trn.Dispose();
                    }
                }
            }
            catch
            {
                throw;
            }
        }
    }

}
