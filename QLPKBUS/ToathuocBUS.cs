using QLPKDTO;
using QLPKDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLPKBUS
{
    public class ToathuocBUS
    {
        private ToathuocDAL ttDAL;
        public ToathuocBUS()
        {
            ttDAL = new ToathuocDAL();
        }
        public bool them(toathuocDTO tt)
        {
            bool re = ttDAL.them(tt);
            return re;
        }
        public string autogenerate_matoa()
        {
            return ttDAL.autogenerate_matoa();
        }
        public List<toathuocDTO> select()
        {
            return ttDAL.select();
        }
    }
}
