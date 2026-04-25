using QLPKDTO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLPKDAL
{
    public class donViDAL
    {
        private string connectionString;
        public donViDAL()
        {
            connectionString = ConfigurationManager.AppSettings["ConnectionString"]; 
        }
        public string ConnectionString { get => connectionString; set => connectionString = value; }
        // Phương thức lấy danh sách các đơn vị từ cơ sở dữ liệu
        public List<donViDTO> getdonvi()
        {
            // Chuỗi truy vấn SQL để lấy dữ liệu từ bảng DonVi
            string query = "SELECT * FROM DonVi";

            // Khai báo danh sách để lưu trữ các đơn vị
            List<donViDTO> lsdv = new List<donViDTO>();

            // Sử dụng kết nối và lệnh SQL để truy vấn cơ sở dữ liệu
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con; // Thiết lập kết nối cho lệnh SQL
                    cmd.CommandType = System.Data.CommandType.Text; // Thiết lập loại lệnh là văn bản
                    cmd.CommandText = query; // Thiết lập nội dung lệnh SQL

                    try
                    {
                        con.Open(); // Mở kết nối đến cơ sở dữ liệu
                        SqlDataReader reader = cmd.ExecuteReader(); // Thực thi lệnh và lấy dữ liệu trả về
                        if (reader.HasRows) // Kiểm tra xem kết quả có hàng dữ liệu hay không
                        {
                            while (reader.Read()) // Duyệt qua từng hàng dữ liệu
                            {
                             
                                donViDTO dv = new donViDTO();
                                dv.MaDonVi =(reader["maDonVi"].ToString()); // Đọc mã đơn vị
                                dv.TenDonVi = reader["tenDonVi"].ToString(); // Đọc tên đơn vị
                                lsdv.Add(dv); // Thêm đơn vị vào danh sách
                            }
                        }

                        con.Close(); 
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Lỗi: " + ex.Message);
                        con.Close(); // Đóng kết nối trong trường hợp xảy ra lỗi
                        return null; // Trả về null để biểu thị rằng có lỗi xảy ra
                    }
                }
            }
            return lsdv;
        }

    }
}
