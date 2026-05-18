using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using QLPKBUS;
using QLPKDTO;
using QLPKDAL;

namespace GUI_QLPK
{
    public partial class KeToa : Form
    {
        PhieukhambenhBUS pkbBUS = new PhieukhambenhBUS();
        public DataTable db1 = new DataTable();
        ToathuocBUS ttBus = new ToathuocBUS();
        ChiTietToaThuocBUS ktBus = new ChiTietToaThuocBUS();
        ThuocService thBus = new ThuocService();
        cachDungBUS cdBus = new cachDungBUS();
        donviBUS donVivBus = new donviBUS();
        LichHenService lhBUS = new LichHenService();

        List<cachdungDTO> listcd;
        List<donViDTO> listdv;

        private int _maPKB;
        private int _maBN;
        private int _maLichHen;
        private DateTime? _ngayTaiKham;
        private lichHenDTO _lhHienTai;

        public KeToa(int maPKB, int maBN, int maLichHen, DateTime? ngayTaiKham, lichHenDTO lhHienTai)
        {
            InitializeComponent();
            _maPKB = maPKB;
            _maBN = maBN;
            _maLichHen = maLichHen;
            _ngayTaiKham = ngayTaiKham;
            _lhHienTai = lhHienTai;

            gird.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            listcd = cdBus.select();
            listdv = donVivBus.select();
            load_data();
        }

        public void load_data()
        {
            db1.Rows.Clear();
            db1.Columns.Clear();

            db1.Columns.Add("Mã thuốc", typeof(int));
            db1.Columns.Add("Tên thuốc", typeof(string));
            db1.Columns.Add("Đơn vị", typeof(string));
            db1.Columns.Add("Đơn giá", typeof(string));
            db1.Columns.Add("Cách dùng", typeof(string));
            db1.Columns.Add("Số lượng", typeof(int));

            List<thuocDTO> listThuoc = thBus.select();
            List<phieukhambenhDTO> listPKB = pkbBUS.select();

            loadData_Vao_Combobox(listThuoc);

            mapkb.Items.Clear();
            foreach (phieukhambenhDTO pkb in listPKB)
                mapkb.Items.Add(pkb.MaPKB);

            // Tự động chọn mã PKB — đặt SAU khi Items load xong
            if (mapkb.Items.Contains(_maPKB))
                mapkb.SelectedItem = _maPKB;
            else
                mapkb.SelectedIndex = -1;

            mapkb.Enabled = false; // Khoá, không cho đổi
            maToa.Text = "Tự động";
            gird.DataSource = db1.DefaultView;
        }

        public void reset()
        {
            db1.Rows.Clear();
            soLuong.Text = "";
            TenThuoc.SelectedIndex = -1;
            maToa.Text = "Tự động";
            gird.DataSource = db1.DefaultView;
        }

