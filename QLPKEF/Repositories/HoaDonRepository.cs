using Microsoft.EntityFrameworkCore;
using QLPKEF.Context;
using QLPKEF.Models;

namespace QLPKEF.Repositories;

/// <summary>
/// Sử dụng LINQ to Entities cho các nghiệp vụ hoá đơn / báo cáo doanh thu.
/// Đồng thời minh hoạ gọi Stored Procedure thông qua EF Core.
/// </summary>
public class HoaDonRepository
{
    private readonly QLPKDbContext _db;
    public HoaDonRepository(QLPKDbContext db) => _db = db;

    /// <summary>Danh sách hoá đơn kèm tên bệnh nhân và thu ngân (Join).</summary>
    public async Task<List<HoaDonDto>> GetAllAsync()
    {
        return await (from h in _db.HoaDons
                      join p in _db.PhieuKhamBenhs on h.MaPKB equals p.MaPKB
                      join bn in _db.BenhNhans on p.MaBenhNhan equals bn.MaBenhNhan
                      join tk in _db.TaiKhoans on h.MaTaiKhoan equals tk.MaTaiKhoan
                      orderby h.NgayLapHoaDon descending
                      select new HoaDonDto
                      {
                          MaHoaDon          = h.MaHoaDon,
                          MaPKB             = h.MaPKB ?? 0,
                          MaTaiKhoan        = h.MaTaiKhoan ?? 0,
                          TenBenhNhan       = bn.TenBenhNhan,
                          TenNhanVienThuNgan = tk.Name,
                          NgayLapHoaDon     = h.NgayLapHoaDon,
                          TienThuoc         = h.TienThuoc ?? 0,
                          TienKham          = h.TienKham ?? 0,
                          TongTien          = h.TongTien ?? 0
                      }).ToListAsync();
    }

    /// <summary>Doanh thu theo tháng (GroupBy LINQ).</summary>
    public async Task<List<DoanhThuTheoNgayDto>> DoanhThuTheoThangAsync(int thang, int nam) =>
        await _db.HoaDons
            .Where(h => h.NgayLapHoaDon!.Value.Month == thang
                     && h.NgayLapHoaDon.Value.Year  == nam)
            .GroupBy(h => h.NgayLapHoaDon!.Value.Date)
            .Select(g => new DoanhThuTheoNgayDto
            {
                Ngay     = g.Key,
                SoHoaDon = g.Count(),
                TongTien = g.Sum(x => x.TongTien ?? 0)
            })
            .OrderBy(x => x.Ngay)
            .ToListAsync();

    /// <summary>Gọi Stored Procedure đã có sẵn trên CSDL qua EF Core.</summary>
    public async Task<int> LapHoaDonAsync(int maPKB, int maTaiKhoan) =>
        await _db.Database.ExecuteSqlInterpolatedAsync(
            $"EXEC sp_LapHoaDonThanhToan @maPKB={maPKB}, @maTaiKhoan={maTaiKhoan}");
}

// =================== DTO ===================
public class HoaDonDto
{
    public int       MaHoaDon            { get; set; }
    public int       MaPKB               { get; set; }
    public int       MaTaiKhoan          { get; set; }
    public string?   TenBenhNhan         { get; set; }
    public string?   TenNhanVienThuNgan  { get; set; }
    public DateTime? NgayLapHoaDon       { get; set; }
    public double    TienThuoc           { get; set; }
    public double    TienKham            { get; set; }
    public double    TongTien            { get; set; }
}

public class DoanhThuTheoNgayDto
{
    public DateTime Ngay     { get; set; }
    public int      SoHoaDon { get; set; }
    public double   TongTien { get; set; }
}
