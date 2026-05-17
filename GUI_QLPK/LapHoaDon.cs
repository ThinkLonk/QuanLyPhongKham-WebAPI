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
        BenhNhanBUS bnBus = new BenhNhanBUS();
        PhieukhambenhBUS pkbBus = new PhieukhambenhBUS();
        CultureInfo culture = new CultureInfo("en-US");
     
        public float tt;
        public float tkham;
        public int maNV;
        public int stt;

        public LapHoaDon(int mataikhoan)
        {
            maNV = mataikhoan;
            InitializeComponent();
            listcd = cdBus.select();
            listdv = donviBus.select();
            load();
            gird.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        // Hàm helper lấy maPKB an toàn, trả -1 nếu không hợp lệ
        private int GetMaPKB()
        {
            if (mapkb.SelectedValue == null)
                return -1;

            return Convert.ToInt32(
                mapkb.SelectedValue
            );
        }

        public void load()
        {
            hdBus = new HoadonBUS();
            stt = 1; // ← reset stt mỗi lần load
            ngaylap.Text = DateTime.Now.Date.ToString("dd/MM/yyyy"); // ← dùng Now thay UtcNow
            mahd.Text = "Tự động";
            load_combobox();
            load_TenBN();
            loadtiendichvu();

            int maPKB = GetMaPKB();
            if (maPKB != -1)
                load_data(maPKB);
        }

        public void load_combobox()
        {
            List<phieukhambenhDTO> listpkb = pkbBus.select();
            List<hoadonDTO> listhd = hdBus.select();
            List<dichvuDTO> listDichVu = dvBus.select();
            List<BenhNhanDTO> listBenhNhan = bnBus.select();
            loadData_Vao_Combobox(listpkb, listhd, listDichVu, listBenhNhan);
        }

        public void load_TenBN()
        {
            BenhNhanBUS bnBus = new BenhNhanBUS();
            List<BenhNhanDTO> listBenhnhan = bnBus.select();
            List<phieukhambenhDTO> listpkb = pkbBus.select();
            loadData_TenBN(listBenhnhan, listpkb);
        }

        private void loadData_Vao_Combobox(List<phieukhambenhDTO> listpkb, List<hoadonDTO> listhd, List<dichvuDTO> listDichVu, List<BenhNhanDTO> listBenhNhan)
        {
            //mapkb.Items.Clear();
            comboDichVu.Items.Clear();

            if (listpkb == null)
            {
                MessageBox.Show("Có lỗi khi lấy thông tin PKB từ DB", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            List<PKBDisplay> dsPKB =
     new List<PKBDisplay>();

            foreach (phieukhambenhDTO pkb in listpkb)
            {
                bool exists = false;

                foreach (hoadonDTO hd in listhd)
                {
                    if (hd.MaPKB == pkb.MaPKB)
                    {
                        exists = true;
                        break;
                    }
                }

                if (!exists)
                {
                    BenhNhanDTO bn =
                        listBenhNhan.FirstOrDefault(
                            x => x.MaBN == pkb.MaBenhNhan
                        );

                    dsPKB.Add(new PKBDisplay
                    {
                        MaPKB = pkb.MaPKB,

                        HienThi =
                            $"PKB {pkb.MaPKB} - {bn?.TenBN}"
                    });
                }
            }
            mapkb.DataSource = null;

            mapkb.DisplayMember = "HienThi";
            mapkb.ValueMember = "MaPKB";
            mapkb.DataSource = dsPKB;

            if (mapkb.Items.Count > 0)
                mapkb.SelectedIndex = 0;

            // Load dịch vụ
            foreach (dichvuDTO dichvu in listDichVu)
                comboDichVu.Items.Add(dichvu.TenDichVu);

            if (comboDichVu.Items.Count > 0)
                comboDichVu.SelectedIndex = 0;
        }

        private void loadData_TenBN(List<BenhNhanDTO> listBenhnhan, List<phieukhambenhDTO> listpkb)
        {
            int maPKB = GetMaPKB();
            if (maPKB == -1)
            {
                tenbn.Text = "";
                ngayTaiKham.Text = "Chưa có";
                return;
            }

            var pkb = listpkb.FirstOrDefault(p => p.MaPKB == maPKB);
            if (pkb == null)
            {
                tenbn.Text = "";
                ngayTaiKham.Text = "Chưa có";
                return;
            }

            var bn = listBenhnhan?.FirstOrDefault(b => b.MaBN == pkb.MaBenhNhan);
            tenbn.Text = bn != null ? bn.TenBN : "";

            // ← xử lý nullable NgayTaiKham
            ngayTaiKham.Text = pkb.NgayTaiKham.HasValue
                ? pkb.NgayTaiKham.Value.ToString("dd/MM/yyyy")
                : "Chưa có";
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            int maPKB = GetMaPKB();
            if (maPKB == -1)
            {
                MessageBox.Show("Vui lòng chọn phiếu khám bệnh", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            hoadonDTO hd = new hoadonDTO();
            hd.MaNVTN = maNV;
            hd.TongTien = tt;
            hd.MaPKB = maPKB;
            hd.NgayLapHoaDon = DateTime.Now.Date; // ← dùng Now
            hd.TienKham = tkham;
            hd.TienThuoc = hdBus.tienthuoc(hd, maPKB);

            // ← xử lý nullable NgayTaiKham
            if (ngayTaiKham.Text == "Chưa có" || string.IsNullOrWhiteSpace(ngayTaiKham.Text))
                hd.NgayTaiKham = null;
            else
            {
                DateTime parsedDate;
                if (DateTime.TryParseExact(ngayTaiKham.Text, "dd/MM/yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
                    hd.NgayTaiKham = parsedDate;
                else
                    hd.NgayTaiKham = null;
            }

            hdBus = new HoadonBUS();
            bool kq = hdBus.them(hd);

            if (!kq)
            {
                MessageBox.Show("Lưu hóa đơn thất bại. Vui lòng kiểm tra lại dữ liệu", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show("Lưu hóa đơn thành công", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Xuất PDF
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

                string folder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string filename = $"HD_{mahd.Text}_{DateTime.Now:yyyyMMddHHmm}.pdf";
                string fullPath = System.IO.Path.Combine(folder, filename);

                xuatpdf(fullPath, tenbn.Text, DateTime.Now, dt, comboDichVu.Text);
                MessageBox.Show($"Xuất hóa đơn thành công!\nĐường dẫn: {fullPath}", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất hóa đơn: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            load();
        }

        public void load_data(int maPKB)
        {
            stt = 1; // ← reset stt mỗi lần load_data
            thBus = new ThuocBUS();
            ktBus = new ChiTietToaThuocBUS();
            List<thuocDTO> listThuoc = thBus.selectbypkb(maPKB);
            List<ChiTietToaThuocDTO> listkethuoc = ktBus.selectbypkb(maPKB);
            loadData_Vao_GridView(listThuoc, listkethuoc);
        }

        private void loadData_Vao_GridView(List<thuocDTO> listThuoc, List<ChiTietToaThuocDTO> listkethuoc)
        {
            if (listThuoc == null || listkethuoc == null)
            {
                MessageBox.Show("Có lỗi khi tải thông tin thuốc từ DB", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                        row["STT"] = stt;
                        row["Tên thuốc"] = th.TenThuoc;

                        var donvi = listdv.FirstOrDefault(d => d.MaDonVi == th.MaDonVi);
                        row["Đơn vị tính"] = donvi != null ? donvi.TenDonVi : "";

                        row["Đơn giá"] = th.DonGia;
                        row["Số lượng"] = kt.SoLuong;
                        row["Thành tiền"] = (kt.SoLuong * th.DonGia).ToString();
                        table.Rows.Add(row);
                        stt++;
                    }
                }
            }

            gird.DataSource = table.DefaultView;
        }

        private void comboDichVu_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadtiendichvu();
        }

        public void loadtiendichvu()
        {
            int maPKB = GetMaPKB();
            if (maPKB == -1) return;

            List<dichvuDTO> listDichVu = dvBus.select();
            string tenDichVuChon = comboDichVu.Text;

            // ← fix BUG H-03: tìm theo TenDichVu thay vì SelectedIndex + 1
            var dichVuChon = listDichVu.FirstOrDefault(d => d.TenDichVu == tenDichVuChon);
            if (dichVuChon == null) return;

            tkham = dichVuChon.TienDichVu;

            decimal valueTienkham = (decimal)tkham;
            tienkham.Text = valueTienkham.ToString("N0", culture);

            decimal valueTthuoc = hdBus.tienthuoc(new hoadonDTO(), maPKB);
            tienthuoc.Text = valueTthuoc.ToString("N0", culture);

            tt = (float)(valueTthuoc + valueTienkham);
            tongtien.Text = ((decimal)tt).ToString("N0", culture);
        }

        private void mapkb_SelectedIndexChanged(object sender, EventArgs e)
        {
            int maPKB = GetMaPKB();
            if (maPKB == -1) return;

            stt = 1; // ← reset stt
            tt = 0;
            hdBus = new HoadonBUS();

            load_TenBN();
            load_data( Convert.ToInt32(mapkb.SelectedValue));

            decimal valueTthuoc = hdBus.tienthuoc(new hoadonDTO(), maPKB);
            tienthuoc.Text = valueTthuoc.ToString("N0", culture);

            decimal valueTienkham;
            if (decimal.TryParse(tienkham.Text, NumberStyles.AllowThousands, culture, out valueTienkham))
                tienkham.Text = valueTienkham.ToString("N0", culture);

            tt = (float)(valueTthuoc + valueTienkham);
            tongtien.Text = ((decimal)tt).ToString("N0", culture);
        }

        private void btnHoanTac_Click(object sender, EventArgs e)
        {
            mahd.Text = "Tự động";
            tongtien.Text = "";
            tienthuoc.Text = "";
            tienkham.Text = "";
            tenbn.Text = "";
            ngayTaiKham.Text = "Chưa có"; // ← không set ngày hôm nay
            ngaylap.Text = DateTime.Now.Date.ToString("dd/MM/yyyy");

            gird.DataSource = null;
            gird.Rows.Clear();
            tt = 0;
            tkham = 0;
            stt = 1;

            load_combobox();
            if (mapkb.Items.Count > 0)
                mapkb.SelectedIndex = 0;
        }

        private void xuatpdf(
     string outPath,
     string name,
     DateTime time,
     DataTable dtThuoc,
     string serviceName)
        {
            string fontPath = @"C:\Windows\Fonts\arial.ttf";

            try
            {
                if (!System.IO.File.Exists(fontPath))
                {
                    throw new Exception(
                        "Không tìm thấy font Arial"
                    );
                }

                PdfWriter writer = new PdfWriter(outPath);

                PdfDocument pdfDoc =
                    new PdfDocument(writer);

                Document document =
                    new Document(
                        pdfDoc,
                        iText.Kernel.Geom.PageSize.A4
                    );

                document.SetMargins(40, 40, 40, 40);

                PdfFont vnFont =
                    PdfFontFactory.CreateFont(
                        fontPath,
                        PdfEncodings.IDENTITY_H,
                        EmbeddingStrategy.PREFER_EMBEDDED
                    );

                document.SetFont(vnFont);
                document.SetFontSize(12);

                // Tiêu đề
                document.Add(
                    new Paragraph("HÓA ĐƠN KHÁM BỆNH")
                        .SetFont(vnFont)
                        .SetFontSize(18)
                        .SetTextAlignment(
                            TextAlignment.CENTER
                        )
                        .SetMarginBottom(20f)
                );

                // Header info
                float[] infoWidths = { 1, 1 };

                Table tblInfo =
                    new Table(
                        UnitValue.CreatePercentArray(
                            infoWidths
                        )
                    )
                    .UseAllAvailableWidth()
                    .SetMarginBottom(0f);

                tblInfo.AddCell(
                    new Cell()
                        .Add(
                            new Paragraph(
                                $"Mã hóa đơn: {mahd.Text}"
                            )
                        )
                        .SetBorder(Border.NO_BORDER)
                        .SetFont(vnFont)
                        .SetFontSize(12)
                );

                tblInfo.AddCell(
                    new Cell()
                        .Add(
                            new Paragraph(
                                $"Mã phiếu khám bệnh: " +
                                $"{Convert.ToInt32(mapkb.SelectedValue)}"
                            )
                        )
                        .SetBorder(Border.NO_BORDER)
                        .SetFont(vnFont)
                        .SetFontSize(12)
                );

                document.Add(tblInfo);

                // Thông tin bệnh nhân
                document.Add(
                    new Paragraph(
                        $"Tên bệnh nhân: {tenbn.Text}\n" +
                        $"Ngày lập hóa đơn: {ngaylap.Text}\n" +
                        $"Ngày tái khám: {ngayTaiKham.Text}\n"
                    )
                    .SetFontSize(12)
                    .SetFont(vnFont)
                    .SetTextAlignment(
                        TextAlignment.LEFT
                    )
                    .SetMarginTop(0f)
                    .SetMarginBottom(20f)
                );

                // Bảng
                float[] colWidths =
                {
            1f, 6f, 2f, 3f, 3f
        };

                Table table =
                    new Table(
                        UnitValue.CreatePercentArray(
                            colWidths
                        )
                    )
                    .UseAllAvailableWidth();

                string[] headers =
                {
            "STT",
            "Tên thuốc, dịch vụ",
            "Số lượng",
            "Đơn giá",
            "Thành tiền"
        };

                foreach (string h in headers)
                {
                    table.AddHeaderCell(
                        new Cell()
                            .Add(
                                new Paragraph(h)
                                    .SetFontSize(12)
                            )
                            .SetFont(vnFont)
                            .SetBackgroundColor(
                                ColorConstants.LIGHT_GRAY
                            )
                            .SetTextAlignment(
                                TextAlignment.CENTER
                            )
                    );
                }

                int rowStt = 1;

                decimal serviceFee = (decimal)tkham;

                // Dòng dịch vụ
                table.AddCell(
                    new Cell()
                        .Add(
                            new Paragraph(
                                rowStt.ToString()
                            )
                        )
                        .SetTextAlignment(
                            TextAlignment.CENTER
                        )
                );

                table.AddCell(
                    new Cell()
                        .Add(
                            new Paragraph(serviceName)
                        )
                );

                table.AddCell(
                    new Cell()
                        .Add(new Paragraph("1"))
                        .SetTextAlignment(
                            TextAlignment.CENTER
                        )
                );

                table.AddCell(
                    new Cell()
                        .Add(
                            new Paragraph(
                                serviceFee.ToString("N0")
                            )
                        )
                        .SetTextAlignment(
                            TextAlignment.RIGHT
                        )
                );

                table.AddCell(
                    new Cell()
                        .Add(
                            new Paragraph(
                                serviceFee.ToString("N0")
                            )
                        )
                        .SetTextAlignment(
                            TextAlignment.RIGHT
                        )
                );

                // Thuốc
                decimal totalThuoc = 0;

                for (int i = 0; i < dtThuoc.Rows.Count; i++)
                {
                    DataRow row = dtThuoc.Rows[i];

                    int qty =
                        Convert.ToInt32(
                            row["Số lượng"]
                        );

                    decimal price =
                        Convert.ToDecimal(
                            row["Đơn giá"]
                        );

                    decimal thanhTien =
                        price * qty;

                    totalThuoc += thanhTien;

                    rowStt++;

                    table.AddCell(
                        new Cell()
                            .Add(
                                new Paragraph(
                                    rowStt.ToString()
                                )
                            )
                            .SetTextAlignment(
                                TextAlignment.CENTER
                            )
                    );

                    table.AddCell(
                        new Cell()
                            .Add(
                                new Paragraph(
                                    row["Tên thuốc"]
                                        ?.ToString() ?? ""
                                )
                            )
                    );

                    table.AddCell(
                        new Cell()
                            .Add(
                                new Paragraph(
                                    qty.ToString()
                                )
                            )
                            .SetTextAlignment(
                                TextAlignment.CENTER
                            )
                    );

                    table.AddCell(
                        new Cell()
                            .Add(
                                new Paragraph(
                                    price.ToString("N0")
                                )
                            )
                            .SetTextAlignment(
                                TextAlignment.RIGHT
                            )
                    );

                    table.AddCell(
                        new Cell()
                            .Add(
                                new Paragraph(
                                    thanhTien.ToString("N0")
                                )
                            )
                            .SetTextAlignment(
                                TextAlignment.RIGHT
                            )
                    );
                }

                document.Add(table);

                // Tổng tiền
                decimal tongCong =
                    serviceFee + totalThuoc;

                document.Add(
                    new Paragraph(
                        $"Tổng cộng: " +
                        $"{tongCong.ToString("N0")} VNĐ"
                    )
                    .SetFont(vnFont)
                    .SetFontSize(12)
                    .SetTextAlignment(
                        TextAlignment.RIGHT
                    )
                    .SetMarginTop(5f)
                );

                // Footer
                document.Add(
                    new Paragraph(
                        "\nCảm ơn quý khách!"
                    )
                    .SetFont(vnFont)
                    .SetFontSize(11)
                    .SetTextAlignment(
                        TextAlignment.CENTER
                    )
                );

                document.Close();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(
                    "Lỗi khi xuất PDF: " +
                    ex.GetBaseException().Message,
                    ex
                );
            }
        }
    }
}