        private void loadData_Vao_Combobox(List<thuocDTO> listThuoc)
        {
            if (listThuoc == null || listThuoc.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu thuốc", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataTable table = new DataTable();
            table.Columns.Add("Mã thuốc", typeof(int));
            table.Columns.Add("Tên thuốc", typeof(string));

            foreach (thuocDTO th in listThuoc)
            {
                DataRow row = table.NewRow();
                row["Mã thuốc"] = th.MaThuoc;
                row["Tên thuốc"] = th.TenThuoc;
                table.Rows.Add(row);
            }

            TenThuoc.DataSource = table;
            TenThuoc.DisplayMember = "Tên thuốc";
            TenThuoc.ValueMember = "Mã thuốc";
            TenThuoc.SelectedIndex = -1;
        }

        private void KeThuoc_Click(object sender, EventArgs e)
        {
            if (mapkb.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn mã phiếu khám bệnh", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (db1.Rows.Count == 0)
            {
                MessageBox.Show("Vui lòng thêm thuốc vào toa", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            List<toathuocDTO> listToa = ttBus.select();
            int maToaDangDung;

            var toaDaCo = listToa.FirstOrDefault(x => x.MaPkb == _maPKB);
            if (toaDaCo != null)
            {
                maToaDangDung = toaDaCo.MaToa;
            }
            else
            {
                toathuocDTO tt = new toathuocDTO();
                tt.MaPkb = _maPKB;
                tt.NgayKetoa = DateTime.Today;

                bool kqToa = ttBus.them(tt);
                if (!kqToa)
                {
                    MessageBox.Show("Lưu toa thuốc thất bại");
                    return;
                }
                maToaDangDung = tt.MaToa;
            }

            bool loi = false;

            for (int i = 0; i < db1.Rows.Count; i++)
            {
                ChiTietToaThuocDTO ct = new ChiTietToaThuocDTO();
                ct.MaToa = maToaDangDung;
                ct.MaThuoc = Convert.ToInt32(gird.Rows[i].Cells[0].Value);

                int soLuongThuoc;
                bool check = int.TryParse(gird.Rows[i].Cells[5].Value.ToString(), out soLuongThuoc);

                if (!check || soLuongThuoc <= 0)
                {
                    MessageBox.Show("Số lượng thuốc không hợp lệ", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    loi = true;
                    break;
                }

                ct.SoLuong = soLuongThuoc;

                bool kqChiTiet = ktBus.kethuoc(ct);
                bool truKho = thBus.truSoLuong(ct.MaThuoc, ct.SoLuong);

                if (!kqChiTiet || !truKho)
                {
                    loi = true;
                    break;
                }
            }

            if (loi)
            {
                MessageBox.Show("Kê toa thất bại", "Result",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                // ✅ Dùng _maLichHen thay vì _maBN
                lhBUS.CapNhatTrangThai(_maLichHen, "Đã khám");

                // ✅ Tạo lịch hẹn tái khám nếu bác sĩ có chọn ngày tái khám
                if (_ngayTaiKham.HasValue && _lhHienTai != null)
                {
                    bool kqTaoLich = lhBUS.ThemLichHenTaiKham(
                        _maBN,
                        _lhHienTai.MaTaiKhoan,
                        _lhHienTai.MaDieuDuong,
                        _ngayTaiKham.Value
                    );

                    if (kqTaoLich)
                        MessageBox.Show("Kê toa thành công!\nĐã tạo lịch hẹn tái khám vào " +
                            _ngayTaiKham.Value.ToString("dd/MM/yyyy"),
                            "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show("Kê toa thành công nhưng tạo lịch tái khám thất bại.",
                            "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show("Kê toa thành công", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                reset();
                this.Close();
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            if (TenThuoc.SelectedIndex < 0)
            {
                MessageBox.Show("Vui lòng chọn thuốc", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int soLuongNhap;
            if (!int.TryParse(soLuong.Text, out soLuongNhap) || soLuongNhap <= 0)
            {
                MessageBox.Show("Vui lòng nhập số lượng là số nguyên dương", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                soLuong.Focus();
                return;
            }

            List<thuocDTO> listThuoc = thBus.select();
            bool daTonTai = false;

            for (int i = 0; i < db1.Rows.Count; i++)
            {
                if (db1.Rows[i]["Tên thuốc"].ToString() == TenThuoc.Text)
                {
                    int slCu = Convert.ToInt32(db1.Rows[i]["Số lượng"]);
                    db1.Rows[i]["Số lượng"] = slCu + soLuongNhap;
                    daTonTai = true;
                    break;
                }
            }

            if (!daTonTai)
            {
                foreach (thuocDTO th in listThuoc)
                {
                    if (th.TenThuoc == TenThuoc.Text)
                    {
                        DataRow row = db1.NewRow();
                        row["Mã thuốc"] = th.MaThuoc;
                        row["Tên thuốc"] = th.TenThuoc;

                        var dv = listdv.FirstOrDefault(x => x.MaDonVi == th.MaDonVi);
                        row["Đơn vị"] = dv != null ? dv.TenDonVi : "";

                        row["Đơn giá"] = th.DonGia;

                        var cd = listcd.FirstOrDefault(x => x.MaCachDung == th.MaCachDung);
                        row["Cách dùng"] = cd != null ? cd.TenCachDung : "";

                        row["Số lượng"] = soLuongNhap;
                        db1.Rows.Add(row);
                        break;
                    }
                }
            }

            gird.DataSource = db1.DefaultView;
            soLuong.Text = "";
            TenThuoc.SelectedIndex = -1;
        }
    }
}