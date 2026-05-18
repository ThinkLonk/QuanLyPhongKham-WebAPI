using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLPKEF.Models;

[Table("Thuoc")]
public class Thuoc
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int MaThuoc { get; set; }

    [Required, MaxLength(50)]
    public string TenThuoc { get; set; } = null!;

    public double DonGia { get; set; }

    public int MaCachDung { get; set; }
    public int MaDonVi { get; set; }

    public int? SoLuong { get; set; }

    [ForeignKey(nameof(MaCachDung))]
    public virtual CachDung? CachDung { get; set; }

    [ForeignKey(nameof(MaDonVi))]
    public virtual DonVi? DonVi { get; set; }

    public virtual ICollection<ChiTietDonThuoc> ChiTietDonThuocs { get; set; } = new List<ChiTietDonThuoc>();
}

[Table("DonVi")]
public class DonVi
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int MaDonVi { get; set; }

    [Required, MaxLength(20)]
    public string TenDonVi { get; set; } = null!;

    public virtual ICollection<Thuoc> Thuocs { get; set; } = new List<Thuoc>();
}

[Table("CachDung")]
public class CachDung
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int MaCachDung { get; set; }

    [Required, MaxLength(50)]
    public string TenCachDung { get; set; } = null!;

    public virtual ICollection<Thuoc> Thuocs { get; set; } = new List<Thuoc>();
}
