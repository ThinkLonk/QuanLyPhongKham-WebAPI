using QLPKDAL;
using QLPKDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLPKBUS
{
    public class ThuocBUS
    {
        private ThuocDAL thDAL;
        public ThuocBUS()
        {
            thDAL = new ThuocDAL();
        }
        public bool them(thuocDTO th)
        {
            bool re = thDAL.them(th);
            return re;
        }
        public bool sua(thuocDTO th, string maThuocold)
        {
            bool re = thDAL.sua(th, maThuocold);
            return re;
        }
        public List<thuocDTO> select()
        {
            return thDAL.select();
        }
        public bool xoa(thuocDTO th)
        {
            bool re = thDAL.xoa(th);
            return re;
        }
        public List<thuocDTO> selectByKeyWord(string sKeyword)
        {
            return thDAL.selectByKeyWord(sKeyword);
        }
        public List<thuocDTO> selectbypkb(string mapkb)
        {
            return thDAL.selectbypkb(mapkb);
        }
        public  string autogenerate_mathuoc()
        {
            return thDAL.autogenerate_mathuoc();
        }
        public List<thuocDTO> baocaobymonth(string month, string year)
        {
            return thDAL.baocaobymonth(month, year);
        }
        public bool kiemTraTrungTen(string tenThuoc)
        {
            return thDAL.KiemTraTenThuocDaTonTai(tenThuoc);
        }
        public bool truSoLuong(string maThuoc, int soLuong)
        {
            return thDAL.truSoLuong(maThuoc, soLuong);
        }
    }
}
