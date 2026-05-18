using Microsoft.AspNetCore.Mvc;
using QLPKEF.Repositories;

namespace QLPKWebAPI.Controllers;


[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class HoaDonController : ControllerBase
{
    private readonly HoaDonRepository _repo;
    public HoaDonController(HoaDonRepository repo) => _repo = repo;

    /// GET /api/hoadon – Toàn bộ hoá đơn.
    [HttpGet]
    public async Task<ActionResult<IEnumerable<HoaDonDto>>> GetAll()
        => Ok(await _repo.GetAllAsync());

    /// GET /api/hoadon/doanhthu?thang=5&nam=2026 – Doanh thu theo tháng
    [HttpGet("doanhthu")]
    public async Task<ActionResult<IEnumerable<DoanhThuTheoNgayDto>>> DoanhThu(
        [FromQuery] int thang, [FromQuery] int nam)
        => Ok(await _repo.DoanhThuTheoThangAsync(thang, nam));

    /// POST /api/hoadon/lap – Lập hoá đơn cho 1 phiếu khám.
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
