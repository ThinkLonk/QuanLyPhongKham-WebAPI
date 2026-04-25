using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using QLPKDTO;
using System.Data.SqlClient;

namespace QLPKDAL
{
    public class PhieukhambenhDAL
    {
        private string connectionString;
        public PhieukhambenhDAL()
        {
            //kết nối
            connectionString = ConfigurationManager.AppSettings["ConnectionString"];
        }
        public string ConnectionString { get => connectionString; set => connectionString = value; }
        //lấy tất cả phiếu khám bệnh
        public List<phieukhambenhDTO> select()
        {
            string query = "SELECT * FROM [PhieuKhamBenh]";
            List<phieukhambenhDTO> lspkb = new List<phieukhambenhDTO>();

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    try
                    {
                        con.Open();
                        SqlDataReader reader = cmd.ExecuteReader(); //thực thi truy vấn

                        while (reader.Read())
                        {
                            phieukhambenhDTO pkb = new phieukhambenhDTO();
                            pkb.MaPKB = (reader["maPKB"].ToString());
                            pkb.TrieuChung = reader["TrieuChung"].ToString();
                            pkb.NgayKham = Convert.ToDateTime(reader["NgayKham"]);
                            pkb.MaBenhNhan = reader["maBenhNhan"].ToString();
                            pkb.MBS = (reader["maTaiKhoan"].ToString());
                            pkb.NgayTaiKham = Convert.ToDateTime(reader["NgayTaiKham"]);
                            pkb.DaGuiMail = Convert.ToBoolean(reader["DaGuiMail"]);
                            lspkb.Add(pkb);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            return lspkb;
        }
        //lấy phiếu khám bệnh theo mã và theo ngày khám
        public List<phieukhambenhDTO> selectByKeyWord(string sKeyword)
        {
            string query = "SELECT * FROM [PhieuKhamBenh] WHERE (maPKB LIKE '%' + @sKeyword + '%') OR (NgayKham = @sKeyword)";
            List<phieukhambenhDTO> lspkb = new List<phieukhambenhDTO>();

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
                            phieukhambenhDTO pkb = new phieukhambenhDTO();
                            pkb.MaPKB = (reader["maPKB"].ToString());
                            pkb.TrieuChung = reader["TrieuChung"].ToString();
                            pkb.NgayKham = Convert.ToDateTime(reader["NgayKham"]);
                            pkb.MaBenhNhan = reader["maBenhNhan"].ToString();

                            lspkb.Add(pkb);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            return lspkb;
        }
        // hàm này dùng để tự động sinh mã phiếu khám bệnh mới
        public string AutoGenerateMaPKB()
        {
            int maPKB = 1;
            string query = "SELECT MAX(CAST(maPKB AS INT)) AS MaxMaPKB FROM [PhieuKhamBenh]"; //lấy mã max hiện tại trả về max

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    try
                    {
                        con.Open();
                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read() && !reader.IsDBNull(0))
                        {
                            maPKB = Convert.ToInt32(reader["MaxMaPKB"]) + 1;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            return maPKB.ToString();
        }
        public bool them(phieukhambenhDTO pkb)
        {
            string query = string.Empty;
            query += "INSERT INTO [PhieuKhamBenh] ([NgayKham],[trieuChung],[maBenhNhan],[maTaiKhoan], [ngayTaiKham]) ";
            query += "VALUES (@NgayKham , @trieuChung, @maBenhNhan, @maTaiKhoan, @ngayTaiKham)";


            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {

                    cmd.Parameters.AddWithValue("@NgayKham", pkb.NgayKham);
                    cmd.Parameters.AddWithValue("@trieuChung", pkb.TrieuChung);
                    cmd.Parameters.AddWithValue("@maBenhNhan", pkb.MaBenhNhan);
                    cmd.Parameters.AddWithValue("@maTaiKhoan", pkb.MBS);
                    cmd.Parameters.AddWithValue("@ngayTaiKham", pkb.NgayTaiKham);

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
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
        public bool CapNhatDaGuiMail(string maPKB, bool daGui)
        {    //cập nhật khi đúng maPKB và trạng thái đã gửi mail
            string query = "UPDATE PhieuKhamBenh SET DaGuiMail = @daGui WHERE maPKB = @maPKB";
            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@daGui", daGui);
                cmd.Parameters.AddWithValue("@maPKB", maPKB);
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

        // Lấy danh sách phiếu khám bệnh theo ngày
        public List<phieukhambenhDTO> selectByDate(DateTime ngay)
        {
            string query = "SELECT * FROM PhieuKhamBenh WHERE CAST(NgayKham AS DATE) = @ngay";
            List<phieukhambenhDTO> ds = new List<phieukhambenhDTO>();

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@ngay", ngay.Date);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    phieukhambenhDTO pkb = new phieukhambenhDTO();
                    pkb.MaPKB = (reader["maPKB"].ToString());
                    pkb.TrieuChung = reader["TrieuChung"].ToString();
                    pkb.NgayKham = Convert.ToDateTime(reader["NgayKham"]);
                    pkb.MaBenhNhan = reader["maBenhNhan"].ToString();
                    pkb.MBS = (reader["maTaiKhoan"].ToString());
                    pkb.NgayTaiKham = Convert.ToDateTime(reader["NgayTaiKham"]);
                    pkb.DaGuiMail = Convert.ToBoolean(reader["DaGuiMail"]);
                    ds.Add(pkb);
                }
            }

            return ds;
        }

    }
}
