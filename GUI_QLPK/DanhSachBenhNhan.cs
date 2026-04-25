using QLPKBUS;
using QLPKDAL;
using QLPKDTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUI_QLPK
{
    public partial class DanhSachBenhNhan : Form
    {
        BenhNhanBUS bnBus = new BenhNhanBUS();
        PhieukhambenhBUS pkbBus = new PhieukhambenhBUS();
        BenhBUS beBus = new BenhBUS();
        ChandoanBUS cdBus = new ChandoanBUS();
        taiKhoanBUS tkBus = new taiKhoanBUS();
        private int stt;

        public DanhSachBenhNhan()
        {
            InitializeComponent();
            load_data();
            gird.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        public void load_data()
        {
            stt = 1;
            bnBus = new BenhNhanBUS();
            beBus = new BenhBUS();
            pkbBus = new PhieukhambenhBUS();
            cdBus = new ChandoanBUS();
            List<BenhNhanDTO> listBenhNhan = bnBus.select();
            List<benhDTO> listBenh = beBus.select();
            List<phieukhambenhDTO> listpkb = pkbBus.select();
            List<chandoanDTO> listcd = cdBus.select();
            List<taiKhoanDTO> listTK = tkBus.select();
            this.loadData_Vao_GridView(listBenhNhan, listBenh, listpkb, listcd, listTK);
        }

        private void loadData_Vao_GridView(List<BenhNhanDTO> listBenhNhan, List<benhDTO> listBenh, List<phieukhambenhDTO> listpkb, List<chandoanDTO> listcd, List<taiKhoanDTO> listTK)
        {

            if (listBenhNhan == null || listpkb == null || listBenh == null || listcd == null)
            {
                System.Windows.Forms.MessageBox.Show("Có lỗi khi lấy thông tin từ DB", "Result", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                return;

            }
            // Đảm bảo listpkb đã được sắp theo mã tăng dần
            listpkb.Sort((x, y) =>(x.MaPKB).CompareTo(y.MaPKB));
            DataTable table = new DataTable();
            table.Columns.Add("Số thứ tự", typeof(int));
            table.Columns.Add("Mã phiếu khám", typeof(string));
            table.Columns.Add("Tên bệnh nhân", typeof(string));
            table.Columns.Add("CCCD", typeof(string));
            table.Columns.Add("Ngày khám", typeof(string));
            table.Columns.Add("Ngày tái khám", typeof(string));
           
            table.Columns.Add("Bác sĩ khám", typeof(string));
            int stt = 1;

            var query = from pkb in listpkb
                        join bn in listBenhNhan on pkb.MaBenhNhan equals bn.MaBN
                        join cd in listcd on pkb.MaPKB equals cd.MaPkb
                        join be in listBenh on cd.MaBenh equals be.MaBenh
                        join tk in listTK on pkb.MBS equals tk.MaTK
                        select new { pkb, bn, be, tk };
            HashSet<string> dsMaPKB = new HashSet<string>();


            foreach (var item in query)
            {
                if (!dsMaPKB.Contains(item.pkb.MaPKB.ToString()))
                {
                    DataRow row = table.NewRow();
                    row["Số thứ tự"] = stt++;
                    row["Mã phiếu khám"] = item.pkb.MaPKB;
                    row["Tên bệnh nhân"] = item.bn.TenBN;
                    row["CCCD"] = item.bn.CanCuocCongDan;

                    // Ép kiểu an toàn (Sử dụng trực tiếp thuộc tính nếu là DateTime)
                    row["Ngày khám"] = item.pkb.NgayKham.ToString("dd/MM/yyyy");
                    row["Ngày tái khám"] = item.pkb.NgayTaiKham.ToString("dd/MM/yyyy");

                   
                    row["Bác sĩ khám"] = item.tk.Name;

                    table.Rows.Add(row);
                    dsMaPKB.Add(item.pkb.MaPKB.ToString());
                }
            }
            gird.DataSource = table.DefaultView;
        }
    }
}
