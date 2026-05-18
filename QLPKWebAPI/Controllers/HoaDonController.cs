using Microsoft.AspNetCore.Mvc;
using QLPKEF.Repositories;

namespace QLPKWebAPI.Controllers;

/// <summary>
/// RESTful Controller cho nghiệp vụ hoá đơn và báo cáo doanh thu.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class HoaDonController : ControllerBase
{
    private readonly HoaDonRepository _repo;
    public HoaDonController(HoaDonRepository repo) => _repo = repo;

    /// <summary>GET /api/hoadon – Toàn bộ hoá đơn.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<HoaDonDto>>> GetAll()
        => Ok(await _repo.GetAllAsync());

    /// <summary>GET /api/hoadon/doanhthu?thang=5&nam=2026 – Doanh thu theo tháng.</summary>
    [HttpGet("doanhthu")]
    public async Task<ActionResult<IEnumerable<DoanhThuTheoNgayDto>>> DoanhThu(
        [FromQuery] int thang, [FromQuery] int nam)
        => Ok(await _repo.DoanhThuTheoThangAsync(thang, nam));

    /// <summary>
    /// POST /api/hoadon/lap – Lập hoá đơn cho 1 phiếu khám.
    /// Gọi Stored Procedure sp_LapHoaDonThanhToan (có Transaction trên CSDL).
    /// </summary>
    [HttpPost("lap")]
    public async Task<IActionResult> Lap([FromBody] LapHoaDonRequest req)
    {
        try
        {
            await _repo.LapHoaDonAsync(req.MaPKB, req.MaTaiKhoan);
            return Ok(new { status = "OK", message = "Lập hoá đơn thành công" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { status = "ERROR", message = ex.Message });
        }
    }
}

public record LapHoaDonRequest(int MaPKB, int MaTaiKhoan);
