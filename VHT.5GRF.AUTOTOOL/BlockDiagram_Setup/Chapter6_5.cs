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
    public partial class Chapter6_5 : Form
    {
        public Chapter6_5()
        {
            InitializeComponent();
        }

        private void btnSetupOK_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Đảm bảo SETUP đúng sơ đồ đo trước khi ấn OK");
            this.Close();
        }

        private void picWiringDiagram_Click(object sender, EventArgs e)
        {

        }
    }
}
