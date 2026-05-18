using Microsoft.EntityFrameworkCore;
using QLPKEF.Context;
using QLPKEF.Repositories;

// =================================================================
//  ASP.NET Core 6 Web API – QLPK
//  Đáp ứng yêu cầu 4.4: Web API + RESTful + JSON
// =================================================================

var builder = WebApplication.CreateBuilder(args);

// Thêm DbContext sử dụng SQL Server (Entity Framework Core)
builder.Services.AddDbContext<QLPKDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// Đăng ký Repository theo Dependency Injection
builder.Services.AddScoped<BenhNhanRepository>();
builder.Services.AddScoped<HoaDonRepository>();

// Hỗ trợ cả JSON (mặc định) và XML formatter
builder.Services.AddControllers()
    .AddXmlSerializerFormatters();

// Swagger (OpenAPI)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() {
        Title       = "QLPK Web API",
        Version     = "v1",
        Description = "Web API quản lý phòng khám – sử dụng EF Core + RESTful + JSON"
    });
});

// CORS cho client web bên ngoài (mở rộng tự do trong môi trường demo)
builder.Services.AddCors(o => o.AddPolicy("Open",
    p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("Open");
app.UseAuthorization();
app.MapControllers();

app.Run();
