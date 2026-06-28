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
    public partial class LoginSetting : Form
    {
        public LoginSetting()
        {
            InitializeComponent();
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            if(textBox1.Text == "1234567a@")
            {
                this.Close();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            bool flag = PassWordCheck();
            if(flag)
            {
                this.Close();
            }          
        }
        public bool PassWordCheck()
        {
            bool flag = false;
            if (textBox1.Text == "1234567a@")
            {
                flag = true;

            }
            return flag;
        }
    }
}
