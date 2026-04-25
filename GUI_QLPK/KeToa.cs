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
        ThuocBUS thBus = new ThuocBUS();
        cachDungBUS cdBus = new cachDungBUS();
        donviBUS donVivBus = new donviBUS();

        List<cachdungDTO> listcd;
        List<donViDTO> listdv;
        string id;

        public KeToa()
        {
            InitializeComponent();

            gird.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            listcd = cdBus.select();
            listdv = donVivBus.select();

            load_data();
        }

        public void load_data()
        {
            db1.Rows.Clear();
            db1.Columns.Clear();

            db1.Columns.Add("Mã thuốc", typeof(string));
            db1.Columns.Add("Tên thuốc", typeof(string));
            db1.Columns.Add("Đơn vị", typeof(string));
            db1.Columns.Add("Đơn giá", typeof(string));
            db1.Columns.Add("Cách dùng", typeof(string));
            db1.Columns.Add("Số lượng", typeof(int));

            List<thuocDTO> listThuoc = thBus.select();
            List<phieukhambenhDTO> listPKB = pkbBUS.select();

            // load combobox thuốc
            loadData_Vao_Combobox(listThuoc);

            // load mã phiếu khám
            mapkb.Items.Clear();
            foreach (phieukhambenhDTO pkb in listPKB)
            {
                mapkb.Items.Add(pkb.MaPKB);
            }
            mapkb.SelectedIndex = -1;

            // tự sinh mã toa
            maToa.Text = ttBus.autogenerate_matoa().ToString();

            gird.DataSource = db1.DefaultView;
        }

        public void reset()
        {
            db1.Rows.Clear();

            mapkb.Text = "";
            soLuong.Text = "";
            TenThuoc.Text = "";

            maToa.Text = ttBus.autogenerate_matoa().ToString();

            gird.DataSource = db1.DefaultView;
        }

        private void loadData_Vao_Combobox(List<thuocDTO> listThuoc)
        {
            if (listThuoc == null || listThuoc.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu thuốc",
                                "Thông báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }

            DataTable table = new DataTable();
            table.Columns.Add("Mã thuốc", typeof(string));
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
            // kiểm tra mã phiếu khám
            if (string.IsNullOrWhiteSpace(mapkb.Text))
            {
                MessageBox.Show("Vui lòng chọn mã phiếu khám bệnh",
                                "Thông báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }

            // kiểm tra đã thêm thuốc chưa
            if (db1.Rows.Count == 0)
            {
                MessageBox.Show("Vui lòng thêm thuốc vào toa",
                                "Thông báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }

            
            List<toathuocDTO> listToa = ttBus.select();

            string maToaDangDung;

            var toaDaCo = listToa
                .FirstOrDefault(x => x.MaPkb == mapkb.Text);
            if (toaDaCo != null)
            {
                // đã có toa → dùng lại toa cũ
                maToaDangDung =toaDaCo.MaToa;
            }
            else
            {
                // chưa có → tạo mới
                toathuocDTO tt = new toathuocDTO();
                tt.MaToa =(maToa.Text);
                tt.MaPkb = (mapkb.Text);
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

            // lưu chi tiết toa + trừ kho
            for (int i = 0; i < db1.Rows.Count; i++)
            {
                ChiTietToaThuocDTO ct = new ChiTietToaThuocDTO();

                ct.MaToa = maToaDangDung;
                ct.MaThuoc = (gird.Rows[i].Cells[0].Value.ToString());

                int soLuongThuoc;

                bool check = int.TryParse(
                    gird.Rows[i].Cells[5].Value.ToString(),
                    out soLuongThuoc
                );

                if (!check || soLuongThuoc <= 0)
                {
                    MessageBox.Show("Số lượng thuốc không hợp lệ",
                                    "Thông báo",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                    loi = true;
                    break;
                }

                ct.SoLuong = soLuongThuoc;

                // lưu chi tiết đơn thuốc
                bool kqChiTiet = ktBus.kethuoc(ct);

                // trừ số lượng thuốc trong kho
                bool truKho = thBus.truSoLuong(ct.MaThuoc, ct.SoLuong);

                if (!kqChiTiet || !truKho)
                {
                    loi = true;
                    break;
                }
            }

            if (loi)
            {
                MessageBox.Show("Kê toa thất bại",
                                "Result",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
            }
            else
            {
                MessageBox.Show("Kê toa thành công",
                                "Result",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);

                reset();
                gird.DataSource = db1.DefaultView;
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            // kiểm tra chọn thuốc
            if (TenThuoc.SelectedIndex < 0)
            {
                MessageBox.Show("Vui lòng chọn thuốc",
                                "Thông báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }

            // kiểm tra số lượng
            int soLuongNhap;

            if (!int.TryParse(soLuong.Text, out soLuongNhap) || soLuongNhap <= 0)
            {
                MessageBox.Show("Vui lòng nhập số lượng là số nguyên dương",
                                "Thông báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                soLuong.Focus();
                return;
            }

            List<thuocDTO> listThuoc = thBus.select();

            bool daTonTai = false;

            // nếu thuốc đã có trong grid → cộng dồn số lượng
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

            // nếu chưa có → thêm mới
            if (!daTonTai)
            {
                foreach (thuocDTO th in listThuoc)
                {
                    if (th.TenThuoc == (TenThuoc.Text))
                    {
                        DataRow row = db1.NewRow();

                        row["Mã thuốc"] = th.MaThuoc;
                        row["Tên thuốc"] = th.TenThuoc;

                        // đơn vị
                        var dv = listdv.FirstOrDefault(x => x.MaDonVi == th.MaDonVi);
                        row["Đơn vị"] = dv != null ? dv.TenDonVi : "";

                        row["Đơn giá"] = th.DonGia;

                        // cách dùng
                        var cd = listcd.FirstOrDefault(x => x.MaCachDung == th.MaCachDung);
                        row["Cách dùng"] = cd != null ? cd.TenCachDung : "";

                        row["Số lượng"] = soLuongNhap;

                        db1.Rows.Add(row);
                        break;
                    }
                }
            }

            gird.DataSource = db1.DefaultView;

            // reset nhập
            soLuong.Text = "";
            TenThuoc.SelectedIndex = -1;
        }
    }
}
