using QLPKDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;

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
            string query = string.Empty;
            query += "INSERT INTO [HoaDon] ([ngayLapHoaDon], [tienThuoc], [tienKham], [tongTien], [maPKB],[maTaiKhoan],[ngayTaiKham])";
            query += "VALUES (@ngayLapHoaDon,@tienThuoc,@tienKham,@tongTien,@maPKB, @maTaiKhoan, @ngayTaiKham)";
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@ngayLapHoaDon", hd.NgayLapHoaDon);
                    cmd.Parameters.AddWithValue("@tienThuoc", hd.TienThuoc);
                    cmd.Parameters.AddWithValue("@tienKham", hd.TienKham);
                    cmd.Parameters.AddWithValue("@tongTien", hd.TongTien);
                    cmd.Parameters.AddWithValue("@maPKB", hd.MaPKB);
                    cmd.Parameters.AddWithValue("@maTaiKhoan", hd.MaNVTN);
                    cmd.Parameters.AddWithValue("@ngayTaiKham", hd.NgayTaiKham);    

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


        public List<hoadonDTO> select()
        {
            string query = string.Empty;
            query += "SELECT * ";
            query += "FROM [HoaDon]";

            List<hoadonDTO> lsHoaDon = new List<hoadonDTO>();

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
                                hoadonDTO hd = new hoadonDTO();
                                hd.MaHoaDon = (reader["maHoaDon"].ToString());
                                hd.NgayLapHoaDon = DateTime.Parse(reader["NgayLapHoaDon"].ToString());
                                hd.TienThuoc = decimal.Parse(reader["tienThuoc"].ToString());
                                hd.TongTien = float.Parse(reader["tongTien"].ToString());
                                hd.TienKham = float.Parse(reader["tienKham"].ToString());
                                hd.MaPKB = (reader["maPKB"].ToString());
                                hd.MaNVTN = (reader["maTaiKhoan"].ToString());
                                hd.NgayTaiKham = DateTime.Parse(reader["NgayTaiKham"].ToString());

                                lsHoaDon.Add(hd);

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
            return lsHoaDon;
        }
        //tong doanh thu
        public decimal doanhthu(string ngayLapHoaDon)
        {
            decimal doanhthu = 0;
            string query = string.Empty;
            query += "SELECT sum (HD.TongTien) as doanhthu FROM HoaDon HD WHERE HD.NgayLapHoaDon=@NgayLapHoaDon";
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@NgayLapHoaDon", ngayLapHoaDon);

                    try
                    {
                        con.Open();
                        SqlDataReader reader = null;
                        reader = cmd.ExecuteReader();
                        if (reader.HasRows == true)
                        {
                            while (reader.Read())
                            {
                                doanhthu = decimal.Parse(reader["doanhthu"].ToString());

                            }
                        }

                        con.Close();
                        con.Dispose();
                    }
                    catch (Exception ex)
                    {
                        con.Close();
                        return 0;
                    }
                }
            }
            return doanhthu;
        }
        public int sobenhnhan(string ngayLapHoaDon)
        {
            int sobn = 0;
            string query = string.Empty;
            query += "SELECT count (HD.maHoaDon) as sobn FROM HoaDon HD WHERE HD.NgayLapHoaDon=@NgayLapHoaDon";
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@NgayLapHoaDon", ngayLapHoaDon);

                    try
                    {
                        con.Open();
                        SqlDataReader reader = null;
                        reader = cmd.ExecuteReader();
                        if (reader.HasRows == true)
                        {
                            while (reader.Read())
                            {
                                sobn = int.Parse(reader["sobn"].ToString());

                            }
                        }

                        con.Close();
                        con.Dispose();
                    }
                    catch (Exception ex)
                    {
                        con.Close();
                        return 0;
                    }
                }
            }
            return sobn;
        }
        public decimal tienthuoc(hoadonDTO hd, string maPKB)
        {
            decimal tien = 0;
            string query = string.Empty;
            query += "SELECT SUM(TH.donGia * KT.soLuong) AS TIENTHUOC ";
            query += "FROM PhieuKhamBenh PKB ";
            query += "JOIN ToaThuoc T ON PKB.maPKB = T.maPKB ";
            query += "JOIN ChiTietDonThuoc KT ON T.maToaThuoc = KT.maToaThuoc ";
            query += "JOIN Thuoc TH ON KT.maThuoc = TH.maThuoc ";
            query += "WHERE PKB.maPKB = @maPKB";

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@maPKB", maPKB);

                    try
                    {
                        con.Open();
                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                if (reader["TIENTHUOC"] != DBNull.Value)
                                {
                                    tien = decimal.Parse(reader["TIENTHUOC"].ToString());
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error calculating medication cost: " + ex.Message);
                    }
                    finally
                    {
                        con.Close();
                    }
                }
            }
            return tien;
        }
        public string autogenerate_mahd()
        {
            int mahd = 1;
            string query = string.Empty;
            query += "SELECT MAX (CAST(KQ.MAHoaDon AS INT)) AS MM from (SELECT CONVERT(float, HoaDon.maHoaDon) AS MAHoaDon FROM HoaDon) AS KQ";

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
                                mahd = int.Parse(reader["MM"].ToString()) + 1;
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
            return mahd.ToString();
        }
        public float tienkham()
        {
            float tien = 0;
            string query = "SELECT tienDichVu FROM DichVu WHERE tenDichVu = @tenDichVu";

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@tenDichVu", "Kham benh");

                    try
                    {
                        con.Open();
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            tien = Convert.ToSingle(result);
                        }
                        con.Close();
                    }
                    catch (Exception ex)
                    {
                        con.Close();
                    }
                }
            }
            return tien;
        }
        public List<hoadonDTO> selectByMonth(string month, string year)
        {
            string query = string.Empty;
            query += " SELECT NgayLapHoaDon ";
            query += " FROM [HoaDon] ";
            query += " WHERE MONTH(NgayLapHoaDon)=@month and YEAR(NgayLapHoaDon)=@year group by NgayLapHoaDon ";


            List<hoadonDTO> lsHoadon = new List<hoadonDTO>();

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@month", month);
                    cmd.Parameters.AddWithValue("@year", year);
                    try
                    {
                        con.Open();
                        SqlDataReader reader = null;
                        reader = cmd.ExecuteReader();
                        if (reader.HasRows == true)
                        {
                            while (reader.Read())
                            {
                                hoadonDTO hd = new hoadonDTO();
                                hd.NgayLapHoaDon = DateTime.Parse(reader["NgayLapHoaDon"].ToString());
                                lsHoadon.Add(hd);

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
            return lsHoadon;
        }
        public float doanhthuMonth(string month, string year)
        {
            float doanhthu = 0;
            string query = string.Empty;
            query += "SELECT sum (HD.TongTien) as doanhthuthang FROM HoaDon HD WHERE MONTH(HD.NgayLapHoaDon)=@month AND YEAR(HD.NgayLapHoaDon)=@year";
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@month", month);
                    cmd.Parameters.AddWithValue("@year", year);

                    try
                    {
                        con.Open();
                        SqlDataReader reader = null;
                        reader = cmd.ExecuteReader();
                        if (reader.HasRows == true)
                        {
                            while (reader.Read())
                            {
                                doanhthu = float.Parse(reader["doanhthuthang"].ToString());

                            }
                        }

                        con.Close();
                        con.Dispose();
                    }
                    catch (Exception ex)
                    {
                        con.Close();
                        return 0;
                    }
                }
            }
            return doanhthu;
        }
    }

}
