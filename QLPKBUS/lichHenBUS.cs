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
        public lichHenBUS() {
            lhDAL = new lichHenDAL();
        }
        public List<lichHenDTO> select()
        {
            return lhDAL.select();
        }
        public string autogenerate_malichhen()
        {
            return lhDAL.AutoGenerateMaLichHen();
        }
        public bool them(lichHenDTO lh)
        {
            bool re = lhDAL.them(lh);
            return re;
        }
        public bool xoa(lichHenDTO lh)
        {
            bool re = lhDAL.xoa(lh);
            return re;
        }

        public bool CapNhatTrangThai(string maBenhNhan, string trangThaiMoi)
        {
            return lhDAL.CapNhatTrangThai(maBenhNhan, trangThaiMoi);
        }
        public List<lichHenDTO> selectByDate(DateTime date)
        {
            return lhDAL.selectByDate(date);
        }
    }
}
