using Microsoft.AspNetCore.Mvc;
using QLPKEF.Models;
using QLPKEF.Repositories;

namespace QLPKWebAPI.Controllers;

/// <summary>
/// RESTful Controller cho bảng BenhNhan.
/// Tuân thủ chuẩn REST: GET / POST / PUT / DELETE và trả JSON.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class BenhNhanController : ControllerBase
{
    private readonly BenhNhanRepository _repo;
    public BenhNhanController(BenhNhanRepository repo) => _repo = repo;

    /// <summary>GET /api/benhnhan – Danh sách bệnh nhân (chưa bị xoá mềm).</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BenhNhan>>> GetAll()
        => Ok(await _repo.GetAllAsync());

    /// <summary>GET /api/benhnhan/search?keyword=... – Tìm kiếm theo tên / CCCD.</summary>
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<BenhNhan>>> Search([FromQuery] string keyword)
        => Ok(await _repo.SearchAsync(keyword ?? string.Empty));

    /// <summary>GET /api/benhnhan/{id}/lichsu – Lịch sử khám bệnh.</summary>
    [HttpGet("{id:int}/lichsu")]
    public async Task<ActionResult<IEnumerable<object>>> LichSu(int id)
        => Ok(await _repo.GetLichSuKhamAsync(id));

    /// <summary>POST /api/benhnhan – Thêm mới.</summary>
    [HttpPost]
    public async Task<ActionResult<BenhNhan>> Create([FromBody] BenhNhan dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var created = await _repo.AddAsync(dto);
        return CreatedAtAction(nameof(GetAll), new { id = created.MaBenhNhan }, created);
    }

    /// <summary>PUT /api/benhnhan/{id} – Cập nhật.</summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] BenhNhan dto)
    {
        if (id != dto.MaBenhNhan) return BadRequest("Mã không trùng khớp");
        var ok = await _repo.UpdateAsync(dto);
        return ok ? NoContent() : NotFound();
    }

    /// <summary>DELETE /api/benhnhan/{id} – Xoá mềm.</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _repo.SoftDeleteAsync(id);
        return ok ? NoContent() : NotFound();
    }
}
