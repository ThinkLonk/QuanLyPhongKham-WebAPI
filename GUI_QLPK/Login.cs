using QLPKBUS;
using QLPKDTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUI_QLPK
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
            matkhau.UseSystemPasswordChar = true;
        }
        AuthService authService = new AuthService();

        private void dangnhap_Click(object sender, EventArgs e)
        {
            int TENTK;
            if (authService.TryLogin(username.Text, matkhau.Text, out TENTK))
            {
                this.Hide();
                QLPMMain main = new QLPMMain(TENTK);
                main.ShowDialog();

                if (main.isLogout)
                {
                    username.Text = "";
                    matkhau.Text = "";
                    this.Show();
                    username.Focus();
                }
                else
                {
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("Sai mật khẩu hoặc tài khoản", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                username.Text = "";
                matkhau.Text = "";
                username.Focus();
            }
        }
    }
}
