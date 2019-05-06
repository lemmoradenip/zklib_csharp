using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OleDb;
using System.Configuration;
using System.Data;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Net.Mail;
using System.Diagnostics;
using System.IO;
namespace AttLogs
{
    class ApplicationSettings
    {
        public static zkemkeeper.CZKEMClass axCZKEM1 = new zkemkeeper.CZKEMClass();  //create stand alone SKD or zkemKeeper

        #region connectionstring
        public static string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["connection"].ConnectionString;
            }
        }
        #endregion

        #region ErrorSettings
        public static void ErrorSettings(out string path, out string emailto, out string deviceip, out string timer)
        {
            //my local variables
            string lpath, lemailto, ldeviceip, ltimer;
            //initialize val
            lpath = "";
            lemailto = "";
            ldeviceip = "";
            ltimer = "";

            try
            {
                //get the connection string from app.config
                OleDbConnection conn = new OleDbConnection(ApplicationSettings.ConnectionString);
                OleDbCommand olecmd = new OleDbCommand("select ip_addr", conn);
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                OleDbDataReader olereader = olecmd.ExecuteReader();
                if (olereader.HasRows)
                {
                    while (olereader.Read())
                    {
                        lpath = olereader["logpath"].ToString();//error drive path
                        lemailto = olereader["email"].ToString();//email address to receipt
                        ldeviceip = olereader["deviceip"].ToString();//ip of the device
                        ltimer = olereader["timer"].ToString();//time frequency
                    }
                }
                else
                {
                    lpath = "empty";
                    lemailto = "empty";
                }
            }
            catch (Exception ex)
            {//if errro occur throw message
                lpath = "Error on  class ApplicationSettings.ErrorSettings  Details: " + ex.Message.ToString();
                path = "Error on  class ApplicationSettings.ErrorSettings  Details: " + ex.Message.ToString();
                ldeviceip = "Error on  class ApplicationSettings.ErrorSettings  Details: " + ex.Message.ToString();
                ltimer = "Error on  class ApplicationSettings.ErrorSettings  Details: " + ex.Message.ToString();
            }
            //return value
            path = lpath;
            emailto = lemailto;
            deviceip = ldeviceip;
            timer = ltimer;
        }

        #endregion

        #region CheckConnection
        public static string CheckConnection(string deviceip)
        {
            string status = "";
            try
            {
                bool bIsConnected = false;//the boolean value identifies whether the device is connected         
                bIsConnected = axCZKEM1.Connect_Net(deviceip, Convert.ToInt32("4370"));
                if (bIsConnected == true)
                {
                    status = "Connected!";
                }
                else
                {
                    status = "Sorry cannot find device with this IP.!";
                }
            }
            catch (Exception ex)
            {
                status = "Error on Class #ApplicationSettings   Method #checkconnection \nDetails:" + ex.Message.ToString();
            }
            return status;
        }
        #endregion

        /*************************************************************************************************
        * Setup for new codes *
        * ************************************************************************************************/

       

    }
}
