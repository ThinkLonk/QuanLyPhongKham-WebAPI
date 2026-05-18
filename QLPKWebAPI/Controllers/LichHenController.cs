using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLPKEF.Context;
using QLPKEF.Models;

namespace QLPKWebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class LichHenController : ControllerBase
{
    private readonly QLPKDbContext _db;
    public LichHenController(QLPKDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await (from l in _db.LichHens
                  join bn in _db.BenhNhans on l.MaBenhNhan equals bn.MaBenhNhan
                  join bs in _db.TaiKhoans on l.MaTaiKhoan equals bs.MaTaiKhoan into bsg
                  from bs in bsg.DefaultIfEmpty()
                  orderby l.NgayHen
                  select new
                  {
                      l.MaLichHen,
                      l.NgayHen,
                      l.MaTaiKhoan,
                      l.MaBenhNhan,
                      l.MaDieuDuong,
                      l.TrangThai,
                      TenBenhNhan = bn.TenBenhNhan,
                      TenBacSi    = bs != null ? bs.Name : null
                  }).ToListAsync());

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] LichHen dto)
    {
        if (dto.NgayHen.Date < DateTime.Today)
            return BadRequest(new { message = "Không thể đặt lịch trong quá khứ" });

        dto.TrangThai ??= "Chờ khám";
        _db.LichHens.Add(dto);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { id = dto.MaLichHen }, dto);
    }

    [HttpPut("{id:int}/trangthai")]
    public async Task<IActionResult> CapNhatTrangThai(int id, [FromBody] string trangThaiMoi)
    {
        var lh = await _db.LichHens.FindAsync(id);
        if (lh == null) return NotFound();
        lh.TrangThai = trangThaiMoi;
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
