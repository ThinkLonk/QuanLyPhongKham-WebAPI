
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
        public string   maBS;
        BenhNhanBUS bnBUS = new BenhNhanBUS();
        BenhBUS beBus = new BenhBUS();
        ChandoanBUS cdBUS = new ChandoanBUS();
        PhieukhambenhBUS pkbBUS = new PhieukhambenhBUS();
        lichHenBUS lhBUS = new lichHenBUS();
        lichHenDAL lichHenDAL = new lichHenDAL();

        private int stt;

        public ThemPhieuKhamBenh(string mabs)
        {
            maBS = mabs;
            InitializeComponent();
            load_combobox_benh();
            gird.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            Load_Gird();
            load_data();

        }
        // load dữ liệu mặc định cho phiếu khám bệnh
        public void load_data()
        {
            maPKB.Text = pkbBUS.autogenerate_mapkb().ToString();
            mabenhnhan.Text = "";
            hoten.Text = "";
            trieuchung.Text = "";
            ngaytaikham.Value = DateTime.Now.AddDays(7);

        }
        private void load_combobox_benh()
        {
            checkedListBoxBenh.Items.Clear();

            List<benhDTO> listBenh = beBus.select();

            foreach (benhDTO be in listBenh)
            {
                checkedListBoxBenh.Items.Add(be.TenBenh);
            }
        }
        // load tên bệnh nhân theo mã bệnh nhân
        private void load_ten(List<BenhNhanDTO> listBenhNhan, string mabn)
        {
            if (listBenhNhan == null)
            {
                System.Windows.Forms.MessageBox.Show("Có lỗi khi lấy thông tin từ DB", "Result", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                return;
            }
            foreach (BenhNhanDTO bn in listBenhNhan)
            {
                if (bn.MaBN.ToString() == mabn)
                {
                    hoten.Text = bn.TenBN;

                }
            }
        }


        private void loadData_Vao_comboboxbe(List<benhDTO> listBenh)
        {

            if (listBenh == null)
            {
                System.Windows.Forms.MessageBox.Show("Có lỗi khi lấy thông tin bệnh từ DB", "Result", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                return;

            }
            foreach (benhDTO be in listBenh)
            {
                checkedListBoxBenh.Items.Clear();



                foreach (benhDTO benh in listBenh)
                {
                    checkedListBoxBenh.Items.Add(benh.TenBenh);
                }
            }
        }
        private void btnLapphieu_Click(object sender, EventArgs e)
        {
            if (maPKB.Text == null || trieuchung.Text == null)
            {
                System.Windows.Forms.MessageBox.Show("Vui lòng nhập đầy đủ thông tin phiếu khám bệnh");
            }
            //kiểm tra ràng buộc

            DateTime ngayTaiKham = ngaytaikham.Value.Date;

            if (ngayTaiKham < DateTime.Today)
            {
                MessageBox.Show("Ngày tái khám không hợp lệ");
                return;
            }
            phieukhambenhDTO pkb = new phieukhambenhDTO();
            chandoanDTO cd = new chandoanDTO();

            List<benhDTO> listBenh = beBus.select();
            pkb.MaPKB = (maPKB.Text);
            pkb.NgayKham = DateTime.UtcNow.Date;
            pkb.TrieuChung = trieuchung.Text;
            pkb.MaBenhNhan = mabenhnhan.Text;
            pkb.NgayTaiKham = ngaytaikham.Value.Date;
            pkb.MBS = maBS;
            bool ADDPKB = pkbBUS.them(pkb); //lưu phiếu
    
           
            bool loiChanDoan = false;
            bool coBenhDuocChon = false;
            foreach (var item in checkedListBoxBenh.CheckedItems)
            {
                string tenBenh = item.ToString();

                var benhTimDuoc = listBenh
                    .FirstOrDefault(x => x.TenBenh == tenBenh);

                if (benhTimDuoc != null)
                {
                    coBenhDuocChon = true;


                    cd.MaPkb = (maPKB.Text);
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

            


            if (!loiChanDoan == true && ADDPKB == true)
            {
                // Cập nhật trạng thái lịch hẹn thành 'Đã khám'
                string maBN = (mabenhnhan.Text);


                lhBUS.CapNhatTrangThai(maBN, "Đã khám");


                System.Windows.Forms.MessageBox.Show("Lập phiếu thành công", "Result");
                load_data();
                Load_Gird();
            }
            else System.Windows.Forms.MessageBox.Show("Lập phiếu thất bại", "Result", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
        }
        public void Load_Gird()
        {
            int stt = 1;

            List<BenhNhanDTO> listBenhNhan = bnBUS.select(); //lấy ds bệnh nhân
            List<lichHenDTO> listLichHen = lhBUS.select(); //lấy ds lịch hẹn
            string mabs = maBS.ToString(); //mabn hiện tại
            List<lichHenDTO> lhbacsi = new List<lichHenDTO>();
            //lọc lịch hẹn của bác sĩ đang đăng nhập
            foreach (lichHenDTO lh in listLichHen)
            {
                //hiện trong ngày của bác sĩ 
                if (lh.MaTaiKhoan == mabs && lh.NgayHen.Date >= DateTime.Today && lh.TrangThai != "Đã khám")
                {
                    lhbacsi.Add(lh);
                }

            }

            DataTable table = new DataTable();
            table.Columns.Add("Số thứ tự", typeof(int));
            table.Columns.Add("Mã bệnh nhân", typeof(string));
            table.Columns.Add("Tên bệnh nhân", typeof(string));
            table.Columns.Add("Ngày sinh", typeof(string));
            table.Columns.Add("Địa chỉ", typeof(string));
            table.Columns.Add("Ngày hẹn", typeof(string));
            table.Columns.Add("Giờ hẹn", typeof(string));
            table.Columns.Add("Trạng thái", typeof(string));
            // dùng HashSet để lưu mã bệnh nhân đang được hiển thị (lưu không trùng)
            HashSet<string> dsMaBN = new HashSet<string>();

            foreach (BenhNhanDTO bn in listBenhNhan)
            {
                foreach (lichHenDTO lh in lhbacsi)
                {
                    if (bn.MaBN.ToString() == lh.MaBenhNhan)
                    {
                        DataRow row = table.NewRow();
                        row["Số thứ tự"] = stt;
                        row["Mã bệnh nhân"] = bn.MaBN;
                        row["Tên bệnh nhân"] = bn.TenBN;
                        row["Ngày sinh"] = DateTime.Parse(bn.NgsinhBN.ToString()).ToString("dd/MM/yyyy");
                        row["Địa chỉ"] = bn.DiachiBN;
                        row["Ngày hẹn"] = lh.NgayHen.ToString("dd/MM/yyyy");
                        row["Giờ hẹn"] = lh.NgayHen.ToString("hh:mm");
                        row["Trạng thái"] = lh.TrangThai;
                        table.Rows.Add(row);
                        dsMaBN.Add(bn.MaBN.ToString());
                        stt += 1;
                    }
                }
            }
            gird.DataSource = table.DefaultView;
            mabenhnhan.Items.Clear();
            foreach (string ma in dsMaBN)
            {
                mabenhnhan.Items.Add(ma);
            }
        }
        // Sự kiện tự động load thông tin lên
        private void gird_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //Kiểm tra dòng hợp lệ
            if (e.RowIndex >= 0 && e.RowIndex < gird.Rows.Count)
            {
                //lấy dòng đang click
                DataGridViewRow row = gird.Rows[e.RowIndex];
                hoten.Text = row.Cells[2].Value.ToString();
                mabenhnhan.Text = row.Cells[1].Value.ToString();
                string ngay = row.Cells[5].Value.ToString();    // "dd/MM/yyyy"
                string gio = row.Cells[6].Value.ToString();     // "HH:mm"
                ngaykham.Text = ngay + " " + gio;
            }
        }

        private void btnKeToa_Click(object sender, EventArgs e)
        {
            KeToa toa = new KeToa();
            toa.Show();
        }

        private void mabenhnhan_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mabenhnhan.SelectedIndex < 0) return;
            //lấy mã đã được chọn
            string selectedMaBN = mabenhnhan.SelectedItem.ToString();
            //lấy danh sách bệnh nhân
            List<BenhNhanDTO> listBenhNhan = bnBUS.select();
            load_ten(listBenhNhan, selectedMaBN);
        }
    }
}
