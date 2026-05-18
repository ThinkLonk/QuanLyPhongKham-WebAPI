using QLPKBUS;
using QLPKDTO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GUI_QLPK
{
    internal class BenhNhanService
    {
        private readonly BenhNhanBUS fallback = new BenhNhanBUS();

        public List<BenhNhanDTO> select()
        {
            List<ApiBenhNhan> data;
            if (ApiClient.TryGet("api/BenhNhan", out data))
            {
                return data.Select(ToDto).ToList();
            }

            return fallback.select();
        }

        public List<BenhNhanDTO> selectByKeyWord(string keyword)
        {
            List<ApiBenhNhan> data;
            string route = "api/BenhNhan/search?keyword=" + Uri.EscapeDataString(keyword ?? string.Empty);
            if (ApiClient.TryGet(route, out data))
            {
                return data.Select(ToDto).ToList();
            }

            return fallback.selectByKeyWord(keyword);
        }

        public bool them(BenhNhanDTO benhNhan)
        {
            ApiBenhNhan created;
            if (ApiClient.TryPost("api/BenhNhan", ToApi(benhNhan), out created))
            {
                return true;
            }

            return fallback.them(benhNhan);
        }

        public bool sua(BenhNhanDTO benhNhan, int maBenhNhan)
        {
            ApiBenhNhan request = ToApi(benhNhan);
            request.MaBenhNhan = maBenhNhan;
            if (ApiClient.TryPut("api/BenhNhan/" + maBenhNhan, request))
            {
                return true;
            }

            return fallback.sua(benhNhan, maBenhNhan);
        }

        public bool xoa(BenhNhanDTO benhNhan)
        {
            if (ApiClient.TryDelete("api/BenhNhan/" + benhNhan.MaBN))
            {
                return true;
            }

            return fallback.xoa(benhNhan);
        }

        private static BenhNhanDTO ToDto(ApiBenhNhan item)
        {
            return new BenhNhanDTO
            {
                MaBN = item.MaBenhNhan,
                TenBN = item.TenBenhNhan,
                NgsinhBN = item.NgaySinh,
                DiachiBN = item.DiaChi,
                CanCuocCongDan = item.CCCD,
                GtBN = item.GioiTinh,
                Email = item.Email
            };
        }

        private static ApiBenhNhan ToApi(BenhNhanDTO item)
        {
            return new ApiBenhNhan
            {
                MaBenhNhan = item.MaBN,
                TenBenhNhan = item.TenBN,
                NgaySinh = item.NgsinhBN,
                DiaChi = item.DiachiBN,
                CCCD = item.CanCuocCongDan,
                GioiTinh = item.GtBN,
                Email = item.Email
            };
        }
    }
}
