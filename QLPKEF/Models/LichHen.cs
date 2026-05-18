using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLPKEF.Models;

[Table("LichHen")]
public class LichHen
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int MaLichHen { get; set; }

    public DateTime NgayHen { get; set; }

    public int? MaTaiKhoan { get; set; }
    public int? MaBenhNhan { get; set; }
    public int? MaDieuDuong { get; set; }

    [MaxLength(30)]
    public string? TrangThai { get; set; }

    [ForeignKey(nameof(MaTaiKhoan))]
    public virtual TaiKhoan? BacSi { get; set; }

    [ForeignKey(nameof(MaBenhNhan))]
    public virtual BenhNhan? BenhNhan { get; set; }
}
