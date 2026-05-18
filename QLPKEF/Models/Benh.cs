using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLPKEF.Models;

[Table("Benh")]
public class Benh
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int MaBenh { get; set; }

    [Required, MaxLength(50)]
    public string TenBenh { get; set; } = null!;

    public virtual ICollection<ChuanDoan> ChuanDoans { get; set; } = new List<ChuanDoan>();
}

[Table("ChuanDoan")]
public class ChuanDoan
{
    public int MaBenh { get; set; }
    public int MaPKB { get; set; }

    [MaxLength(255)]
    public string? TenChuanDoan { get; set; }

    [ForeignKey(nameof(MaBenh))]
    public virtual Benh? Benh { get; set; }

    [ForeignKey(nameof(MaPKB))]
    public virtual PhieuKhamBenh? PhieuKhamBenh { get; set; }
}
