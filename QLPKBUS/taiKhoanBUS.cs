using QLPKDAL;
using QLPKDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLPKBUS
{
    public class taiKhoanBUS
    {

        private taiKhoanDAL tkDAL;
        public taiKhoanBUS()
        {
            tkDAL = new taiKhoanDAL();
        }
        public bool them(taiKhoanDTO tk)
        {

            if (!tkDAL.KiemTraTonTai(tk))
            {
                bool re = tkDAL.them(tk);
                return re;
            }
            else return false;
          
        }
        public bool sua(taiKhoanDTO tk, string maTaiKhoanold)
        {
            bool re = tkDAL.sua(tk, maTaiKhoanold);
            return re;
        }
        public bool xoa(taiKhoanDTO tk)
        {
            bool re = tkDAL.xoa(tk);
            return re;
        }
        public List<taiKhoanDTO> select()
        {
            return tkDAL.select();
        }
        public int autogenerate_maTaiKhoan()
        {
            return tkDAL.autogenerate_maTaiKhoan();
        }

        public List<taiKhoanDTO> selectByKeyWord(string sKeyword)
        {
            return tkDAL.selectByKeyWord(sKeyword);
        }
    }
}
