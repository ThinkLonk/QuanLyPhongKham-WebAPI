using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLPKEF.Context;
using QLPKEF.Models;
using QLPKEF.Samples;

namespace QLPKWebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ThuocController : ControllerBase
{
    private readonly QLPKDbContext _db;
    public ThuocController(QLPKDbContext db) => _db = db;

    ///GET /api/thuoc – Danh sách thuốc 
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _db.Thuocs
            .Include(t => t.DonVi)
            .Include(t => t.CachDung)
            .Select(t => new
            {
                t.MaThuoc, t.TenThuoc, t.DonGia, t.SoLuong, t.MaDonVi, t.MaCachDung,
                TenDonVi    = t.DonVi!.TenDonVi,
                TenCachDung = t.CachDung!.TenCachDung
            }).ToListAsync());

    /// GET /api/thuoc/saphet?nguong=10 
    [HttpGet("saphet")]
    public async Task<IActionResult> SapHet([FromQuery] int nguong = 10)
    {
        var all = await _db.Thuocs.ToListAsync();
        var data = LinqToXmlExamples.LocThuocSapHet(all, nguong)
            .Select(t => new { t.MaThuoc, t.TenThuoc, t.SoLuong });
        return Ok(data);
    }

    ///GET /api/thuoc/export-xml – Xuất danh mục thuốc dạng XML 
    [HttpGet("export-xml")]
    public async Task<IActionResult> ExportXml()
    {
        var tmp = Path.Combine(Path.GetTempPath(), $"DanhMucThuoc_{DateTime.Now:yyyyMMddHHmmss}.xml");
        await LinqToXmlExamples.ExportDanhMucThuocAsync(_db, tmp);
        var bytes = await System.IO.File.ReadAllBytesAsync(tmp);
        return File(bytes, "application/xml", Path.GetFileName(tmp));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Thuoc dto)
    {
        _db.Thuocs.Add(dto);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { id = dto.MaThuoc }, dto);
    }
}
