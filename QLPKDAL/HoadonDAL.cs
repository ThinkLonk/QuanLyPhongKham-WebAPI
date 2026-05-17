using QLPKDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace QLPKDAL
{
    public class HoadonDAL
    {
        private string connectionString;
        public HoadonDAL()
        {
            connectionString = ConfigurationManager.AppSettings["ConnectionString"];
        }
        public string ConnectionString { get => connectionString; set => connectionString = value; }

        public bool them(hoadonDTO hd)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                // Sử dụng Stored Procedure sp_LapHoaDonThanhToan đã tạo
                using (SqlCommand cmd = new SqlCommand("sp_LapHoaDonThanhToan", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@maPKB", hd.MaPKB);
                    cmd.Parameters.AddWithValue("@maTaiKhoan", hd.MaNVTN);

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Lỗi khi lập hóa đơn: " + ex.Message);
                    }
                }
            }
        }

        public List<hoadonDTO> select()
        {
            List<hoadonDTO> lsHoaDon = new List<hoadonDTO>();
            // Sử dụng View v_DanhSachHoaDon
            string query = "SELECT * FROM v_DanhSachHoaDon";

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.CommandType = CommandType.Text;
                    try
                    {
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                hoadonDTO hd = new hoadonDTO();

                                hd.MaHoaDon =
                                    int.Parse(reader["maHoaDon"].ToString());

                                hd.MaPKB =
                                    int.Parse(reader["maPKB"].ToString());

                                hd.MaNVTN =
                                    int.Parse(reader["maTaiKhoan"].ToString());

                                hd.NgayLapHoaDon =
                                    DateTime.Parse(
                                        reader["ngayLapHoaDon"].ToString());

                                hd.TienThuoc =
                                    decimal.Parse(
                                        reader["tienThuoc"].ToString());

                                hd.TienKham =
                                    float.Parse(
                                        reader["tienKham"].ToString());

                                hd.TongTien =
                                    float.Parse(
                                        reader["tongTien"].ToString());

                                lsHoaDon.Add(hd);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Lỗi khi tải danh sách hóa đơn: " + ex.Message);
                    }
                }
            }
            return lsHoaDon;
        }

        public decimal doanhthu(string ngayLapHoaDon)
        {
            decimal doanhthu = 0;
            string query = "SELECT SUM(tongTien) AS doanhthu FROM HoaDon WHERE ngayLapHoaDon = @NgayLapHoaDon";
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@NgayLapHoaDon", ngayLapHoaDon);
                    try
                    {
                        con.Open();
                        object result = cmd.ExecuteScalar();
                        if (result != DBNull.Value && result != null)
                        {
                            doanhthu = Convert.ToDecimal(result);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Lỗi khi tính doanh thu: " + ex.Message);
                    }
                }
            }
            return doanhthu;
        }

        public int sobenhnhan(string ngayLapHoaDon)
        {
            int sobn = 0;
            string query = "SELECT COUNT(maHoaDon) AS sobn FROM HoaDon WHERE ngayLapHoaDon = @NgayLapHoaDon";
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@NgayLapHoaDon", ngayLapHoaDon);
                    try
                    {
                        con.Open();
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            sobn = Convert.ToInt32(result);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Lỗi khi đếm số bệnh nhân: " + ex.Message);
                    }
                }
            }
            return sobn;
        }

        public decimal tienthuoc(hoadonDTO hd, int maPKB) // Changed parameter to int
        {
            decimal tien = 0;
            // Sử dụng FUNCTION f_TinhTienThuoc đã tạo
            string query = "SELECT dbo.f_TinhTienThuoc(@maPKB)";

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@maPKB", maPKB);
                    try
                    {
                        con.Open();
                        object result = cmd.ExecuteScalar();
                        if (result != DBNull.Value && result != null)
                        {
                            tien = Convert.ToDecimal(result);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Lỗi khi tính tiền thuốc: " + ex.Message);
                    }
                }
            }
            return tien;
        }



        public float tienkham()
        {
            float tien = 0;
            string query = "SELECT tienDichVu FROM DichVu WHERE tenDichVu = @tenDichVu";

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@tenDichVu", "Khám bệnh");
                    try
                    {
                        con.Open();
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            tien = Convert.ToSingle(result);
                        }
                    }
                    catch
                    {
                        tien = 50000; // Default fallback
                    }
                }
            }
            return tien;
        }

        public List<hoadonDTO> selectByMonth(string month, string year)
        {
            string query = @"
                SELECT ngayLapHoaDon 
                FROM HoaDon 
                WHERE MONTH(ngayLapHoaDon) = @month AND YEAR(ngayLapHoaDon) = @year 
                GROUP BY ngayLapHoaDon";

            List<hoadonDTO> lsHoadon = new List<hoadonDTO>();

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@month", month);
                    cmd.Parameters.AddWithValue("@year", year);
                    try
                    {
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                hoadonDTO hd = new hoadonDTO();
                                hd.NgayLapHoaDon = DateTime.Parse(reader["ngayLapHoaDon"].ToString());
                                lsHoadon.Add(hd);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Lỗi khi tải hóa đơn theo tháng: " + ex.Message);
                    }
                }
            }
            return lsHoadon;
        }

        public float doanhthuMonth(string month, string year)
        {
            float doanhthu = 0;
            string query = "SELECT SUM(tongTien) AS doanhthuthang FROM HoaDon WHERE MONTH(ngayLapHoaDon) = @month AND YEAR(ngayLapHoaDon) = @year";
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@month", month);
                    cmd.Parameters.AddWithValue("@year", year);
                    try
                    {
                        con.Open();
                        object result = cmd.ExecuteScalar();
                        if (result != DBNull.Value && result != null)
                        {
                            doanhthu = Convert.ToSingle(result);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Lỗi khi tính doanh thu tháng: " + ex.Message);
                    }
                }
            }
            return doanhthu;
        }
    }
}
