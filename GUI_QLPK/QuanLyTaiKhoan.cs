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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.Windows.Forms.DataVisualization.Charting;

namespace GUI_QLPM
{
    public partial class QuanLyTaiKhoan : Form
    {
        public DataTable db1 = new DataTable("TaiKhoan");
        taiKhoanBUS tkBus = new taiKhoanBUS();
        taiKhoanDTO tk = new taiKhoanDTO();
        loaiTaiKhoanBUS loaitkBUS = new loaiTaiKhoanBUS();
        loaiTaiKhoanDTO loaitk = new loaiTaiKhoanDTO();
        private string temp;
        private string temp_ma;

        public QuanLyTaiKhoan()
        {
            InitializeComponent();
            load_data();
            gird.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }


        private void load()
        {
            db1.Clear();
        }

        public void load_data()
        {
            List<taiKhoanDTO> listTK = tkBus.select();
            List<loaiTaiKhoanDTO> listLoaiTK = loaitkBUS.select();
            this.loadData_Vao_GridView(listTK, listLoaiTK);
            load_combobox();
        }
        private void loadData_Vao_GridView(List<taiKhoanDTO> listTk, List<loaiTaiKhoanDTO> listLoaiTK)
        {

            if (listTk == null)
            {
                System.Windows.Forms.MessageBox.Show("Có lỗi khi lấy thông tin từ DB", "Result", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                return;
            }
            DataTable table = new DataTable();
            table.Columns.Add("Mã tài khoản", typeof(int));
            table.Columns.Add("Tên tài khoản", typeof(string));
            table.Columns.Add("Mật khẩu", typeof(string));
            table.Columns.Add("Họ và tên", typeof(string));
            table.Columns.Add("Loại tài khoản", typeof(string));

            foreach (taiKhoanDTO tk in listTk)
            {
                DataRow row = table.NewRow();
                row["Mã tài khoản"] = tk.MaTK;
                row["Tên tài khoản"] = tk.Username;
                row["Mật khẩu"] = tk.Password;
                row["Họ và tên"] = tk.Name;
                foreach (loaiTaiKhoanDTO loaitk in listLoaiTK)
                {
                    if (tk.MaLoai == loaitk.MaRole)
                    {
                        row["Loại tài khoản"] = loaitk.TenLoaiTaiKhoan;
                    }
                }
                table.Rows.Add(row);
            }
            gird.DataSource = table.DefaultView;
        }

        private void Them_Click(object sender, EventArgs e)
        {
          if (string.IsNullOrWhiteSpace(username.Text) ||
        string.IsNullOrWhiteSpace(password.Text) ||
        string.IsNullOrWhiteSpace(hoten.Text))
            {
                MessageBox.Show(
                    "Vui lòng nhập đầy đủ thông tin tài khoản",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            taiKhoanDTO tk = new taiKhoanDTO();

            tk.Username = username.Text.Trim();
            tk.Password = password.Text.Trim();
            tk.Name = hoten.Text.Trim();
            tk.MaLoai = comboBoxRole.SelectedIndex + 1;

            bool kq = tkBus.them(tk);

            if (!kq)
            {
                MessageBox.Show(
                    "Thêm tài khoản thất bại",
                    "Result",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
            else
            {
                MessageBox.Show(
                    "Thêm tài khoản thành công",
                    "Result",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                load_data();
                this.Close();
            }
        }

        private void TimKiem_Click(object sender, EventArgs e)
        {
            tkBus = new taiKhoanBUS();
            loaitkBUS = new loaiTaiKhoanBUS();
            string sKeyword = search.Text;
            if (string.IsNullOrEmpty(sKeyword)) // Tìm tất cả nếu không có từ khóa
            {
                List<taiKhoanDTO> listTaiKhoan = tkBus.select();
                List<loaiTaiKhoanDTO> listLoaiTaiKhoan = loaitkBUS.select();
                this.loadData_Vao_GridView(listTaiKhoan, listLoaiTaiKhoan);
            }
            else
            {
                List<taiKhoanDTO> listTaiKhoan = tkBus.selectByKeyWord(sKeyword);
                List<loaiTaiKhoanDTO> listLoaiTaiKhoan = loaitkBUS.select();
                this.loadData_Vao_GridView(listTaiKhoan, listLoaiTaiKhoan);
            }
        }
        //load role
        public void load_combobox()
        {
            List<loaiTaiKhoanDTO> listRole = loaitkBUS.select();
            if (listRole == null)
            {
                MessageBox.Show("Có lỗi khi lấy role từ DB", "Result",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                return;
            }

            // Bỏ qua admin
            List<loaiTaiKhoanDTO> listRoleFiltered = new List<loaiTaiKhoanDTO>();
            foreach (loaiTaiKhoanDTO role in listRole)
            {
                if (!string.Equals(role.TenLoaiTaiKhoan.Trim(), "Quản trị viên",
                                   StringComparison.OrdinalIgnoreCase))
                {
                    listRoleFiltered.Add(role);
                }
            }

            comboBoxRole.DataSource = null;  // reset
            comboBoxRole.DisplayMember = "TenLoaiTaiKhoan"; // hiển thị tên
            comboBoxRole.ValueMember = "MaRole";          // giữ ID role
            comboBoxRole.DataSource = listRoleFiltered;  // nạp DS

            if (comboBoxRole.Items.Count > 0)
                comboBoxRole.SelectedIndex = 0;
        }


        private void Sua_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(temp_ma))
            {
                MessageBox.Show(
                    "Vui lòng chọn tài khoản cần cập nhật!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            tk.Username = username.Text;
            tk.Password = password.Text;
            tk.Name = hoten.Text;
            if (comboBoxRole.SelectedValue != null)
                tk.MaLoai = Convert.ToInt32(comboBoxRole.SelectedValue);

            bool kq = tkBus.sua(tk, int.Parse(temp_ma));
            if (!kq)
                System.Windows.Forms.MessageBox.Show("Update tài khoản thất bại. Vui lòng kiểm tra lại dữ liệu", "Result", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            else
            {
                System.Windows.Forms.MessageBox.Show("Update tài khoản thành công", "Result");
                load_data();
            }
        }
        private void gird_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < gird.Rows.Count)
            {
                DataGridViewRow row = gird.Rows[e.RowIndex];
                temp_ma = row.Cells[0].Value.ToString();
                username.Text = row.Cells[1].Value.ToString();
                password.Text = row.Cells[2].Value.ToString();
                hoten.Text = row.Cells[3].Value.ToString();
                string roleName = "";
                if (row.Cells[4].Value != null)
                {
                    roleName = row.Cells[4].Value.ToString();
                }
                int idx = comboBoxRole.FindStringExact(roleName);
                if (idx >= 0)
                {
                    comboBoxRole.SelectedIndex = idx;
                }
                else
                {
                    comboBoxRole.SelectedIndex = -1; // không tìm thấy => clear selection
                }
            }
        }
        private void QuayLai_Click(object sender, EventArgs e)
        {
            search.Text = string.Empty;
            username.Text = string.Empty;
            password.Text = string.Empty;
            hoten.Text = string.Empty;
            comboBoxRole.SelectedIndex = 0;
        }
    }
}
