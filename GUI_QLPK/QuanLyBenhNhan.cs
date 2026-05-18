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
    public partial class QuanLyBenhNhan : Form
    {
        public DataTable db1 = new DataTable("BenhNhan");
        BenhNhanService bnBus = new BenhNhanService();
        BenhNhanDTO bn = new BenhNhanDTO();
        PhieukhambenhBUS pkbBUS = new PhieukhambenhBUS();
        private int temp_ma; //lưu mabn
        private int maloai;
        public QuanLyBenhNhan( int maloaiTk)
        {
            InitializeComponent();
            maloai = maloaiTk;
            load_data();
            gird.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            kiemtraquyen(); // Gọi hàm kiểm tra phân quyền
        }
        private void kiemtraquyen()
        {
            if (maloai == 3) // Nếu là nhân viên lễ tân   
            {
                xoa.Enabled = false;               
            }
        }
        private void load()
        {
            db1.Clear();
            db1.Columns.Add("MaHD", typeof(System.Int32));
        }
        public void load_data()
        {
            List<BenhNhanDTO> listBenhNhan = bnBus.select();
            this.loadData_Vao_GridView(listBenhNhan);
            hoten.Text = "";
            ngaysinh.Text = DateTime.UtcNow.Date.ToString();
            gioitinh.Text = "";
            diachi.Text = "";
            macccd.Text = "";
            email.Text = "";
        }
        private void loadData_Vao_GridView(List<BenhNhanDTO> listBenhNhan)
        {
            BindingSource bs = new BindingSource();

            if (listBenhNhan == null)
            {
                System.Windows.Forms.MessageBox.Show("Có lỗi khi lấy thông tin từ DB", "Result", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                return;

            }

            DataTable table = new DataTable();
            table.Columns.Add("Mã bệnh nhân", typeof(int));
            table.Columns.Add("Tên bệnh nhân", typeof(string));
            table.Columns.Add("Ngày sinh", typeof(string));
            table.Columns.Add("Địa chỉ", typeof(string));
            table.Columns.Add("Giới tính", typeof(string));
            table.Columns.Add("CCCD", typeof(string));
            table.Columns.Add("Email", typeof(string));


            foreach (BenhNhanDTO bn in listBenhNhan)
            {
                DataRow row = table.NewRow();
                row["Mã bệnh nhân"] = bn.MaBN;
                row["Tên bệnh nhân"] = bn.TenBN;
                row["Ngày sinh"] = DateTime.Parse(bn.NgsinhBN.ToString()).ToString("yyyy-MM-dd");
                row["Địa chỉ"] = bn.DiachiBN;
                row["Giới tính"] = bn.GtBN;
                row["CCCD"] = bn.CanCuocCongDan;
                row["Email"] = bn.Email;

                table.Rows.Add(row);
            }
            gird.DataSource = table.DefaultView;
            bs.ResetBindings(false);
        }

        private void TimKiem_Click(object sender, EventArgs e)
        {
            string sKeyword = search.Text;
            if (sKeyword == null || sKeyword == string.Empty || sKeyword.Length == 0) // tìm tất cả
            {
                List<BenhNhanDTO> listBenhNhan = bnBus.select();
                this.loadData_Vao_GridView(listBenhNhan);
            }
            else
            {
                List<BenhNhanDTO> listBenhNhan = bnBus.selectByKeyWord(sKeyword);
                this.loadData_Vao_GridView(listBenhNhan);
            }

        }
        //click để load dữ liệu lên giao diện
        private void gird_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < gird.Rows.Count)
            {
                DataGridViewRow row = gird.Rows[e.RowIndex];

                hoten.Text = row.Cells[1].Value?.ToString();

                // Xử lý riêng cho ngày sinh
                if (row.Cells[2].Value != null && row.Cells[2].Value != DBNull.Value)
                {
                    DateTime dt = Convert.ToDateTime(row.Cells[2].Value);
                    ngaysinh.Text = dt.ToString("yyyy-MM-dd"); // Định dạng ngày/tháng/năm
                }

                diachi.Text = row.Cells[3].Value?.ToString();
                gioitinh.Text = row.Cells[4].Value?.ToString();
                macccd.Text = row.Cells[5].Value?.ToString();
                email.Text = row.Cells[6].Value?.ToString();
                temp_ma = int.Parse(row.Cells[0].Value?.ToString());
            }
        }

        private void Sua_Click(object sender, EventArgs e)
        {
            bn.TenBN = hoten.Text;
            bn.DiachiBN = diachi.Text;
            bn.NgsinhBN = DateTime.Parse(ngaysinh.Text);
            bn.GtBN = gioitinh.Text;
            bn.CanCuocCongDan = macccd.Text;
            bn.Email = email.Text;
            bool kq = bnBus.sua(bn, temp_ma);
            if (!kq)
                System.Windows.Forms.MessageBox.Show("Cập nhật bênh nhân thất bại. Vui lòng kiểm tra lại dữ liệu", "Result", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            else
            {
                System.Windows.Forms.MessageBox.Show("Cập nhật bệnh nhân thành công", "Result");
                load_data();
            }
        }

        private void xoa_Click(object sender, EventArgs e)
        {
            List<phieukhambenhDTO> pkb = pkbBUS.select();
            //có pkb thì ko xóa được
            foreach (phieukhambenhDTO phieukhambenh in pkb)
            {
                if (phieukhambenh.MaBenhNhan == temp_ma)
                {

                    System.Windows.Forms.MessageBox.Show("Xóa bệnh nhân thất bại. Bệnh nhân đã khám", "Result", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                    return;
                }
            }
            bn.MaBN = temp_ma;
            bool kq = bnBus.xoa(bn);
            if (!kq)
            {
                System.Windows.Forms.MessageBox.Show("Xóa bệnh nhân thất bại. Vui lòng kiểm tra lại dữ liệu", "Result", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Xóa bệnh nhân thành công", "Result");
                load_data();
            }
        }

        private void Them_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(hoten.Text) ||
        string.IsNullOrWhiteSpace(gioitinh.Text) ||
        string.IsNullOrWhiteSpace(diachi.Text) ||
        string.IsNullOrWhiteSpace(macccd.Text) ||
        string.IsNullOrWhiteSpace(email.Text))
            {
                MessageBox.Show(
                    "Vui lòng nhập đầy đủ thông tin bệnh nhân",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            if (ngaysinh.Value.Date > DateTime.Today)
            {
                MessageBox.Show(
                    "Ngày sinh không hợp lệ",
                    "Warning",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            BenhNhanDTO bn = new BenhNhanDTO();

            bn.TenBN = hoten.Text.Trim();
            bn.GtBN = gioitinh.Text.Trim();
            bn.NgsinhBN = ngaysinh.Value;
            bn.DiachiBN = diachi.Text.Trim();
            bn.CanCuocCongDan = macccd.Text.Trim();
            bn.Email = email.Text.Trim();

            List<BenhNhanDTO> danhSach = bnBus.select();

            if (danhSach.Any(b => b.CanCuocCongDan == bn.CanCuocCongDan))
            {
                MessageBox.Show(
                    "CCCD đã tồn tại",
                    "Warning",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            bool kq = bnBus.them(bn);

            if (kq)
            {
                MessageBox.Show(
                    "Thêm bệnh nhân thành công",
                    "Result",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                load_data();

                hoten.Clear();
                gioitinh.Text = "";
                diachi.Clear();
                macccd.Clear();
                email.Clear();

                ngaysinh.Value = DateTime.Now;
            }
            else
            {
                MessageBox.Show(
                    "Thêm bệnh nhân thất bại",
                    "Result",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        private void HoanTac_Click(object sender, EventArgs e)
        {
            hoten.Text= string.Empty;
            ngaysinh.Text = DateTime.UtcNow.Date.ToString();
            gioitinh.Text = string.Empty;
            diachi.Text = string.Empty;
            macccd.Text = string.Empty;
            temp_ma = 0;
            email.Text = string.Empty;
            load_data();
        }
    }
}
