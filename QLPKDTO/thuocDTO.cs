using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLPKDTO
{
    public class thuocDTO
    {
        private string maThuoc;
        private string tenThuoc;
        private float donGia;
        private string maCachDung;
        private string maDonVi;
        private int soLuong;

        public string MaThuoc { get => maThuoc; set => maThuoc = value; }
        public string TenThuoc { get => tenThuoc; set => tenThuoc = value; }
        public float DonGia { get => donGia; set => donGia = value; }
        public string MaCachDung { get => maCachDung; set => maCachDung = value; }
        public string MaDonVi { get => maDonVi; set => maDonVi = value; }
        public int SoLuong { get => soLuong; set => soLuong = value; }
    }
}
