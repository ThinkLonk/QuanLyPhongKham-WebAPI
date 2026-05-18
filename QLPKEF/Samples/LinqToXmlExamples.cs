using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using QLPKEF.Context;
using QLPKEF.Models;

namespace QLPKEF.Samples;

/// <summary>
/// Minh hoạ LINQ to XML – Yêu cầu 4.3 của giảng viên.
/// Đọc / xuất danh mục thuốc dưới dạng file XML để trao đổi dữ liệu
/// với các phần mềm thứ ba (vd. hệ thống bảo hiểm y tế).
/// </summary>
public static class LinqToXmlExamples
{
    /// <summary>
    /// Xuất danh mục thuốc trong CSDL ra file XML.
    /// Tận dụng LINQ to Entities (truy vấn) + LINQ to XML (sinh tài liệu XML).
    /// </summary>
    public static async Task ExportDanhMucThuocAsync(QLPKDbContext db, string filePath)
    {
        var thuocs = await db.Thuocs
            .Include(t => t.DonVi)
            .Include(t => t.CachDung)
            .ToListAsync();

        var xdoc = new XDocument(
            new XDeclaration("1.0", "utf-8", "yes"),
            new XComment("Danh mục thuốc xuất từ hệ thống QLPK"),
            new XElement("DanhSachThuoc",
                new XAttribute("ngayXuat", DateTime.Now.ToString("yyyy-MM-dd")),
                from t in thuocs
                select new XElement("Thuoc",
                    new XAttribute("ma", t.MaThuoc),
                    new XElement("Ten",      t.TenThuoc),
                    new XElement("DonGia",   t.DonGia),
                    new XElement("DonVi",    t.DonVi?.TenDonVi),
                    new XElement("CachDung", t.CachDung?.TenCachDung),
                    new XElement("SoLuong",  t.SoLuong ?? 0))));

        xdoc.Save(filePath);
    }

    /// <summary>
    /// Nhập danh mục thuốc từ file XML vào CSDL (LINQ to XML đọc, LINQ to Entities ghi).
    /// </summary>
    public static async Task<int> ImportDanhMucThuocAsync(QLPKDbContext db, string filePath)
    {
        var xdoc = XDocument.Load(filePath);

        var imports = xdoc.Descendants("Thuoc")
            .Select(x => new Thuoc
            {
                TenThuoc = (string)x.Element("Ten")!,
                DonGia   = (double)x.Element("DonGia")!,
                SoLuong  = (int?)x.Element("SoLuong"),
                MaDonVi    = 1,    // sẽ ánh xạ thực tế khi tích hợp
                MaCachDung = 1
            })
            .ToList();

        db.Thuocs.AddRange(imports);
        return await db.SaveChangesAsync();
    }

    /// <summary>
    /// LINQ to Objects: lọc thuốc tồn kho thấp từ danh sách đã có trong RAM.
    /// </summary>
    public static IEnumerable<Thuoc> LocThuocSapHet(IEnumerable<Thuoc> nguon, int nguong = 10) =>
        nguon.Where(t => (t.SoLuong ?? 0) <= nguong)
             .OrderBy(t => t.SoLuong)
             .ThenBy(t => t.TenThuoc);
}
