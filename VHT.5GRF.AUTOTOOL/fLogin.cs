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

namespace _5GAutoTool
{
    public partial class fLogin : Form
    {
        public fLogin()
        {
            InitializeComponent();
            main = new Form5GAT();
        }
        Form5GAT main;

        public void login()
        {
            try
            {
                if (string.IsNullOrEmpty(txbUsername.Text.Trim()) || string.IsNullOrEmpty(txbPassword.Text.Trim()))
                {
                    MessageBox.Show("Thông tin đăng nhập chưa đầy đủ");
                }
                else
                {
                    //APIToolTest api = new APIToolTest();
                    //if (api.Login(txbUsername.Text.Trim(), txbPassword.Text.Trim()) == 1)
                    //{
                    //    main.insert(txbUsername.Text.Trim());
                    //    this.Hide();
                    //    main.ShowDialog();
                    //    this.Show();
                    //}
                    if (txbUsername.Text == "Administrator" && txbPassword.Text == "1")
                    {
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
                MessageBox.Show("Kết nối với MES_M1 lỗi" + "\n" + "Vui lòng kiểm tra kết nối mạng!" + "\n" + "Detail:" + ex);
            }
        }

        private void TxtUserID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
            {
                txbPassword.Focus();
            }
        }

        private void TxtPW_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                txbUsername.Focus();
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
            txbUsername.Focus();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            login();
        }
    }
}
