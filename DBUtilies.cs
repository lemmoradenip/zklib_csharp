using System.Net;
using System;
using System.Data;
using System.Configuration;
using System.Web;
//using System.Web.Security;
//using System.Web.UI;
//using System.Web.UI.WebControls;
//using System.Web.UI.WebControls.WebParts;
//using System.Web.UI.HtmlControls;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Data.OleDb;
using System.IO;
using System.Diagnostics;
//using System.DirectoryServices;
//using System.Linq;
using System.Data.Odbc;
//using Oracle.ManagedDataAccess.Client;

namespace AttLogs
{
    public class DBUtilies
    {
        private string DBConnectionString;
        private SqlConnection sqlcon;
        private DataTable dt;
        private SqlCommand sqlcmd;
        private string userid;
        private DateTime datelog;
        private int verifymode;
        private string deviceip;
        private string inoutmode;

        private const string ArtarConString = "Artar_webConnectionString";
        private string ARTARDBconnection = ConfigurationManager.ConnectionStrings["Artar_webConnectionString"].ConnectionString;
        private string ACTADBconnection = ConfigurationManager.ConnectionStrings["ACtatek_WebdbConnectionString"].ConnectionString;
        public DBUtilies()
        {
            // by default connection string will come from text file name DBString
        }

        public string UserId
        {
            get { return this.userid; }
            set { this.userid = value; }
        }
        public DateTime Datelog
        {
            get { return this.datelog; }
            set { this.datelog = value; }
        }

        public int VerifyMode
        {
            get { return this.verifymode; }
            set { this.verifymode = value; }
        }
        public string DeviceIP
        {
            get { return this.deviceip; }
            set { this.deviceip = value; }

        }
        public string InOutMode
        {
            get { return this.inoutmode; }
            set { this.inoutmode = value; }
        }



        /// <summary>
        /// Get data from Database
        /// </summary>
        /// <param name="Query"></param>
        /// <param name="ErrKeyword"></param>
        /// <returns>DataTable</returns>
        public DataTable GetData(string Query, string ErrKeyword)
        {
            DataTable result = new DataTable();
            try
            {

                sqlcon = new SqlConnection(this.ARTARDBconnection);
                sqlcmd = new SqlCommand(Query, sqlcon);
                if (sqlcon.State == ConnectionState.Closed)
                {
                    sqlcon.Open();
                }
                using (SqlDataAdapter sda = new SqlDataAdapter())
                {
                    sqlcmd.CommandType = CommandType.Text;
                    sqlcmd.CommandTimeout = 0;
                    sda.SelectCommand = sqlcmd;
                    sda.SelectCommand.CommandTimeout = 0;
                    sda.Fill(result);
                }
            }
            catch (Exception ex)
            {

                throw new Exception(String.Format(ex.Message.ToString() + " {0}", ErrKeyword));
            }
            return result;
        }

        /// <summary>
        ///  ExecuteNonQuery
        /// </summary>
        /// <param name="command"></param>
        /// <returns>int</returns>
        public int ExecuteNonQuery(SqlCommand command)
        {
            //Constructor DBUtil dbutil = new DBUtil(DBUtil.DBNames.ArtarWeb.ToString());
            SqlConnection connection = new SqlConnection(this.ARTARDBconnection);

            int rowsAffected = 0;
            try
            {
                //command.CommandText = query;
                command.Connection = connection;
                command.CommandTimeout = connection.ConnectionTimeout;
                //command.CommandType = CommandType.Text;
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                rowsAffected = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                //throw new Exception(String.Format((ex.Message.ToString().Contains("duplicate") ? "Record already Exist!" : ex.Message.ToString())));
                throw new Exception(ex.Message.ToString());
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
                connection.Dispose();
                command.Dispose();
            }
            return rowsAffected;
        }

        /// <summary>
        /// userid,datetimelog,verifymode,deviceip,inoutmode
        /// </summary>
        /// <returns>int</returns>
        public int FetchData()
        {
            SqlCommand sqlcmd = new SqlCommand("usp_fetchattendancedata");
            sqlcmd.CommandType = CommandType.StoredProcedure;
            sqlcmd.Parameters.AddWithValue("@userid", this.userid);
            sqlcmd.Parameters.AddWithValue("@datetimelog", this.datelog);
            sqlcmd.Parameters.AddWithValue("@verifymode", this.verifymode);
            sqlcmd.Parameters.AddWithValue("@deviceip", this.deviceip);
            sqlcmd.Parameters.AddWithValue("@inoutmode", this.inoutmode);           
            return this.ExecuteNonQuery(sqlcmd);
        }


    }
}
