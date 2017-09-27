using Nop.Core.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

/// <summary>
/// Summary description for DBManager
/// </summary>
/// 
namespace Nop.Plugin.DataAccess.GBS
{
    public class DBManager
    {

        public static string getGBSOrderID(int nopID)
        {
            DBManager dbmanager = new DBManager();
            Dictionary<string, string> paramDic = new Dictionary<string, string>();
            paramDic.Add("@nopID", nopID.ToString());
            string select = "Select gbsOrderID from tblNOPOrder where nopID = " + nopID + "";
            DataView dView = dbmanager.GetParameterizedDataView(select, paramDic);  //dbmanager.GetDataView(select);

            if (dView.Count > 0)
            {
                return dView[0]["gbsOrderID"].ToString();
            }
            else
            {
                return "Row Not Found";
            }
            
        }


        private DbConnection dbConnection = null;
        
        public DBManager()
        {
            this.dbConnection = new SqlConnection();
            dbConnection.ConnectionString = new DataSettingsManager().LoadSettings().DataConnectionString;
        }
        public DBManager( string connectionString)
        {
            this.dbConnection = new SqlConnection();

            if (!string.IsNullOrEmpty(connectionString))
            {
                dbConnection.ConnectionString = connectionString;
            } else
            {
                dbConnection.ConnectionString = new DataSettingsManager().LoadSettings().DataConnectionString;
            }
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
                return null;
            }


        }
        public DataView GetParameterizedDataView(string query, Dictionary<string, Object> myDict)
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
                return null;
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
                throw ex;
            }

        }


        public void SetParameterizedQueryNoData(string query, Dictionary<string, Object> myDict)
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
            catch (SqlException ex)
            {
                throw ex;
            }

        }


    }


}
