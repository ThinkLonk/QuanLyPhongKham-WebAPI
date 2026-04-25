using QLPKDTO;
using QLPKDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLPKBUS
{
    public class HoadonBUS
    {
        private HoadonDAL hdDAL;
        public HoadonBUS()
        {
            hdDAL = new HoadonDAL();
        }
        public bool them(hoadonDTO hd)
        {
            bool re = hdDAL.them(hd);
            return re;
        }
        public List<hoadonDTO> select()
        {
            return hdDAL.select();
        }
        public decimal doanhthu(string ngHD)
        {
            return hdDAL.doanhthu(ngHD);
        }
        public int sobenhnhan(string ngHD)
        {
            return hdDAL.sobenhnhan(ngHD);
        }
        public List<hoadonDTO> selectByMonth(string month, string year)
        {
            return hdDAL.selectByMonth(month, year);
        }
        public string autogenerate_mahd()
        {
            return hdDAL.autogenerate_mahd();
        }
        public decimal tienthuoc(hoadonDTO hd, string mapkb)
        {
            decimal re = hdDAL.tienthuoc(hd, mapkb);
            return re;
        }
        public float tienkham()
        {
            float re = hdDAL.tienkham();
            return re;
        }
        public float doanhthuMonth(string month, string year)
        {
            return hdDAL.doanhthuMonth(month, year);
        }
    }
}
