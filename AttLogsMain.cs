/**********************************************************
 * Demo for Standalone SDK.Created by Darcy on Oct.15 2009*
***********************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Configuration;
namespace AttLogs
{
    public partial class AttLogsMain : Form
    {
        String ARTARDBconnection = ConfigurationManager.ConnectionStrings["Artar_webConnectionString"].ConnectionString;
        DBUtilies dbutilies = new DBUtilies();
        public AttLogsMain()
        {
            InitializeComponent();

        }

        //Create Standalone SDK class dynamicly.
        public zkemkeeper.CZKEMClass axCZKEM1 = new zkemkeeper.CZKEMClass();

        /*************************************************************************************************
        * Before you refer to this demo,we strongly suggest you read the development manual deeply first.*
        * This part is for demonstrating the communication with your device.                             *
        * ************************************************************************************************/
        #region Communication
        private bool bIsConnected = false;//the boolean value identifies whether the device is connected
        private int iMachineNumber = 1;//the serial number of the device.After connecting the device ,this value will be changed.

        //If your device supports the TCP/IP communications, you can refer to this.
        //when you are using the tcp/ip communication,you can distinguish different devices by their IP address.
        private void btnConnect_Click(object sender, EventArgs e)
        {

            if (txtIP.Text.Trim() == "" || txtPort.Text.Trim() == "")
            {
                MessageBox.Show("IP and Port cannot be null", "Error");
                return;
            }
            int idwErrorCode = 0;

            Cursor = Cursors.WaitCursor;
            if (btnConnect.Text == "DisConnect")
            {
                axCZKEM1.Disconnect();
                bIsConnected = false;
                btnConnect.Text = "Connect";
                lblState.Text = "Current State:DisConnected";
                Cursor = Cursors.Default;
                return;
            }

            bIsConnected = axCZKEM1.Connect_Net(txtIP.Text, Convert.ToInt32(txtPort.Text));

            if (bIsConnected == true)
            {
                btnConnect.Text = "DisConnect";
                btnConnect.Refresh();
                lblState.Text = "Current State:Connected";
                iMachineNumber = 1;//In fact,when you are using the tcp/ip communication,this parameter will be ignored,that is any integer will all right.Here we use 1.
                axCZKEM1.RegEvent(iMachineNumber, 65535);//Here you can register the realtime events that you want to be triggered(the parameters 65535 means registering all)
            }
            else
            {
                axCZKEM1.GetLastError(ref idwErrorCode);
                MessageBox.Show("Unable to connect the device,ErrorCode=" + idwErrorCode.ToString(), "Error");
            }
            Cursor = Cursors.Default;
        }

        //If your device supports the SerialPort communications, you can refer to this.
        private void btnRsConnect_Click(object sender, EventArgs e)
        {
            if (cbPort.Text.Trim() == "" || cbBaudRate.Text.Trim() == "" || txtMachineSN.Text.Trim() == "")
            {
                MessageBox.Show("Port,BaudRate and MachineSN cannot be null", "Error");
                return;
            }
            int idwErrorCode = 0;
            //accept serialport number from string like "COMi"
            int iPort;
            string sPort = cbPort.Text.Trim();
            for (iPort = 1; iPort < 10; iPort++)
            {
                if (sPort.IndexOf(iPort.ToString()) > -1)
                {
                    break;
                }
            }

            Cursor = Cursors.WaitCursor;
            if (btnRsConnect.Text == "Disconnect")
            {
                axCZKEM1.Disconnect();
                bIsConnected = false;
                btnRsConnect.Text = "Connect";
                btnRsConnect.Refresh();
                lblState.Text = "Current State:Disconnected";
                Cursor = Cursors.Default;
                return;
            }

            iMachineNumber = Convert.ToInt32(txtMachineSN.Text.Trim());//when you are using the serial port communication,you can distinguish different devices by their serial port number.
            bIsConnected = axCZKEM1.Connect_Com(iPort, iMachineNumber, Convert.ToInt32(cbBaudRate.Text.Trim()));

            if (bIsConnected == true)
            {
                btnRsConnect.Text = "Disconnect";
                btnRsConnect.Refresh();
                lblState.Text = "Current State:Connected";

                axCZKEM1.RegEvent(iMachineNumber, 65535);//Here you can register the realtime events that you want to be triggered(the parameters 65535 means registering all)
            }
            else
            {
                axCZKEM1.GetLastError(ref idwErrorCode);
                MessageBox.Show("Unable to connect the device,ErrorCode=" + idwErrorCode.ToString(), "Error");
            }

            Cursor = Cursors.Default;
        }

        #endregion

        /*************************************************************************************************
        * Before you refer to this demo,we strongly suggest you read the development manual deeply first.*
        * This part is for demonstrating operations with(read/get/clear) the attendance records.         *
        * ************************************************************************************************/
        #region AttLogs

        //Download the attendance records from the device(For both Black&White and TFT screen devices).
        private void btnGetGeneralLogData_Click(object sender, EventArgs e)
        {
            if (bIsConnected == false)
            {
                MessageBox.Show("Please connect the device first", "Error");
                return;
            }
            int idwErrorCode = 0;
            string sdwEnrollNumber = "";
            int idwVerifyMode = 0;
            int idwInOutMode = 0;
            int idwYear = 0;
            int idwMonth = 0;
            int idwDay = 0;
            int idwHour = 0;
            int idwMinute = 0;
            int idwSecond = 0;
            int idwWorkcode = 0;
            int iGLCount = 0;
            int iIndex = 0;

            Cursor = Cursors.WaitCursor;
            lvLogs.Items.Clear();
            axCZKEM1.EnableDevice(iMachineNumber, false);//disable the device
            if (axCZKEM1.ReadGeneralLogData(iMachineNumber))//read all the attendance records to the memory
            {
                while (axCZKEM1.SSR_GetGeneralLogData(iMachineNumber, out sdwEnrollNumber, out idwVerifyMode,
                            out idwInOutMode, out idwYear, out idwMonth, out idwDay, out idwHour, out idwMinute, out idwSecond, ref idwWorkcode))//get records from the memory
                {
                    iGLCount++;
                    lvLogs.Items.Add(iGLCount.ToString());
                    lvLogs.Items[iIndex].SubItems.Add(sdwEnrollNumber);//modify by Darcy on Nov.26 2009
                    lvLogs.Items[iIndex].SubItems.Add(idwVerifyMode.ToString());
                    lvLogs.Items[iIndex].SubItems.Add(idwInOutMode.ToString());
                    lvLogs.Items[iIndex].SubItems.Add(idwYear.ToString() + "-" + idwMonth.ToString() + "-" + idwDay.ToString() + " " + idwHour.ToString() + ":" + idwMinute.ToString() + ":" + idwSecond.ToString());
                    lvLogs.Items[iIndex].SubItems.Add(idwWorkcode.ToString());
                    iIndex++;
                }
            }
            else
            {
                Cursor = Cursors.Default;
                axCZKEM1.GetLastError(ref idwErrorCode);

                if (idwErrorCode != 0)
                {
                    MessageBox.Show("Reading data from terminal failed,ErrorCode: " + idwErrorCode.ToString(), "Error");
                }
                else
                {
                    MessageBox.Show("No data from terminal returns!", "Error");
                }
            }
            axCZKEM1.EnableDevice(iMachineNumber, true);//enable the device
            Cursor = Cursors.Default;
        }

        //Clear all attendance records from terminal
        private void btnClearGLog_Click(object sender, EventArgs e)
        {
            if (bIsConnected == false)
            {
                MessageBox.Show("Please connect the device first", "Error");
                return;
            }
            int idwErrorCode = 0;

            lvLogs.Items.Clear();
            axCZKEM1.EnableDevice(iMachineNumber, false);//disable the device
            if (axCZKEM1.ClearGLog(iMachineNumber))
            {
                axCZKEM1.RefreshData(iMachineNumber);//the data in the device should be refreshed
                MessageBox.Show("All att Logs have been cleared from teiminal!", "Success");
            }
            else
            {
                axCZKEM1.GetLastError(ref idwErrorCode);
                MessageBox.Show("Operation failed,ErrorCode=" + idwErrorCode.ToString(), "Error");
            }
            axCZKEM1.EnableDevice(iMachineNumber, true);//enable the device
        }

        //Get the count of attendance records in from ternimal
        private void btnGetDeviceStatus_Click(object sender, EventArgs e)
        {
            if (bIsConnected == false)
            {
                MessageBox.Show("Please connect the device first", "Error");
                return;
            }
            int idwErrorCode = 0;
            int iValue = 0;

            axCZKEM1.EnableDevice(iMachineNumber, false);//disable the device
            if (axCZKEM1.GetDeviceStatus(iMachineNumber, 6, ref iValue)) //Here we use the function "GetDeviceStatus" to get the record's count.The parameter "Status" is 6.
            {
                MessageBox.Show("The count of the AttLogs in the device is " + iValue.ToString(), "Success");
            }
            else
            {
                axCZKEM1.GetLastError(ref idwErrorCode);
                MessageBox.Show("Operation failed,ErrorCode=" + idwErrorCode.ToString(), "Error");
            }
            axCZKEM1.EnableDevice(iMachineNumber, true);//enable the device
        }
        #endregion



        /*************************************************************************************************
        * This overrides code for attendance logs, every setup is connected to database    *
        * ************************************************************************************************/
        #region NewCodes
        string deviceip, port;

        // String connection = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\\Attendance\\ARTAR_db_2000.mdb;Persist Security Info=false";
        #region Getdate     
        public DataTable Getdata(string query)
        {
            OleDbCommand sqlcmd;
            DataTable dt3 = new DataTable();
            try
            {
                OleDbConnection conn = new OleDbConnection(ARTARDBconnection);
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                sqlcmd = new OleDbCommand(query, conn);
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                using (OleDbDataAdapter sda = new OleDbDataAdapter())
                {
                    sqlcmd.CommandType = CommandType.Text;
                    sda.SelectCommand = sqlcmd;
                    sda.Fill(dt3);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error on Function Getdata  details:" + ex.Message.ToString());
            }
            return dt3;
        }
        #endregion

        
        public void AutoConnect(object sender, EventArgs e)
        {
            try
            {
                int idwErrorCode = 0;
                Cursor = Cursors.WaitCursor;

                string errologpath, emailto, deviceip, timer;
                DataTable dt = new DataTable();

                dt = dbutilies.GetData("SELECT IP_ADDR,PORT FROM XATT_IP where enabled=1 order by id ","GetData>AutoConnect");
                for (int x = 0; x <= dt.Rows.Count - 1; x++)//if more than one device
                {
                    txtIP.Text = dt.Rows[x][0].ToString().Trim();//ip address
                    txtPort.Text = dt.Rows[x][1].ToString().Trim();//device port

                    //connect device            
                    bool Devicestatus = this.ConnectDevice(dt.Rows[x][0].ToString().Trim(), dt.Rows[x][1].ToString().Trim());
                  //  MessageBox.Show("IP" + dt.Rows[x][0].ToString().Trim() + "\n Status:" + Devicestatus.ToString());

                    //fetch data to database
                    GetAttendanceLog(txtIP.Text.ToString());
                }
                //deviceip = dt.Rows[]
            }
            catch (Exception ex)
            {
                MessageBox.Show("Source AutoConnect() method\n "+ex.Message.ToString());
            }
        }

        #region Getattendancelog
        public void GetAttendanceLog(string xdeviceip)
        {
            if (bIsConnected == false)
            {
                MessageBox.Show(String.Format("Please connect the device ({0}) first",xdeviceip.ToString()), "Error");
                return;
            }
            int idwErrorCode = 0;
            string sdwEnrollNumber = "";
            int idwVerifyMode = 0;
            int idwInOutMode = 0;
            int idwYear = 0;
            int idwMonth = 0;
            int idwDay = 0;
            int idwHour = 0;
            int idwMinute = 0;
            int idwSecond = 0;
            int idwWorkcode = 0;

            int iGLCount = 0;
            int iIndex = 0;
            int recordslogs = 0;
            Cursor = Cursors.WaitCursor;
            lvLogs.Items.Clear();
            axCZKEM1.EnableDevice(iMachineNumber, true);//disable the device with
            if (axCZKEM1.ReadGeneralLogData(iMachineNumber))//read all the attendance records to the memory
            {
                while (axCZKEM1.SSR_GetGeneralLogData(iMachineNumber, out sdwEnrollNumber, out idwVerifyMode,
                            out idwInOutMode, out idwYear, out idwMonth, out idwDay, out idwHour, out idwMinute, out idwSecond, ref idwWorkcode))//get records from the memory
                {

                    // get and format date logs
                    string datelog = idwYear.ToString() + "-" + idwMonth.ToString() + "-" + idwDay.ToString() + " " + idwHour.ToString() + ":" + idwMinute.ToString() + ":" + idwSecond.ToString();

                    //insert to database
                    // InserttoDB(sdwEnrollNumber, Convert.ToDateTime(datelog), idwInOutMode, xdeviceip);
                    //*8010
                    dbutilies.UserId = sdwEnrollNumber;
                    dbutilies.Datelog = Convert.ToDateTime(datelog);
                    dbutilies.VerifyMode = idwVerifyMode;
                    dbutilies.DeviceIP = xdeviceip;
                    dbutilies.InOutMode = idwInOutMode.ToString();
                    int recordcount = dbutilies.FetchData();//record counted
                    //MessageBox.Show("Userid" + sdwEnrollNumber + "\n Datelog:" + Convert.ToDateTime(datelog).ToString());
                    recordslogs++;
                }

            }
            else
            {
                Cursor = Cursors.Default;
                axCZKEM1.GetLastError(ref idwErrorCode);

                if (idwErrorCode != 0)
                {
                    MessageBox.Show("Reading data from terminal failed,ErrorCode: " + idwErrorCode.ToString(), "Error");
                    //   SendEmail("Reading data from terminal failed. Error occur on line series 1500+.,ErrorCode: " + idwErrorCode.ToString());
                }
                else
                {
                    MessageBox.Show("No data from terminal returns!", "Error");
                    //  SendEmail("No data from terminal returns. Error occur on line series 1500+!");
                }
            }
            axCZKEM1.EnableDevice(iMachineNumber, true);//enable the device
            //Cursor = Cursors.Default;
            this.Visible = false;
            // this.Close();
        }
        #endregion

        #region connectTCPIP
        /// <summary>
        /// Connect Device using TCP/IP 
        /// </summary>
        /// <param name="IP"></param>
        /// <param name="Port"></param>
        /// <returns>bool</returns>
        private bool ConnectDevice(string IP, string Port)
        {

            if (IP == "" || Port == "")
            {
                MessageBox.Show("IP and Port cannot be null", "Error");
                return false;
            }
            int idwErrorCode = 0;

            Cursor = Cursors.WaitCursor;
            if (btnConnect.Text == "DisConnect")
            {
                axCZKEM1.Disconnect();
                bIsConnected = false;
                btnConnect.Text = "Connect";
                lblState.Text = "Current State:DisConnected";
                Cursor = Cursors.Default;
                return false;
            }

            bIsConnected = axCZKEM1.Connect_Net(txtIP.Text, Convert.ToInt32(txtPort.Text));

            if (bIsConnected == true)
            {
               // btnConnect.Text = "DisConnect";
                btnConnect.Refresh();
                lblState.Text = "Current State:Connected";
                iMachineNumber = 1;//In fact,when you are using the tcp/ip communication,this parameter will be ignored,that is any integer will all right.Here we use 1.
                axCZKEM1.RegEvent(iMachineNumber, 65535);//Here you can register the realtime events that you want to be triggered(the parameters 65535 means registering all)
            }
            else
            {
                axCZKEM1.GetLastError(ref idwErrorCode);
                MessageBox.Show("Unable to connect the device,ErrorCode=" + idwErrorCode.ToString(), "Error");
            }

            Cursor = Cursors.Default;
            return bIsConnected;// return result from tcp ip connection
         
        }
        #endregion
        /// <summary>
        /// Download biometric data from device to database
        /// </summary>
        #region DownloadBioTemplate
        private void DownloadBioTemplate()
        {
            if (bIsConnected == false)
            {
                MessageBox.Show("Please connect the device first!", "Error");
                return;
            }

            string sName = "";
            string sPassword = "";
            int iPrivilege = 0;
            bool bEnabled = false;

            int idwFingerIndex = 0;
            string sTmpData = "";
            int iTmpLength = 0;
            int iFlag = 0;

            //lvDownload.Items.Clear();
            //lvDownload.BeginUpdate();
            axCZKEM1.EnableDevice(iMachineNumber, false);
            Cursor = Cursors.WaitCursor;

            axCZKEM1.ReadAllUserID(iMachineNumber);//read all the user information to the memory
            axCZKEM1.ReadAllTemplate(iMachineNumber);//read all the users' fingerprint templates to the memory

            int sdwEnrollNumber = 0;
            while (axCZKEM1.GetAllUserInfo(iMachineNumber, ref sdwEnrollNumber, ref sName, ref sPassword, ref iPrivilege, ref bEnabled))//get all the users' information from the memory
            {
                for (idwFingerIndex = 0; idwFingerIndex < 10; idwFingerIndex++)
                {
                    if (axCZKEM1.GetUserTmpExStr(iMachineNumber, sdwEnrollNumber.ToString(), idwFingerIndex, out iFlag, out sTmpData, out iTmpLength))//get the corresponding templates string and length from the memory
                    {
                        ListViewItem list = new ListViewItem();
                        list.Text = sdwEnrollNumber.ToString();
                        list.SubItems.Add(sName);
                        list.SubItems.Add(idwFingerIndex.ToString());
                        list.SubItems.Add(sTmpData);
                        list.SubItems.Add(iPrivilege.ToString());
                        list.SubItems.Add(sPassword);
                        if (bEnabled == true)
                        {
                            list.SubItems.Add("true");
                        }
                        else
                        {
                            list.SubItems.Add("false");
                        }
                        list.SubItems.Add(iFlag.ToString());
                        //lvDownload.Items.Add(list);

                    }
                }
            }

            //lvDownload.EndUpdate();
            axCZKEM1.EnableDevice(iMachineNumber, true);
            Cursor = Cursors.Default;
        }
        #endregion

        private void AttLogsMain_Load(object sender, EventArgs e)
        {
            //auto connect 
            //if process instance exist  do not run
            if (System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Length > 1) System.Diagnostics.Process.GetCurrentProcess().Kill();
            ShowInTaskbar = false;

            // get application settings as needed
            //string errologpath, emailto, deviceip, timer;
            //  ApplicationSettings.ErrorSettings(out errologpath, out emailto, out deviceip, out timer);


            this.Hide();//hide the form
            AutoConnect(sender, e);//Check device connectivity and Capture data

            this.Close();//EXIT FORM
        }
        #endregion
    }
}