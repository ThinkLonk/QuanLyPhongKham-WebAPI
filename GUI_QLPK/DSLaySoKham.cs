using QLPKBUS;
using QLPKDAL;
using QLPKDTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUI_QLPK
{
    public partial class DSLaySoKham : Form
    {
        System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("en-US");
        BenhNhanService bnBus = new BenhNhanService();
        LichHenService lhBus = new LichHenService();
        taiKhoanBUS tkBus = new taiKhoanBUS();
        private int stt;
        loaiTaiKhoanBUS loaitkBUS = new loaiTaiKhoanBUS();

        public DSLaySoKham()
        {
            InitializeComponent();
            nhapngay.Text = DateTime.Now.ToString("yyyy-MM-dd");
            load_data();    
            gird.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
        private void load_data()
        {
            stt = 1;
            bnBus = new BenhNhanService();
            lhBus = new LichHenService();
            List<BenhNhanDTO> listBenhNhan = bnBus.select();
            List<lichHenDTO> listlh = lhBus.select();
            List<taiKhoanDTO> listTK = tkBus.select();
            List<loaiTaiKhoanDTO> listLoaiTk = loaitkBUS.select();
            loadData_Vao_GridView(listBenhNhan, listlh, listTK, listLoaiTk);
        }
        public void loadData_Vao_GridView(List<BenhNhanDTO> listBenhNhan, List<lichHenDTO> listlh, List<taiKhoanDTO> listTK, List<loaiTaiKhoanDTO> listLoaiTk)
        {
            if (listBenhNhan == null || listlh == null)
            {
                System.Windows.Forms.MessageBox.Show("Có lỗi khi lấy thông tin từ DB", "Result", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                return;
            }

            DataTable table = new DataTable();
            table.Columns.Add("Số thứ tự", typeof(int));
            table.Columns.Add("Mã bệnh nhân", typeof(int));
            table.Columns.Add("Tên bệnh nhân", typeof(string));
            table.Columns.Add("Ngày khám", typeof(string));
            table.Columns.Add("Giờ khám", typeof(string));
            table.Columns.Add("Bác sĩ khám", typeof(string));
            table.Columns.Add("Trạng thái", typeof(string));
            table.Columns.Add("Nhân viên đăng ký", typeof(string));

            if (listBenhNhan != null && listlh != null && listTK != null)
            {
                
                var query = from lh in listlh
                            join bn in listBenhNhan on lh.MaBenhNhan equals bn.MaBN
                            select new { lh, bn };

                foreach (var item in query)
                {
                    DataRow row = table.NewRow();
                    row["Số thứ tự"] = stt++;
                    row["Mã bệnh nhân"] = item.bn.MaBN;
                    row["Tên bệnh nhân"] = item.bn.TenBN;
                    row["Trạng thái"] = item.lh.TrangThai;
                    row["Ngày khám"] = item.lh.NgayHen.ToString("yyyy-MM-dd");
                    row["Giờ khám"] = item.lh.NgayHen.ToString("HH:mm");

                    // Tìm bác sĩ và điều dưỡng trong listTK (Dùng FirstOrDefault để tránh lặp)
                    var bacSi = listTK.FirstOrDefault(t => t.MaTK == item.lh.MaTaiKhoan);
                    var dieuDuong = listTK.FirstOrDefault(t => t.MaTK == item.lh.MaDieuDuong);

                    row["Bác sĩ khám"] = bacSi?.Name ?? "Chưa có";
                    row["Nhân viên đăng ký"] = dieuDuong?.Name ?? "Chưa có";

                    table.Rows.Add(row);
                }
            }
            gird.DataSource = table.DefaultView;
        }

        private void timkiem_Click(object sender, EventArgs e)
        {
            List<BenhNhanDTO> listBenhNhan = bnBus.select();
            List<lichHenDTO> listlh = lhBus.select();
            List<taiKhoanDTO> listTK = tkBus.select();
            List<loaiTaiKhoanDTO> listLoaiTk = loaitkBUS.select();
            List<lichHenDTO> fill = new List<lichHenDTO>();
            foreach (lichHenDTO lh in listlh)
            {
                if (lh.NgayHen.Date == nhapngay.Value.Date)
                {
                    fill.Add(lh);
                }
            }
            stt = 1;
            loadData_Vao_GridView(listBenhNhan, fill, listTK, listLoaiTk);
        }
        private void btn_hoantac_Click(object sender, EventArgs e)
        {
            load_data();
        }
    }
}
