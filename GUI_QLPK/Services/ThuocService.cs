using QLPKBUS;
using QLPKDTO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GUI_QLPK
{
    internal class ThuocService
    {
        private readonly ThuocBUS fallback = new ThuocBUS();

        public List<thuocDTO> select()
        {
            List<ApiThuoc> data;
            if (ApiClient.TryGet("api/Thuoc", out data))
            {
                return data.Select(ToDto).ToList();
            }

            return fallback.select();
        }

        public List<thuocDTO> selectByKeyWord(string keyword)
        {
            List<thuocDTO> all = select();
            if (all == null || string.IsNullOrWhiteSpace(keyword))
            {
                return all;
            }

            return all
                .Where(t => !string.IsNullOrEmpty(t.TenThuoc) &&
                            t.TenThuoc.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();
        }

        public bool them(thuocDTO thuoc)
        {
            ApiThuoc created;
            if (ApiClient.TryPost("api/Thuoc", ToApi(thuoc), out created))
            {
                return true;
            }

            return fallback.them(thuoc);
        }

        public bool sua(thuocDTO thuoc, int maThuoc)
        {
            return fallback.sua(thuoc, maThuoc);
        }

        public bool xoa(thuocDTO thuoc)
        {
            return fallback.xoa(thuoc);
        }

        public List<thuocDTO> selectbypkb(int maPkb)
        {
            return fallback.selectbypkb(maPkb);
        }

        public List<thuocDTO> baocaobymonth(string month, string year)
        {
            return fallback.baocaobymonth(month, year);
        }

        public bool kiemTraTrungTen(string tenThuoc)
        {
            List<thuocDTO> data = select();
            if (data != null)
            {
                return data.Any(t => string.Equals(t.TenThuoc, tenThuoc, StringComparison.OrdinalIgnoreCase));
            }

            return fallback.kiemTraTrungTen(tenThuoc);
        }

        public bool truSoLuong(int maThuoc, int soLuong)
        {
            return fallback.truSoLuong(maThuoc, soLuong);
        }

        private static thuocDTO ToDto(ApiThuoc item)
        {
            return new thuocDTO
            {
                MaThuoc = item.MaThuoc,
                TenThuoc = item.TenThuoc,
                DonGia = (float)item.DonGia,
                MaCachDung = item.MaCachDung,
                MaDonVi = item.MaDonVi,
                SoLuong = item.SoLuong ?? 0
            };
        }

        private static ApiThuoc ToApi(thuocDTO item)
        {
            return new ApiThuoc
            {
                MaThuoc = item.MaThuoc,
                TenThuoc = item.TenThuoc,
                DonGia = item.DonGia,
                MaCachDung = item.MaCachDung,
                MaDonVi = item.MaDonVi,
                SoLuong = item.SoLuong
            };
        }
    }
}
