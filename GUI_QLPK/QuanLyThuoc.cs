using QLPKBUS;
using QLPKDAL;
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
using System.Windows.Forms.DataVisualization.Charting;
using System.Globalization;


namespace GUI_QLPK
{
    public partial class QuanLyThuoc : Form
    {
        public DataTable db1 = new DataTable("Thuoc");
        ThuocService thBus = new ThuocService();
        thuocDTO th = new thuocDTO();
        cachDungBUS cdBUS = new cachDungBUS();
        donviBUS donViBUS = new donviBUS();
        List<cachdungDTO> listcd;
        List<donViDTO> listdv;
        private int temp;

        public QuanLyThuoc()
        {
            InitializeComponent();
            load();
            load_data();
            gird.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }


        private void load()
        {
            mathuoc.Text = "Tự động";
            listcd = cdBUS.select();
            listdv = donViBUS.select();
            this.load_combobox(listdv, listcd); //tải dữ liệu vào combobox
            tenthuoc.Text = "";
            donvitinh.Text = "";
            dongia.Text = "";
            cachdung.Text = "";
            soluong.Value = 0;
        }
        public void load_data()
        {
            List<thuocDTO> listThuoc = thBus.select();
            this.loadData_Vao_GridView(listThuoc);
            tenthuoc.Text = "";
            donvitinh.Text = "";
            dongia.Text = "";
            cachdung.Text = "";
            soluong.Value = 0;
        }

