using iText.Layout.Element;
using QLPKBUS;
using QLPKDAL;
using QLPKDTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace GUI_QLPK
{
    public partial class Home : Form
    {
        BenhNhanService bnBus = new BenhNhanService();
        LichHenService lichHenBus = new LichHenService();
        PhieukhambenhBUS pkbBus = new PhieukhambenhBUS();
        ChiTietToaThuocBUS ctThuocBus = new ChiTietToaThuocBUS();
        public Home()
        {
            InitializeComponent();
            load_data_lichhen();
            load_data_trieuchungphobien();
            hienThiCanhBaoThuocTrongKhung();

        }
        private void Home_Load(object sender, EventArgs e)
        {
            // 1. Đếm tổng bệnh nhân
            List<BenhNhanDTO> dsBenhNhan = bnBus.select();
            int tongBN = dsBenhNhan.Count;
            tongbenhnhan.Text = tongBN.ToString();

            // 2. Lấy ngày hiện tại và lịch hẹn trong ngày hôm nay
            DateTime homNay = DateTime.Today;
            List<lichHenDTO> dsLichHenHomNay = lichHenBus.selectByDate(homNay);
            lb_lichhen.Text = dsLichHenHomNay.Count.ToString();

            List<ChiTietToaThuocDTO> danhSachToa = ctThuocBus.selectByDate(DateTime.Today);
            int soLuongToa = danhSachToa.Count;
            lb_thuoc.Text = soLuongToa.ToString();

            // 5. Lấy danh sách phiếu khám bệnh trong ngày hôm nay
            List<phieukhambenhDTO> dsPhieuKhamHomNay = pkbBus.selectByDate(homNay);
            int tongPhieuKhamHomNay = dsPhieuKhamHomNay.Count;
            lb_phieu.Text = tongPhieuKhamHomNay.ToString();
            //load_data_bn();
        }
        public void load_data_lichhen()
        {
            lichHenBus = new LichHenService();
            bnBus = new BenhNhanService();
            List<lichHenDTO> listlh = lichHenBus.select();
            List<BenhNhanDTO> listbn = bnBus.select();
            this.loadData_Vao_GridView(listlh, listbn);
        }
        private void loadData_Vao_GridView(List<lichHenDTO> listLichhen, List<BenhNhanDTO> listbn)
        {
            DateTime homNay = DateTime.Today;
            if (listLichhen == null || listbn == null)
            {
                System.Windows.Forms.MessageBox.Show("Có lỗi khi lấy thông tin load thông tin thuốc từ DB", "Result", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                return;
            }
            DataTable table = new DataTable();
            table.Columns.Add("Mã bệnh nhân");
            table.Columns.Add("Tên bệnh nhân");
            table.Columns.Add("Giờ hẹn");

            foreach (lichHenDTO lh in listLichhen)
            {
                // chỉ lấy hẹn hôm nay + trạng thái "Chờ khám"
                string trangThai = lh.TrangThai == null ? "" : lh.TrangThai.Trim();
                bool chuaKham = string.Equals(trangThai, "Chờ khám", StringComparison.OrdinalIgnoreCase); //so sanh ko phan biet hoa thuong
                if (lh.NgayHen.Date == homNay && chuaKham)
                {
                    foreach (BenhNhanDTO bn in listbn)
                    {
                        if (bn.MaBN == lh.MaBenhNhan)
                        {
                            DataRow row = table.NewRow();
                            row["Mã bệnh nhân"] = bn.MaBN;
                            row["Tên bệnh nhân"] = bn.TenBN;
                            row["Giờ hẹn"] = lh.NgayHen.ToString("HH:mm");
                            table.Rows.Add(row);
                        }
                    }
                }
            }
            gird1.DataSource = table.DefaultView;
        }
        private void load_data_trieuchungphobien()
        {
            List<phieukhambenhDTO> dsPKB = pkbBus.selectByDate(DateTime.Today);

            if (dsPKB == null || dsPKB.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu phiếu khám hôm nay để thống kê triệu chứng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Đếm từng loại triệu chứng
            Dictionary<string, int> thongKeTrieuChung = new Dictionary<string, int>();
            foreach (phieukhambenhDTO pkb in dsPKB)
            {
                string trieuChung = pkb.TrieuChung != null ? pkb.TrieuChung.Trim() : ""; //lấy triệu chứng từ pkb

                if (trieuChung == "") continue;

                if (thongKeTrieuChung.ContainsKey(trieuChung)) //kiem tra co ton tai khong
                {
                    thongKeTrieuChung[trieuChung]++;
                }
                else
                {
                    thongKeTrieuChung.Add(trieuChung, 1);
                }
            }

            // Xóa dữ liệu cũ
            chart1.Series.Clear();
            chart1.Titles.Clear();
            chart1.Titles.Add("Tỷ lệ triệu chứng phổ biến trong ngày");

            // Tạo series biểu đồ tròn
            Series series = new Series();
            series.ChartType = SeriesChartType.Pie;
            series.IsValueShownAsLabel = true;
            series.Font = new Font("Times New Roman", 11);
            series.Label = "#PERCENT{P1}"; // hiển thị phần trăm trên biểu đồ

            int tong = 0;
            //tổng triệu chứng
            foreach (KeyValuePair<string, int> kvp in thongKeTrieuChung)
            {
                tong += kvp.Value;
            }

            foreach (KeyValuePair<string, int> kvp in thongKeTrieuChung)
            {
                int count = kvp.Value; //số lần xuất hiện
                double tile = (double)count / tong * 100; //tỉ lệ %

                int index = series.Points.AddY(count); //thêm điểm vào series
                series.Points[index].LegendText = kvp.Key; //hiển thị nhãn
            }

            chart1.Series.Add(series);
        }
        private void hienThiCanhBaoThuocTrongKhung()
        {
            ThuocService thuocBus = new ThuocService();
            List<thuocDTO> dsThuoc = thuocBus.select();
            List<thuocDTO> thuocSapHet = new List<thuocDTO>();

            foreach (thuocDTO thuoc in dsThuoc)
            {
                if (thuoc.SoLuong <= 10)
                {
                    thuocSapHet.Add(thuoc);
                }
            }
            //liệt kê
            if (thuocSapHet.Count > 0)
            {
                StringBuilder sb = new StringBuilder(); //dùng để thông báo
                sb.AppendLine("Các thuốc sắp hết:");
                foreach (thuocDTO thuoc in thuocSapHet)
                {
                    sb.AppendLine($"- {thuoc.TenThuoc}: còn {thuoc.SoLuong} viên");
                }

                lb_thongbao.Text = sb.ToString();
                lb_thongbao.ForeColor = Color.Red;
            }
            else
            {
                lb_thongbao.Text = "Tồn kho thuốc ổn định.";
                lb_thongbao.ForeColor = Color.Green;
            }

            lb_thongbao.AutoSize = true;
        }



    }
}
