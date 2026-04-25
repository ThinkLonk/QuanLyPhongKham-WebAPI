using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLPKDAL;
using QLPKDTO;

namespace QLPKBUS
{
    public class BenhBUS
    {
        private BenhDAL beDAL;
        public BenhBUS()
        {
            beDAL = new BenhDAL(); // Khởi tạo DAL
        }
        public bool them(benhDTO be)
        {
            bool re = beDAL.them(be);
            return re;
        }
        public bool sua(benhDTO be, string maBenhold)
        {
            bool re = beDAL.sua(be, maBenhold);
            return re;
        }
        public bool xoa(benhDTO be)
        {
            bool re = beDAL.xoa(be);
            return true;
        }
        // Phương thức lấy danh sách các bệnh
        public List<benhDTO> select()
        {
            return beDAL.select(); // Gọi phương thức select từ DAL để lấy danh sách bệnh
        }
        public List<benhDTO> selectByKeyWord(string sKeyword)
        {
            return beDAL.selectByKeyWord(sKeyword);
        }
        public string autogenerate_mabenh()
        {
            return beDAL.autogenerate_mabenh();
        }
        public bool kiemTraTrungTen(string tenBenh)
        {
            return beDAL.kiemTraTrungTen(tenBenh);
        }
    }
}
