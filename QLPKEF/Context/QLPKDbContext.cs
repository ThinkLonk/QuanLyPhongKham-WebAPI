using Microsoft.EntityFrameworkCore;
using QLPKEF.Models;

namespace QLPKEF.Context;

/// <summary>
/// DbContext của hệ thống Quản lý Phòng khám.
/// Đáp ứng yêu cầu Entity Framework Code First (DbContext + DbSet) và Database First
/// (sử dụng kèm dotnet ef dbcontext scaffold).
///
/// Code First (tạo CSDL từ entity):
///     dotnet ef migrations add InitialCreate --project QLPKEF
///     dotnet ef database update             --project QLPKEF
///
/// Database First (sinh entity từ CSDL có sẵn):
///     dotnet ef dbcontext scaffold \
///         "Server=localhost;Database=QLPK;Trusted_Connection=True;TrustServerCertificate=True" \
///         Microsoft.EntityFrameworkCore.SqlServer \
///         --output-dir Models --context QLPKDbContext --force
/// </summary>
public class QLPKDbContext : DbContext
{
    // ============================ DbSet ===========================
    public DbSet<TaiKhoan>          TaiKhoans          => Set<TaiKhoan>();
    public DbSet<Role>              Roles              => Set<Role>();
    public DbSet<BenhNhan>          BenhNhans          => Set<BenhNhan>();
    public DbSet<LichHen>           LichHens           => Set<LichHen>();
    public DbSet<PhieuKhamBenh>     PhieuKhamBenhs     => Set<PhieuKhamBenh>();
    public DbSet<Benh>              Benhs              => Set<Benh>();
    public DbSet<ChuanDoan>         ChuanDoans         => Set<ChuanDoan>();
    public DbSet<Thuoc>             Thuocs             => Set<Thuoc>();
    public DbSet<DonVi>             DonVis             => Set<DonVi>();
    public DbSet<CachDung>          CachDungs          => Set<CachDung>();
    public DbSet<ToaThuoc>          ToaThuocs          => Set<ToaThuoc>();
    public DbSet<ChiTietDonThuoc>   ChiTietDonThuocs   => Set<ChiTietDonThuoc>();
    public DbSet<HoaDon>            HoaDons            => Set<HoaDon>();
    public DbSet<DichVu>            DichVus            => Set<DichVu>();

    // ============================ Cấu hình ========================
    public QLPKDbContext() { }

    public QLPKDbContext(DbContextOptions<QLPKDbContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Fallback connection nếu chưa được cấu hình từ bên ngoài (vd. WebAPI)
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(
                "Server=localhost;Database=QLPK;Trusted_Connection=True;TrustServerCertificate=True;",
                sql => sql.EnableRetryOnFailure());
        }
    }

    // ============================ Fluent API ======================
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Composite Key cho ChuanDoan & ChiTietDonThuoc
        modelBuilder.Entity<ChuanDoan>()
            .HasKey(c => new { c.MaBenh, c.MaPKB });

        modelBuilder.Entity<ChiTietDonThuoc>()
            .HasKey(c => new { c.MaThuoc, c.MaToaThuoc });

        // Unique constraint
        modelBuilder.Entity<BenhNhan>()
            .HasIndex(b => b.CCCD)
            .IsUnique();

        modelBuilder.Entity<TaiKhoan>()
            .HasIndex(t => t.UserName)
            .IsUnique();

        modelBuilder.Entity<Thuoc>()
            .HasIndex(t => t.TenThuoc)
            .IsUnique();

        modelBuilder.Entity<DichVu>()
            .HasIndex(d => d.TenDichVu)
            .IsUnique();

        // Default values
        modelBuilder.Entity<BenhNhan>()
            .Property(b => b.IsDeleted)
            .HasDefaultValue(false);

        modelBuilder.Entity<PhieuKhamBenh>()
            .Property(p => p.NgayKham)
            .HasDefaultValueSql("(getdate())");

        modelBuilder.Entity<PhieuKhamBenh>()
            .Property(p => p.DaGuiMail)
            .HasDefaultValue(false);

        modelBuilder.Entity<LichHen>()
            .Property(l => l.TrangThai)
            .HasDefaultValue("Chờ khám");

        // CHECK constraint
        modelBuilder.Entity<Thuoc>()
            .ToTable(t => t.HasCheckConstraint("chk_SoLuongThuoc", "[soLuong] >= 0"));

        modelBuilder.Entity<DichVu>()
            .ToTable(t => t.HasCheckConstraint("chk_TienDichVu", "[tienDichVu] >= 0"));

        modelBuilder.Entity<ChiTietDonThuoc>()
            .ToTable(t => t.HasCheckConstraint("chk_SoLuongKe", "[soLuong] > 0"));

        // Quan hệ điều dưỡng trong LichHen (FK riêng tới TaiKhoan)
        modelBuilder.Entity<LichHen>()
            .HasOne<TaiKhoan>()
            .WithMany()
            .HasForeignKey(l => l.MaDieuDuong)
            .OnDelete(DeleteBehavior.NoAction);

        // Tránh cascade cycle giữa TaiKhoan và PhieuKhamBenh/HoaDon
        modelBuilder.Entity<PhieuKhamBenh>()
            .HasOne(p => p.TaiKhoan)
            .WithMany(t => t.PhieuKhams)
            .HasForeignKey(p => p.MaTaiKhoan)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<HoaDon>()
            .HasOne(h => h.TaiKhoan)
            .WithMany(t => t.HoaDons)
            .HasForeignKey(h => h.MaTaiKhoan)
            .OnDelete(DeleteBehavior.NoAction);

        base.OnModelCreating(modelBuilder);
    }
}
