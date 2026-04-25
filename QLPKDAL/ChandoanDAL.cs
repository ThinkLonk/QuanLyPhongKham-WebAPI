using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLPKDTO;

namespace QLPKDAL
{
    public class ChandoanDAL
    {
        private string connectionString;
        public ChandoanDAL()
        {
            // Đọc chuỗi kết nối từ cấu hình ứng dụng
            connectionString = ConfigurationManager.AppSettings["ConnectionString"];
        }
        // Thuộc tính cho chuỗi kết nối
        public string ConnectionString { get => connectionString; set => connectionString = value; }
        // Phương thức lấy danh sách các chẩn đoán
        public List<chandoanDTO> select()
        {
            // Chuỗi truy vấn SQL để lấy danh sách chẩn đoán
            string query = string.Empty;
            query += "SELECT * ";
            query += "FROM [ChuanDoan]";

            List<chandoanDTO> lscd = new List<chandoanDTO>(); // Danh sách để chứa kết quả

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
                                chandoanDTO cd = new chandoanDTO(); // Tạo đối tượng ChandoanDTO
                                cd.MaPkb = (reader["maPKB"].ToString()); // Gán giá trị cho MaPkb
                                cd.MaBenh = (reader["maBenh"].ToString());
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
        // Phương thức thêm mới một chẩn đoán
        public bool them(chandoanDTO cd)
        {
            // Chuỗi truy vấn SQL để thêm mới chẩn đoán
            string query = string.Empty;
            query += "INSERT INTO [ChuanDoan] ([maBenh], [maPKB],[trieuChung],[tenChuanDoan]) ";
            query += "VALUES (@maBenh,@maPKB,@trieuchung,@ten)";

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con; // Kết nối lệnh với cơ sở dữ liệu
                    cmd.CommandType = System.Data.CommandType.Text; // Kiểu lệnh là văn bản
                    cmd.CommandText = query; // Gán chuỗi truy vấn cho lệnh
                    cmd.Parameters.AddWithValue("@maBenh", cd.MaBenh); // Gán giá trị cho tham số @maBenh
                    cmd.Parameters.AddWithValue("@maPKB", cd.MaPkb); // Gán giá trị cho tham số @maPKB
                    cmd.Parameters.AddWithValue("@trieuchung", cd.TrieuChung);
                    cmd.Parameters.AddWithValue("@ten", cd.TenChuanDoan);
                    
                    try
                    {
                        con.Open(); // Mở kết nối
                        cmd.ExecuteNonQuery(); // Thực thi lệnh thêm mới
                        con.Close(); // Đóng kết nối
                        con.Dispose(); // Giải phóng tài nguyên kết nối
                    }
                    catch (Exception ex)
                    {
                        con.Close(); // Đóng kết nối khi có lỗi
                        return false; // Trả về false nếu có lỗi
                    }
                }
            }
            return true;
        }
        public List<chandoanDTO> selectByKeyWord(string sKeyword)
        {
            string query = "SELECT * FROM [Chandoan] WHERE (MaPkb LIKE '%' + @sKeyword + '%') OR (MaBenh LIKE '%' + @sKeyword + '%')";
            List<chandoanDTO> list = new List<chandoanDTO>();

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@sKeyword", sKeyword);

                    try
                    {
                        con.Open();
                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            chandoanDTO cd = new chandoanDTO();
                            cd.MaPkb = (reader["MaPkb"].ToString());
                            cd.MaBenh = (reader["MaBenh"].ToString());

                            list.Add(cd);
                        }
                    }
                    catch (Exception ex)
                    {
                        con.Close(); // Đóng kết nối khi có lỗi
                        return null; // Trả về null nếu có lỗi
                    }
                }
            }
            return list;
        }


    }
}
