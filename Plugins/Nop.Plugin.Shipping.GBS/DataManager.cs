using Newtonsoft.Json;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Plugin.Shipping.GBS.Models.Product;
using Nop.Services.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace Nop.Plugin.DataAccess.GBS
{
    public class DataManager
    {
        public ILogger _logger = EngineContext.Current.Resolve<ILogger>();
        public static ProductGBSModel GetGBSShippingCategory(int pID, string tableName)
        {
            ProductGBSModel productgbsModel = new ProductGBSModel();
            DataManager dbmanager = new DataManager();
            Dictionary<string, string> paramDic = new Dictionary<string, string>();
            paramDic.Add("@ProductID", pID.ToString());
            string select = "Select ProductID, shippingCategoryA, shippingCategoryB from " + tableName + " where ProductID = " + pID + "";
            DataView dView = dbmanager.GetParameterizedDataView(select, paramDic);  

            if (dView.Count > 0)
            {
                productgbsModel.ProductID = int.Parse(dView[0][0].ToString());
                productgbsModel.ShippingCategoryA = dView[0][1].ToString();
                productgbsModel.ShippingCategoryB = dView[0][2].ToString();

                return productgbsModel; 
            }
            else
            {
                productgbsModel.ProductID = 0;
                productgbsModel.ShippingCategoryB = null;
                productgbsModel.ShippingCategoryA = null;
                return productgbsModel; //"Row Not Found";
            }

        }
        public static string UpdateGBSShippingCategory(ProductGBSModel productgbsModel, string tableName)
        {
            DataManager dbmanager = new DataManager();
            Dictionary<string, string> paramDic = new Dictionary<string, string>();
            paramDic.Add("@ProductID", productgbsModel.ProductID.ToString());
            paramDic.Add("@ShippingCategoryA", productgbsModel.ShippingCategoryA.ToString());
            paramDic.Add("@ShippingCategoryB", productgbsModel.ShippingCategoryB.ToString());
            string select = "UPDATE " + tableName + " SET ProductID = @ProductID, ShippingCategoryA = @ShippingCategoryA , ShippingCategoryB = @ShippingCategoryB WHERE  ProductID = " + int.Parse(productgbsModel.ProductID.ToString()) + "";
            DataView dView = dbmanager.GetParameterizedDataView(select, paramDic);  

            if (dView != null)
            {
                return "Success";
            }
            else
            {                
                return "Fail" ; //"NO Row Updated";
            }
        }
        public static string AddGBSShippingCategory(ProductGBSModel productgbsModel, string tableName)
        {
            ProductGBSModel productModel = new ProductGBSModel();
            DataManager dbmanager = new DataManager();
            Dictionary<string, string> paramDic = new Dictionary<string, string>();
            paramDic.Add("@ProductID", productgbsModel.ProductID.ToString());
            paramDic.Add("@ShippingCategoryA", productgbsModel.ShippingCategoryA.ToString());
            paramDic.Add("@ShippingCategoryB", productgbsModel.ShippingCategoryB.ToString());
            string select = "INSERT INTO " + tableName + "(ProductID, shippingCategoryA,shippingCategoryB)  VALUES ( @ProductID , @ShippingCategoryA , @ShippingCategoryB )";
            DataView dView = dbmanager.GetParameterizedDataView(select, paramDic);  //dbmanager.GetDataView(select);

            if (dView != null)
            {
                return "Success";
            }
            else
            {
                return "Fail"; //"No Row Inserted";
            }
        }
        private DbConnection dbConnection = null;

        public DataManager()
        {
            this.dbConnection = new SqlConnection();
            dbConnection.ConnectionString = new DataSettingsManager().LoadSettings().DataConnectionString;
        }

        public void Open()
        {
            if (dbConnection.State != ConnectionState.Open)
                dbConnection.Open();
        }

        public void Close()
        {
            if (dbConnection.State != ConnectionState.Closed)
                dbConnection.Close();
        }


        public DbConnection Connection
        {
            get
            {
                return dbConnection;
            }
        }



        public DataView GetDataView(String sqlQuery)
        {

            Open();
            //new DataSettingsManager().LoadSettings().
            using (DbDataAdapter dbDataAdapter = new SqlDataAdapter())
            {
                try
                {

                    dbDataAdapter.SelectCommand = new SqlCommand();
                    dbDataAdapter.SelectCommand.CommandText = sqlQuery;
                    dbDataAdapter.SelectCommand.Connection = this.Connection;
                    DataSet ds = new DataSet();
                    dbDataAdapter.Fill(ds);
                    DataView dv = new DataView();
                    dv = ds.Tables[0].DefaultView;
                    Close();
                    return dv;
                }

                catch (Exception ex)
                {
                    _logger.Error("SQL Exception in Shipping Datamanager GetDataView - query : " + sqlQuery, ex);
                    return null;
                }
                finally
                {
                    Close();
                }

            }



        }

        public DataView RemoveDuplicateRows(DataView dTable, string colName)
        {
            Hashtable hTable = new Hashtable();
            ArrayList duplicateList = new ArrayList();

            //Add list of all the unique item value to hashtable, which stores combination of key, value pair.
            //And add duplicate item value in arraylist.
            foreach (DataRow drow in dTable.Table.Rows)
            {
                if (hTable.Contains(drow[colName]))
                    duplicateList.Add(drow);
                else
                    hTable.Add(drow[colName], string.Empty);
            }

            //Removing a list of duplicate items from datatable.
            foreach (DataRow dRow in duplicateList)
                dTable.Table.Rows.Remove(dRow);

            //Datatable which contains unique records will be return as output.
            return dTable;
        }


        public DataView GetParameterizedDataView(string query, Dictionary<string, string> myDict)
        {
            try
            {

                using (SqlCommand cmd = new SqlCommand(query))
                {

                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = (SqlConnection)this.dbConnection;
                    if (myDict.Count > 0)
                    {
                        foreach (var item in myDict)
                        {
                            cmd.Parameters.AddWithValue(item.Key, item.Value);
                        }
                    }
                    Open();
                    SqlDataReader sqlReader = cmd.ExecuteReader();
                    DataView dv = new DataView();
                    DataTable dt = new DataTable();
                    dt.Load(sqlReader);
                    dv = dt.DefaultView;
                    Close();

                    return dv;
                }




            }
            catch (SqlException ex)
            {
                _logger.Error("SQL Exception in Shipping Datamanager GetParameterizedDataView - query : " + query + " " + JsonConvert.SerializeObject(myDict, Formatting.Indented), ex);
                return null;
            }
            finally
            {
                Close();
            }

        }

        public void SetParameterizedQueryNoData(string query, Dictionary<string, string> myDict)
        {
            try
            {

                using (SqlCommand cmd = new SqlCommand(query))
                {

                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = (SqlConnection)this.dbConnection;
                    if (myDict.Count > 0)
                    {
                        foreach (var item in myDict)
                        {
                            cmd.Parameters.AddWithValue(item.Key, item.Value);
                        }
                    }
                    Open();
                    cmd.ExecuteNonQuery();

                }
                Close();

            }
            catch (Exception ex)
            {
                _logger.Error("SQL Exception in Shipping Datamanager SetParameterizedQueryNoData - query : " + query + " " + JsonConvert.SerializeObject(myDict, Formatting.Indented), ex);
                throw ex;
            }
            finally
            {
                Close();
            }
        }
    }
}