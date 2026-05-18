using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLPKEF.Models;

[Table("HoaDon")]
public class HoaDon
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int MaHoaDon { get; set; }

    public DateTime? NgayLapHoaDon { get; set; }

    public double? TienThuoc { get; set; }
    public double? TienKham { get; set; }
    public double? TongTien { get; set; }

    public DateTime? NgayTaiKham { get; set; }

    public int? MaPKB { get; set; }
    public int? MaTaiKhoan { get; set; }

    [ForeignKey(nameof(MaPKB))]
    public virtual PhieuKhamBenh? PhieuKhamBenh { get; set; }

    [ForeignKey(nameof(MaTaiKhoan))]
    public virtual TaiKhoan? TaiKhoan { get; set; }
}

[Table("DichVu")]
public class DichVu
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int MaDichVu { get; set; }

    [Required, MaxLength(50)]
    public string TenDichVu { get; set; } = null!;

    public double TienDichVu { get; set; }
}
