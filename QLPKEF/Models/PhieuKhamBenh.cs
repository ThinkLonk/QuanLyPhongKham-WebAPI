using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLPKEF.Models;

[Table("PhieuKhamBenh")]
public class PhieuKhamBenh
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int MaPKB { get; set; }

    public DateTime? NgayKham { get; set; }

    [MaxLength(255)]
    public string? TrieuChung { get; set; }

    public int? MaBenhNhan { get; set; }
    public int? MaTaiKhoan { get; set; }

    public DateTime? NgayTaiKham { get; set; }
    public bool DaGuiMail { get; set; }

    [ForeignKey(nameof(MaBenhNhan))]
    public virtual BenhNhan? BenhNhan { get; set; }

    [ForeignKey(nameof(MaTaiKhoan))]
    public virtual TaiKhoan? TaiKhoan { get; set; }

    public virtual ICollection<ToaThuoc> ToaThuocs { get; set; } = new List<ToaThuoc>();
    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
    public virtual ICollection<ChuanDoan> ChuanDoans { get; set; } = new List<ChuanDoan>();
}
