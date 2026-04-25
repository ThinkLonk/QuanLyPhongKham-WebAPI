using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using QLPKBUS;
using QLPKDTO;

namespace GUI_QLPM
{
    public partial class ThemTaiKhoan : Form
    {
        taiKhoanBUS tkBus = new taiKhoanBUS();
        loaiTaiKhoanBUS ltkBus = new loaiTaiKhoanBUS();
        public ThemTaiKhoan()
        {
            InitializeComponent();
            load();
        }

        private void load()
        {
            taiKhoanBUS tkBus = new taiKhoanBUS();
            maTaiKhoan.Text = tkBus.autogenerate_maTaiKhoan().ToString();
            load_combobox();
        }

        private void Them_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(maTaiKhoan.Text) || string.IsNullOrEmpty(username.Text) || string.IsNullOrEmpty(password.Text) || string.IsNullOrEmpty(hoTen.Text))
            {
                System.Windows.Forms.MessageBox.Show("Vui lòng nhập đầy đủ thông tin Tài Khoản", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                taiKhoanDTO tk = new taiKhoanDTO();
                tk.MaTK = (maTaiKhoan.Text);
                tk.Username = username.Text;
                tk.Password = password.Text;
                tk.Name = hoTen.Text;
                tk.MaLoai = (comboBoxRole.SelectedIndex + 1).ToString();

                tkBus = new taiKhoanBUS();
                bool kq = tkBus.them(tk);
                if (!kq)
                    System.Windows.Forms.MessageBox.Show("Thêm Tài Khoản thất bại. Vui lòng kiểm tra lại dữ liệu", "Result", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                else
                {
                    System.Windows.Forms.MessageBox.Show("Thêm tài khoản thành công", "Result");
                    this.Close();
                }

            }

        }
        public void load_combobox()
        {
            List<loaiTaiKhoanDTO> listRole = ltkBus.select();
            this.loadData_Vao_Combobox(listRole);
        }
        private void loadData_Vao_Combobox(List<loaiTaiKhoanDTO> listRole)
        {
            //mapkb.Items.Clear();

            comboBoxRole.Items.Clear();
            if (listRole == null)
            {
                System.Windows.Forms.MessageBox.Show("Có lỗi khi lấy thông tin nạp vào combox pkb từ DB", "Result", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                return;
            }
            foreach (loaiTaiKhoanDTO role in listRole)
            {
                if (role.TenLoaiTaiKhoan.Trim() != "Quản trị viên") // bỏ qua admin
                {
                    comboBoxRole.Items.Add(role.TenLoaiTaiKhoan);
                }
            }

            if (comboBoxRole.Items.Count > 0)
                comboBoxRole.SelectedIndex = 0;

        }

        private void comboBoxRole_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadRole();
        }

        private void loadRole()
        {
            int selectedIndex = comboBoxRole.SelectedIndex;
            List<loaiTaiKhoanDTO> listRole = ltkBus.select();

        }
    }
}
