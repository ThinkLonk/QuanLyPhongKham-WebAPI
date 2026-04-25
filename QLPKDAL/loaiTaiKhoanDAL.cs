using QLPKDTO;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLPKDAL
{
    public class loaiTaiKhoanDAL
    {
        private string connectionString;

        public loaiTaiKhoanDAL()
        {
            connectionString = System.Configuration.ConfigurationManager.AppSettings["ConnectionString"];
        }
        public string ConnectionString
        {
            get => connectionString;
            set => connectionString = value;
        }
        public List<loaiTaiKhoanDTO> select()
        {
            string query = string.Empty;
            query += "SELECT * ";
            query += "FROM [Roles]";

            List<loaiTaiKhoanDTO> lsloaitk = new List<loaiTaiKhoanDTO>();

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
                                loaiTaiKhoanDTO loaitk = new loaiTaiKhoanDTO();
                                loaitk.TenLoaiTaiKhoan = reader["tenRole"].ToString();
                                loaitk.MaRole =(reader["maRole"].ToString());

                                lsloaitk.Add(loaitk);
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
            return lsloaitk;
        }
    }
}
