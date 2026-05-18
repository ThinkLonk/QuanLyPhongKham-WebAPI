using QLPKBUS;
using QLPKDAL;
using QLPKDTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using System.Runtime.Remoting.Messaging;
using iText.Kernel.Colors;
using iText.IO.Font;
using static iText.Kernel.Font.PdfFontFactory;
using iText.Layout.Borders;
using iTextImage = iText.Layout.Element.Image; 
namespace GUI_QLPK
{
    public partial class BaoCaoDoanhThu : Form
    {
        HoaDonService hdBus = new HoaDonService();
        public int stt;
        System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("en-US");
        public BaoCaoDoanhThu()
        {
            InitializeComponent();
            gird.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            nam.SelectedItem = "2025";
            thang.SelectedItem = "7";
        }
        public void load_data()
        {
            stt = 1;
            string month= thang.Text.ToString();
            string year = nam.Text.ToString();
            hdBus = new HoaDonService();
            List<hoadonDTO> listHoadonMonth = hdBus.selectByMonth(month, year);
            this.loadData_Vao_GirdView(listHoadonMonth);
            Dictionary<string, float> dataByMonth = new Dictionary<string, float>();
            for(int mon = 1; mon <= 12; mon++)
            {
                //lấy tên tháng bằng culture hiện tại
                string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(mon);
                //lấy doanh thu tháng hiện tại
                float doanhthu = hdBus.doanhthuMonth(mon.ToString(), year);
                if(doanhthu > 0)
                {
                    dataByMonth.Add(monthName, doanhthu); //thêm doanh thu vào dictionary
                }
            }
            //xóa all dữ liệu 
            chart1.Series.Clear();
            //xóa khu vực biểu đồ
            chart1.ChartAreas.Clear();
            //tạo series mới
            ChartArea chartArea = chart1.ChartAreas.Add("chartArea");

             //3) Tạo series mới kiểu Column
            Series series = chart1.Series.Add("Doanh thu năm " + year);
            series.ChartType = SeriesChartType.Column;

            // 4) Cấu hình trục X
            chartArea.AxisX.IsLabelAutoFit = true;
            chartArea.AxisX.Interval = 1;                  // mỗi tháng một nhãn
            chartArea.AxisX.LabelStyle.Angle = -45;         // xoay nghiêng 45° cho dễ đọc

            foreach (KeyValuePair<string, float> item in dataByMonth)
            {
                series.Points.AddXY(item.Key, item.Value);
            }

            //tạo dictionary để lưu doanh thu theo ngày
            Dictionary<string, float> dataByDate = new Dictionary<string, float>();
            for (int day = 1; day <= DateTime.DaysInMonth(int.Parse(year), int.Parse(month)); day++)
            {
                string ngayLapHD = new DateTime(int.Parse(year), int.Parse(month), day).ToString("yyyy-MM-dd");
                float doanhThu = float.Parse(hdBus.doanhthu(ngayLapHD).ToString());

                if (doanhThu > 0)
                {
                    dataByDate.Add(ngayLapHD, doanhThu);
                }
            }

            chart2.Series.Clear();
            chart2.ChartAreas.Clear();
            ChartArea chartArea2 = chart2.ChartAreas.Add("chartArea"); //tạo khu vực biểu đồ mới
            Series series2 = chart2.Series.Add("Doanh thu tháng " + month); //tạo serise mới kiểu column
            series2.ChartType = SeriesChartType.Column;

            foreach (var item in dataByDate)
            {
                series2.Points.AddXY(item.Key, item.Value);
            }
        }
        //table
        private void loadData_Vao_GirdView(List<hoadonDTO> listhoadon)
        {
            if (listhoadon == null)
            {
                System.Windows.Forms.MessageBox.Show("Có lỗi khi lấy thông tin từ DB", "Result", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                return;
            }
            decimal tongdoanhthu = 0;
            DataTable table = new DataTable();
            table.Columns.Add("Số Thứ Tự", typeof(int));
            table.Columns.Add("Ngày Lập Hóa Đơn", typeof(string));
            table.Columns.Add("Tổng Số Bệnh Nhân", typeof(int));
            table.Columns.Add("Doanh Thu", typeof(string));
            table.Columns.Add("Tỷ Lệ", typeof(string));
            // Tính tổng doanh thu
            foreach (hoadonDTO hd in listhoadon)
            {
                string ngkham = DateTime.Parse(hd.NgayLapHoaDon.ToString()).ToString("yyyy-MM-dd");
                tongdoanhthu += decimal.Parse(hdBus.doanhthu(ngkham).ToString());
            }
            foreach(hoadonDTO hd in listhoadon)
            {
                DataRow row = table.NewRow();
                //format cho ngày tháng
                string ngkham = DateTime.Parse(hd.NgayLapHoaDon.ToString()).ToString("yyyy-MM-dd");
                row["Ngày Lập Hóa Đơn"] = DateTime.Parse(ngkham.ToString()).ToString("dd-MM-yyyy");
                //lấy số bệnh nhân trong ngày
                row["Tổng Số Bệnh Nhân"] = int.Parse(hdBus.sobenhnhan(ngkham).ToString());
                //lấy doanh thu trong ngày và chuyển đổi sang chuỗi
                string valueDoanhthu = hdBus.doanhthu(ngkham).ToString(CultureInfo.InvariantCulture);
                decimal parsedDoanhthu;
                // Format doanh thu với "en-US" culture. Có dấu phân cách số
                if (decimal.TryParse(valueDoanhthu, NumberStyles.Number, culture, out parsedDoanhthu))
                {
                    // Chuyển đổi doanh thu sang chuỗi với định dạng "N0" (số nguyên không có phần thập phân)
                    row["Doanh Thu"] = parsedDoanhthu.ToString("N0", culture);
                }
                // Tính tỷ lệ doanh thu so với tổng doanh thu
                row["Tỷ Lệ"] = Math.Round(((double)float.Parse(hdBus.doanhthu(ngkham).ToString()) / (double)tongdoanhthu) * 100, 2).ToString() + "%";
                row["Số Thứ Tự"] = stt;
                table.Rows.Add(row);
                stt += 1;
            }
            gird.DataSource = table.DefaultView;
        }

        private void xem_Click(object sender, EventArgs e)
        {
            load_data();
        }

        private void btn_Xuatpdf_Click(object sender, EventArgs e)
        {
            string fontPath = @"C:\Windows\Fonts\arial.ttf";
            SaveFileDialog save = new SaveFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf",
                FileName = "BaoCaoDoanhThu.pdf"
            };

            if (save.ShowDialog() == DialogResult.OK)
            {
                using (PdfWriter writer = new PdfWriter(save.FileName))
                {
                    using (PdfDocument pdf = new PdfDocument(writer))
                    {
                        Document doc = new Document(pdf, iText.Kernel.Geom.PageSize.A4.Rotate());
                        PdfFont font = PdfFontFactory.CreateFont( fontPath,PdfEncodings.IDENTITY_H,EmbeddingStrategy.PREFER_EMBEDDED);

                        // 1. Tiêu đề
                        Paragraph title = new Paragraph("BÁO CÁO DOANH THU")
                            .SetFont(font)
                            .SetFontSize(18)
                            .SetTextAlignment(TextAlignment.CENTER);
                        doc.Add(title);

                        string thangNam = $"Tháng {thang.Text} Năm {nam.Text}";
                        Paragraph subtitle = new Paragraph(thangNam)
                            .SetFont(font)
                            .SetFontSize(14)
                            .SetTextAlignment(TextAlignment.CENTER);
                        doc.Add(subtitle);

                        // Tạo bảng 2 cột để chứa 2 biểu đồ ngang hàng
                        Table chartTable = new Table(UnitValue.CreatePercentArray(new float[] { 50, 50 })).UseAllAvailableWidth();

                        // chart1
                        using (MemoryStream ms1 = new MemoryStream())
                        {
                            chart1.SaveImage(ms1, ChartImageFormat.Png);
                            iTextImage img1 = new iTextImage(iText.IO.Image.ImageDataFactory.Create(ms1.ToArray()));
                            img1.SetAutoScale(true);
                            chartTable.AddCell(new Cell().Add(img1).SetBorder(Border.NO_BORDER));
                        }

                        // chart2
                        using (MemoryStream ms2 = new MemoryStream())
                        {
                            chart2.SaveImage(ms2, ChartImageFormat.Png);
                            iTextImage img2 = new iTextImage(iText.IO.Image.ImageDataFactory.Create(ms2.ToArray()));
                            img2.SetAutoScale(true);
                            chartTable.AddCell(new Cell().Add(img2).SetBorder(Border.NO_BORDER));
                        }

                        // Thêm bảng 2 biểu đồ vào tài liệu
                        doc.Add(chartTable);


                        // 2. Bảng từ DataGridView
                        Table pdfTable = new Table(gird.Columns.Count, true);
                        pdfTable.SetWidth(UnitValue.CreatePercentValue(100));

                        // Tiêu đề cột
                        foreach (DataGridViewColumn col in gird.Columns)
                        {
                            Cell headerCell = new Cell()
                                .Add(new Paragraph(col.HeaderText).SetFont(font))
                                .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                                .SetTextAlignment(TextAlignment.CENTER);
                            pdfTable.AddHeaderCell(headerCell);
                        }

                        // Dữ liệu từng dòng
                        foreach (DataGridViewRow row in gird.Rows)
                        {
                            if (!row.IsNewRow)
                            {
                                foreach (DataGridViewCell cell in row.Cells)
                                {
                                    pdfTable.AddCell(new Paragraph(cell.Value?.ToString() ?? "").SetFont(font));
                                }
                            }
                        }
                        doc.Add(pdfTable);
                        doc.Close();
                    }
                }

                MessageBox.Show("Xuất PDF thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
