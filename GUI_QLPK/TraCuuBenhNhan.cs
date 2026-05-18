using QLPKBUS;
using QLPKDAL;
using QLPKDTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUI_QLPK
{
    public partial class TraCuuBenhNhan : Form
    {
        //public DataTable db1 = new DataTable("BenhNhan");
        BenhNhanService bnBus = new BenhNhanService();
        BenhNhanDTO bn = new BenhNhanDTO();
        PhieukhambenhBUS pkbBUS = new PhieukhambenhBUS();
        ChandoanBUS cdBUS = new ChandoanBUS();
        BenhBUS benhBUS = new BenhBUS();
        private string temp_ma;
        public TraCuuBenhNhan()
        {
            InitializeComponent();
            load_data();
            gird.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
        //public void load()
        //{
        //    db1.Clear();
        //    db1.Columns.Add("MaHD", typeof(System.Int32));
        //}
        public void load_data()
        {
            bnBus = new BenhNhanService();
            List<BenhNhanDTO> listBenhNhan = bnBus.select();
            List<phieukhambenhDTO> listPhieuKham = pkbBUS.select();
            List<chandoanDTO> listChandoan = cdBUS.select();
            List<benhDTO> listBenh = benhBUS.select();
            this.loadData_Vao_GridView(listBenhNhan, listPhieuKham, listChandoan, listBenh);
        }
        //load dữ liệu vào girdview
        private void loadData_Vao_GridView(List<BenhNhanDTO> listBenhNhan, List<phieukhambenhDTO> listPhieuKham, List<chandoanDTO> listChuanDoan, List<benhDTO> listBenh)
        {

            if (listBenhNhan == null)
            {
                System.Windows.Forms.MessageBox.Show("Có lỗi khi lấy thông tin từ DB", "Result", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                return;

            }

            DataTable table = new DataTable();
            table.Columns.Add("Mã bệnh nhân", typeof(int));
            table.Columns.Add("Tên bệnh nhân", typeof(string));
            table.Columns.Add("Ngày sinh", typeof(string));
            table.Columns.Add("Địa chỉ", typeof(string));
            table.Columns.Add("Giới tính", typeof(string));
            table.Columns.Add("CCCD", typeof(string));  
            table.Columns.Add("TrieuChung", typeof(string));
            table.Columns.Add("Tên bệnh", typeof(string));


            foreach (BenhNhanDTO bn in listBenhNhan)
            {
                DataRow row = table.NewRow();
                row["Mã bệnh nhân"] = bn.MaBN;
                row["Tên bệnh nhân"] = bn.TenBN;
                row["Ngày sinh"] = DateTime.Parse(bn.NgsinhBN.ToString()).ToString("dd/MM/yyyy");
                row["Địa chỉ"] = bn.DiachiBN;
                row["Giới tính"] = bn.GtBN;
                row["CCCD"] = bn.CanCuocCongDan;
                // Tìm phiếu khám bệnh ứng với bệnh nhân hiện tại
                phieukhambenhDTO phieu = null;
                foreach (phieukhambenhDTO p in listPhieuKham)
                {
                    if (p.MaBenhNhan == bn.MaBN)
                    {
                        phieu = p;
                        break;
                    }
                }
                // Tìm chẩn đoán theo mã PKB (nếu có phiếu khám)
                chandoanDTO chuanDoan = null;
                if (phieu != null)
                {
                    foreach (chandoanDTO cd in listChuanDoan)
                    {
                        if (cd.MaPkb == phieu.MaPKB)
                        {
                            chuanDoan = cd;
                            break;
                        }
                    }
                }
                // Tìm tên bệnh từ mã bệnh (nếu có chẩn đoán)
                if (chuanDoan != null)
                {
                    foreach (benhDTO b in listBenh)
                    {
                        if (b.MaBenh == chuanDoan.MaBenh)
                        {
                            row["Tên bệnh"] = b.TenBenh;
                            break;
                        }
                    }
                }
                if (row["Tên bệnh"] == null || row["Tên bệnh"].ToString() == "")
                {
                    row["Tên bệnh"] = "Chưa có";
                }
                if (phieu != null)
                {
                    row["TrieuChung"] = phieu.TrieuChung;
                }
                else
                {
                    row["TrieuChung"] = "Chưa có";
                }
                table.Rows.Add(row);
            }
            gird.DataSource = table.DefaultView;
        }

        private void timkiem_Click(object sender, EventArgs e)
        {
            bnBus = new BenhNhanService();
            string sKeyword = nhaptukhoa.Text;
            List<phieukhambenhDTO> listPhieuKham = pkbBUS.select();
            List<chandoanDTO> listChuanDoan = cdBUS.select();
            List<benhDTO> listBenh = benhBUS.select();
            if (sKeyword == null || sKeyword == string.Empty || sKeyword.Length == 0) // tìm tất cả
            {
                List<BenhNhanDTO> listBenhNhan = bnBus.select();

                this.loadData_Vao_GridView(listBenhNhan, listPhieuKham, listChuanDoan, listBenh);
            }
            else
            {
                List<BenhNhanDTO> listBenhNhan = bnBus.selectByKeyWord(sKeyword);
                this.loadData_Vao_GridView(listBenhNhan, listPhieuKham, listChuanDoan,listBenh);
            }
        }
    }
}
