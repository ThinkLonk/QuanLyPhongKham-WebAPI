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
    public class cachDungDAL
    {
        private string connectionString;
        public cachDungDAL() 
        {
            // Đọc chuỗi kết nối từ cấu hình ứng dụng
            connectionString = ConfigurationManager.AppSettings["ConnectionString"];
        }

        // Thuộc tính cho chuỗi kết nối
        public string ConnectionString { get => connectionString; set => connectionString = value; }
        // Phương thức lấy danh sách các cách dùng
        public List<cachdungDTO> getcachdung()
        {
            // Chuỗi truy vấn SQL để lấy danh sách các cách dùng
            string query = string.Empty;
            query += "SELECT * FROM CachDung";

            List<cachdungDTO> lscd = new List<cachdungDTO>(); // Danh sách để chứa kết quả

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con; // Kết nối lệnh với cơ sở dữ liệu
                    cmd.CommandType = System.Data.CommandType.Text; // Kiểu lệnh là văn bản
                    cmd.CommandText = query; // Gán chuỗi truy vấn cho lệnh

                    try
                    {
                        con.Open(); // Mở kết nối
                        SqlDataReader reader = cmd.ExecuteReader(); // Thực thi lệnh và nhận kết quả
                        if (reader.HasRows == true) // Kiểm tra nếu có kết quả trả về
                        {
                            while (reader.Read()) // Đọc từng dòng kết quả
                            {
                                cachdungDTO cd = new cachdungDTO(); 
                                cd.TenCachDung = reader["tenCachDung"].ToString(); 
                                cd.MaCachDung = (reader["maCachDung"].ToString()); 
                                lscd.Add(cd); // Thêm vào danh sách
                            }
                        }

                        con.Close(); // Đóng kết nối
                        con.Dispose(); // Giải phóng tài nguyên kết nối
                    }
                    catch (Exception ex)
                    {
                        con.Close(); // Đóng kết nối khi có lỗi
                        return null; // Trả về null nếu có lỗi
                    }
                }
            }
            return lscd;
        }
    }
}
