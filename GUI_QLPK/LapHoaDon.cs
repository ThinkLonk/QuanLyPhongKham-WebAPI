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
using System.Globalization;

namespace GUI_QLPK
{
    public partial class LapHoaDon : Form
    {
        HoadonBUS hdBus = new HoadonBUS();
        ThuocBUS thBus = new ThuocBUS();
        ChiTietToaThuocBUS ktBus = new ChiTietToaThuocBUS();
        DichvuBUS dvBus = new DichvuBUS();
        cachDungBUS cdBus = new cachDungBUS();
        donviBUS donviBus = new donviBUS();
        List<cachdungDTO> listcd;
        List<donViDTO> listdv;

        PhieukhambenhBUS pkbBus = new PhieukhambenhBUS();
        System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("en-US");
        public float tt;
        public float tkham;
        public string maNV;
        public int stt;
        public LapHoaDon(string mataikhoan)
        {
            maNV = mataikhoan; //lưu NV đăng nhập
            InitializeComponent();
            listcd = cdBus.select();
            listdv = donviBus.select();
            
            load();
            //tự động điều chỉnh độ rộng
            gird.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
        public void load()
        {
            PhieukhambenhBUS pkb = new PhieukhambenhBUS();
            hdBus = new HoadonBUS();
            ngaylap. Text = DateTime.UtcNow.Date.ToString("dd/MM/yyyy");
            mahd.Text = hdBus.autogenerate_mahd().ToString();
            load_combobox();
            load_TenBN();
            loadtiendichvu();
            load_data(mapkb.Text);
        }
        //load PKB & dịch vụ
        public void load_combobox()
        {
            BenhNhanBUS bnBus = new BenhNhanBUS();
            List<phieukhambenhDTO> listpkb = pkbBus.select();
            List<hoadonDTO> listhd = hdBus.select();
            List<dichvuDTO> listdv = dvBus.select();
            this.loadData_Vao_Combobox(listpkb, listhd, listdv);
        }
        public void load_TenBN()
        {
            BenhNhanBUS bnBus = new BenhNhanBUS();
            List<BenhNhanDTO> listBenhnhan = bnBus.select();
            List<phieukhambenhDTO> listpkb = pkbBus.select();
            this.loadData_TenBN(listBenhnhan, listpkb);
        }
        //nạp mã PKB & dịch vụ
        private void loadData_Vao_Combobox(List<phieukhambenhDTO> listpkb, List<hoadonDTO> listhd, List<dichvuDTO> listdv)
        {
            mapkb.Items.Clear();

            comboDichVu.Items.Clear();
            if (listpkb == null)
            {
                System.Windows.Forms.MessageBox.Show("Có lỗi khi lấy thông tin nạp vào combox pkb từ DB", "Result", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                return;
            }
            // Chọn ra các PKB CHƯA có hoá đơn
            foreach (phieukhambenhDTO pkb in listpkb)
            {
                bool exists = false;
                foreach (hoadonDTO hd in listhd)
                {
                    if (hd.MaPKB == pkb.MaPKB)
                    {
                        exists = true;
                        break; // Nếu tìm thấy khớp, thoát khỏi vòng lặp
                    }
                }

                if (!exists)
                {
                    mapkb.Items.Add(pkb.MaPKB);
                }

            }
            if (mapkb.Items.Count > 0)
            { mapkb.SelectedIndex = 0; }

            //nạp dịch vụ
            foreach (dichvuDTO dichvu in listdv)
            {

                comboDichVu.Items.Add(dichvu.TenDichVu);
                comboDichVu.SelectedIndex = 0;
            }
        }
        //Hiển thị tên BN và ngày tái khám theo PKB
        private void loadData_TenBN(List<BenhNhanDTO> listBenhnhan, List<phieukhambenhDTO> listpkb)
        {
            foreach (phieukhambenhDTO pkb in listpkb)
            {
                if (pkb.MaPKB == (mapkb.Text))
                {
                    if (listBenhnhan == null)
                    {
                        MessageBox.Show("Có lỗi khi lấy thông tin tên bệnh nhân từ DB", "Result", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                        return;
                    }

                    foreach (BenhNhanDTO bn in listBenhnhan)
                    {
                        if (bn.MaBN == pkb.MaBenhNhan)
                        {
                            tenbn.Text = bn.TenBN;

                        }
                    }
                    // Nếu có ngày tái khám, hiển thị nó
                    if (pkb.NgayTaiKham == null || pkb.NgayTaiKham == DateTime.MinValue)
                    {
                        ngayTaiKham.Text = DateTime.UtcNow.Date.ToString();
                    }
                    else
                        ngayTaiKham.Text = pkb.NgayTaiKham.ToString("dd/MM/yyyy");
                }
            }
        }


        private void btnLuu_Click(object sender, EventArgs e)
        {
            hoadonDTO hd = new hoadonDTO();
            hd.MaNVTN = maNV;
            hd.TongTien = tt;
            hd.MaPKB =(mapkb.Text);
            hd.NgayLapHoaDon = DateTime.UtcNow.Date;
            hd.TienKham = tkham;
            hd.TienThuoc = hdBus.tienthuoc(hd, mapkb.Text);
            hd.NgayTaiKham = DateTime.ParseExact(ngayTaiKham.Text,"dd/MM/yyyy",System.Globalization.CultureInfo.InvariantCulture).Date; ;
            hdBus = new HoadonBUS();
            bool kq = hdBus.them(hd);
            if (kq == false)
                System.Windows.Forms.MessageBox.Show("Lưu hóa đơn thất bại. Vui lòng kiểm tra lại dũ liệu", "Result", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
            else
            {
                System.Windows.Forms.MessageBox.Show("Lưu hóa đơn thành công", "Result");
            }
            try
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("Tên thuốc");
                dt.Columns.Add("Số lượng");
                dt.Columns.Add("Đơn giá");
                dt.Columns.Add("Thành tiền");

                foreach (DataGridViewRow row in gird.Rows)
                {
                    if (row.IsNewRow) continue;
                    DataRow dr = dt.NewRow();
                    dr["Tên thuốc"] = row.Cells["Tên thuốc"].Value;
                    dr["Số lượng"] = row.Cells["Số lượng"].Value;
                    dr["Đơn giá"] = row.Cells["Đơn giá"].Value;
                    dr["Thành tiền"] = row.Cells["Thành tiền"].Value;
                    dt.Rows.Add(dr);
                }
                //  Đặt đường dẫn lưu file
                string folder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string filename = $"HD_{mahd.Text}_{DateTime.Now:yyyyMMddHHmm}.pdf";
                string fullPath = System.IO.Path.Combine(folder, filename);
                string name = tenbn.Text;
                string service = comboDichVu.Text;
                DateTime time = DateTime.Parse(ngaylap.Text);
                // Gọi hàm xuất PDF
                try
                {
                    xuatpdf(fullPath, name, time, dt,service );
                    MessageBox.Show($"Xuất hóa đơn thành công!\nĐường dẫn: {fullPath}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xuất hóa đơn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch(Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Xuất hóa đơn thất bại. Vui lòng kiểm tra lại dũ liệu", "Result", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
            }
            load();
        }

        public void load_data(string mapkb)
        {
            thBus = new ThuocBUS();
            ktBus = new ChiTietToaThuocBUS();
            List<thuocDTO> listThuoc = thBus.selectbypkb(mapkb);
            List<ChiTietToaThuocDTO> listkethuoc = ktBus.selectbypkb(mapkb);
            this.loadData_Vao_GridView(listThuoc, listkethuoc);
        }
        private void loadData_Vao_GridView(List<thuocDTO> listThuoc, List<ChiTietToaThuocDTO> listkethuoc)
        {

            if (listThuoc == null || listkethuoc == null)
            {
                System.Windows.Forms.MessageBox.Show("Có lỗi khi lấy thông tin load thông tin thuốc từ DB", "Result", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                return;
            }

            DataTable table = new DataTable();
            table.Columns.Add("STT", typeof(int));
            table.Columns.Add("Tên thuốc", typeof(string));
            table.Columns.Add("Đơn vị tính", typeof(string));
            table.Columns.Add("Đơn giá", typeof(string));
            table.Columns.Add("Số lượng", typeof(string));
            table.Columns.Add("Thành tiền", typeof(string));
            foreach (thuocDTO th in listThuoc)
            {
                foreach (ChiTietToaThuocDTO kt in listkethuoc)
                {
                    if (th.MaThuoc == kt.MaThuoc)
                    {

                        DataRow row = table.NewRow();
                        row["Tên thuốc"] = th.TenThuoc;
                        foreach (donViDTO donvi in listdv)
                        {
                            if (donvi.MaDonVi == th.MaDonVi)
                            {
                                row["Đơn vị tính"] = donvi.TenDonVi;
                            }

                        }
                        row["Đơn giá"] = th.DonGia;
                        row["Số lượng"] = kt.SoLuong;
                        row["Thành tiền"] = (kt.SoLuong * th.DonGia).ToString();
                        row["STT"] = stt;
                        table.Rows.Add(row);
                        stt += 1;
                    }
                }
            }
            gird.DataSource = table.DefaultView;
        }

        private void comboDichVu_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadtiendichvu();
        }
        //tính tiền 
        public void loadtiendichvu()
        {
            int selectedIndex = comboDichVu.SelectedIndex; 
            List<dichvuDTO> listdv = dvBus.select();
            foreach (dichvuDTO d in listdv)
            {
                if (selectedIndex + 1 == int.Parse(d.MaDichVu)) //giả định ID = vị trí+1
                {
                    tienkham.Text = d.TienDichVu.ToString();
                    tkham = d.TienDichVu;
                    //format tiền khám N0
                    decimal valueTienkham;
                    if (decimal.TryParse(tienkham.Text, System.Globalization.NumberStyles.AllowThousands, culture, out valueTienkham))
                    {
                        tienkham.Text = String.Format(culture, "{0:N0}", valueTienkham);
                        tienkham.Select(tienkham.Text.Length, 0);
                    }
                    hoadonDTO hd = new hoadonDTO();
                    // Lấy tiền thuốc và chuyển đổi sang kiểu decimal
                    string tthuoc = hdBus.tienthuoc(hd, mapkb.Text).ToString();

                    // Chuyển đổi tiền thuốc sang kiểu decimal và định dạng lại chuỗi
                    decimal valueTthuoc;
                    if (decimal.TryParse(tthuoc, System.Globalization.NumberStyles.AllowThousands, culture, out valueTthuoc))
                    {
                        tienthuoc.Text = String.Format(culture, "{0:N0}", valueTthuoc);
                        tienthuoc.Select(tienthuoc.Text.Length, 0);
                    }

                    // Tính tổng tiền và chuyển đổi sang chuỗi
                    tt = (float)valueTthuoc + (float)valueTienkham;
                    decimal valueTongtien = (decimal)tt;

                    // Định dạng tổng tiền
                    tongtien.Text = String.Format(culture, "{0:N0}", valueTongtien);
                    tongtien.Select(tongtien.Text.Length, 0);
                }

            }
        }
        //khi nguoi dung chon pkb khac tinh toan lai
        private void mapkb_SelectedIndexChanged(object sender, EventArgs e)
        {
            load_data(mapkb.Text);
            load_TenBN();
            tt = 0;
            stt = 1;
            hdBus = new HoadonBUS();
            hoadonDTO hd = new hoadonDTO();
            load_TenBN();
            load_data(mapkb.Text);

            // Lấy tiền thuốc và chuyển đổi sang kiểu decimal
            string tthuoc = hdBus.tienthuoc(hd, mapkb.Text).ToString();

            // Chuyển đổi tiền thuốc sang kiểu decimal và định dạng lại chuỗi
            decimal valueTthuoc;
            if (decimal.TryParse(tthuoc, System.Globalization.NumberStyles.AllowThousands, culture, out valueTthuoc))
            {
                tienthuoc.Text = String.Format(culture, "{0:N0}", valueTthuoc);
                tienthuoc.Select(tienthuoc.Text.Length, 0);
            }

            // Chuyển đổi tiền khám sang kiểu decimal và định dạng lại chuỗi
            decimal valueTienkham;
            if (decimal.TryParse(tienkham.Text, System.Globalization.NumberStyles.AllowThousands, culture, out valueTienkham))
            {
                tienkham.Text = String.Format(culture, "{0:N0}", valueTienkham);
                tienkham.Select(tienkham.Text.Length, 0);
            }
            // Tính tổng tiền và chuyển đổi sang chuỗi
            tt = (float)valueTthuoc + (float)valueTienkham;
            decimal valueTongtien = (decimal)tt;

            // Định dạng tổng tiền
            tongtien.Text = String.Format(culture, "{0:N0}", valueTongtien);
            tongtien.Select(tongtien.Text.Length, 0);
        }

        private void btnHoanTac_Click(object sender, EventArgs e)
        {
            mahd.Text = hdBus.autogenerate_mahd().ToString();
            tongtien.Text = "";
            tienthuoc.Text = "";
            tienkham.Text = "";
            tenbn.Text = "";

            load_combobox();
            if (mapkb.Items.Count > 0)
            {
                mapkb.SelectedIndex = 0;
            }

            ngaylap.Text = DateTime.Today.ToString("dd/MM/yyyy");
            ngayTaiKham.Text = DateTime.Today.ToString("dd/MM/yyyy");

            gird.DataSource = null;
            gird.Rows.Clear();
            tt = 0;
            tkham = 0;
            stt = 1;
        }
        private void xuatpdf(string outPath, string name, DateTime time, DataTable dtThuoc, string serviceName)
        {
            string fontPath = @"C:\Windows\Fonts\arial.ttf";
            try
            {
                //khởi tạo pdf writer và document
                PdfWriter writer = new PdfWriter(outPath);
                PdfDocument pdfDoc = new PdfDocument(writer);
                //tạo document cỡ a4 lề 40
                Document document = new Document(pdfDoc, iText.Kernel.Geom.PageSize.A4);
                document.SetMargins(40, 40, 40, 40);

                //tải font chữ
                PdfFont vnFont = PdfFontFactory.CreateFont(
                    fontPath,
                    PdfEncodings.IDENTITY_H,             
                    EmbeddingStrategy.PREFER_EMBEDDED     
                    );
                document.SetFont(vnFont);
                document.SetFontSize(12);
                //tieu đề
                Paragraph header = new Paragraph("HÓA ĐƠN KHÁM BỆNH")
                        .SetFont(vnFont)
                        .SetFontSize(18)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetMarginBottom(20f);
                document.Add(header);

                float[] infoWidths = { 1, 1 };
                Table tblInfo = new Table(UnitValue.CreatePercentArray(infoWidths))
                    .UseAllAvailableWidth()
                    .SetMarginBottom(0f);

                // cell bên trái: mã phiếu khám
                tblInfo.AddCell(new Cell()
                    .Add(new Paragraph($"Mã hóa đơn: {mahd.Text}"))
                    .SetBorder(Border.NO_BORDER)
                    .SetFont(vnFont)
                    .SetFontSize(12));
                
                // cell bên phải: mã hóa đơn
                tblInfo.AddCell(new Cell()
                    .Add(new Paragraph($"Mã phiếu khám bệnh: {mapkb.Text}"))
                    .SetBorder(Border.NO_BORDER)
                    .SetFont(vnFont)
                    .SetFontSize(12));
                document.Add(tblInfo);

                //thông tin bệnh nhân
                Paragraph info = new Paragraph($"Tên bệnh nhân: {tenbn.Text}\n" +
                                                $"Ngày lập hóa đơn: {ngaylap.Text}\n" +
                                                $"Ngày tái khám: {ngayTaiKham.Text}\n")
                        .SetFontSize(12)
                        .SetFont(vnFont)
                        .SetTextAlignment(TextAlignment.LEFT)
                        .SetMarginTop(0f)
                        .SetMarginBottom(20f);
                document.Add(info);
                //tạo bảng thuốc
                float[] colWidths = { 1f, 6f, 2f, 3f, 3f };
                Table table = new Table(UnitValue.CreatePercentArray(colWidths)).UseAllAvailableWidth();
                //thêm tiêu đề cột
                string[] headers = { "STT", "Tên thuốc, dịch vụ", "Số lượng", "Đơn giá","Thành tiền" };
                for (int i = 0; i < headers.Length; i++)
                {
                    Cell cell = new Cell()
                         .Add(new Paragraph(headers[i]).SetFontSize(12))
                         .SetFont(vnFont)
                         .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                        .SetTextAlignment(TextAlignment.CENTER);
                    table.AddHeaderCell(cell);
                }
                int stt = 1;
                decimal serviceFee = decimal.Parse(tienkham.Text, NumberStyles.AllowThousands, culture);
                decimal serviceTotal = serviceFee * 1;

                table.AddCell(new Cell().Add(new Paragraph(stt.ToString())).SetTextAlignment(TextAlignment.CENTER));
                table.AddCell(new Cell().Add(new Paragraph(serviceName)).SetTextAlignment(TextAlignment.LEFT));
                table.AddCell(new Cell().Add(new Paragraph("1")).SetTextAlignment(TextAlignment.CENTER));
                table.AddCell(new Cell().Add(new Paragraph(serviceFee.ToString("N0"))).SetTextAlignment(TextAlignment.RIGHT));
                table.AddCell(new Cell().Add(new Paragraph(serviceTotal.ToString("N0"))).SetTextAlignment(TextAlignment.RIGHT));
                //thêm dữ liệu vào bảng
                decimal totalThuoc = 0;
                for (int i = 0; i < dtThuoc.Rows.Count; i++)
                {
                    DataRow row = dtThuoc.Rows[i];
                    int qty = Convert.ToInt32(row["Số lượng"]);
                    decimal price = Convert.ToDecimal(row["Đơn giá"]);
                    decimal thanhTien = price * qty;
                    totalThuoc += thanhTien;
                    stt++;
                    table.AddCell(new Cell().Add(new Paragraph((stt).ToString())).SetTextAlignment(TextAlignment.CENTER));
                    table.AddCell(new Cell().Add(new Paragraph(row["Tên thuốc"].ToString())).SetTextAlignment(TextAlignment.LEFT));
                    table.AddCell(new Cell().Add(new Paragraph(row["Số lượng"].ToString())).SetTextAlignment(TextAlignment.CENTER));
                    decimal unitPrice = Convert.ToDecimal(row["Đơn giá"]);
                    table.AddCell(new Cell().Add(new Paragraph(unitPrice.ToString("N0"))).SetTextAlignment(TextAlignment.RIGHT));
                    decimal thanhtien = Convert.ToDecimal(row["Thành tiền"]);
                    table.AddCell(new Cell().Add(new Paragraph(thanhtien.ToString("N0"))).SetTextAlignment(TextAlignment.RIGHT));
                }
                document.Add(table);
                decimal tongCong = serviceTotal + totalThuoc;
                document.Add(new Paragraph($"Tổng cộng: {tongCong.ToString("N0")} VNĐ")
                                .SetFont(vnFont).SetFontSize(12)
                                .SetTextAlignment(TextAlignment.RIGHT)
                                .SetMarginTop(5f));
                //đóng document
                document.Close();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Lỗi khi xuất PDF: " + ex.GetBaseException().Message, ex);
            }
        }
    }
}
