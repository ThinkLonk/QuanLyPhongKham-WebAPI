using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLPKDTO
{
    public class hoadonDTO
    {
        private string maHoaDon;
        private DateTime ngayLapHoaDon;
        private decimal tienThuoc;
        private float tienKham;
        private float tongTien;
        private string maPKB;
        private string maNVTN; // Mã nhân viên thu ngân
        private DateTime ngayTaiKham;
        public string MaPKB { get => maPKB; set => maPKB = value; }
        public string MaHoaDon { get => maHoaDon; set => maHoaDon = value; }
        public DateTime NgayLapHoaDon { get => ngayLapHoaDon; set => ngayLapHoaDon = value; }
        public decimal TienThuoc { get => tienThuoc; set => tienThuoc = value; }
        public float TienKham { get => tienKham; set => tienKham = value; }
        public float TongTien { get => tongTien; set => tongTien = value; }
        public string MaNVTN { get => maNVTN; set => maNVTN = value; }
        public DateTime NgayTaiKham { get => ngayTaiKham; set => ngayTaiKham = value; }
    }
}
