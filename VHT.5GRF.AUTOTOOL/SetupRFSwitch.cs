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
    public partial class SetupRFSwitch : Form
    {
        /// <summary>
        /// Define class: SetupRFSwitch
        /// </summary>
        public SetupRFSwitch()
        {
            InitializeComponent();
            cboIP1.Text = "10.0.0.25";
            cboIP2.Text = "10.0.0.24";
            cboIP3.Text = "10.0.0.22";
            //rfSwitch = new RFSwitch();

        }
        RFSwitch rfSwitch;
        private Form5GAT mainForm = null;
        bool flagCon1 = false, flagCon2 = false, flagCon3 = false;
        /// <summary>
        /// Define class: SetupRFSwitch with 1 Argument
        /// </summary>
        /// <param name="callingForm"></param>
        public SetupRFSwitch(Form callingForm)
        {
            mainForm = callingForm as Form5GAT;
            InitializeComponent();
        }
        public void updateIP(string ipRFS1, string ipRFS2, string ipRFS3)
        {
            cboIP1.Text = ipRFS1;
            cboIP2.Text = ipRFS2;
            cboIP3.Text = ipRFS3;
            Console.WriteLine("RF switch 1 IP:" + ipRFS1);
            Console.WriteLine("RF switch 2 IP:" + ipRFS2);
            Console.WriteLine("RF switch 3 IP:" + ipRFS3);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Port"></param>
        /// <param name="Ax"></param>
        public void RFSwitchSetup(out string rfs1Ax, out string rfs2Ax, out string rfs3Ax)
        {
            rfs1Ax = "A1";
            rfs2Ax = "A2";
            rfs3Ax = "A3";
            if (cbxRF1A1.Checked)
            {
                rfs1Ax = "A1";
            }
            else
            {
                rfs1Ax = "A2";
            }
            if (cbxRF2A1.Checked)
            {
                rfs2Ax = "A1";
            }
            else
            {
                rfs2Ax = "A2";
            }
            if (cbxRF3A1.Checked)
            {
                rfs3Ax = "A1";
            }
            else
            {
                rfs3Ax = "A2";
            }
        }

        /// <summary>
        /// Khai báo biến
        /// </summary>
        string[] portNode = new string[8];
        string N1, N2, N3, N4, N5, N6, N7, N8, N9, N10, N11, N12, N13, N14, N15, N16;
        string[] N = new string[16];

        string[] arrN = new string[16];
        string[] arrA = new string[2];
        string[] arrSwitchA = new string[2];

        //+++++++++++++++++++ EVENT HANDLER +++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// BUTTON 1: btnCheckConn
        /// Kiểm tra kết nối với RFSwitch
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            if (cboIP1.Text != "" && cboIP2.Text != "" && cboIP3.Text != "")
            {
                if (flagCon1 && flagCon2 && flagCon3)
                {
                    Console.WriteLine("RFSwitch Connected");
                }
                else
                {
                    string IP = cboIP1.Text;
                    bool checkCON = false;
                    this.mainForm.RFSwitchConnection1(IP, 23, out checkCON);
                    if (checkCON)
                    {
                        flagCon1 = true;
                        lbStatus1.Text = "Connected";
                        lbStatus1.ForeColor = System.Drawing.Color.Green;
                        //rfSwitch = mainForm.rfSwitch1;
                    }
                    else
                    {
                        flagCon1 = false;
                        lbStatus1.Text = "No Connection";
                        lbStatus1.ForeColor = System.Drawing.Color.Red;
                    }
                    IP = cboIP2.Text;
                    this.mainForm.RFSwitchConnection2(IP, 23, out checkCON);
                    if (checkCON)
                    {
                        flagCon2 = true;
                        lbStatus2.Text = "Connected";
                        lbStatus2.ForeColor = System.Drawing.Color.Green;
                    }
                    else
                    {
                        flagCon2 = false;
                        lbStatus2.Text = "No Connection";
                        lbStatus2.ForeColor = System.Drawing.Color.Red;
                    }
                    IP = cboIP3.Text;
                    this.mainForm.RFSwitchConnection3(IP, 23, out checkCON);
                    if (checkCON)
                    {
                        flagCon3 = true;
                        lbStatus3.Text = "Connected";
                        lbStatus3.ForeColor = System.Drawing.Color.Green;
                    }
                    else
                    {
                        flagCon3 = false;
                        lbStatus3.Text = "No Connection";
                        lbStatus3.ForeColor = System.Drawing.Color.Red;
                    }
                }
            }
            else if (cboIP2.Text == "" && cboIP3.Text == "")
            {
                if (flagCon1)
                {
                    Console.WriteLine("RFSwitch Connected");
                }
                else
                {
                    string IP = cboIP1.Text;
                    bool checkCON = false;
                    this.mainForm.RFSwitchConnection1(IP, 23, out checkCON);
                    if (checkCON)
                    {
                        flagCon1 = true;
                        lbStatus1.Text = "Connected";
                        lbStatus1.ForeColor = System.Drawing.Color.Green;
                    }
                    else
                    {
                        flagCon1 = false;
                        lbStatus1.Text = "No Connection";
                        lbStatus1.ForeColor = System.Drawing.Color.Red;
                    }
                }
            }

        }

        /// <summary>
        /// BUTTON 5: btnSwitchSetup_Click EventHandler
        /// </summary>
        private void btnSwitchSetup_Click(object sender, EventArgs e)
        {
            // đọc giá trị setting vào mảng
            //CopyNodetoPortArray(portArr);
            // LoadPorttoNodeArray(arrN, "cboPortN");

            //disable all comboboxes to lock Settings
            foreach (Control itemControl in this.Controls)
            {
                ComboBox cboSelect = itemControl as ComboBox;
                if (cboSelect != null)
                {
                    cboSelect.Enabled = false;
                }
            }
            this.Close();
        }

        // COMBOBOX VALUE CHANGED EVENT=====================

        /// <summary>
        /// cboPortN1_SelectedValueChanged
        /// </summary>
        private void cboPortN1_SelectedValueChanged(object sender, EventArgs e)
        {
            ComboBox cboNx = sender as ComboBox;
            cboPortNSelect(cboNx, 16);
        }

        /// <summary>
        /// cboPortN2_SelectedValueChanged
        /// </summary>
        private void cboPortN2_SelectedValueChanged(object sender, EventArgs e)
        {
            ComboBox cboNx = sender as ComboBox;
            cboPortNSelect(cboNx, 16);
        }

        /// <summary>
        /// cboPortN3_SelectedValueChanged
        /// </summary>
        private void cboPortN3_SelectedValueChanged(object sender, EventArgs e)
        {
            ComboBox cboNx = sender as ComboBox;
            cboPortNSelect(cboNx, 16);
        }
        /// <summary>
        /// cboPortN4_SelectedValueChanged
        /// </summary>
        private void cboPortN4_SelectedValueChanged(object sender, EventArgs e)
        {
            ComboBox cboNx = sender as ComboBox;
            cboPortNSelect(cboNx, 16);
        }

        /// <summary>
        /// cboPortN5_SelectedValueChanged
        /// </summary>
        private void cboPortN5_SelectedValueChanged(object sender, EventArgs e)
        {
            ComboBox cboNx = sender as ComboBox;
            cboPortNSelect(cboNx, 16);
        }

        private void btnSwitch_Click(object sender, EventArgs e)
        {
            string Aport = cboA.Text;
            string Nport = cboN.Text;
            string Respon;
            if (cboSwitch.Text == "SWITCH1")
            {
                this.mainForm.SetSwitchRFS1(Aport, Nport, out Respon);
                if (Respon == "1 - Success")
                {
                    lblSTT.Text = "Connected";
                    lblSTT.ForeColor = System.Drawing.Color.Green;
                }
                else
                {
                    lblSTT.Text = "No Connection";
                    lblSTT.ForeColor = System.Drawing.Color.Red;
                }

            }
            else if (cboSwitch.Text == "SWITCH2")
            {
                this.mainForm.SetSwitchRFS2(Aport, Nport, out Respon);
                if (Respon == "1 - Success")
                {
                    lblSTT.Text = "Connected";
                    lblSTT.ForeColor = System.Drawing.Color.Green;
                }
                else
                {
                    lblSTT.Text = "No Connection";
                    lblSTT.ForeColor = System.Drawing.Color.Red;
                }
            }
            else if (cboSwitch.Text == "SWITCH3")
            {
                this.mainForm.SetSwitchRFS3(Aport, Nport, out Respon);
                if (Respon == "1 - Success")
                {
                    lblSTT.Text = "Connected";
                    lblSTT.ForeColor = System.Drawing.Color.Green;
                }
                else
                {
                    lblSTT.Text = "No Connection";
                    lblSTT.ForeColor = System.Drawing.Color.Red;
                }
            }

        }

        private void cbxRF2A1_CheckedChanged(object sender, EventArgs e)
        {
            if (cbxRF2A1.Checked)
            {
                cbxRF2A2.Checked = false;
            }
        }

        private void cbxRF2A2_CheckedChanged(object sender, EventArgs e)
        {
            if (cbxRF2A2.Checked)
            {
                cbxRF2A1.Checked = false;
            }
        }

        private void cbxRF3A1_CheckedChanged(object sender, EventArgs e)
        {
            if (cbxRF3A1.Checked)
            {
                cbxRF3A2.Checked = false;
            }
        }

        private void cbxRF3A2_CheckedChanged(object sender, EventArgs e)
        {
            if (cbxRF3A2.Checked)
            {
                cbxRF3A1.Checked = false;
            }
        }

        private void cbxRF1A1_CheckedChanged(object sender, EventArgs e)
        {
            if (cbxRF1A1.Checked)
            {
                cbxRF1A2.Checked = false;
            }
        }

        private void cboSwitch_SelectedValueChanged(object sender, EventArgs e)
        {
            switch (cboSwitch.SelectedItem?.ToString())
            {
                case "SWITCH1":
                    rfSwitch = mainForm.rfSwitch1;
                    break;
                case "SWITCH2":
                    rfSwitch = mainForm.rfSwitch2;
                    break;
                case "SWITCH3":
                    rfSwitch = mainForm.rfSwitch3;
                    break;
                default:
                    break;
            }
            if (rfSwitch.Model.Contains("MN=RC-2SPDT-A18"))
            {
                cboA.Items.Clear();
                cboA.Items.Add("A");
                cboA.Items.Add("B");

                cboN.Items.Clear();
                cboN.Items.Add("0");
                cboN.Items.Add("1");

            }
            else if (rfSwitch.Model.Contains("MN=ZTVX-16"))
            {
                cboA.Items.Clear();
                cboA.Items.Add("A1");
                cboA.Items.Add("A2");

                cboN.Items.Clear();
                for (int i = 1; i <= 16; i++)
                {
                    cboN.Items.Add($"N{i}");
                }
            }
            else if (rfSwitch.Model.Contains("MN=ZTVX-8"))
            {
                cboA.Items.Clear();
                cboA.Items.Add("A1");
                cboA.Items.Add("A2");

                cboN.Items.Clear();
                for (int i = 1; i <= 8; i++)
                {
                    cboN.Items.Add($"N{i}");
                }                
            }
            cboA.SelectedIndex = 0;
            cboN.SelectedIndex = 0;
        }

        private void cbxRF1A2_CheckedChanged(object sender, EventArgs e)
        {
            if (cbxRF1A2.Checked)
            {
                cbxRF1A1.Checked = false;
            }
        }

        /// <summary>
        /// cboPortN6_SelectedValueChanged
        /// </summary>
        private void cboPortN6_SelectedValueChanged(object sender, EventArgs e)
        {
            ComboBox cboNx = sender as ComboBox;
            cboPortNSelect(cboNx, 16);
        }

        private void picSWInterface_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// cboPortN7_SelectedValueChanged
        /// </summary>
        private void cboPortN7_SelectedValueChanged(object sender, EventArgs e)
        {
            ComboBox cboNx = sender as ComboBox;
            cboPortNSelect(cboNx, 16);
        }

        /// <summary>
        /// cboPortN8_SelectedValueChanged
        /// </summary>
        private void cboPortN8_SelectedValueChanged(object sender, EventArgs e)
        {
            ComboBox cboNx = sender as ComboBox;
            cboPortNSelect(cboNx, 16);
        }

        /// <summary>
        /// cboPortN9_SelectedValueChanged
        /// </summary>
        private void cboPortN9_SelectedValueChanged(object sender, EventArgs e)
        {
            ComboBox cboNx = sender as ComboBox;
            cboPortNSelect(cboNx, 16);
        }

        /// <summary>
        /// cboPortN10_SelectedValueChanged
        /// </summary>
        private void cboPortN10_SelectedValueChanged(object sender, EventArgs e)
        {
            ComboBox cboNx = sender as ComboBox;
            cboPortNSelect(cboNx, 16);
        }

        /// <summary>
        /// cboPortN11_SelectedValueChanged
        /// </summary>
        private void cboPortN11_SelectedValueChanged(object sender, EventArgs e)
        {
            ComboBox cboNx = sender as ComboBox;
            cboPortNSelect(cboNx, 16);
        }

        /// <summary>
        /// cboPortN12_SelectedValueChanged
        /// </summary>
        private void cboPortN12_SelectedValueChanged(object sender, EventArgs e)
        {
            ComboBox cboNx = sender as ComboBox;
            cboPortNSelect(cboNx, 16);
        }

        /// <summary>
        /// cboPortN13_SelectedValueChanged
        /// </summary>
        private void cboPortN13_SelectedValueChanged(object sender, EventArgs e)
        {
            ComboBox cboNx = sender as ComboBox;
            cboPortNSelect(cboNx, 16);
        }

        /// <summary>
        /// cboPortN14_SelectedValueChanged
        /// </summary>
        private void cboPortN14_SelectedValueChanged(object sender, EventArgs e)
        {
            ComboBox cboNx = sender as ComboBox;
            cboPortNSelect(cboNx, 16);
        }

        /// <summary>
        /// cboPortN15_SelectedValueChanged
        /// </summary>
        private void cboPortN15_SelectedValueChanged(object sender, EventArgs e)
        {
            ComboBox cboNx = sender as ComboBox;
            cboPortNSelect(cboNx, 16);
        }

        /// <summary>
        /// cboPortN16_SelectedValueChanged
        /// </summary>
        private void cboPortN16_SelectedValueChanged(object sender, EventArgs e)
        {
            ComboBox cboNx = sender as ComboBox;
            cboPortNSelect(cboNx, 16);
        }

        //++++++++++++++++++++++ VOID REFERENCE +++++++++++++++++++++=+++++++++++++++++++++++++++++++++++++++++++++

        //VOID PORT SELECTION

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cboPortN"></param> Port Number Selection from ComboBox
        /// <param name="cboNumber"></param> Number of Combobox that has the Nameprefix is "cboPortN"
        private void cboPortNSelect(ComboBox cboPort, int cboNumber)
        {
            for (int i = 0; i < cboNumber; i++)
            {
                string cboNIndex = "cboPortN" + (i + 1);
                ComboBox cboN = (ComboBox)this.Controls.Find(cboNIndex, true)[0];

                if (cboN.Name != cboPort.Name && cboN.Text == cboPort.Text)
                {
                    cboN.Text = "N/A";
                }
            }
        }
        /// <summary>
        /// cboPortA1_SelectedValueChanged
        /// </summary>
        private void cboPortA1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cboAx = sender as ComboBox;
            cboPortASelect(cboAx, 2);
        }
        /// <summary>
        /// cboPortA2_SelectedValueChanged
        /// </summary>
        private void cboPortA2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cboAx = sender as ComboBox;
            cboPortASelect(cboAx, 2);
        }
        /// </summary>
        /// <param name="cboPortA"></param> Port Number Selection from ComboBox
        /// <param name="cboNumber"></param> Number of Combobox that has the Nameprefix is "cboPortN"
        private void cboPortASelect(ComboBox cboPort, int cboNumber)
        {
            for (int i = 0; i < cboNumber; i++)
            {
                string cboNIndex = "cboPortA" + (i + 1);
                ComboBox cboA = (ComboBox)this.Controls.Find(cboNIndex, true)[0];

                if (cboA.Name != cboPort.Name && cboA.Text == cboPort.Text)
                {
                    cboA.Text = "N/A";
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="cboNameprefix"></param>
        //private void LoadPorttoNodeArray(string[] arr, string cboNameprefix) 
        //{
        //    for (int i = 0; i < arr.Length; i++) 
        //    {
        //        ComboBox comboBox = (ComboBox)this.Controls.Find(cboNameprefix + (i + 1), true)[0];
        //        arr[i] = comboBox.Text;           
        //    }
        //} 
    }
}