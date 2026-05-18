using QLPKBUS;
using QLPKDTO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GUI_QLPK
{
    internal class LichHenService
    {
        private readonly lichHenBUS fallback = new lichHenBUS();

        public List<lichHenDTO> select()
        {
            List<ApiLichHen> data;
            if (ApiClient.TryGet("api/LichHen", out data))
            {
                return data.Select(ToDto).ToList();
            }

            return fallback.select();
        }

        public bool them(lichHenDTO lichHen)
        {
            ApiLichHen created;
            if (ApiClient.TryPost("api/LichHen", ToApi(lichHen), out created))
            {
                return true;
            }

            return fallback.them(lichHen);
        }

        public bool xoa(lichHenDTO lichHen)
        {
            return fallback.xoa(lichHen);
        }

        public bool CapNhatTrangThai(int maLichHen, string trangThaiMoi)
        {
            if (ApiClient.TryPut("api/LichHen/" + maLichHen + "/trangthai", trangThaiMoi))
            {
                return true;
            }

            return fallback.CapNhatTrangThai(maLichHen, trangThaiMoi);
        }

        public List<lichHenDTO> selectByDate(DateTime date)
        {
            List<lichHenDTO> data = select();
            if (data != null)
            {
                return data
                    .Where(l => l.NgayHen.Date == date.Date &&
                                l.TrangThai != "Đã khám" &&
                                l.TrangThai != "Chờ kê thuốc")
                    .ToList();
            }

            return fallback.selectByDate(date);
        }

        public bool ThemLichHenTaiKham(int maBenhNhan, int maTaiKhoan, int maDieuDuong, DateTime ngayTaiKham)
        {
            lichHenDTO lichHen = new lichHenDTO
            {
                MaBenhNhan = maBenhNhan,
                MaTaiKhoan = maTaiKhoan,
                MaDieuDuong = maDieuDuong,
                NgayHen = ngayTaiKham,
                TrangThai = "Chờ khám"
            };

            return them(lichHen);
        }

        private static lichHenDTO ToDto(ApiLichHen item)
        {
            return new lichHenDTO
            {
                MaLichHen = item.MaLichHen,
                MaBenhNhan = item.MaBenhNhan ?? 0,
                MaTaiKhoan = item.MaTaiKhoan ?? 0,
                MaDieuDuong = item.MaDieuDuong ?? 0,
                NgayHen = item.NgayHen,
                TrangThai = item.TrangThai
            };
        }

        private static ApiLichHen ToApi(lichHenDTO item)
        {
            return new ApiLichHen
            {
                MaLichHen = item.MaLichHen,
                MaBenhNhan = item.MaBenhNhan,
                MaTaiKhoan = item.MaTaiKhoan,
                MaDieuDuong = item.MaDieuDuong,
                NgayHen = item.NgayHen,
                TrangThai = item.TrangThai
            };
        }
    }
}
