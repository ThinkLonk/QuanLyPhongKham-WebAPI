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
    public class taiKhoanDAL
    {
        
        private string connectionString;
        public taiKhoanDAL()
        {
            //khởi tạo chuỗi kết nối từ file cấu hình
            connectionString = ConfigurationManager.AppSettings["ConnectionString"];
        }
        public string ConnectionString { get => connectionString; set => connectionString = value;}

        public bool KiemTraTonTai(taiKhoanDTO tk)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                string query = "SELECT COUNT(*) FROM [TaiKhoan] WHERE Username = @TenDN";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@TenDN", tk.Username);
                    try
                    {
                        con.Open();
                        // Dùng ExecuteScalar để lấy con số COUNT(*)
                        int count = (int)cmd.ExecuteScalar();
                        con.Close();

                        return count > 0; // Nếu lớn hơn 0 nghĩa là đã tồn tại
                    }
                    catch (Exception ex)
                    {
                        con.Close();
                        return false;
                    }
                }
            }
        }


        public bool them(taiKhoanDTO tk)
        {
            string query = string.Empty;
            query += "INSERT INTO [TaiKhoan] ([userName],[passWord],[name],[maRole])";
            query += "VALUES (@userName,@passWord,@name,@maRole)";
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@userName", tk.Username);
                    cmd.Parameters.AddWithValue("@passWord", tk.Password);
                    cmd.Parameters.AddWithValue("@name", tk.Name);
                    cmd.Parameters.AddWithValue("@maRole", tk.MaLoai);
                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                        con.Dispose();
                    }
                    catch (Exception ex)
                    {
                        con.Close();
                        return false;
                    }
                }
            }
            return true;
        }

        public bool sua(taiKhoanDTO tk, string maTaiKhoanold)
        {
            string query = string.Empty;
            query += "update [TaiKhoan]";
            query += "set userName=@userName,passWord=@passWord,name=@name,maRole=@maRole where maTaiKhoan=@maTaiKhoanold";

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@userName", tk.Username);
                    cmd.Parameters.AddWithValue("@passWord", tk.Password);
                    cmd.Parameters.AddWithValue("@name", tk.Name);
                    cmd.Parameters.AddWithValue("@maRole", tk.MaLoai);
                    cmd.Parameters.AddWithValue("@maTaiKhoanold", maTaiKhoanold);
                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        con.Close();
                        return false;
                    }
                }

                return true;
            }

        }
        public bool xoa(taiKhoanDTO tk)
        {
            string query = string.Empty;
            query += "delete from [TaiKhoan]";
            query += "where userName=@userName";

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@userName", tk.Username);
                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                        con.Dispose();
                    }
                    catch (Exception ex)
                    {
                        con.Close();
                        return false;
                    }
                }

                return true;
            }
        }
        public List<taiKhoanDTO> select()
        {
            //khởi tạo chuỗi rổng
            string query = string.Empty;
            //câu lệnh truy vấn
            query += "SELECT *";
            query += " FROM TaiKhoan";
            List<taiKhoanDTO> lsTK= new List<taiKhoanDTO>();

            //tạo kết nối đến cơ sở dữ liệu SQL Server
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                //tạo đối tượng SqlCommand để thực thi câu lệnh SQL
                using (SqlCommand cmd = new SqlCommand())
                {
                    //gán kết nối cho SqlCommand
                    cmd.Connection = con;
                    cmd.CommandType = System.Data.CommandType.Text; // chỉ định loại câu lệnh là văn bản (text)
                    cmd.CommandText = query;

                    try
                    {
                        //mở kết nối đến cơ sở dữ liệu
                        con.Open();
                        SqlDataReader reader = null;
                        reader = cmd.ExecuteReader();
                        //kiểm tra xem có dữ liệu trả về hay không
                        if (reader.HasRows == true)
                        {
                            //nếu có dữ liệu thì đọc từng dòng dữ liệu
                            while (reader.Read())
                            {
                                taiKhoanDTO tk = new taiKhoanDTO();
                                tk.Name = reader["Name"].ToString();
                                tk.Username = reader["username"].ToString();
                                tk.Password = reader["password"].ToString();
                                tk.MaLoai = int.Parse(reader["maRole"].ToString());
                                tk.MaTK = int.Parse(reader["maTaiKhoan"].ToString());


                                lsTK.Add(tk);
                            }
                        }
                        con.Close();
                        con.Dispose(); // giải phóng tài nguyên kết nối
                    }
                    catch(Exception ex)
                    {
                        con.Close();
                        return null;
                    }
                }
            }
            return lsTK;
        }
        public List<taiKhoanDTO> selectByKeyWord(string sKeyword)
        {
            string query = string.Empty;
            query += " SELECT * ";
            query += " FROM [TaiKhoan] ";
            query += " WHERE ([name] LIKE CONCAT('%',@sKeyword,'%')) ";
            query += " OR ([userName] LIKE CONCAT('%',@sKeyword,'%'))";

            List<taiKhoanDTO> lsTaiKhoan = new List<taiKhoanDTO>();

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@sKeyword", sKeyword);
                    try
                    {
                        con.Open();
                        SqlDataReader reader = null;
                        reader = cmd.ExecuteReader();
                        if (reader.HasRows == true)
                        {
                            while (reader.Read())
                            {
                                taiKhoanDTO tk = new taiKhoanDTO();
                                tk.MaTK = int.Parse(reader["maTaiKhoan"].ToString());
                                tk.Username = reader["userName"].ToString();
                                tk.Password = reader["password"].ToString();
                                tk.Name = reader["name"].ToString();
                                tk.MaLoai = int.Parse(reader["maRole"].ToString());

                                lsTaiKhoan.Add(tk);

                            }
                        }

                        con.Close();
                        con.Dispose();
                    }
                    catch (Exception ex)
                    {
                        con.Close();
                        return null;
                    }
                }
            }
            return lsTaiKhoan;
        }
        public int autogenerate_maTaiKhoan()
        {
            int maTK = 1;
            string query = string.Empty;
            query += "SELECT MAX (KQ.maTaiKhoan) AS MM from (SELECT CONVERT(float, TaiKhoan.maTaiKhoan) AS maTaiKhoan FROM TaiKhoan) AS KQ";

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = query;

                    try
                    {
                        con.Open();
                        SqlDataReader reader = null;
                        reader = cmd.ExecuteReader();
                        if (reader.HasRows == true)
                        {
                            while (reader.Read())
                            {
                                maTK = int.Parse(reader["MM"].ToString()) + 1;
                            }
                        }

                        con.Close();
                        con.Dispose();
                    }
                    catch (Exception ex)
                    {
                        con.Close();

                    }
                }
            }
            return maTK;
        }

    }
}
