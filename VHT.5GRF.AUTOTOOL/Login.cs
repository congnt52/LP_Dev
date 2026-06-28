using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MES_DLL;
using SSO_DLL;
using System.Runtime.InteropServices;

namespace _5GAutoTool
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
            main = new Form5GAT();
        }
        Form5GAT main;
        private void BtnLogin_Click(object sender, EventArgs e)
        {
            login();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        public void login()
        {
            try
            {
                if (string.IsNullOrEmpty(txtUserID.Text.Trim()) || string.IsNullOrEmpty(txtPW.Text.Trim()))
                {
                    MessageBox.Show("Thông tin đăng nhập chưa đầy đủ");
                }
                else
                {
                    APIToolTest api = new APIToolTest();
                    if (api.Login(txtUserID.Text.Trim(), txtPW.Text.Trim()) == 1)
                    {
                        Console.WriteLine("Đăng nhập VPSSERP thành công");
                        main.insert(txtUserID.Text.Trim());
                        this.Hide();
                        main.ShowDialog();
                        this.Show();
                    }
                    else
                    {
                        MessageBox.Show("Đăng nhập VPSSERP thất bại!");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                MessageBox.Show("Kết nối với MES_M1 lỗi" + "\n" + "Vui lòng kiểm tra kết nối mạng!");
            }
        }

        private void TxtUserID_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Down)
            {
                txtPW.Focus();
            }
        }

        private void TxtPW_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                txtUserID.Focus();
            }
        }

        private void TxtPW_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
            {
                btnLogin.Focus();
            }
        }
        private void BtnLogin_Enter(object sender, EventArgs e)
        {
            login();
        }

        private void Login_Load(object sender, EventArgs e)
        {
            txtUserID.Focus();
        }
    }
}
