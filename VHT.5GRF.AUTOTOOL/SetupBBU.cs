using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _5GAutoTool
{
    public partial class SetupBBU : Form
    {
        public SetupBBU()
        {
            InitializeComponent();
        }
        //Khai bao cac bien global
        string manufacturer = "";
        string serial = "";
        string IP = "";
        string instrument = "";
        string user = "";
        string pass = "";
        bool connStatus = false;
        //CONFIGURATION
        private Form5GAT mainForm;
        string connectionSTT = "ERROR";
        public SetupBBU(Form callingForm)
        {
            mainForm = callingForm as Form5GAT;
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Hide();   //chỉ đóng cửa sổ thôi à???

        }
        public void updateIP(string ipDU)
        {
            cboIP.Text = ipDU;
        }
        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (connectionSTT != "DU_Connected")
            {
                try
                {
                    IP = cboIP.Text;
                    instrument = cboBBUList.Text;
                    user = txtUser.Text;
                    pass = txtPass.Text;
                    this.mainForm.BBUConnection(instrument, IP, user, pass, out connStatus);
                }
                catch
                {
                    lblStatus.Text = "ERROR";
                    lblStatus.ForeColor = System.Drawing.Color.Red;
                    txtBBURespond.Text = "ERROR";
                }
                if (connStatus == true)
                {
                    if (instrument == "XEON D")
                    {
                        connectionSTT = "DU_Connected";
                        lblMfrName.Text = "DELL";
                        lbSearialNumber.Text = "";
                        lblModel.Text = "XEON D";
                        txtBBURespond.Text = "Connected";
                        lblStatus.Text = "Connected";
                        lblStatus.ForeColor = System.Drawing.Color.Green;
                    }
                }
                else
                {
                    connectionSTT = "Disconected";
                    txtBBURespond.Text = "No Connection !";
                    lblStatus.Text = "Not Connected";
                    lblStatus.ForeColor = System.Drawing.Color.Red;
                }
            }
            else
            {
                Console.WriteLine("XEON Connected");
            }
        }
        public void infor(out string instrument, out string IP , out int port, out string user, out string pass)
        {
            IP = cboIP.Text;
            instrument = cboBBUList.Text;
            user = txtUser.Text;
            pass = txtPass.Text;
            port = int.Parse(cboPort.Text);
        }
        public void statusConnection()
        {
            connectionSTT = "DU_Connected";
            lblMfrName.Text = "DELL";
            lbSearialNumber.Text = "XEON Gold";                           
            lblModel.Text = "R740";
            txtBBURespond.Text = "Connected";
            lblStatus.Text = "Connected";
            lblStatus.ForeColor = System.Drawing.Color.Green;
        }
        public void statusConnection(out string IP, out string sttConnect)
        {
            sttConnect = connectionSTT;
            IP = cboIP.Text;
        }
        public void flagConn(string flagConn)
        {
            connectionSTT = flagConn;
            if (connectionSTT == "DU_Disconnected")
            {
                lblStatus.Text = "Disconnected";
                lblStatus.ForeColor = System.Drawing.Color.Red;
            }
            else if (connectionSTT == "DU_Connected")
            {
                lblStatus.Text = "Connected";
                lblStatus.ForeColor = System.Drawing.Color.Green;
            }
        }

        private void cboBBUList_SelectedValueChanged(object sender, EventArgs e)
        {
            if(cboBBUList.Text == "XEON Gold")
            {
                cboIP.Text = "192.168.120.3";
                txtUser.Text = "root";
                txtPass.Text = "1";
            }
            else if(cboBBUList.Text == "DL380")
            {
                cboIP.Text = "192.168.120.186";
                txtUser.Text = "root";
                txtPass.Text = "root";
            }
            else if (cboBBUList.Text == "XEON D")
            {
                cboIP.Text = "192.168.120.187";
                txtUser.Text = "root";
                txtPass.Text = "root";
            }
            else
            {

            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            lblStatus.ForeColor = System.Drawing.Color.Red;
            lblStatus.Text = "Disconnected";
            connectionSTT = "DU_Disconnected";
        }

        private void CboIP_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void GrpVSA_Enter(object sender, EventArgs e)
        {

        }

        private void TxtBBURespond_TextChanged(object sender, EventArgs e)
        {

        }

        private void TxtPass_TextChanged(object sender, EventArgs e)
        {

        }

        private void TxtUser_TextChanged(object sender, EventArgs e)
        {

        }

        private void LblResponse_Click(object sender, EventArgs e)
        {

        }

        private void LblPassword_Click(object sender, EventArgs e)
        {

        }

        private void LblConnPort_Click(object sender, EventArgs e)
        {

        }

        private void LblUser_Click(object sender, EventArgs e)
        {

        }

        private void LblInterface_Click(object sender, EventArgs e)
        {

        }

        private void CboInterface_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void CboPort_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void CboBBUList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void LblAddress_Click(object sender, EventArgs e)
        {

        }

        private void LblBBUName_Click(object sender, EventArgs e)
        {

        }
    }
}
