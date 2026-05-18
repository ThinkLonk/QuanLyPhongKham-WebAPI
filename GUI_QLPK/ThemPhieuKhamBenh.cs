using QLPKDTO;
using QLPKBUS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using QLPKDAL;

namespace GUI_QLPK
{
    public partial class ThemPhieuKhamBenh : Form
    {
        public int maBS;
        BenhNhanService bnBUS = new BenhNhanService();
        BenhBUS beBus = new BenhBUS();
        ChandoanBUS cdBUS = new ChandoanBUS();
        PhieukhambenhBUS pkbBUS = new PhieukhambenhBUS();
        LichHenService lhBUS = new LichHenService();
        lichHenDAL lichHenDAL = new lichHenDAL();

        private int _maPKBVuaLap = -1;
        private int _maBNVuaLap = -1;
        private int _maLichHen = -1;

        public ThemPhieuKhamBenh(int mabs)
        {
            maBS = mabs;
            InitializeComponent();
            load_combobox_benh();
            gird.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            Load_Gird();
            load_data();
            btnKeToa.Enabled = false;
            btnKeToa.Visible = false;
        }

        public void load_data()
        {
            maPKB.Text = "Tự động";
            mabenhnhan.Text = "";
            hoten.Text = "";
            trieuchung.Text = "";
            ngaytaikham.Checked = false;
            ngaytaikham.ShowCheckBox = true;
            ngaytaikham.CustomFormat = "dd/MM/yyyy";
            ngaytaikham.Format = DateTimePickerFormat.Custom;
            ngaytaikham.Value = DateTime.Now.AddDays(7);
            nudGio.Value = 8;
            nudPhut.Value = 0;
        }

        private void load_combobox_benh()
        {
            checkedListBoxBenh.Items.Clear();
            List<benhDTO> listBenh = beBus.select();
            foreach (benhDTO be in listBenh)
                checkedListBoxBenh.Items.Add(be.TenBenh);
        }

