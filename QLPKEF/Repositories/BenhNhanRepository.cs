using Microsoft.EntityFrameworkCore;
using QLPKEF.Context;
using QLPKEF.Models;

namespace QLPKEF.Repositories;

/// <summary>
/// Ví dụ Repository sử dụng LINQ to Entities trên DbContext.
/// </summary>
public class BenhNhanRepository
{
    private readonly QLPKDbContext _db;
    public BenhNhanRepository(QLPKDbContext db) => _db = db;

    /// <summary>Lấy toàn bộ bệnh nhân chưa bị xoá mềm.</summary>
    public async Task<List<BenhNhan>> GetAllAsync() =>
        await _db.BenhNhans.AsNoTracking()
                           .Where(b => !b.IsDeleted)
                           .OrderBy(b => b.TenBenhNhan)
                           .ToListAsync();

    /// <summary>Tìm bệnh nhân theo từ khoá (tên / CCCD).</summary>
    public async Task<List<BenhNhan>> SearchAsync(string keyword) =>
        await _db.BenhNhans.AsNoTracking()
                           .Where(b => !b.IsDeleted
                                    && (b.TenBenhNhan.Contains(keyword)
                                     || b.CCCD.Contains(keyword)))
                           .ToListAsync();

    /// <summary>Lấy lịch sử khám của một bệnh nhân (LINQ Join nhiều bảng).</summary>
    public async Task<List<object>> GetLichSuKhamAsync(int maBenhNhan)
    {
        var query = from pkb in _db.PhieuKhamBenhs
                    where pkb.MaBenhNhan == maBenhNhan
                    join hd in _db.HoaDons on pkb.MaPKB equals hd.MaPKB into hdGroup
                    from hd in hdGroup.DefaultIfEmpty()
                    select new
                    {
                        pkb.MaPKB,
                        pkb.NgayKham,
                        pkb.TrieuChung,
                        TienKham = hd != null ? hd.TienKham : 0,
                        TongTien = hd != null ? hd.TongTien : 0
                    };
        return await query.Cast<object>().ToListAsync();
    }

    public async Task<BenhNhan> AddAsync(BenhNhan entity)
    {
        _db.BenhNhans.Add(entity);
        await _db.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> UpdateAsync(BenhNhan entity)
    {
        _db.BenhNhans.Update(entity);
        return await _db.SaveChangesAsync() > 0;
    }

    /// <summary>Xoá mềm (đặt cờ IsDeleted = true).</summary>
    public async Task<bool> SoftDeleteAsync(int id)
    {
        var b = await _db.BenhNhans.FindAsync(id);
        if (b == null) return false;
        b.IsDeleted = true;
        return await _db.SaveChangesAsync() > 0;
    }
}
