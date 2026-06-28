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
    public partial class SetupRRU : Form
    {
        public SetupRRU()
        {
            InitializeComponent();
        }
        private Form5GAT mainForm = null;
        string connectionSTT = "ERROR";
        public SetupRRU(Form callingForm)
        {
            mainForm = callingForm as Form5GAT;
            InitializeComponent();
        }

        string manufacturer = "";
        string serial = "";
        string IP = "";
        string instrument = "";
        string user = "";
        string passWord = "";
        bool connStatus = false;
        private void BtnConnect_Click(object sender, EventArgs e)
        {
            IP = cboIP.Text;
            user = txtPass.Text;
            passWord = txtPass.Text;
            bool flag = false;
            this.mainForm.RRUConnection(IP, user, passWord , out flag);
        }
        public void rruInfo(out string ruIP, out string ruUser, out string ruPassword)
        {
            ruIP = cboIP.Text;
            ruUser = txtPass.Text;
            ruPassword = txtPass.Text;
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cboIP_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
