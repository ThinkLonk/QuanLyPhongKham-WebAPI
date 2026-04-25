using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLPKDTO;



namespace QLPKDAL
{
    public class BenhNhanDAL
    {
        private string connectionString;

        public BenhNhanDAL()
        {
            // Đọc chuỗi kết nối từ cấu hình ứng dụng
            connectionString = ConfigurationManager.AppSettings["ConnectionString"];
        }

        // Thuộc tính cho chuỗi kết nối
        public string ConnectionString { get => connectionString; set => connectionString = value; }

        public bool Them(BenhNhanDTO bn)
        {
            // Chuỗi truy vấn SQL để thêm bệnh nhân
            string query = string.Empty;
            query += "INSERT INTO [BenhNhan] ([tenBenhNhan], [gioiTinh], [ngaySinh], [diaChi], [CCCD],[email])";
            query += "VALUES ( @tenBenhNhan, @gioiTinh, @ngaySinh, @diaChi, @cccd, @email)";

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con; // Kết nối lệnh với cơ sở dữ liệu
                    cmd.CommandType = System.Data.CommandType.Text; // Kiểu lệnh là văn bản
                    cmd.CommandText = query; // Gán chuỗi truy vấn cho lệnh
                    cmd.Parameters.AddWithValue("@tenBenhNhan", bn.TenBN); // Thêm tham số cho lệnh SQL
                    cmd.Parameters.AddWithValue("@gioiTinh", bn.GtBN);
                    cmd.Parameters.AddWithValue("@ngaySinh", bn.NgsinhBN);
                    cmd.Parameters.AddWithValue("@diaChi", bn.DiachiBN);
                    cmd.Parameters.AddWithValue("@cccd", bn.CanCuocCongDan);
                    cmd.Parameters.AddWithValue("@email", bn.Email);

                    try
                    {
                        con.Open(); // Mở kết nối
                        cmd.ExecuteNonQuery(); // Thực thi lệnh không trả về kết quả
                        con.Close(); // Đóng kết nối
                    }
                    catch (Exception ex)
                    {
                        con.Close(); // Đóng kết nối khi có lỗi
                        return false;
                    }
                }
            }
            return true; // Trả về true nếu thành công
        }

        // Phương thức sửa một bệnh nhân
        public bool Sua(BenhNhanDTO bn, string maBNold)
        {
            string query = string.Empty;
            query += "UPDATE [BenhNhan] ";
            query += "SET tenBenhNhan=@tenBenhNhan, gioiTinh=@gioiTinh, ngaySinh=@ngaySinh, diaChi=@diaChi, CCCD=@cccd, email=@email ";
            query += "WHERE maBenhNhan=@maBNold";

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.CommandType = System.Data.CommandType.Text; // Kiểu lệnh là văn bản

                    // Thêm tham số và giá trị của chúng
                    cmd.Parameters.AddWithValue("@tenBenhNhan", bn.TenBN);
                    cmd.Parameters.AddWithValue("@gioiTinh", bn.GtBN);
                    cmd.Parameters.AddWithValue("@ngaySinh", bn.NgsinhBN);
                    cmd.Parameters.AddWithValue("@diaChi", bn.DiachiBN);
                    cmd.Parameters.AddWithValue("@maBNold", maBNold);
                    cmd.Parameters.AddWithValue("@cccd", bn.CanCuocCongDan);
                    cmd.Parameters.AddWithValue("@email", bn.Email);
                    try
                    {
                        con.Open(); // Mở kết nối
                        cmd.ExecuteNonQuery(); // Thực thi lệnh không trả về kết quả
                        con.Close(); // Đóng kết nối
                    }
                    catch (Exception ex)
                    {
                        // Xử lý lỗi
                        Console.WriteLine("Lỗi: " + ex.Message);
                        con.Close(); // Đóng kết nối khi có lỗi
                        return false; // Trả về false nếu có lỗi
                    }
                }
            }
            return true; // Trả về true nếu thành công
        }

        // Phương thức xóa một bệnh nhân
        public bool Xoa(BenhNhanDTO bn)
        {
            // Chuỗi truy vấn SQL để xóa bệnh nhân
            string query = string.Empty;
            query += "DELETE FROM [BenhNhan] ";
            query += "WHERE maBenhNhan=@MaBN";

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con; // Kết nối lệnh với cơ sở dữ liệu
                    cmd.CommandType = System.Data.CommandType.Text; // Kiểu lệnh là văn bản
                    cmd.CommandText = query; // Gán chuỗi truy vấn cho lệnh
                    cmd.Parameters.AddWithValue("@MaBN", bn.MaBN); // Thêm tham số cho lệnh SQL

                    try
                    {
                        con.Open(); // Mở kết nối
                        cmd.ExecuteNonQuery(); // Thực thi lệnh không trả về kết quả
                        con.Close(); // Đóng kết nối
                    }
                    catch (Exception)
                    {
                        con.Close(); // Đóng kết nối khi có lỗi
                        return false; // Trả về false nếu có lỗi
                    }
                }
            }
            return true; // Trả về true nếu thành công
        }
        // Phương thức chọn tất cả các bệnh nhân
        public List<BenhNhanDTO> select()
        {
            // Chuỗi truy vấn SQL để chọn tất cả các bệnh nhân
            string query = string.Empty;
            query += "SELECT * ";
            query += "FROM [BenhNhan]";

            List<BenhNhanDTO> lsBenhNhan = new List<BenhNhanDTO>(); // Danh sách để chứa kết quả

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
                        if (reader.HasRows)
                        {
                            while (reader.Read()) // Đọc từng dòng kết quả
                            {
                                BenhNhanDTO bn = new BenhNhanDTO(); 
                                bn.MaBN = reader["maBenhNhan"].ToString(); 
                                bn.TenBN = reader["tenBenhNhan"].ToString(); 
                                bn.GtBN = reader["gioiTinh"].ToString();
                                bn.NgsinhBN = DateTime.Parse(reader["ngaySinh"].ToString());
                                bn.DiachiBN = reader["diaChi"].ToString();
                                bn.CanCuocCongDan = reader["CCCD"].ToString();
                                bn.Email = reader["email"].ToString();
                                lsBenhNhan.Add(bn); // Thêm vào danh sách
                            }
                        }
                        con.Close(); // Đóng kết nối
                    }
                    catch (Exception)
                    {
                        con.Close(); // Đóng kết nối khi có lỗi
                        return null; // Trả về null nếu có lỗi
                    }
                }
            }
            return lsBenhNhan;
        }
        // Phương thức chọn bệnh nhân theo từ khóa
        public List<BenhNhanDTO> SelectByKeyWord(string sKeyword)
        {
            // Chuỗi truy vấn SQL để chọn bệnh nhân theo từ khóa
            string query = "SELECT * FROM BenhNhan WHERE ";
            bool isNumber = int.TryParse(sKeyword, out int maBN);

            if (isNumber)
            {
                query += "maBenhNhan = @maBN ";
            }
            else
            {
                query += "(tenBenhNhan LIKE CONCAT('%', @sKeyword, '%') ";
                query += "OR CCCD LIKE CONCAT('%', @sKeyword, '%'))";
            }

            List<BenhNhanDTO> lsBenhNhan = new List<BenhNhanDTO>(); //ds chứa kết quả

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.CommandType = System.Data.CommandType.Text;  // Kiểu lệnh là văn bản

                    if (isNumber)
                        //là số thì gán
                        cmd.Parameters.AddWithValue("@maBN", maBN);
                    else
                        cmd.Parameters.AddWithValue("@sKeyword", sKeyword);

                    try
                    {
                        con.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())// Đọc từng dòng kết quả
                            {
                                BenhNhanDTO bn = new BenhNhanDTO();
                                bn.MaBN = reader["maBenhNhan"].ToString();
                                bn.TenBN = reader["tenBenhNhan"].ToString();
                                bn.GtBN = reader["gioiTinh"].ToString();
                                bn.NgsinhBN = DateTime.Parse(reader["ngaySinh"].ToString());
                                bn.DiachiBN = reader["diaChi"].ToString();
                                bn.CanCuocCongDan = reader["CCCD"].ToString();
                                bn.Email = reader["Email"].ToString();
                                lsBenhNhan.Add(bn);
                            }
                        }
                        con.Close();
                    }
                    catch (Exception)
                    {
                        con.Close();
                        return null;
                    }
                }
            }

            return lsBenhNhan;
        }


        // Phương thức tự động tạo mã bệnh nhân
        public string AutoGenerateMaBN()
        {
            int maBN = 1; // Biến để chứa mã bệnh nhân mới, bắt đầu từ 1
            // Chuỗi truy vấn SQL để lấy mã bệnh nhân lớn nhất
            string query = string.Empty;
            query += "SELECT MAX(CAST(maBenhNhan AS INT)) AS MaxMaBN FROM [BenhNhan]";

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
                        if (reader.HasRows)
                        {
                            while (reader.Read()) // Đọc từng dòng kết quả
                            {
                                if (reader["MaxMaBN"] != DBNull.Value)
                                {
                                    maBN = int.Parse(reader["MaxMaBN"].ToString()) + 1; // Tạo mã bệnh nhân mới bằng cách tăng mã bệnh nhân lớn nhất lên 1
                                }
                            }
                        }
                        con.Close(); // Đóng kết nối
                    }
                    catch (Exception)
                    {
                        con.Close(); // Đóng kết nối khi có lỗi
                    }
                }
            }
            return maBN.ToString();
        }
 
    }
}
