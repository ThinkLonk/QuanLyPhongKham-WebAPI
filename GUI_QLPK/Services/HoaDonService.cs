using QLPKBUS;
using QLPKDTO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace GUI_QLPK
{
    internal class HoaDonService
    {
        private readonly HoadonBUS fallback = new HoadonBUS();

        public bool them(hoadonDTO hoaDon)
        {
            ApiLapHoaDonRequest request = new ApiLapHoaDonRequest
            {
                MaPKB = hoaDon.MaPKB,
                MaTaiKhoan = hoaDon.MaNVTN
            };

            object result;
            if (ApiClient.TryPost("api/HoaDon/lap", request, out result))
            {
                return true;
            }

            return fallback.them(hoaDon);
        }

        public List<hoadonDTO> select()
        {
            List<ApiHoaDon> data;
            if (ApiClient.TryGet("api/HoaDon", out data))
            {
                return data.Select(ToDto).ToList();
            }

            return fallback.select();
        }

        public decimal doanhthu(string ngayLapHoaDon)
        {
            DateTime ngay;
            if (DateTime.TryParse(ngayLapHoaDon, out ngay))
            {
                ApiDoanhThuTheoNgay item = GetDoanhThuNgay(ngay);
                if (item != null)
                {
                    return Convert.ToDecimal(item.TongTien);
                }
            }

            return fallback.doanhthu(ngayLapHoaDon);
        }

        public int sobenhnhan(string ngayLapHoaDon)
        {
            DateTime ngay;
            if (DateTime.TryParse(ngayLapHoaDon, out ngay))
            {
                ApiDoanhThuTheoNgay item = GetDoanhThuNgay(ngay);
                if (item != null)
                {
                    return item.SoHoaDon;
                }
            }

            return fallback.sobenhnhan(ngayLapHoaDon);
        }

        public List<hoadonDTO> selectByMonth(string month, string year)
        {
            List<ApiDoanhThuTheoNgay> data = GetDoanhThuThang(month, year);
            if (data != null)
            {
                return data.Select(d => new hoadonDTO
                {
                    NgayLapHoaDon = d.Ngay,
                    TongTien = (float)d.TongTien
                }).ToList();
            }

            return fallback.selectByMonth(month, year);
        }

        public decimal tienthuoc(hoadonDTO hoaDon, int maPkb)
        {
            return fallback.tienthuoc(hoaDon, maPkb);
        }

        public float tienkham()
        {
            return fallback.tienkham();
        }

        public float doanhthuMonth(string month, string year)
        {
            List<ApiDoanhThuTheoNgay> data = GetDoanhThuThang(month, year);
            if (data != null)
            {
                return (float)data.Sum(x => x.TongTien);
            }

            return fallback.doanhthuMonth(month, year);
        }

        private static hoadonDTO ToDto(ApiHoaDon item)
        {
            return new hoadonDTO
            {
                MaHoaDon = item.MaHoaDon,
                MaPKB = item.MaPKB,
                MaNVTN = item.MaTaiKhoan,
                NgayLapHoaDon = item.NgayLapHoaDon ?? DateTime.MinValue,
                TienThuoc = Convert.ToDecimal(item.TienThuoc),
                TienKham = (float)item.TienKham,
                TongTien = (float)item.TongTien
            };
        }

        private static ApiDoanhThuTheoNgay GetDoanhThuNgay(DateTime ngay)
        {
            List<ApiDoanhThuTheoNgay> data = GetDoanhThuThang(ngay.Month.ToString(CultureInfo.InvariantCulture), ngay.Year.ToString(CultureInfo.InvariantCulture));
            if (data == null) return null;

            return data.FirstOrDefault(x => x.Ngay.Date == ngay.Date);
        }

        private static List<ApiDoanhThuTheoNgay> GetDoanhThuThang(string month, string year)
        {
            int thang;
            int nam;
            if (!int.TryParse(month, out thang) || !int.TryParse(year, out nam))
            {
                return null;
            }

            List<ApiDoanhThuTheoNgay> data;
            string route = "api/HoaDon/doanhthu?thang=" + thang + "&nam=" + nam;
            return ApiClient.TryGet(route, out data) ? data : null;
        }
    }
}
