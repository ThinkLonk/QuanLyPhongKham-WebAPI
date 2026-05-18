using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLPKEF.Models;

[Table("ToaThuoc")]
public class ToaThuoc
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int MaToaThuoc { get; set; }

    public DateTime? NgayKeToa { get; set; }
    public int? MaPKB { get; set; }

    [ForeignKey(nameof(MaPKB))]
    public virtual PhieuKhamBenh? PhieuKhamBenh { get; set; }

    public virtual ICollection<ChiTietDonThuoc> ChiTiets { get; set; } = new List<ChiTietDonThuoc>();
}

[Table("ChiTietDonThuoc")]
public class ChiTietDonThuoc
{
    public int MaThuoc { get; set; }
    public int MaToaThuoc { get; set; }
    public int SoLuong { get; set; }

    [ForeignKey(nameof(MaThuoc))]
    public virtual Thuoc? Thuoc { get; set; }

    [ForeignKey(nameof(MaToaThuoc))]
    public virtual ToaThuoc? ToaThuoc { get; set; }
}
