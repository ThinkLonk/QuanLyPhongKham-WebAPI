using iText.IO.Font;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using QLPKBUS;
using QLPKDAL;
using QLPKDTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static iText.Kernel.Font.PdfFontFactory;

namespace GUI_QLPK
{
    public partial class BaoCaoSuDungThuoc : Form
    {
        ThuocBUS thBus = new ThuocBUS();
        ChiTietToaThuocBUS ktBus = new ChiTietToaThuocBUS();
        donviBUS dvBus = new donviBUS();
        List<donViDTO> listDonVi;
        public int stt;
        public BaoCaoSuDungThuoc()
        {
            InitializeComponent();
            dvBus = new donviBUS();

            // Lấy danh sách đơn vị ở đây
            listDonVi = dvBus.select();
            gird.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
        public void load_data()
        {
            stt = 1;
            string month = thang.Text.ToString();
            string year = nam.Text.ToString();
            thBus = new ThuocBUS();
            ktBus = new ChiTietToaThuocBUS();
            List<thuocDTO> listThuoc = thBus.baocaobymonth(month, year);
            List<ChiTietToaThuocDTO> listkethuoc = ktBus.baocaobymonth(month, year);
            this.loadData_Vao_GridView(listThuoc, listkethuoc, month, year);

        }
        private void loadData_Vao_GridView(List<thuocDTO> listThuoc, List<ChiTietToaThuocDTO> listkethuoc, string month, string year)
        {
            if (listThuoc == null || listkethuoc == null)
            {
                System.Windows.Forms.MessageBox.Show("Có lỗi khi lấy thông tin từ DB", "Result", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                return;
            }
            DataTable table = new DataTable();
            table.Columns.Add("Số Thứ Tự", typeof(int));
            table.Columns.Add("Tên Thuốc", typeof(string));
            table.Columns.Add("Đơn Vị Tính", typeof(string));
            table.Columns.Add("Số Lần Dùng", typeof(int));
            table.Columns.Add("Số Lượng", typeof(int));
            foreach (thuocDTO th in listThuoc)
            {
                foreach (ChiTietToaThuocDTO kt in listkethuoc)
                {
                    if (th.MaThuoc == kt.MaThuoc)
                    {

                        DataRow row = table.NewRow();
                        row["Số Thứ Tự"] = stt;
                        row["Tên Thuốc"] = th.TenThuoc;
                        donViDTO dv = listDonVi.Find(x => x.MaDonVi == th.MaDonVi); //tìm trong listdonvi lấy ra tên đơn vị
                        row["Đơn Vị Tính"] = dv.TenDonVi;
                        row["Số Lượng"] = kt.SoLuong;
                        row["Số Lần Dùng"] = ktBus.solandungbymonth(th.MaThuoc.ToString(), month, year);
                        table.Rows.Add(row);
                        stt += 1;
                    }
                }
            }
            gird.DataSource = table.DefaultView;

        }

        private void xem_Click(object sender, EventArgs e)
        {
            load_data();
        }

        private void btn_Xuatpdf_Click(object sender, EventArgs e)
        {
            string fontPath = @"C:\Windows\Fonts\arial.ttf"; //font trong máy
            //tạo hộp thoại chọn nơi lưu
            SaveFileDialog save = new SaveFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf", //chỉ định file
                FileName = "BaoCaoSuDungThuoc.pdf" //tên gợi ý
            };
            //mở hộp thoại bấm save
            if (save.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    PdfWriter writer = new PdfWriter(save.FileName);
                    PdfDocument pdfDoc = new PdfDocument(writer);
                    Document doc = new Document(pdfDoc, iText.Kernel.Geom.PageSize.A4);
                    doc.SetMargins(40, 40, 40, 40); //lề trang

                    PdfFont vnFont = PdfFontFactory.CreateFont(fontPath, PdfEncodings.IDENTITY_H, EmbeddingStrategy.PREFER_EMBEDDED); 
                    doc.SetFont(vnFont);
                    doc.SetFontSize(12);

                    // 1. Tiêu đề
                    Paragraph title = new Paragraph("BÁO CÁO SỬ DỤNG THUỐC")
                        .SetFont(vnFont)
                        .SetFontSize(18)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetMarginBottom(10f);
                    doc.Add(title);

                    // 2. Tháng - Năm
                    string thangNam = $"Tháng {thang.Text} Năm {nam.Text}";
                    Paragraph subtitle = new Paragraph(thangNam)
                        .SetFont(vnFont)
                        .SetFontSize(14)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetMarginBottom(20f);
                    doc.Add(subtitle);

                    // 3. Bảng dữ liệu
                    float[] colWidths = Enumerable.Repeat(1f, gird.Columns.Count).ToArray(); //tạo dãy có n cột mỗi cột = 1f(tỉ lệ chia độ rộng cột)
                    Table table = new Table(UnitValue.CreatePercentArray(colWidths)).UseAllAvailableWidth(); //chia theo tỉ lệ và bảng chiếm hết bề ngang vùng in

                    // Tiêu đề cột
                    foreach (DataGridViewColumn col in gird.Columns)
                    {
                        Cell headerCell = new Cell()
                            .Add(new Paragraph(col.HeaderText).SetFont(vnFont))
                            .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                            .SetTextAlignment(TextAlignment.CENTER);
                        table.AddHeaderCell(headerCell);
                    }

                    // Dữ liệu
                    foreach (DataGridViewRow row in gird.Rows)
                    {
                        if (!row.IsNewRow)
                        {
                            foreach (DataGridViewCell cell in row.Cells)
                            {
                                string text = cell.Value?.ToString() ?? "";
                                Cell dataCell = new Cell()
                                    .Add(new Paragraph(text).SetFont(vnFont))
                                    .SetTextAlignment(TextAlignment.LEFT);
                                table.AddCell(dataCell);
                            }
                        }
                    }

                    doc.Add(table);
                    doc.Close();
                    MessageBox.Show("Xuất PDF thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xuất PDF: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
