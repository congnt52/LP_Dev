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
    public partial class SetupAttenuator : Form
    {
        public SetupAttenuator()
        {
            InitializeComponent();
        }
        public void attSetup(string[] AttDL, string[] AttUL, string AttIMD, string AttIMD1)
        {
            //lbDLPort1.Text = AttDL[0];
            //lbDLPort2.Text = AttDL[1];
            //lbDLPort3.Text = AttDL[2];
            //lbDLPort4.Text = AttDL[3];
            //lbDLPort5.Text = AttDL[4];
            //lbDLPort6.Text = AttDL[5];
            //lbDLPort7.Text = AttDL[6];
            //lbDLPort8.Text = AttDL[7];
            //lbDLPort9.Text = AttDL[8];
            //lbDLPort10.Text = AttDL[9];
            //lbDLPort11.Text = AttDL[10];
            //lbDLPort12.Text = AttDL[11];
            //lbDLPort13.Text = AttDL[12];
            //lbDLPort14.Text = AttDL[13];
            //lbDLPort15.Text = AttDL[14];
            //lbDLPort16.Text = AttDL[15];
            //lbDLPort17.Text = AttDL[16];
            //lbDLPort18.Text = AttDL[17];
            //lbDLPort19.Text = AttDL[18];
            //lbDLPort20.Text = AttDL[19];
            //lbDLPort21.Text = AttDL[20];
            //lbDLPort22.Text = AttDL[21];
            //lbDLPort23.Text = AttDL[22];
            //lbDLPort24.Text = AttDL[23];
            //lbDLPort25.Text = AttDL[24];
            //lbDLPort26.Text = AttDL[25];
            //lbDLPort27.Text = AttDL[26];
            //lbDLPort28.Text = AttDL[27];
            //lbDLPort29.Text = AttDL[28];
            //lbDLPort30.Text = AttDL[29];
            //lbDLPort31.Text = AttDL[30];
            //lbDLPort32.Text = AttDL[31];

            //lbULPort1.Text = AttUL[0];
            //lbULPort2.Text = AttUL[1];
            //lbULPort3.Text = AttUL[2];
            //lbULPort4.Text = AttUL[3];
            //lbULPort5.Text = AttUL[4];
            //lbULPort6.Text = AttUL[5];
            //lbULPort7.Text = AttUL[6];
            //lbULPort8.Text = AttUL[7];
            //lbULPort9.Text = AttUL[8];
            //lbULPort10.Text = AttUL[9];
            //lbULPort11.Text = AttUL[10];
            //lbULPort12.Text = AttUL[11];
            //lbULPort13.Text = AttUL[12];
            //lbULPort14.Text = AttUL[13];
            //lbULPort15.Text = AttUL[14];
            //lbULPort16.Text = AttUL[15];
            //lbULPort17.Text = AttUL[16];
            //lbULPort18.Text = AttUL[17];
            //lbULPort19.Text = AttUL[18];
            //lbULPort20.Text = AttUL[19];
            //lbULPort21.Text = AttUL[20];
            //lbULPort22.Text = AttUL[21];
            //lbULPort23.Text = AttUL[22];
            //lbULPort24.Text = AttUL[23];
            //lbULPort25.Text = AttUL[24];
            //lbULPort26.Text = AttUL[25];
            //lbULPort27.Text = AttUL[26];
            //lbULPort28.Text = AttUL[27];
            //lbULPort29.Text = AttUL[28];
            //lbULPort30.Text = AttUL[29];
            //lbULPort31.Text = AttUL[30];
            //lbULPort32.Text = AttUL[31];
            UpdateAttDL(AttDL);
            UpdateAttUL(AttUL);
            lbATTIMD.Text = AttIMD;
            lbATTNotchFilter.Text = AttIMD1;
        }
        public void UpdateAttDL(string[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                string lblDLIndex = "lbDLPort" + (i + 1); ;
                Label lblDL = (Label)this.Controls.Find(lblDLIndex, true)[0];
                lblDL.Text = arr[i];
            }
        }
        public void UpdateAttUL(string[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                string lblULIndex = "lbULPort" + (i + 1); ;
                Label lblUL = (Label)this.Controls.Find(lblULIndex, true)[0];
                lblUL.Text = arr[i];
            }
        }
        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOk_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
