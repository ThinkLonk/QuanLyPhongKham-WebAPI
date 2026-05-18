using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLPKEF.Models;

[Table("BenhNhan")]
public class BenhNhan
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int MaBenhNhan { get; set; }

    [Required, MaxLength(50)]
    public string TenBenhNhan { get; set; } = null!;

    public DateTime NgaySinh { get; set; }

    [Required, MaxLength(50)]
    public string DiaChi { get; set; } = null!;

    [Required, MaxLength(12)]
    public string CCCD { get; set; } = null!;

    [Required, MaxLength(50)]
    public string GioiTinh { get; set; } = null!;

    [MaxLength(100)]
    public string? Email { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<PhieuKhamBenh> PhieuKhams { get; set; } = new List<PhieuKhamBenh>();
    public virtual ICollection<LichHen> LichHens { get; set; } = new List<LichHen>();
}
