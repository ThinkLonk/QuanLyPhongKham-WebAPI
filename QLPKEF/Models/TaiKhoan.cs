using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLPKEF.Models;

/// <summary>
/// Entity ánh xạ tới bảng dbo.TaiKhoan trên SQL Server.
/// Sử dụng cho cả mô hình Code First lẫn Database First.
/// </summary>
[Table("TaiKhoan")]
public class TaiKhoan
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int MaTaiKhoan { get; set; }

    [Required, MaxLength(50)]
    public string UserName { get; set; } = null!;

    [Required, MaxLength(255)]
    public string PassWord { get; set; } = null!;

    [Required, MaxLength(50)]
    public string Name { get; set; } = null!;

    public int? MaRole { get; set; }

    // Navigation property
    [ForeignKey(nameof(MaRole))]
    public virtual Role? Role { get; set; }

    public virtual ICollection<PhieuKhamBenh> PhieuKhams { get; set; } = new List<PhieuKhamBenh>();
    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
}
