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
    public class ToathuocDAL
    {
        private string connectionString;

        public ToathuocDAL()
        {
            connectionString = ConfigurationManager.AppSettings["ConnectionString"];
        }
        public string ConnectionString { get => connectionString; set => connectionString = value; }
        public bool them(toathuocDTO tt)
        {
            string query = @"INSERT INTO ToaThuoc
                     (ngayKeToa, maPKB)
                     VALUES
                     (@ngayKeToa, @maPKB)";

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ngayKeToa", tt.NgayKetoa);
                    cmd.Parameters.AddWithValue("@maPKB", tt.MaPkb);

                    try
                    {
                        con.Open();

                        int rows = cmd.ExecuteNonQuery();

                        return rows > 0;
                    }
                    catch (Exception ex)
                    {

                        throw new Exception(ex.Message);
                        return false;
                    }
                }
            }
        }
        public List<toathuocDTO> select()
        {
            string query = string.Empty;
            query += "SELECT * ";
            query += "FROM [ToaThuoc]";

            List<toathuocDTO> lsToaThuoc = new List<toathuocDTO>();

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
                                toathuocDTO tt = new toathuocDTO();
                                tt.MaToa = (reader["maToaThuoc"].ToString());
                                tt.NgayKetoa = Convert.ToDateTime(reader["ngayKeToa"]);
                                tt.MaPkb =( reader["maPKB"].ToString());

                                lsToaThuoc.Add(tt);
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
            return lsToaThuoc;
        }

        public string autogenerate_matoa()
        {
            int matoa = 1;
            string query = string.Empty;
            query += "SELECT MAX (KQ.MATOA) AS MM from (SELECT CONVERT(float, ToaThuoc.maToaThuoc) AS MATOA FROM ToaThuoc ) AS KQ";

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
                                matoa = int.Parse(reader["MM"].ToString()) + 1;
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
            return matoa.ToString();
        }
    }
}
