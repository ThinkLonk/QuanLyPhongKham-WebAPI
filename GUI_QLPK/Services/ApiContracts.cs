using System;

namespace GUI_QLPK
{
    internal class ApiBenhNhan
    {
        public int MaBenhNhan { get; set; }
        public string TenBenhNhan { get; set; }
        public DateTime NgaySinh { get; set; }
        public string DiaChi { get; set; }
        public string CCCD { get; set; }
        public string GioiTinh { get; set; }
        public string Email { get; set; }
    }

    internal class ApiThuoc
    {
        public int MaThuoc { get; set; }
        public string TenThuoc { get; set; }
        public double DonGia { get; set; }
        public int MaCachDung { get; set; }
        public int MaDonVi { get; set; }
        public int? SoLuong { get; set; }
    }

    internal class ApiLichHen
    {
        public int MaLichHen { get; set; }
        public DateTime NgayHen { get; set; }
        public int? MaTaiKhoan { get; set; }
        public int? MaBenhNhan { get; set; }
        public int? MaDieuDuong { get; set; }
        public string TrangThai { get; set; }
    }

    internal class ApiHoaDon
    {
        public int MaHoaDon { get; set; }
        public int MaPKB { get; set; }
        public int MaTaiKhoan { get; set; }
        public DateTime? NgayLapHoaDon { get; set; }
        public double TienThuoc { get; set; }
        public double TienKham { get; set; }
        public double TongTien { get; set; }
    }

    internal class ApiDoanhThuTheoNgay
    {
        public DateTime Ngay { get; set; }
        public int SoHoaDon { get; set; }
        public double TongTien { get; set; }
    }

    internal class ApiLapHoaDonRequest
    {
        public int MaPKB { get; set; }
        public int MaTaiKhoan { get; set; }
    }

    internal class ApiLoginRequest
    {
        public string UserName { get; set; }
        public string PassWord { get; set; }
    }

    internal class ApiLoginResponse
    {
        public int MaTaiKhoan { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string TenRole { get; set; }
    }
}
