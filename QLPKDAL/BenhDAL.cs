using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using QLPKDTO;
using System.Configuration;

namespace QLPKDTO
{
    public class BenhDAL
    {
        private string connectionString;
        public BenhDAL()
        {
            // Đọc chuỗi kết nối từ cấu hình ứng dụng
            connectionString = ConfigurationManager.AppSettings["ConnectionString"];
        }
        // Thuộc tính cho chuỗi kết nối
        public string ConnectionString { get => connectionString; set => connectionString = value; }

        // Phương thức thêm mới một bệnh
        public bool them(benhDTO be)
        {
            // Chuỗi truy vấn SQL để thêm bệnh
            string query = string.Empty;
            query += "INSERT INTO [Benh] ([tenBenh])";
            query += "VALUES (@tenBenh)";
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con; // Kết nối lệnh với cơ sở dữ liệu
                    cmd.CommandType = System.Data.CommandType.Text; // Kiểu lệnh là văn bản
                    cmd.CommandText = query; // Gán chuỗi truy vấn cho lệnh

                    // Thêm tham số cho lệnh SQL
                    cmd.Parameters.AddWithValue("@tenBenh", be.TenBenh);
                    try
                    {
                        con.Open(); // Mở kết nối
                        cmd.ExecuteNonQuery(); // Thực thi lệnh không trả về kết quả
                        con.Close(); // Đóng kết nối
                        con.Dispose(); // Giải phóng tài nguyên
                    }
                    catch (Exception ex)
                    {
                        con.Close(); // Đóng kết nối khi có lỗi
                        return false; // Trả về false nếu có lỗi
                    }
                }
            }
            return true; // Trả về true nếu thành công
        }

        // Phương thức sửa một bệnh
        public bool sua(benhDTO be, string maBenhold)
        {
            // Chuỗi truy vấn SQL để cập nhật bệnh
            string query = string.Empty;
            query += "update [Benh]";
            query += "set tenBenh=@tenBenh where maBenh=@maBenhold";

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con; // Kết nối lệnh với cơ sở dữ liệu
                    cmd.CommandType = System.Data.CommandType.Text; // Kiểu lệnh là văn bản
                    cmd.CommandText = query; // Gán chuỗi truy vấn cho lệnh

                    // Thêm tham số cho lệnh SQL
                    cmd.Parameters.AddWithValue("@tenBenh", be.TenBenh);
                    cmd.Parameters.AddWithValue("@maBenhold", maBenhold);
                    try
                    {
                        con.Open(); // Mở kết nối
                        cmd.ExecuteNonQuery(); // Thực thi lệnh không trả về kết quả
                        con.Close(); // Đóng kết nối
                        con.Dispose(); // Giải phóng tài nguyên
                    }
                    catch (Exception ex)
                    {
                        con.Close(); // Đóng kết nối khi có lỗi
                        return false; // Trả về false nếu có lỗi
                    }
                }
                return true; // Trả về true nếu thành công
            }
        }

        // Phương thức xóa một bệnh
        public bool xoa(benhDTO be)
        {
            // Chuỗi truy vấn SQL để xóa bệnh
            string query = string.Empty;
            query += "delete from [Benh]";
            query += "where maBenh=@maB";

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con; // Kết nối lệnh với cơ sở dữ liệu
                    cmd.CommandType = System.Data.CommandType.Text; // Kiểu lệnh là văn bản
                    cmd.CommandText = query; // Gán chuỗi truy vấn cho lệnh

                    // Thêm tham số cho lệnh SQL
                    cmd.Parameters.AddWithValue("@maB", be.MaBenh);
                    try
                    {
                        con.Open(); // Mở kết nối
                        cmd.ExecuteNonQuery(); // Thực thi lệnh không trả về kết quả
                        con.Close(); // Đóng kết nối
                        con.Dispose(); // Giải phóng tài nguyên
                    }
                    catch (Exception ex)
                    {
                        con.Close(); // Đóng kết nối khi có lỗi
                        return false; // Trả về false nếu có lỗi
                    }
                }
                return true; // Trả về true nếu thành công
            }
        }
        // Phương thức chọn tất cả các bệnh
        public List<benhDTO> select()
        {
            // Chuỗi truy vấn SQL để chọn tất cả các bệnh
            string query = string.Empty;
            query += "SELECT * ";
            query += "FROM [Benh]";

            List<benhDTO> lsBenh = new List<benhDTO>(); // Danh sách để chứa kết quả

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
                        SqlDataReader reader = null;
                        reader = cmd.ExecuteReader(); // Thực thi lệnh và nhận kết quả
                        if (reader.HasRows == true)
                        {
                            while (reader.Read()) // Đọc từng dòng kết quả
                            {
                                benhDTO be = new benhDTO(); // Tạo đối tượng BenhDTO
                                be.MaBenh = (reader["maBenh"].ToString()); // Gán giá trị cho MaBenh
                                be.TenBenh = reader["tenBenh"].ToString(); // Gán giá trị cho TenBenh
                                lsBenh.Add(be); // Thêm vào danh sách
                            }
                        }
                        con.Close(); // Đóng kết nối
                        con.Dispose(); // Giải phóng tài nguyên
                    }
                    catch (Exception ex)
                    {
                        con.Close(); // Đóng kết nối khi có lỗi
                        return null; // Trả về null nếu có lỗi
                    }
                }
            }
            return lsBenh; // Trả về danh sách các bệnh
        }
        // Phương thức chọn bệnh theo từ khóa
        public List<benhDTO> selectByKeyWord(string sKeyword)
        {
            // Chuỗi truy vấn SQL để chọn bệnh theo từ khóa
            string query = string.Empty;
            query += " SELECT * ";
            query += " FROM [Benh]";
            query += " WHERE ([maBenh] LIKE CONCAT('%',@sKeyword,'%'))";
            query += " OR ([tenBenh] LIKE CONCAT('%',@sKeyword,'%'))";

            List<benhDTO> lsBenh = new List<benhDTO>(); // Danh sách để chứa kết quả

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con; // Kết nối lệnh với cơ sở dữ liệu
                    cmd.CommandType = System.Data.CommandType.Text; // Kiểu lệnh là văn bản
                    cmd.CommandText = query; // Gán chuỗi truy vấn cho lệnh

                    // Thêm tham số cho lệnh SQL
                    cmd.Parameters.AddWithValue("@sKeyword", sKeyword);
                    try
                    {
                        con.Open(); // Mở kết nối
                        SqlDataReader reader = null;
                        reader = cmd.ExecuteReader(); // Thực thi lệnh và nhận kết quả
                        if (reader.HasRows == true)
                        {
                            while (reader.Read()) // Đọc từng dòng kết quả
                            {
                                benhDTO be = new benhDTO(); // Tạo đối tượng BenhDTO
                                be.MaBenh = (reader["maBenh"].ToString()); // Gán giá trị cho MaBenh
                                be.TenBenh = reader["tenBenh"].ToString(); // Gán giá trị cho TenBenh
                                lsBenh.Add(be); // Thêm vào danh sách
                            }
                        }
                        con.Close(); // Đóng kết nối
                        con.Dispose(); // Giải phóng tài nguyên
                    }
                    catch (Exception ex)
                    {
                        con.Close(); // Đóng kết nối khi có lỗi
                        return null; // Trả về null nếu có lỗi
                    }
                }
            }
            return lsBenh; // Trả về danh sách các bệnh theo từ khóa
        }
        // Phương thức tự động tạo mã bệnh mới
        public string autogenerate_mabenh()
        {
            int mabenh = 1; // Biến để chứa mã bệnh mới, bắt đầu từ 1
            // Chuỗi truy vấn SQL để lấy mã bệnh lớn nhất
            string query = string.Empty;
            query += "SELECT MAX (KQ.MABENH) AS MM from (SELECT CONVERT(float, Benh.maBenh) AS MABENH FROM Benh ) AS KQ";

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
                        SqlDataReader reader = null;
                        reader = cmd.ExecuteReader(); // Thực thi lệnh và nhận kết quả
                        if (reader.HasRows == true)
                        {
                            while (reader.Read()) // Đọc từng dòng kết quả
                            {
                                mabenh = int.Parse(reader["MM"].ToString()) + 1; // Tạo mã bệnh mới bằng cách tăng mã bệnh lớn nhất lên 1
                            }
                        }
                        con.Close(); // Đóng kết nối
                        con.Dispose(); // Giải phóng tài nguyên
                    }
                    catch (Exception ex)
                    {
                        con.Close(); // Đóng kết nối khi có lỗi
                    }
                }
            }
            return mabenh.ToString();
        }
        public bool kiemTraTrungTen(string tenBenh)
        {
            string query = "SELECT COUNT(*) FROM [Benh] WHERE tenBenh = @tenBenh";
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@tenBenh", tenBenh);
                    try
                    {
                        con.Open();
                        int count = (int)cmd.ExecuteScalar();
                        return count > 0; // true = có trùng
                    }
                    catch
                    {
                        return false; // lỗi thì cho false
                    }
                }
            }
        }

    }
}
