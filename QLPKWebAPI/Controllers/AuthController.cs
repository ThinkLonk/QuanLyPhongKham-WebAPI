using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLPKEF.Context;

namespace QLPKWebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly QLPKDbContext _db;
    public AuthController(QLPKDbContext db) => _db = db;

    ///POST /api/auth/login – Kiểm tra đăng nhập (LINQ to Entities).
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var tk = await _db.TaiKhoans
            .Include(t => t.Role)
            .Where(t => t.UserName == req.UserName && t.PassWord == req.PassWord)
            .Select(t => new
            {
                t.MaTaiKhoan, t.UserName, t.Name,
                TenRole = t.Role != null ? t.Role.TenRole : null
            })
            .FirstOrDefaultAsync();

        if (tk == null)
            return Unauthorized(new { message = "Sai tài khoản hoặc mật khẩu" });
        return Ok(tk);
    }
}

public record LoginRequest(string UserName, string PassWord);
