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
    public class ChiTietToaThuocDAL
    {
        private string connectionString;
        public ChiTietToaThuocDAL() 
        {
            connectionString = ConfigurationManager.AppSettings["ConnectionString"];
        }
        public string ConnectionString { get => connectionString; set => connectionString = value;}
        public bool kethuoc(ChiTietToaThuocDTO kt)
        {
            string query = string.Empty;
            query += "INSERT INTO [ChiTietDonThuoc] ([maToaThuoc], [maThuoc],[soLuong])";
            query += "VALUES (@maToaThuoc,@maThuoc,@soLuong)";
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@maToaThuoc", kt.MaToa);
                    cmd.Parameters.AddWithValue("@maThuoc", kt.MaThuoc);
                    cmd.Parameters.AddWithValue("@soLuong", kt.SoLuong);

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
        public List<ChiTietToaThuocDTO> selectbypkb(string mapkb)
        {
            string query = @"
            SELECT KT.maToaThuoc, KT.maThuoc, KT.soLuong 
            FROM PhieuKhamBenh PKB 
            JOIN ToaThuoc T ON PKB.maPKB = T.maPKB 
            JOIN ChiTietDonThuoc KT ON T.maToaThuoc = KT.maToaThuoc 
            JOIN Thuoc TH ON KT.maThuoc = TH.maThuoc 
            WHERE PKB.maPKB = @mapkb";

            List<ChiTietToaThuocDTO> lskethuoc = new List<ChiTietToaThuocDTO>();

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
                                    ChiTietToaThuocDTO kt = new ChiTietToaThuocDTO();
                                    kt.SoLuong = int.Parse(reader["soLuong"].ToString());
                                    kt.MaToa = (reader["maToaThuoc"].ToString());
                                    kt.MaThuoc = (reader["maThuoc"].ToString());
                                    lskethuoc.Add(kt);
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
            return lskethuoc;
        }
        public List<ChiTietToaThuocDTO> baocaobymonth(string month, string year)
        {
            string query = string.Empty;
            //lấy danh sách các loại thuốc và tổng số lượng đã kê trong một tháng/năm
            query += "SELECT TH.maThuoc, TH.tenThuoc, sum (KT.soLuong) as soLuong FROM ToaThuoc T JOIN ChiTietDonThuoc KT ON T.maToaThuoc=KT.maToaThuoc JOIN Thuoc TH ON KT.maThuoc=TH.maThuoc WHERE MONTH(T.ngayKeToa)=@month and year(T.ngayKeToa)=@year group by TH.maThuoc,TH.tenThuoc";


            List<ChiTietToaThuocDTO> lskethuoc = new List<ChiTietToaThuocDTO>();

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
                                ChiTietToaThuocDTO kt = new ChiTietToaThuocDTO();
                                kt.SoLuong = int.Parse(reader["soLuong"].ToString());
                                kt.MaThuoc = (reader["maThuoc"].ToString());
                                lskethuoc.Add(kt);

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
            return lskethuoc;
        }
        public int solandungbymonth(string mathuoc, string month, string year)
        {
            int SLD = 0;
            string query = string.Empty;
            //số lần thuốc đc kê trong tháng/năm
            query += "SELECT  count (KT.maToaThuoc) as SLD FROM ToaThuoc T JOIN ChiTietDonThuoc KT ON T.maToaThuoc=KT.maToaThuoc JOIN Thuoc TH ON KT.maThuoc=TH.maThuoc WHERE MONTH(T.ngayKeToa)=@month and year(T.ngayKeToa)=@year and TH.maThuoc=@mathuoc";


            using (SqlConnection con = new SqlConnection(ConnectionString))
            {

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@month", month);
                    cmd.Parameters.AddWithValue("@year", year);
                    cmd.Parameters.AddWithValue("@mathuoc", mathuoc);
                    try
                    {
                        con.Open();
                        SqlDataReader reader = null;
                        reader = cmd.ExecuteReader();
                        if (reader.HasRows == true)
                        {
                            while (reader.Read())
                            {
                                SLD = int.Parse(reader["SLD"].ToString());
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
            return SLD;
        }
        public List<ChiTietToaThuocDTO> selectByDate(DateTime ngay)
        {
            List<ChiTietToaThuocDTO> list = new List<ChiTietToaThuocDTO>();

            string query = @"
                        SELECT CT.maThuoc, CT.maToaThuoc, CT.soLuong
                        FROM ChiTietDonThuoc CT
                        INNER JOIN ToaThuoc TT ON CT.maToaThuoc = TT.maToaThuoc
                        WHERE CAST(TT.ngayKeToa AS DATE) = @ngay";


            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ngay", ngay.Date);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    ChiTietToaThuocDTO kt = new ChiTietToaThuocDTO();
                    kt.SoLuong = int.Parse(reader["soLuong"].ToString());
                    kt.MaToa = (reader["maToaThuoc"].ToString());
                    kt.MaThuoc = (reader["maThuoc"].ToString());

                    list.Add(kt);
                }

                reader.Close();
            }

            return list;
        }

    }
}
