using QLPKDTO;
using QLPKDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLPKBUS
{
    public class lichHenBUS
    {
        private lichHenDAL lhDAL;

        public lichHenBUS()
        {
            lhDAL = new lichHenDAL();
        }

        public List<lichHenDTO> select()
        {
            return lhDAL.select();
        }

        public bool them(lichHenDTO lh)
        {
            return lhDAL.them(lh);
        }

        public bool xoa(lichHenDTO lh)
        {
            return lhDAL.xoa(lh);
        }

        public bool CapNhatTrangThai(int maLichHen, string trangThaiMoi)
        {
            return lhDAL.CapNhatTrangThai(maLichHen, trangThaiMoi);
        }

        public List<lichHenDTO> selectByDate(DateTime date)
        {
            return lhDAL.selectByDate(date);
        }

        /// <summary>
        /// Tạo lịch hẹn tái khám mới dựa trên lịch hẹn hiện tại
        /// Copy lại maTaiKhoan, maDieuDuong từ lịch hẹn cũ
        /// TrangThai mặc định = "Chờ khám"
        /// </summary>
        public bool ThemLichHenTaiKham(int maBenhNhan, int maTaiKhoan, int maDieuDuong, DateTime ngayTaiKham)
        {
            lichHenDTO lhMoi = new lichHenDTO();
            lhMoi.MaBenhNhan = maBenhNhan;
            lhMoi.MaTaiKhoan = maTaiKhoan;
            lhMoi.MaDieuDuong = maDieuDuong;
            lhMoi.NgayHen = ngayTaiKham;
            lhMoi.TrangThai = "Chờ khám";
            return lhDAL.them(lhMoi);
        }
    }
}