using QLPKDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Collections;

namespace QLPKDAL
{
    public class lichHenDAL
    {
        private string connectionString;
        public lichHenDAL()
        {
            connectionString = System.Configuration.ConfigurationManager.AppSettings["ConnectionString"];
        }
        public string ConnectionString { get => connectionString; set => connectionString = value; }

        public List<lichHenDTO> select()
        {
            string query = "SELECT * FROM [LichHen]"; //lấy all lịch hẹn
            List<lichHenDTO> lslichHen = new List<lichHenDTO>();

            using (SqlConnection con = new SqlConnection(ConnectionString)) //khởi tạo kết nối csdl
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    try
                    {
                        con.Open();
                        SqlDataReader reader = cmd.ExecuteReader(); //đọc dl từng dòng

                        while (reader.Read())
                        {
                            lichHenDTO lh = new lichHenDTO();
                            lh.MaLichHen = reader["maLichHen"].ToString();
                            lh.MaBenhNhan = reader["maBenhNhan"].ToString();
                            lh.MaTaiKhoan = reader["maTaiKhoan"].ToString();
                            lh.MaDieuDuong = (reader["maDieuDuong"].ToString());
                            lh.NgayHen = DateTime.Parse(reader["ngayHen"].ToString());
                            lh.TrangThai = reader["trangThai"].ToString();
                            lslichHen.Add(lh);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            return lslichHen;
        }
        public bool them(lichHenDTO lh)
        {
            string query = string.Empty;
            query = "INSERT INTO [LichHen] (maBenhNhan, maTaiKhoan, ngayHen, trangThai, maDieuDuong) VALUES (@maBenhNhan, @maTaiKhoan, @ngayHen, @trangThai, @maDieuDuong)";
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@maBenhNhan", lh.MaBenhNhan);
                    cmd.Parameters.AddWithValue("@maTaiKhoan", lh.MaTaiKhoan);
                    cmd.Parameters.AddWithValue("@maDieuDuong", lh.MaDieuDuong);
                    cmd.Parameters.AddWithValue("@ngayHen", lh.NgayHen);
                    cmd.Parameters.AddWithValue("@trangThai", lh.TrangThai);
                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery(); //chạy sql insert
                        con.Close();
                    }
                    catch (Exception ex)
                    {
                        con.Close();
                        Console.WriteLine(ex.Message);
                        return false;
                    }
                }
            }
            return true;
        }
        //tự động tạo mã lịch hẹn
        public string AutoGenerateMaLichHen()
        {
            int maLichHen = 1;
            string query = "";
            query += "SELECT  MAX(CAST(maLichHen AS INT)) AS MaxMaLH FROM[LichHen]";

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    try
                    {
                        con.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.Read() && !reader.IsDBNull(0)) //kiểm tra xem có giá trị nào trong cột MaxMaLH không
                        {
                            maLichHen = int.Parse(reader["MaxMaLH"].ToString()) + 1;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            return maLichHen.ToString();
        }
        public bool xoa(lichHenDTO lh)
        {
            string query = string.Empty;
            query += "delete from [LichHen]"; //câu lệnh xóa
            query += "where maLichHen=@maLichHen";

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@maLichHen", lh.MaLichHen);
                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                        con.Dispose(); //giải phóng
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

        public bool CapNhatTrangThai(string maBenhNhan,string trangThaiMoi)
        {
            string query = "UPDATE LichHen SET trangThai = @trangThaiMoi WHERE maBenhNhan = @maBenhNhan";
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@trangThaiMoi", trangThaiMoi);
                cmd.Parameters.AddWithValue("@maBenhNhan", maBenhNhan);
             

                try
                {
                    con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
                catch
                {
                    return false;
                }
            }
        }
        //lấy ds lịch hẹn theo ngày
        public List<lichHenDTO> selectByDate(DateTime ngay)
        {
            string query = "SELECT * FROM LichHen WHERE CAST(NgayHen AS DATE) = @ngay";
            List<lichHenDTO> ds = new List<lichHenDTO>();

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@ngay", ngay.Date);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader(); //thực thi truy vấn và lấy dữ liệu

                while (reader.Read())
                {
                    lichHenDTO lh = new lichHenDTO();
                    lh.MaLichHen = reader["maLichHen"].ToString();
                    lh.MaBenhNhan = reader["maBenhNhan"].ToString();
                    lh.MaTaiKhoan = reader["maTaiKhoan"].ToString();
                    lh.MaDieuDuong = (reader["maDieuDuong"].ToString());
                    lh.NgayHen = DateTime.Parse(reader["ngayHen"].ToString());
                    lh.TrangThai = reader["trangThai"].ToString();
                    ds.Add(lh);
                }
            }

            return ds;
        }
  

    }
}
