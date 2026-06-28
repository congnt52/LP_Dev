using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Net;
using System.Net.Sockets;
using System.IO;

namespace _5GAutoTool
{
    public partial class SetupVSA : Form
    {
        private LogAdapter log = LogAdapter.Instance;
        public SetupVSA()
        {
            InitializeComponent();           
        }

        /*................................Khai bao Class va bien global...............................*/
        //Khai bao cac bien global
        VSA VSA;
        //CONFIGURATION
        private Form5GAT mainForm = null;
        string connectionSTT = "ERROR";
        public SetupVSA(Form callingForm)
        {           
            mainForm = callingForm as Form5GAT;
            InitializeComponent();
            VSA = new VSA(mainForm);
        }
        //public SetupVSA(string instrumentName)
        //{
        //    InitializeComponent();
        //    cboVSAList.Text = instrumentName;
        //}
        //Khai bao cac class 

        /*............................Cac ham click Button tren giao dien..............................*/
        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Close(); 
        }
        public void updateIP(string VSA_name, string ipVSA)
        {
            cboVSAList.Text = VSA_name;
            cboIP.Text = ipVSA;
            Console.WriteLine(VSA_name + " IP: " + ipVSA);
        }
        private void setConnectedState(string manufacturer, string model, string serial)
        {
            connectionSTT = "VSA_Connected";
            lblMfrName.Text = manufacturer;
            lbSearialNumber.Text = model;
            lblModel.Text = serial;
            txtRespond.Text = manufacturer + '/' + model + '/' + serial;
            lblStatus.Text = "Connected";
            lblStatus.ForeColor = System.Drawing.Color.Green;
        }
        private void btnCheckInfo_Click(object sender, EventArgs e)
        {
            if (connectionSTT != "VSA_Connected")
            {
                string manufacturer = "ERROR";
                string model = "";
                string serial = "";
                string Cmd = "*IDN?";
                string IP = cboIP.Text;
                string instrument = cboVSAList.Text;
                this.mainForm.VSAConnection(IP, Cmd, instrument, out manufacturer, out model, out serial);
                if (instrument == model || instrument.Contains(model))
                {
                    setConnectedState(manufacturer, model, serial);
                }
                else
                {
                    connectionSTT = "VSA_Disconected";
                    txtRespond.Text = "No Connection !";
                    lblStatus.Text = "No Connection";
                    lblStatus.ForeColor = System.Drawing.Color.Red;
                }
            }
            else
            {

            }    
        }
        public void info(out string instrument, out string IP)
        {
            IP = cboIP.Text;
            instrument = cboVSAList.Text;
        }
        public void statusConnection(string manufacturer, string model, string serial)
        {
            connectionSTT = "VSA_Connected";
            lblMfrName.Text = manufacturer;
            lbSearialNumber.Text = model;
            lblModel.Text = serial;
            txtRespond.Text = manufacturer + '/' + model + '/' + serial;
            lblStatus.Text = "Connected";
            lblStatus.ForeColor = System.Drawing.Color.Green;
        }
        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                string Cmd = cboCmd.Text;
                string respond = "";
                VSA.SendCmd(Cmd, out respond);
                txtRespond.Text = respond;
            }
            catch
            {
                txtRespond.Text = "ERROR";
            }
        }

        public void StatusConnection(out string IP, out string sttConnect)
        {
            sttConnect = connectionSTT;
            IP = cboIP.Text;
        }

        public void flagConn(string flagConn)
        {
            connectionSTT = flagConn;
            if (connectionSTT == "VSA_Disconnected")
            {
                lblStatus.Text = "Disconnected";
                lblStatus.ForeColor = System.Drawing.Color.Red;
            }
            else if (connectionSTT == "VSA_Connected")
            {
                lblStatus.Text = "Connected";
                lblStatus.ForeColor = System.Drawing.Color.Green;
            }
        }

        private void cboInstrument_TextChanged(object sender, EventArgs e)
        {
            switch(cboVSAList.Text)
            {
                case "FSV3030":
                    cboIP.Text = "192.168.185.212";
                    break;
                case "N9020A":
                    cboIP.Text = "10.0.0.110";
                    break;
                case "N9020B":
                    cboIP.Text = "192.168.100.10";
                    break;
                case "M9410A":
                    cboIP.Text = "4";
                    break;
                case "IQFR1-RU":
                    cboIP.Text = "192.168.100.254"; break;
                case "FSW26":
                    cboIP.Text = "10.0.0.112"; break;
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            this.mainForm.VSADisconnect();
            connectionSTT = "VSA_Disconnected";
            lblStatus.Text = "Disconnected";
            lblStatus.ForeColor = System.Drawing.Color.Red;
        }

        private void SetupVSA_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.VSAIp=cboIP.Text;
            Properties.Settings.Default.Save();
        }

        private void SetupVSA_Load(object sender, EventArgs e)
        {
            cboIP.Text=Properties.Settings.Default.VSAIp;
        }
    }
}
