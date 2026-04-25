using QLPKDTO;
using QLPKDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLPKBUS
{
    public class DichvuBUS
    {
        private DichvuDAL dvDAL;
        public DichvuBUS()
        {
            dvDAL = new DichvuDAL();
        }
        public bool them(dichvuDTO qd)
        {
            bool re = dvDAL.them(qd);
            return re;
        }
        public bool sua(dichvuDTO dv, string maDichVuOld)
        {
            bool re = dvDAL.sua(dv, maDichVuOld);
            return re;
        }
        public bool xoa(dichvuDTO dv)
        {
            bool re = dvDAL.xoa(dv);
            return re;
        }
        public List<dichvuDTO> select()
        {
            return dvDAL.select();
        }
        public List<dichvuDTO> selectByKeyWord(string sKeyword)
        {
            return dvDAL.selectByKeyWord(sKeyword);
        }
        public string autogenerate_madv()
        {
            return dvDAL.autogenerate_madv();
        }
    }
}
