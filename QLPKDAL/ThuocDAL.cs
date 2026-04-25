using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLPKDTO;
using System.Configuration;
using System.Data.SqlClient;

namespace QLPKDAL
{
    public class ThuocDAL
    {
        private string connectionString;
        public ThuocDAL()
        {
            connectionString = ConfigurationManager.AppSettings["ConnectionString"];
        }
        public string ConnectionString { get => connectionString; set => connectionString = value; }
        public bool them(thuocDTO th)
        {
            string query = string.Empty;
            query += "INSERT INTO [Thuoc] ([tenThuoc],[maDonVi],[Dongia],[maCachDung],[soLuong])";
            query += "VALUES (@tenThuoc,@donVi,@Dongia,@CachDung, @soLuong)";
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@tenThuoc", th.TenThuoc);
                    cmd.Parameters.AddWithValue("@donVi", th.MaDonVi);
                    cmd.Parameters.AddWithValue("@Dongia", th.DonGia);
                    cmd.Parameters.AddWithValue("@CachDung", th.MaCachDung);
                    cmd.Parameters.AddWithValue("@soluong", th.SoLuong);
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
        public bool sua(thuocDTO th, string maThuocold)
        {
            string query = string.Empty;
            query += "update [Thuoc]";
            query += "set tenThuoc=@tenThuoc,maDonVi=@DonVi,Dongia=@Dongia,maCachDung=@CachDung, soLuong=@soLuong where maThuoc=@maThuocold";

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@tenThuoc", th.TenThuoc);
                    cmd.Parameters.AddWithValue("@DonVi", th.MaDonVi);
                    cmd.Parameters.AddWithValue("@Dongia", th.DonGia);
                    cmd.Parameters.AddWithValue("@CachDung", th.MaCachDung);
                    cmd.Parameters.AddWithValue("@maThuocold", maThuocold);
                    cmd.Parameters.AddWithValue("@soLuong", th.SoLuong);
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
        public bool xoa(thuocDTO th)
        {
            string query = string.Empty;
            query += "delete from [Thuoc]";
            query += "where maThuoc=@maThuoc";

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@maThuoc", th.MaThuoc);
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
        public List<thuocDTO> select()
        {
            string query = string.Empty;
            query += "SELECT * ";
            query += "FROM [Thuoc]";

            List<thuocDTO> lsThuoc = new List<thuocDTO>();

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
                                thuocDTO th = new thuocDTO();
                                th.MaThuoc = (reader["maThuoc"].ToString());
                                th.TenThuoc = (reader["tenThuoc"].ToString());
                                th.MaDonVi = (reader["maDonVi"].ToString());
                                th.MaCachDung = (reader["maCachDung"].ToString());
                                th.DonGia = float.Parse(reader["donGia"].ToString());
                                th.SoLuong = int.Parse(reader["soLuong"].ToString());
                                lsThuoc.Add(th);

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
            return lsThuoc;
        }
        public List<thuocDTO> selectByKeyWord(string sKeyword)
        {
            string query = string.Empty;
            query += " SELECT * ";
            query += " FROM [Thuoc]";
            query += " WHERE ([maThuoc] LIKE CONCAT('%',@sKeyword,'%'))";
            query += " OR ([tenThuoc] LIKE CONCAT('%',@sKeyword,'%'))";

            List<thuocDTO> lsThuoc = new List<thuocDTO>();

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
                                thuocDTO th = new thuocDTO();
                                th.MaThuoc = (reader["maThuoc"].ToString());
                                th.TenThuoc = (reader["tenThuoc"].ToString());
                                th.MaDonVi = (reader["maDonVi"].ToString());
                                th.MaCachDung = (reader["maCachDung"].ToString());
                                th.DonGia = float.Parse(reader["donGia"].ToString());
                                th.SoLuong = int.Parse(reader["soLuong"].ToString());
                                lsThuoc.Add(th);

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
            return lsThuoc;
        }
        public string autogenerate_mathuoc()
        {
            int mathuoc = 1;
            string query = string.Empty;
            query += "SELECT MAX (CAST(KQ.MATHUOC AS INT)) AS MM from (SELECT CONVERT(float, Thuoc.maThuoc) AS MATHUOC FROM Thuoc) AS KQ";

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
                                mathuoc = int.Parse(reader["MM"].ToString()) + 1;
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
            return mathuoc.ToString();
        }
        //danh sách các thuoc da ke trong pkb
        public List<thuocDTO> selectbypkb(string mapkb)
        {
            string query = @"
        SELECT TH.maThuoc, TH.tenThuoc, TH.maCachDung, TH.maDonVi, TH.donGia 
        FROM PhieuKhamBenh PKB 
        JOIN ToaThuoc T ON PKB.maPKB = T.maPKB 
        JOIN ChiTietDonThuoc KT ON T.maToaThuoc = KT.maToaThuoc 
        JOIN Thuoc TH ON KT.maThuoc = TH.maThuoc 
        WHERE PKB.maPKB = @mapkb";

            List<thuocDTO> lsThuoc = new List<thuocDTO>();

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@mapkb", mapkb);
                    try
                    {
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    thuocDTO th = new thuocDTO
                                    {
                                        MaThuoc = (reader["maThuoc"].ToString()),
                                        TenThuoc = (reader["tenThuoc"].ToString()),
                                        MaDonVi = (reader["maDonVi"].ToString()),
                                        MaCachDung = (reader["maCachDung"].ToString()),
                                        DonGia = float.Parse(reader["donGia"].ToString())
                                    };

                                    lsThuoc.Add(th);
                                }
                            }
                        }
                        con.Close();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                        con.Close();
                        return null;
                    }
                }
            }
            return lsThuoc;
        }

        public List<thuocDTO> baocaobymonth(string month, string year)
        {
            string query = string.Empty;
            //lấy ds các loại thuốc đã được kê trong các toa thuốc theo tháng/năm
            query += "SELECT TH.maThuoc, TH.tenThuoc, TH.maDonVi FROM ToaThuoc T JOIN ChiTietDonThuoc KT ON T.maToaThuoc=KT.maToaThuoc JOIN Thuoc TH ON KT.maThuoc=TH.maThuoc WHERE MONTH(T.ngayKeToa)=@month and YEAR(T.ngayKeToa)=@year group by TH.maThuoc,TH.tenThuoc,TH.maDonVi";


            List<thuocDTO> lsThuoc = new List<thuocDTO>();

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
                                thuocDTO th = new thuocDTO();
                                th.MaThuoc = (reader["maThuoc"].ToString());
                                th.TenThuoc = (reader["tenThuoc"].ToString());
                                th.MaDonVi = (reader["maDonVi"].ToString());
                                lsThuoc.Add(th);

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
            return lsThuoc;
        }
        public bool KiemTraTenThuocDaTonTai(string tenThuoc)
        {
            string query = "SELECT COUNT(*) FROM Thuoc WHERE tenThuoc = @tenThuoc";
            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@tenThuoc", tenThuoc);
                try
                {
                    con.Open();
                    int count = (int)cmd.ExecuteScalar();
                    con.Close();
                    return count > 0;
                }
                catch
                {
                    con.Close();
                    return false;
                }
            }
        }
        public bool truSoLuong(string maThuoc, int soLuongTru)
        {
            string query = @"UPDATE Thuoc
                     SET soLuong = soLuong - @soLuongTru
                     WHERE maThuoc = @maThuoc
                     AND soLuong >= @soLuongTru";

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@maThuoc", maThuoc);
                cmd.Parameters.AddWithValue("@soLuongTru", soLuongTru);

                try
                {
                    con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
                catch
                {
                    con.Close();
                    return false;
                }
            }
        }

    }
}
