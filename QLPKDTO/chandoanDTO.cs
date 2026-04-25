using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLPKDTO
{
    public class chandoanDTO
    {
        private string maPkb;
        private string maBenh;
        private string tenChuanDoan;
        private string trieuChung;
        public string MaPkb { get => maPkb; set => maPkb = value; }
        public string MaBenh { get => maBenh; set => maBenh = value; }
        public string TenChuanDoan { get => tenChuanDoan; set => tenChuanDoan = value; }
        public string TrieuChung { get => trieuChung; set => trieuChung = value; }
    }
}
