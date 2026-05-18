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

namespace GUI_QLPK
{
    public partial class DanhSachHoaDon : Form
    {
        System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("en-US");
        BenhNhanService bnBus = new BenhNhanService();
        PhieukhambenhBUS pkbBus = new PhieukhambenhBUS();
        BenhBUS beBus = new BenhBUS();
        ChandoanBUS cdBus = new ChandoanBUS();
        HoaDonService hdBus = new HoaDonService();
        taiKhoanBUS tkBus = new taiKhoanBUS();
        private int stt;
        public DanhSachHoaDon()
        {
            InitializeComponent();
            load_data();
            gird.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
        private void load_data()
        {
            stt = 1;
            bnBus = new BenhNhanService();
            beBus = new BenhBUS();
            pkbBus = new PhieukhambenhBUS();
            cdBus = new ChandoanBUS();
            List<BenhNhanDTO> listBenhNhan = bnBus.select();
            List<benhDTO> listBenh = beBus.select();
            List<phieukhambenhDTO> listpkb = pkbBus.select();
            List<chandoanDTO> listcd = cdBus.select();
            List<hoadonDTO> listhd = hdBus.select();
            List<taiKhoanDTO> listTK = tkBus.select();
            this.loadData_Vao_GridView(listBenhNhan, listpkb, listhd, listTK);
        }
        private void loadData_Vao_GridView(
              List<BenhNhanDTO> listBenhNhan,
              List<phieukhambenhDTO> listpkb,
              List<hoadonDTO> listhd,
              List<taiKhoanDTO> listTK)
        {
            if (listBenhNhan == null || listpkb == null || listhd == null || listTK == null)
            {
                MessageBox.Show("Có lỗi khi lấy dữ liệu từ DB",
                                "Result",
                                MessageBoxButtons.OKCancel,
                                MessageBoxIcon.Error);
                return;
            }

            DataTable table = new DataTable();

            table.Columns.Add("Số thứ tự", typeof(int));
            table.Columns.Add("Tên bệnh nhân", typeof(string));
            table.Columns.Add("Ngày khám", typeof(string));
            table.Columns.Add("Ngày tái khám", typeof(string));
            table.Columns.Add("Tiền khám", typeof(string));
            table.Columns.Add("Tiền thuốc", typeof(string));
            table.Columns.Add("Tổng tiền", typeof(string));
            table.Columns.Add("Ngày lập", typeof(string));
            table.Columns.Add("Nhân viên thu ngân", typeof(string));

            var result = (
                from pkb in listpkb
                join bn in listBenhNhan
                    on pkb.MaBenhNhan equals bn.MaBN
                join hd in listhd
                    on pkb.MaPKB equals hd.MaPKB
                join tk in listTK
                    on hd.MaNVTN equals tk.MaTK
                orderby pkb.MaPKB
                select new
                {
                    TenBenhNhan = bn.TenBN,
                    NgayKham = pkb.NgayKham,
                    NgayTaiKham = pkb.NgayTaiKham,
                    TienKham = hd.TienKham,
                    TienThuoc = hd.TienThuoc,
                    TongTien = hd.TongTien,
                    NgayLap = hd.NgayLapHoaDon,
                    ThuNgan = tk.Name
                }
            ).ToList();

            int stt = 1;

            foreach (var item in result)
            {
                DataRow row = table.NewRow();

                row["Số thứ tự"] = stt++;
                row["Tên bệnh nhân"] = item.TenBenhNhan;
                row["Ngày khám"] = item.NgayKham.ToString("dd/MM/yyyy");

                if (item.NgayTaiKham != null)
                    row["Ngày tái khám"] = Convert.ToDateTime(item.NgayTaiKham).ToString("dd/MM/yyyy");
                else
                    row["Ngày tái khám"] = "Chưa có";

                row["Tiền khám"] = item.TienKham.ToString("N0", culture);
                row["Tiền thuốc"] = item.TienThuoc.ToString("N0", culture);
                row["Tổng tiền"] = item.TongTien.ToString("N0", culture);
                row["Ngày lập"] = item.NgayLap.ToString("dd/MM/yyyy");
                row["Nhân viên thu ngân"] = item.ThuNgan;

                table.Rows.Add(row);
            }

            gird.DataSource = table.DefaultView;
        }
    }
}
