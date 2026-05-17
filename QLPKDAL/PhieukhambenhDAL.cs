using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using QLPKDTO;
using System.Data.SqlClient;
using System.Data;

namespace QLPKDAL
{
    public class PhieukhambenhDAL
    {
        private string connectionString;
        public PhieukhambenhDAL()
        {
            connectionString = ConfigurationManager.AppSettings["ConnectionString"];
        }
        public string ConnectionString { get => connectionString; set => connectionString = value; }

        public List<phieukhambenhDTO> select()
        {
            // Sử dụng View v_ChiTietPhieuKham
            string query = "SELECT * FROM v_ChiTietPhieuKham";
            List<phieukhambenhDTO> lspkb = new List<phieukhambenhDTO>();

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    try
                    {
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                phieukhambenhDTO pkb = new phieukhambenhDTO();
                                pkb.MaPKB = int.Parse(reader["maPKB"].ToString());
                                pkb.TrieuChung = reader["trieuChung"].ToString();
                                pkb.NgayKham = Convert.ToDateTime(reader["ngayKham"]);
                                pkb.MaBenhNhan = int.Parse(reader["maBenhNhan"].ToString());
                                pkb.MBS = int.Parse(reader["maTaiKhoan"].ToString());
                                pkb.NgayTaiKham = reader["ngayTaiKham"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["ngayTaiKham"]);
                                pkb.DaGuiMail = false; 
                                lspkb.Add(pkb);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Lỗi khi tải danh sách phiếu khám: " + ex.Message);
                    }
                }
            }
            return lspkb;
        }

        public List<phieukhambenhDTO> selectByKeyWord(string sKeyword)
        {
            // Sử dụng View v_ChiTietPhieuKham
            string query = "SELECT * FROM v_ChiTietPhieuKham WHERE maPKB LIKE @key OR tenBenhNhan LIKE @key";
            List<phieukhambenhDTO> lspkb = new List<phieukhambenhDTO>();

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@key", "%" + sKeyword + "%");

                    try
                    {
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                phieukhambenhDTO pkb = new phieukhambenhDTO();
                                pkb.MaPKB = int.Parse(reader["maPKB"].ToString());
                                pkb.TrieuChung = reader["trieuChung"].ToString();
                                pkb.NgayKham = Convert.ToDateTime(reader["ngayKham"]);
                                pkb.MaBenhNhan = int.Parse(reader["maBenhNhan"].ToString());
                                lspkb.Add(pkb);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Lỗi khi tìm kiếm phiếu khám: " + ex.Message);
                    }
                }
            }
            return lspkb;
        }



        public bool them(phieukhambenhDTO pkb)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                // Sử dụng Procedure sp_TaoPhieuKhamVaToaThuoc (Tạo cả PKB và ToaThuoc rỗng)
                using (SqlCommand cmd = new SqlCommand("sp_TaoPhieuKhamVaToaThuoc", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@trieuChung", pkb.TrieuChung);
                    cmd.Parameters.AddWithValue("@maBenhNhan", pkb.MaBenhNhan);
                    cmd.Parameters.AddWithValue("@maTaiKhoan", pkb.MBS);
                    cmd.Parameters.AddWithValue( "@ngayTaiKham",   pkb.NgayTaiKham ?? (object)DBNull.Value);

                    try
                    {
                        con.Open();
                        // ExecuteScalar để lấy NewPKBID được trả về từ Procedure
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            pkb.MaPKB = Convert.ToInt32(result);
                        }
                        return true;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Lỗi khi tạo phiếu khám và toa thuốc: " + ex.Message);
                    }
                }
            }
        }

        public bool CapNhatDaGuiMail(int maPKB, bool daGui)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                // Sử dụng Procedure sp_CapNhatDaGuiMail
                using (SqlCommand cmd = new SqlCommand("sp_CapNhatDaGuiMail", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@maPKB", maPKB);
                    cmd.Parameters.AddWithValue("@daGui", daGui);
                    try
                    {
                        con.Open();
                        return cmd.ExecuteNonQuery() > 0;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Lỗi khi cập nhật trạng thái gửi mail: " + ex.Message);
                    }
                }
            }
        }

        public List<phieukhambenhDTO> selectByDate(DateTime ngay)
        {
            string query = "SELECT * FROM v_ChiTietPhieuKham WHERE CAST(ngayKham AS DATE) = @ngay";
            List<phieukhambenhDTO> ds = new List<phieukhambenhDTO>();

            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@ngay", ngay.Date);
                try
                {
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            phieukhambenhDTO pkb = new phieukhambenhDTO();
                            pkb.MaPKB = int.Parse(reader["maPKB"].ToString());
                            pkb.TrieuChung = reader["trieuChung"].ToString();
                            pkb.NgayKham = Convert.ToDateTime(reader["ngayKham"]);
                            pkb.MaBenhNhan = int.Parse(reader["maBenhNhan"].ToString());
                            ds.Add(pkb);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Lỗi khi lấy phiếu khám theo ngày: " + ex.Message);
                }
            }
            return ds;
        }
    }
}
