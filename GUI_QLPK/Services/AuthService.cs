using QLPKBUS;
using QLPKDTO;
using System.Collections.Generic;

namespace GUI_QLPK
{
    internal class AuthService
    {
        private readonly taiKhoanBUS fallback = new taiKhoanBUS();

        public bool TryLogin(string username, string password, out int maTaiKhoan)
        {
            maTaiKhoan = 0;

            ApiLoginResponse response;
            ApiLoginRequest request = new ApiLoginRequest
            {
                UserName = username,
                PassWord = password
            };

            if (ApiClient.TryPost("api/Auth/login", request, out response) && response != null)
            {
                maTaiKhoan = response.MaTaiKhoan;
                return true;
            }

            List<taiKhoanDTO> listTk = fallback.select();
            foreach (taiKhoanDTO taiKhoan in listTk)
            {
                if (taiKhoan.Username == username && taiKhoan.Password == password)
                {
                    maTaiKhoan = taiKhoan.MaTK;
                    return true;
                }
            }

            return false;
        }
    }
}
