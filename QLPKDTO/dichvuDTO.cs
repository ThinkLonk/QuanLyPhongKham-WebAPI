using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLPKDTO
{
    public class dichvuDTO
    {
        private string maDichVu;
        private string tenDichVu;
        private float tienDichVu;

        public string MaDichVu { get => maDichVu; set => maDichVu = value; }
        public string TenDichVu { get => tenDichVu; set => tenDichVu = value; }
        public float TienDichVu { get => tienDichVu; set => tienDichVu = value; }
    }
}