        private void load_ten(List<BenhNhanDTO> listBenhNhan, int mabn)
        {
            if (listBenhNhan == null)
            {
                MessageBox.Show("Có lỗi khi lấy thông tin từ hệ thống", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var benhNhan = listBenhNhan.FirstOrDefault(bn => bn.MaBN == mabn);
            if (benhNhan != null)
                hoten.Text = benhNhan.TenBN;
        }

        private void btnLapphieu_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(trieuchung.Text))
            {
                MessageBox.Show("Vui lòng nhập triệu chứng", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(mabenhnhan.Text))
            {
                MessageBox.Show("Vui lòng chọn bệnh nhân", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_maLichHen == -1)
            {
                MessageBox.Show("Vui lòng chọn bệnh nhân từ danh sách.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kiểm tra ngày tái khám nếu có tick
            DateTime? ngayTaiKhamValue = null;
            if (ngaytaikham.Checked)
            {
                DateTime ngayTaiKham = ngaytaikham.Value.Date;
                DateTime ngayGioTaiKham = ngayTaiKham
                    .AddHours((double)nudGio.Value)
                    .AddMinutes((double)nudPhut.Value);
                if (ngayTaiKham <= DateTime.Today)
                {
                    MessageBox.Show("Ngày tái khám phải sau ngày hôm nay", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                ngayTaiKhamValue = ngayGioTaiKham;
            }

            phieukhambenhDTO pkb = new phieukhambenhDTO();
            List<benhDTO> listBenh = beBus.select();

            pkb.NgayKham = DateTime.Now.Date;
            pkb.TrieuChung = trieuchung.Text.Trim();
            pkb.MaBenhNhan = int.Parse(mabenhnhan.Text);
            pkb.NgayTaiKham = ngayTaiKhamValue;
            pkb.MBS = maBS;

            bool ADDPKB = pkbBUS.them(pkb);

            bool loiChanDoan = false;
            bool coBenhDuocChon = false;
            bool laBenhKhongBenh = false;

            if (ADDPKB)
            {
                chandoanDTO cd = new chandoanDTO();
                foreach (var item in checkedListBoxBenh.CheckedItems)
                {
                    string tenBenh = item.ToString();
                    var benhTimDuoc = listBenh.FirstOrDefault(x => x.TenBenh == tenBenh);

                    if (benhTimDuoc != null)
                    {
                        coBenhDuocChon = true;

                        if (tenBenh.Trim().ToLower() == "không bị bệnh")
                            laBenhKhongBenh = true;

                        cd.MaPkb = pkb.MaPKB;
                        cd.MaBenh = benhTimDuoc.MaBenh;
                        cd.TenChuanDoan = benhTimDuoc.TenBenh;
                        cd.TrieuChung = trieuchung.Text.Trim();

                        bool kqCD = cdBUS.them(cd);
                        if (!kqCD)
                        {
                            loiChanDoan = true;
                            break;
                        }
                    }
                }
            }

            if (!loiChanDoan && ADDPKB)
            {
                _maPKBVuaLap = pkb.MaPKB;
                _maBNVuaLap = pkb.MaBenhNhan;

                if (!coBenhDuocChon || laBenhKhongBenh)
                {
                    // Không bệnh → Đã khám
                    lhBUS.CapNhatTrangThai(_maLichHen, "Đã khám");

                    // Nếu có tick tái khám → tạo lịch hẹn mới ngay
                    if (ngayTaiKhamValue.HasValue)
                    {
                        List<lichHenDTO> listLH = lhBUS.select();
                        var lhHienTai = listLH.FirstOrDefault(x => x.MaLichHen == _maLichHen);

                        if (lhHienTai != null)
                        {
                            bool kqTaoLich = lhBUS.ThemLichHenTaiKham(
                                pkb.MaBenhNhan,
                                lhHienTai.MaTaiKhoan,
                                lhHienTai.MaDieuDuong,
                                ngayTaiKhamValue.Value
                         
                            );

                            if (!kqTaoLich)
                                MessageBox.Show("Tạo lịch tái khám thất bại.", "Cảnh báo",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }

                    btnKeToa.Visible = false;
                    btnKeToa.Enabled = false;
                }
                else
                {
                    // Có bệnh → Chờ kê thuốc
                    lhBUS.CapNhatTrangThai(_maLichHen, "Chờ kê thuốc");
                    btnKeToa.Visible = true;
                    btnKeToa.Enabled = true;
                    btnLapphieu.Visible = false;
                    btnLapphieu.Enabled = false;
                }

                MessageBox.Show("Lập phiếu thành công", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                Load_Gird();
            }
            else
            {
                MessageBox.Show("Lập phiếu thất bại", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void Load_Gird()
        {
            int stt = 1;
            List<BenhNhanDTO> listBenhNhan = bnBUS.select();
            List<lichHenDTO> listLichHen = lhBUS.select();
            List<lichHenDTO> lhbacsi = listLichHen
                .Where(lh => lh.MaTaiKhoan == maBS && lh.NgayHen.Date >= DateTime.Today)
                .ToList();

            DataTable table = new DataTable();
            table.Columns.Add("Số thứ tự", typeof(int));
            table.Columns.Add("Mã lịch hẹn", typeof(int));
            table.Columns.Add("Mã bệnh nhân", typeof(int));
            table.Columns.Add("Tên bệnh nhân", typeof(string));
            table.Columns.Add("Ngày sinh", typeof(string));
            table.Columns.Add("Địa chỉ", typeof(string));
            table.Columns.Add("Ngày hẹn", typeof(string));
            table.Columns.Add("Giờ hẹn", typeof(string));
            table.Columns.Add("Trạng thái", typeof(string));

            HashSet<int> dsMaBN = new HashSet<int>();

            foreach (BenhNhanDTO bn in listBenhNhan)
            {
                foreach (lichHenDTO lh in lhbacsi)
                {
                    if (bn.MaBN == lh.MaBenhNhan && lh.TrangThai != "Đã khám")
                    {
                        DataRow row = table.NewRow();
                        row["Số thứ tự"] = stt;
                        row["Mã lịch hẹn"] = lh.MaLichHen;
                        row["Mã bệnh nhân"] = bn.MaBN;
                        row["Tên bệnh nhân"] = bn.TenBN;
                        row["Ngày sinh"] = Convert.ToDateTime(bn.NgsinhBN).ToString("dd/MM/yyyy");
                        row["Địa chỉ"] = bn.DiachiBN;
                        row["Ngày hẹn"] = lh.NgayHen.ToString("dd/MM/yyyy");
                        row["Giờ hẹn"] = lh.NgayHen.ToString("HH:mm");
                        row["Trạng thái"] = lh.TrangThai;
                        table.Rows.Add(row);

                        if (lh.TrangThai == "Chờ khám")
                            dsMaBN.Add(bn.MaBN);

                        stt++;
                    }
                }
            }

            gird.DataSource = table.DefaultView;
            gird.RowPrePaint -= gird_RowPrePaint;
            gird.RowPrePaint += gird_RowPrePaint;

            mabenhnhan.Items.Clear();
            foreach (int ma in dsMaBN)
                mabenhnhan.Items.Add(ma);
        }

        private void gird_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < gird.Rows.Count)
            {
                var cell = gird.Rows[e.RowIndex].Cells["Trạng thái"];
                string trangThai = cell?.Value?.ToString();

                if (trangThai == "Đã khám")
                {
                    gird.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightGreen;
                    gird.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.DarkGreen;
                }
                else if (trangThai == "Chờ kê thuốc")
                {
                    gird.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightYellow;
                    gird.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.DarkOrange;
                }
                else
                {
                    gird.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
                    gird.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
                }
            }
        }

        private void gird_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= gird.Rows.Count) return;

            DataGridViewRow row = gird.Rows[e.RowIndex];

            if (row.Cells["Mã lịch hẹn"].Value != DBNull.Value)
                _maLichHen = Convert.ToInt32(row.Cells["Mã lịch hẹn"].Value);

            string trangThai = row.Cells["Trạng thái"].Value?.ToString();

            hoten.Text = row.Cells["Tên bệnh nhân"].Value?.ToString();
            mabenhnhan.Text = row.Cells["Mã bệnh nhân"].Value?.ToString();
            string ngay = row.Cells["Ngày hẹn"].Value?.ToString();
            string gio = row.Cells["Giờ hẹn"].Value?.ToString();
            ngaykham.Text = ngay + " " + gio;

            if (trangThai == "Chờ khám")
            {
                btnLapphieu.Visible = true;
                btnLapphieu.Enabled = true;
                btnKeToa.Visible = false;
                btnKeToa.Enabled = false;
                _maPKBVuaLap = -1;
                _maBNVuaLap = -1;
            }
            else if (trangThai == "Chờ kê thuốc")
            {
                btnLapphieu.Visible = false;
                btnLapphieu.Enabled = false;

                _maBNVuaLap = Convert.ToInt32(row.Cells["Mã bệnh nhân"].Value);

                // Tìm PKB mới nhất của BN này
                List<phieukhambenhDTO> listPKB = pkbBUS.select();
                var pkbCuaBN = listPKB
                    .Where(x => x.MaBenhNhan == _maBNVuaLap)
                    .OrderByDescending(x => x.MaPKB)
                    .FirstOrDefault();

                if (pkbCuaBN != null)
                    _maPKBVuaLap = pkbCuaBN.MaPKB;

                btnKeToa.Visible = true;
                btnKeToa.Enabled = true;
            }
        }

        private void btnKeToa_Click(object sender, EventArgs e)
        {
            if (_maPKBVuaLap == -1 || _maBNVuaLap == -1)
            {
                MessageBox.Show("Không tìm thấy thông tin phiếu khám hợp lệ.", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Lấy ngày tái khám nếu có tick
            DateTime? ngayTaiKham = ngaytaikham.Checked ? ngaytaikham.Value.Date : (DateTime?)null;

            // Lấy thông tin lịch hẹn hiện tại để copy maTaiKhoan, maDieuDuong
            List<lichHenDTO> listLH = lhBUS.select();
            var lhHienTai = listLH.FirstOrDefault(x => x.MaLichHen == _maLichHen);

            KeToa toa = new KeToa(_maPKBVuaLap, _maBNVuaLap, _maLichHen, ngayTaiKham, lhHienTai);
            toa.ShowDialog();

            Load_Gird();
            btnKeToa.Visible = false;
            btnKeToa.Enabled = false;
            btnLapphieu.Visible = true;
            btnLapphieu.Enabled = true;
            _maPKBVuaLap = -1;
            _maBNVuaLap = -1;
            _maLichHen = -1;
        }

        private void mabenhnhan_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mabenhnhan.SelectedIndex < 0) return;
            int selectedMaBN = int.Parse(mabenhnhan.SelectedItem.ToString());
            List<BenhNhanDTO> listBenhNhan = bnBUS.select();
            load_ten(listBenhNhan, selectedMaBN);
        }

        private void maPKB_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2CheckBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }
    }
}