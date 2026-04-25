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
        taiKhoanBUS tkBUS = new taiKhoanBUS();
        taiKhoanDTO tk = new taiKhoanDTO();
        loaiTaiKhoanBUS loaitkBUS = new loaiTaiKhoanBUS();
        loaiTaiKhoanDTO loaitk = new loaiTaiKhoanDTO();

        Boolean check = false;

        private void dangnhap_Click(object sender, EventArgs e)
        {
            List<taiKhoanDTO> listTk = tkBUS.select();
            List<loaiTaiKhoanDTO> listLoaiTk = loaitkBUS.select();
            string TENTK = "";
            foreach(taiKhoanDTO tk in listTk)
            {
                if(tk.Username == username.Text)
                {
                    if(tk.Password == matkhau.Text)
                    {
                        check = true;
                        TENTK = tk.MaTK; 
                    }
                }
            }
            if (check)
            {
                this.Hide();
                QLPMMain main = new QLPMMain(TENTK);
                main.Show();
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
