using Microsoft.AspNetCore.Mvc;
using QLPKEF.Models;
using QLPKEF.Repositories;

namespace QLPKWebAPI.Controllers;




[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class BenhNhanController : ControllerBase
{
    private readonly BenhNhanRepository _repo;
    public BenhNhanController(BenhNhanRepository repo) => _repo = repo;

    /// GET /api/benhnhan – Danh sách bệnh nhân
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BenhNhan>>> GetAll()
        => Ok(await _repo.GetAllAsync());

    ///GET /api/benhnhan/search?keyword=... – Tìm kiếm theo tên / CCCD
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<BenhNhan>>> Search([FromQuery] string keyword)
        => Ok(await _repo.SearchAsync(keyword ?? string.Empty));

    /// GET /api/benhnhan/{id}/lichsu – Lịch sử khám bệnh
    [HttpGet("{id:int}/lichsu")]
    public async Task<ActionResult<IEnumerable<object>>> LichSu(int id)
        => Ok(await _repo.GetLichSuKhamAsync(id));

    /// POST /api/benhnhan – Thêm mới.
    [HttpPost]
    public async Task<ActionResult<BenhNhan>> Create([FromBody] BenhNhan dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var created = await _repo.AddAsync(dto);
        return CreatedAtAction(nameof(GetAll), new { id = created.MaBenhNhan }, created);
    }

    /// PUT /api/benhnhan/{id} – Cập nhật.
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] BenhNhan dto)
    {
        if (id != dto.MaBenhNhan) return BadRequest("Mã không trùng khớp");
        var ok = await _repo.UpdateAsync(dto);
        return ok ? NoContent() : NotFound();
    }

    ///DELETE /api/benhnhan/{id} – Xoá 
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _repo.SoftDeleteAsync(id);
        return ok ? NoContent() : NotFound();
    }
}