        private void loadData_Vao_GridView(List<thuocDTO> listThuoc)
        {

            if (listThuoc == null)
            {
                System.Windows.Forms.MessageBox.Show("Có lỗi khi lấy thông tin từ DB", "Result", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                return;
            }
            DataTable table = new DataTable();
            table.Columns.Add("Mã thuốc", typeof(int));
            table.Columns.Add("Tên thuốc", typeof(string));
            table.Columns.Add("Đơn vị tính", typeof(string));
            table.Columns.Add("Số lượng", typeof(int));
            table.Columns.Add("Đơn giá", typeof(decimal));
            table.Columns.Add("Cách dùng", typeof(string));
            foreach (thuocDTO th in listThuoc)
            {
                DataRow row = table.NewRow();
                row["Mã thuốc"] = th.MaThuoc;
                row["Tên thuốc"] = th.TenThuoc;
                foreach (donViDTO donvi in listdv)
                {
                    if (donvi.MaDonVi == th.MaDonVi)
                    {
                        row["Đơn vị tính"] = donvi.TenDonVi;
                    }
                }
                row["Số lượng"] = th.SoLuong;
                row["Đơn giá"] = Convert.ToDecimal(th.DonGia); //éo định dạng
                foreach (cachdungDTO cd in listcd)
                {
                    if (cd.MaCachDung == th.MaCachDung)
                    {
                        row["Cách dùng"] = cd.TenCachDung;
                    }

                }
                table.Rows.Add(row);
            }
            gird.DataSource = table.DefaultView;
            gird.Columns["Đơn giá"].DefaultCellStyle.Format = "N0"; 
        }

        private void load_combobox(List<donViDTO> listdv, List<cachdungDTO> listcd)
        {
            if (listdv == null || listcd == null)
            {
                System.Windows.Forms.MessageBox.Show("Có lỗi khi lấy thông tin từ DB", "Result", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                return;
            }
            DataTable table = new DataTable();
            DataTable table1 = new DataTable();

            table.Columns.Add("Đơn vị", typeof(string));
            table1.Columns.Add("Cách dùng", typeof(string));
            foreach (donViDTO dv in listdv)
            {
                DataRow row = table.NewRow();
                row["Đơn vị"] = dv.TenDonVi;
                table.Rows.Add(row);
            }
            foreach (cachdungDTO cd in listcd)
            {
                DataRow row = table1.NewRow();
                row["Cách dùng"] = cd.TenCachDung;
                table1.Rows.Add(row);
            }
            // Đặt dữ liệu vào ComboBox donvitinh từ table
            donvitinh.DataSource = table;
            // Đặt trường cần hiển thị trong ComboBox donvitinh là "donVi"
            donvitinh.DisplayMember = "Đơn vị";
            // Đặt dữ liệu vào ComboBox cachdung từ table1
            cachdung.DataSource = table1;
            // Đặt trường cần hiển thị trong ComboBox cachdung là "cachDung"
            cachdung.DisplayMember = "Cách dùng";

        }

        private void TimKiem_Click(object sender, EventArgs e)
        {
            string sKeyword = key.Text;
            if (string.IsNullOrEmpty(sKeyword)) // Tìm tất cả nếu không có từ khóa
            {
                List<thuocDTO> listthuoc = thBus.select();
                this.loadData_Vao_GridView(listthuoc);
            }
            else
            {
                List<thuocDTO> listthuoc = thBus.selectByKeyWord(sKeyword);
                this.loadData_Vao_GridView(listthuoc);
            }
        }

        private void Them_Click(object sender, EventArgs e)
        {
            decimal gia;
            if (!decimal.TryParse(dongia.Text.Trim(), out gia) || gia <= 0)
            {
                System.Windows.Forms.MessageBox.Show("Vui lòng nhập số dương và không được để trống", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dongia.Text = "";
                dongia.Focus();
                return;
            }
            // Kiểm tra tên thuốc trùng
            if (thBus.kiemTraTrungTen(tenthuoc.Text.Trim()))
            {
                System.Windows.Forms.MessageBox.Show("Tên thuốc đã tồn tại, vui lòng nhập tên khác.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tenthuoc.Focus();
                return;
            }


            if (string.IsNullOrEmpty(mathuoc.Text) || string.IsNullOrEmpty(tenthuoc.Text) || string.IsNullOrEmpty(cachdung.Text) || string.IsNullOrEmpty(donvitinh.Text))
            {
                System.Windows.Forms.MessageBox.Show("Vui lòng nhập đầy đủ thông tin loại thuốc", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            thuocDTO th = new thuocDTO();
            th.TenThuoc = (tenthuoc.Text);
            th.DonGia = (float)gia;// ép decimal sang float
            th.SoLuong = (int)soluong.Value;
            foreach (donViDTO donvi in listdv)
            {
                if (donvi.TenDonVi == donvitinh.Text)
                {
                    th.MaDonVi = donvi.MaDonVi;
                }
            }
            foreach (cachdungDTO cd in listcd)
            {
                if (cd.TenCachDung == cachdung.Text)
                {
                    th.MaCachDung = cd.MaCachDung;
                }
            }
            thBus = new ThuocService();
            bool kq = thBus.them(th);
            if (!kq)
                System.Windows.Forms.MessageBox.Show("Thêm Thuốc thất bại. Vui lòng kiểm tra lại dữ liệu", "Result", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            else
            {
                System.Windows.Forms.MessageBox.Show("Thêm Thuốc thành công", "Result");
                load_data();
                load();
            }
        }

        private void Sua_Click(object sender, EventArgs e)
        {
            th.TenThuoc = (tenthuoc.Text);
            foreach (donViDTO donvi in listdv)
            {
                if (donvi.TenDonVi == donvitinh.Text)
                {
                    th.MaDonVi = donvi.MaDonVi;
                }
            }
            foreach (cachdungDTO cd in listcd)
            {
                if (cd.TenCachDung == cachdung.Text)
                {
                    th.MaCachDung = cd.MaCachDung;
                }
            }
            decimal gia;
            if (!decimal.TryParse(dongia.Text.Trim(), out gia) && gia <= 0)
            {
                System.Windows.Forms.MessageBox.Show("Vui lòng nhập số dương và không được để trống", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dongia.Focus();
                return;
            }
            th.DonGia = (float)gia;
            th.SoLuong = (int)soluong.Value;
            bool kq = thBus.sua(th, temp);
            if (!kq)
                System.Windows.Forms.MessageBox.Show("Cập nhật thuốc thất bại. Vui lòng kiểm tra lại dữ liệu", "Result", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            else
            {
                System.Windows.Forms.MessageBox.Show("Cập nhật thuốc thành công", "Result");
                load_data();
                load();
            }
        }

        private void Xoa_Click(object sender, EventArgs e)
        {
            int maThuoc;

            if (!int.TryParse(mathuoc.Text.Trim(), out maThuoc))
            {
                MessageBox.Show(
                    "Vui lòng chọn thuốc cần xóa!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            th.MaThuoc = maThuoc;
            th.TenThuoc = (tenthuoc.Text);
            foreach (donViDTO donvi in listdv)
            {
                if (donvi.TenDonVi == donvitinh.Text)
                {
                    th.MaDonVi = donvi.MaDonVi;
                }
            }
            foreach (cachdungDTO cd in listcd)
            {
                if (cd.TenCachDung == cachdung.Text)
                {
                    th.MaCachDung = cd.MaCachDung;
                }
            }
            decimal gia;
            if (!decimal.TryParse(dongia.Text.Trim(), out gia))
            {
                th.DonGia =(float)gia ;
            }
            
            th.SoLuong = (int)soluong.Value;
            bool kq = thBus.xoa(th);
            if (!kq)
                System.Windows.Forms.MessageBox.Show("Xóa thuốc thất bại. Vui lòng kiểm tra lại dữ liệu", "Result", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            else
            {
                System.Windows.Forms.MessageBox.Show("Xóa thuốc thành công", "Result");
                load_data();
                load();
            }
        }

        private void grid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < gird.Rows.Count)
            {
                DataGridViewRow row = gird.Rows[e.RowIndex];
                mathuoc.Text = row.Cells[0].Value.ToString();
                tenthuoc.Text = row.Cells[1].Value.ToString();
                donvitinh.Text = row.Cells[2].Value.ToString();
                dongia.Text = Convert.ToDecimal(row.Cells[4].Value).ToString("N0");
                cachdung.Text = row.Cells[5].Value.ToString();
                soluong.Value = Convert.ToInt32(row.Cells[3].Value);

                temp = Convert.ToInt32(row.Cells[0].Value);
            }
        }

        private void HoanTac_Click(object sender, EventArgs e)
        {
            key.Text = string.Empty;
            mathuoc.Text = string.Empty;
            tenthuoc.Text = string.Empty;
            dongia.Text = string.Empty;
            dongia.Text = string.Empty;
            cachdung.Text = string.Empty;
            donvitinh.Text = string.Empty;
            soluong.Value = 0;
        }
    }

}
