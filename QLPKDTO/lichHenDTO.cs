using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLPKDTO
{
    public class lichHenDTO
    {
        private string maLichHen;
        private string maBenhNhan;
        private string maTaiKhoan; //cua benh nhan
        private DateTime ngayHen;
        private string trangThai;
        private string maDieuDuong;
        public string MaLichHen { get => maLichHen; set => maLichHen = value; }
        public string MaBenhNhan { get => maBenhNhan; set => maBenhNhan = value; }
        public string MaTaiKhoan { get => maTaiKhoan; set => maTaiKhoan = value; }
        public DateTime NgayHen { get => ngayHen; set => ngayHen = value; }
        public string TrangThai { get => trangThai; set => trangThai = value; }
        public string MaDieuDuong { get => maDieuDuong; set => maDieuDuong = value; }
    }
}
