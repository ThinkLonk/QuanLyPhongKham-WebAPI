using QLPKDTO;
using QLPKDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLPKBUS
{
    public class PhieukhambenhBUS
    {
        private PhieukhambenhDAL pkbDAL;
        public PhieukhambenhBUS()
        {
            pkbDAL = new PhieukhambenhDAL();
        }
        public List<phieukhambenhDTO> select()
        {
            return pkbDAL.select();
        }
        public List<phieukhambenhDTO> selectByKeyWord(string sKeyword)
        {
            return pkbDAL.selectByKeyWord(sKeyword);
        }
        public string autogenerate_mapkb()
        {
            return pkbDAL.AutoGenerateMaPKB();
        }
        public bool them(phieukhambenhDTO pkb)
        {
            bool re = pkbDAL.them(pkb);
            return re;
        }
        public bool CapNhatDaGuiMail(string maPKB, bool daGui)
        {
            return pkbDAL.CapNhatDaGuiMail(maPKB, daGui);
        }
        public List<phieukhambenhDTO> selectByDate(DateTime ngay)
        {
            return pkbDAL.selectByDate(ngay);
        }
    }
}
