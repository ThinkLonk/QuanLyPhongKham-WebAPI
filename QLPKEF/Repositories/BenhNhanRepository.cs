using Microsoft.EntityFrameworkCore;
using QLPKEF.Context;
using QLPKEF.Models;

namespace QLPKEF.Repositories;


/// Ví dụ Repository sử dụng LINQ to Entities trên DbContext.

public class BenhNhanRepository
{
    private readonly QLPKDbContext _db;
    public BenhNhanRepository(QLPKDbContext db) => _db = db;

    /// Lấy toàn bộ bệnh nhân chưa bị xoá 
    public async Task<List<BenhNhan>> GetAllAsync() =>
        await _db.BenhNhans.AsNoTracking()
                           .Where(b => !b.IsDeleted)
                           .OrderBy(b => b.TenBenhNhan)
                           .ToListAsync();

    /// Tìm bệnh nhân theo từ khoá (tên / CCCD).
    public async Task<List<BenhNhan>> SearchAsync(string keyword) =>
        await _db.BenhNhans.AsNoTracking()
                           .Where(b => !b.IsDeleted
                                    && (b.TenBenhNhan.Contains(keyword)
                                     || b.CCCD.Contains(keyword)))
                           .ToListAsync();

    /// Lấy lịch sử khám của một bệnh nhân 
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

    /// Xoá mềm (đặt cờ IsDeleted = true).
    public async Task<bool> SoftDeleteAsync(int id)
    {
        var b = await _db.BenhNhans.FindAsync(id);
        if (b == null) return false;
        b.IsDeleted = true;
        return await _db.SaveChangesAsync() > 0;
    }
}
