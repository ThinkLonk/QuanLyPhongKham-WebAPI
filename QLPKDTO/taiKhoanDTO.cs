using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLPKDTO
{
    public class taiKhoanDTO
    {
        private string name;
        private string username;
        private string password;
        private string maLoai;
        private string maTK;

        public string Name { get => name; set => name = value; }
        public string Password { get => password; set => password = value; }
        public string Username { get => username; set => username = value; }
        public string MaLoai { get => maLoai; set => maLoai = value; }
        public string MaTK { get => maTK; set => maTK = value; }

    }
}
