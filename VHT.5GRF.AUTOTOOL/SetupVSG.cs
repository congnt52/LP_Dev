using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace _5GAutoTool
{
    /// <summary>
    /// Giao diện cấu hình cho máy đo VSG
    /// </summary>
    public partial class SetupVSG : Form
    {
        /// <summary>
        /// SetupVSG
        /// </summary>
        public SetupVSG()
        {
            InitializeComponent();
        }
        /*................................Khai bao Class va bien global...............................*/
        //Khai bao cac bien global
        VSG VSG1, VSG2, VSG3;
        //CONFIGURATION
        private Form5GAT mainForm = null;
        string connectionSTT1 = "ERROR", connectionSTT2 = "ERROR" , connectionSTT3 = "ERROR";
        /// <summary>
        /// SetupVSG
        /// </summary>
        /// <param name="callingForm"></param>
        public SetupVSG(Form callingForm)
        {
            mainForm = callingForm as Form5GAT;           
            InitializeComponent();
            VSG1 = new VSG(mainForm);
            VSG2 = new VSG(mainForm);
            VSG3 = new VSG(mainForm);
        }
        /// <summary>
        /// SetupVSG
        /// </summary>
        /// <param name="VSGName"></param> Gán giá trị tên máy đo mặc định ban đầu trong cửa sổ chọn ComboBox
        //public SetupVSG(string VSGName)
        //{
        //    InitializeComponent();
        //    cboVSGList1.Text = VSGName;

        //}
        public void updateIP(string ipVSG1, string ipVSG2, string ipVSG3)
        {
            cboIP1.Text = ipVSG1;
            cboIP2.Text = ipVSG2;
            cboIP3.Text = ipVSG3;
        }
        private void btnCheckInf_Click(object sender, EventArgs e)
        {
            btnSend.Enabled = true;
            string manufacturer1 = "";
            string model1 = "";
            string serial1 = "";
            string Cmd1 = "*IDN?";
            string IP1 = cboIP1.Text;
            string instrument1 = cboVSGList1.Text;

            string manufacturer2 = "";
            string model2 = "";
            string serial2 = "";
            string Cmd2 = "*IDN?";
            string IP2 = cboIP2.Text;
            string instrument2 = cboVSGList2.Text;

            string manufacturer3 = "";
            string model3 = "";
            string serial3 = "";
            string Cmd3 = "*IDN?";
            string IP3 = cboIP3.Text;
            string instrument3 = cboVSGList3.Text;
            if (SL_1VSG.Checked)
            {
                if(connectionSTT1 != "VSG1_Connected")
                {
                    this.mainForm.VSG1Connection(IP1, Cmd1, instrument1, out manufacturer1, out model1, out serial1);
                    if (model1 == "CMW" || manufacturer1 == "Agilent Technologies" || manufacturer1 == "Rohde&Schwarz" || model1 == "N5182B"|| model1 == "M9410A")
                    {
                        connectionSTT1 = "VSG1_Connected";
                        lblMfrName1.Text = manufacturer1;
                        lbSearialNumber1.Text = serial1;
                        lblModel1.Text = model1;
                        lblStatus1.Text = "Connected";
                        lblStatus1.ForeColor = System.Drawing.Color.Green;
                        
                    }
                    else
                    {
                        connectionSTT1 = "VSG1_Disconected";
                        lblStatus1.Text = "Not Connected";
                        lblStatus1.ForeColor = System.Drawing.Color.Red;
                    }
                }
                
            }
            else if (SL_2VSG.Checked)
            {
                if (connectionSTT1 != "VSG1_Connected" || connectionSTT2 != "VSG2_Connected")
                {
                    VSG1.Instrument(instrument1);
                    this.mainForm.VSG1Connection(IP1, Cmd1, instrument1, out manufacturer1, out model1, out serial1);
                    if (model1 == "CMW" || manufacturer1 == "Agilent Technologies" || manufacturer1 == "Rohde&Schwarz" || manufacturer1 == "Keysight" || model1 == "N5182B" || model1 == "M9410A")
                    {
                        connectionSTT1 = "VSG1_Connected";
                        lblMfrName1.Text = manufacturer1;
                        lbSearialNumber1.Text = serial1;
                        lblModel1.Text = model1;
                        lblStatus1.Text = "Connected";
                        lblStatus1.ForeColor = System.Drawing.Color.Green;
                    }
                    else
                    {
                        connectionSTT1 = "VSG1_Disconected";
                        lblStatus1.Text = "Not Connected";
                        lblStatus1.ForeColor = System.Drawing.Color.Red;
                    }
                    System.Threading.Thread.Sleep(300);
                    this.mainForm.VSG2Connection(IP2, Cmd2, instrument2, out manufacturer2, out model2, out serial2);
                    if (model2 == "CMW" || model2 == "SGS" || model2 == "N5182B" || manufacturer2 == "Agilent Technologies" || manufacturer2 == "Rohde&Schwarz" || model2 == "M9410A")
                    {
                        connectionSTT2 = "VSG2_Connected";
                        lblMfrName2.Text = manufacturer2;
                        lbSearialNumber2.Text = serial2;
                        lblModel2.Text = model2;
                        lblStatus2.Text = "Connected";
                        lblStatus2.ForeColor = System.Drawing.Color.Green;
                    }
                    else
                    {
                        connectionSTT2 = "VSG2_Disconected";
                        lblStatus2.Text = "Not Connected";
                        lblStatus2.ForeColor = System.Drawing.Color.Red;
                    }
                }                   
            }
            else if (SL_3VSG.Checked)
            {
                if (connectionSTT1 != "VSG1_Connected" || connectionSTT2 != "VSG2_Connected" || connectionSTT3 != "VSG3_Connected")
                {                   
                    this.mainForm.VSG1Connection(IP1, Cmd1, instrument1, out manufacturer1, out model1, out serial1);
                    if (model1 == "CMW" || manufacturer1 == "Agilent Technologies" || manufacturer1 == "Rohde&Schwarz" || model1 == "N5182B")
                    {
                        connectionSTT1 = "VSG1_Connected";
                        lblMfrName1.Text = manufacturer1;
                        lbSearialNumber1.Text = serial1;
                        lblModel1.Text = model1;
                        lblStatus1.Text = "Connected";
                        lblStatus1.ForeColor = System.Drawing.Color.Green;
                    }
                    else
                    {
                        connectionSTT1 = "VSG1_Disconected";
                        lblStatus1.Text = "Not Connected";
                        lblStatus1.ForeColor = System.Drawing.Color.Red;
                    }
                    System.Threading.Thread.Sleep(300);
                    this.mainForm.VSG2Connection(IP2, Cmd2, instrument2, out manufacturer2, out model2, out serial2);
                    if (model2 == "CMW" || model2 == "SGS" || model2 == "N5182B" || manufacturer2 == "Agilent Technologies" || manufacturer2 == "Rohde&Schwarz" || model2 == "M9410A")
                    {
                        connectionSTT2 = "VSG2_Connected";
                        lblMfrName2.Text = manufacturer2;
                        lbSearialNumber2.Text = serial2;
                        lblModel2.Text = model2;
                        lblStatus2.Text = "Connected";
                        lblStatus2.ForeColor = System.Drawing.Color.Green;
                    }
                    else
                    {
                        connectionSTT2 = "VSG2_Disconected";
                        lblStatus2.Text = "Not Connected";
                        lblStatus2.ForeColor = System.Drawing.Color.Red;
                    }
                    System.Threading.Thread.Sleep(300);
                    this.mainForm.VSG3Connection(IP3, Cmd3, instrument3, out manufacturer3, out model3, out serial3);
                    if (model3 == "CMW" || model3 == "SGS" || model3 == "N5182B" || manufacturer3 == "Agilent Technologies" || manufacturer3 == "Rohde&Schwarz" || model3 == "M9410A")
                    {
                        connectionSTT3 = "VSG3_Connected";
                        lblMfrName3.Text = manufacturer3;
                        lbSearialNumber3.Text = serial3;
                        lblModel3.Text = model3;
                        lblStatus3.Text = "Connected";
                        lblStatus3.ForeColor = System.Drawing.Color.Green;
                    }
                    else
                    {
                        connectionSTT3 = "VSG3_Disconected";
                        lblStatus3.Text = "Not Connected";
                        lblStatus3.ForeColor = System.Drawing.Color.Red;
                    }
                }
            }            
        }
        public void StatusConnection1(out string IP1, out string sttConnect1)
        {
            sttConnect1 = connectionSTT1;
            IP1 = cboIP1.Text;
        }

        public void flagConn1(string flagConn1)
        {
            connectionSTT1 = flagConn1;
            if (connectionSTT1 == "VSG1_Disconnected")
            {
                lblStatus1.Text = "Disconnected";
                lblStatus1.ForeColor = System.Drawing.Color.Red;
            }
            else if (connectionSTT1 == "VSG1_Connected")
            {
                lblStatus1.Text = "Connected";
                lblStatus1.ForeColor = System.Drawing.Color.Green;
            }
        }
        public void StatusConnection2(out string IP2, out string sttConnect2)
        {
            sttConnect2 = connectionSTT2;
            IP2 = cboIP2.Text;
        }

        public void flagConn2(string flagConn2)
        {
            connectionSTT2 = flagConn2;
            if (connectionSTT2 == "VSG2_Disconnected")
            {
                lblStatus2.Text = "Disconnected";
                lblStatus2.ForeColor = System.Drawing.Color.Red;
            }
            else if (connectionSTT2 == "VSG2_Connected")
            {
                lblStatus2.Text = "Connected";
                lblStatus2.ForeColor = System.Drawing.Color.Green;
            }
        }
        public void StatusConnection3(out string IP3, out string sttConnect3)
        {
            sttConnect3 = connectionSTT3;
            IP3 = cboIP3.Text;
        }

        public void flagConn3(string flagConn3)
        {
            connectionSTT3 = flagConn3;
            if (connectionSTT3 == "VSG3_Disconnected")
            {
                lblStatus3.Text = "Disconnected";
                lblStatus3.ForeColor = System.Drawing.Color.Red;
            }
            else if (connectionSTT3 == "VSG3_Connected")
            {
                lblStatus3.Text = "Connected";
                lblStatus3.ForeColor = System.Drawing.Color.Green;
            }
        }
        private void cboInstrument_TextChanged(object sender, EventArgs e)
        {            
            if (cboVSGList1.Text == "N5182B")
            {
                if (File.Exists(@"..\..\..\images\" + "VSG2" + ".png"))
                {
                    picVSG.Image = Image.FromFile(@"..\..\..\images\" + "VSG2" + ".png");
                }
                else if (File.Exists(@"\images\" + "VSG2" + ".png"))
                {
                    picVSG.Image = Image.FromFile(@"\images\" + "VSG2" + ".png");
                }
            }
            if (cboVSGList1.Text == "SMW200A")
            {
                if (File.Exists(@"..\..\..\images\" + "SMW200A" + ".png"))
                {
                    picVSG.Image = Image.FromFile(@"..\..\..\images\" + "SMW200A" + ".png");
                }
                else if (File.Exists(@"\images\" + "SMW200A" + ".png"))
                {
                    picVSG.Image = Image.FromFile(@"\images\" + "SMW200A" + ".png");
                }

            }
            if (cboVSGList2.Text == "N5182B")
            {
                if (File.Exists(@"..\..\..\images\" + "VSG2" + ".png"))
                {
                    picVSG2.Image = Image.FromFile(@"..\..\..\images\" + "VSG2" + ".png");
                }
                if (File.Exists(@"\images\" + "VSG2" + ".png"))
                {
                    picVSG2.Image = Image.FromFile(@"\images\" + "VSG2" + ".png");
                }

            }
            if (cboVSGList2.Text == "SMW200A")
            {
                if (File.Exists(@"..\..\..\images\" + "SMW200A" + ".png"))
                {
                    picVSG2.Image = Image.FromFile(@"..\..\..\images\" + "SMW200A" + ".png");
                }
                if (File.Exists(@"\images\" + "SMW200A" + ".png"))
                {
                    picVSG2.Image = Image.FromFile(@"\images\" + "SMW200A" + ".png");
                }

            }
            if (cboVSGList2.Text == "SGS100A")
            {
                if (File.Exists(@"..\..\..\images\" + "SGS100A" + ".png"))
                {
                    picVSG2.Image = Image.FromFile(@"..\..\..\images\" + "SGS100A" + ".png");
                }
                if (File.Exists(@"\images\" + "SGS100A" + ".png"))
                {
                    picVSG2.Image = Image.FromFile(@"\images\" + "SGS100A" + ".png");
                }
            }
            if (cboVSGList3.Text == "N5182B")
            {
                if (File.Exists(@"..\..\..\images\" + "VSG2" + ".png"))
                {
                    picVSG3.Image = Image.FromFile(@"..\..\..\images\" + "VSG2" + ".png");
                }
                if (File.Exists(@"\images\" + "VSG2" + ".png"))
                {
                    picVSG3.Image = Image.FromFile(@"\images\" + "VSG2" + ".png");
                }

            }
            if (cboVSGList3.Text == "SMW200A")
            {
                if (File.Exists(@"..\..\..\images\" + "SMW200A" + ".png"))
                {
                    picVSG3.Image = Image.FromFile(@"..\..\..\images\" + "SMW200A" + ".png");
                }
                if (File.Exists(@"\images\" + "SMW200A" + ".png"))
                {
                    picVSG3.Image = Image.FromFile(@"\images\" + "SMW200A" + ".png");
                }

            }
            if (cboVSGList3.Text == "SGS100A")
            {
                if (File.Exists(@"..\..\..\images\" + "SGS100A" + ".png"))
                {
                    picVSG3.Image = Image.FromFile(@"..\..\..\images\" + "SGS100A" + ".png");
                }
                if (File.Exists(@"\images\" + "SGS100A" + ".png"))
                {
                    picVSG3.Image = Image.FromFile(@"\images\" + "SGS100A" + ".png");
                }

            }
        }
        private void btnSend_Click(object sender, EventArgs e)
        {
            string vsg = cboVSG.Text;
            string Cmd = cboCmd.Text;
            string respond = "";
            this.mainForm.VSGSendCmd(vsg, Cmd, out respond);
            txtRespond.Text = respond;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SL_1VSG_CheckedChanged(object sender, EventArgs e)
        {
            cboVSGList1.Enabled = true;
            cboIP1.Enabled = true;
            cboVSGList2.Enabled = false;
            cboIP2.Enabled = false;
            cboVSGList3.Enabled = false;
            cboIP3.Enabled = false;
        }

        private void SL_2VSG_CheckedChanged(object sender, EventArgs e)
        {
            cboVSGList1.Enabled = true;
            cboIP1.Enabled = true;
            cboVSGList2.Enabled = true;
            cboIP2.Enabled = true;
            cboVSGList3.Enabled = false;
            cboIP3.Enabled = false;
        }

        private void SL_3VSG_CheckedChanged(object sender, EventArgs e)
        {
            cboVSGList1.Enabled = true;
            cboIP1.Enabled = true;
            cboVSGList2.Enabled = true;
            cboIP2.Enabled = true;
            cboVSGList3.Enabled = true;
            cboIP3.Enabled = true;
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            if (SL_1VSG.Checked)
            {
                this.mainForm.VSGD1isconnect();
                connectionSTT1 = "VSG1_Disconnected";
                lblStatus1.Text = "Disconnected";
                lblStatus1.ForeColor = System.Drawing.Color.Red;
            }
            else if (SL_2VSG.Checked)
            {
                this.mainForm.VSGD2isconnect();
                connectionSTT1 = "VSG1_Disconnected";
                lblStatus1.Text = "Disconnected";
                lblStatus1.ForeColor = System.Drawing.Color.Red;
                connectionSTT2 = "VSG2_Disconnected";
                lblStatus2.Text = "Disconnected";
                lblStatus2.ForeColor = System.Drawing.Color.Red;
            }
            else if (SL_3VSG.Checked)
            {
                this.mainForm.VSGD3isconnect();
                connectionSTT1 = "VSG1_Disconnected";
                lblStatus1.Text = "Disconnected";
                lblStatus1.ForeColor = System.Drawing.Color.Red;
                connectionSTT2 = "VSG2_Disconnected";
                lblStatus2.Text = "Disconnected";
                lblStatus2.ForeColor = System.Drawing.Color.Red;
                connectionSTT3 = "VSG3_Disconnected";
                lblStatus3.Text = "Disconnected";
                lblStatus3.ForeColor = System.Drawing.Color.Red;
            }          
        }

        private void cboIP1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cboIP = sender as ComboBox;
            cboIPSelect(cboIP, 3);
        }

        private void cboIP2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cboIP = sender as ComboBox;
            cboIPSelect(cboIP, 3);
        }

        private void cboIP3_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cboIP = sender as ComboBox;
            cboIPSelect(cboIP, 3);
        }
        private void cboIPSelect(ComboBox cboIP, int cboNumber)
        {
            for (int i = 0; i < cboNumber; i++)
            {
                string cboNIndex = "cboIP" + (i + 1);
                ComboBox cboIPx = (ComboBox)this.Controls.Find(cboNIndex, true)[0];

                if (cboIPx.Name != cboIP.Name && cboIPx.Text == cboIP.Text)
                {
                    cboIPx.Text = "";
                }
            }
        }

    }
}

