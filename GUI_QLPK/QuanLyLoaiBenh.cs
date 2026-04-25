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
using System.Windows.Forms.DataVisualization.Charting;

namespace GUI_QLPK
{
    public partial class QuanLyLoaiBenh : Form
    {
        public DataTable db1 = new DataTable("tblBENH");
        BenhBUS beBus = new BenhBUS();
        benhDTO be = new benhDTO();

        private string temp;
        public QuanLyLoaiBenh()
        {
            InitializeComponent();
            load_data();
            gird.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            tenBenh.Text = " ";
        }

        private void load()
        {
            db1.Clear();
        }
        public void load_data()
        {
            List<benhDTO> listBenh = beBus.select();
            this.loadData_Vao_GridView(listBenh);
            tenBenh.Text = " ";
            maBenh.Text = beBus.autogenerate_mabenh().ToString();

        }
        private void loadData_Vao_GridView(List<benhDTO> listBenh)
        {

            if (listBenh == null)
            {
                System.Windows.Forms.MessageBox.Show("Có lỗi khi lấy thông tin từ DB", "Result", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                return;
            }
            DataTable table = new DataTable();
            table.Columns.Add("Mã bệnh", typeof(string));
            table.Columns.Add("Tên Bệnh", typeof(string));
            foreach (benhDTO be in listBenh)
            {
                DataRow row = table.NewRow();
                row["Mã bệnh"] = be.MaBenh;
                row["Tên bệnh"] = be.TenBenh;

                table.Rows.Add(row);
            }
            gird.DataSource = table.DefaultView;
        }

        private void TimKiem_Click(object sender, EventArgs e)
        {
            string sKeyword = key.Text;
            if (sKeyword == null || sKeyword == string.Empty || sKeyword.Length == 0) // tìm tất cả
            {
                List<benhDTO> listBenh = beBus.select();
                this.loadData_Vao_GridView(listBenh);
            }
            else
            {
                List<benhDTO> listBenh = beBus.selectByKeyWord(sKeyword);
                this.loadData_Vao_GridView(listBenh);
            }
        }

        private void Them_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(maBenh.Text) || string.IsNullOrEmpty(tenBenh.Text))
            {
                System.Windows.Forms.MessageBox.Show("Vui lòng điền đầy đủ thông tin", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                // Kiểm tra trùng tên bệnh
                if (beBus.kiemTraTrungTen(tenBenh.Text.Trim()))
                {
                    System.Windows.Forms.MessageBox.Show("Tên bệnh đã tồn tại. Vui lòng nhập lại", "Result", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                    tenBenh.Focus();
                    return;
                }
                benhDTO be = new benhDTO();
                be.TenBenh = tenBenh.Text;

                bool kq = beBus.them(be);
                if (!kq)
                    System.Windows.Forms.MessageBox.Show("Thêm bệnh thất bại. Vui lòng kiểm tra lại dữ liệu", "Result", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                else
                {
                    System.Windows.Forms.MessageBox.Show("Thêm bệnh thành công", "Result");
                    load_data();
                    load();
                }
            }
        }

        private void Sua_Click(object sender, EventArgs e)
        {
            be.MaBenh = (maBenh.Text);
            be.TenBenh = tenBenh.Text;

            bool kq = beBus.sua(be, temp);
            if (!kq)
                System.Windows.Forms.MessageBox.Show("Cập nhật loại bệnh thất bại. Vui lòng kiểm tra lại dữ liệu", "Result", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            else
            {
                System.Windows.Forms.MessageBox.Show("Cập nhật loại bệnh thành công", "Result");
                load_data();
                load();
            }
        }

        private void Xoa_Click(object sender, EventArgs e)
        {
            be.MaBenh = (temp);
            beBus = new BenhBUS();
            bool kq = beBus.xoa(be);
            if (kq == false)
                System.Windows.Forms.MessageBox.Show("Xóa loại bệnh thất bại. Vui lòng kiểm tra lại dữ liệu", "Result", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

            else
                System.Windows.Forms.MessageBox.Show("Xóa loại bệnh thành công", "Result");
            load_data();
            load();
        }

        private void HoanTac_Click(object sender, EventArgs e)
        {
            key.Text = string.Empty;
            //maBenh.Text = string.Empty;
            tenBenh.Text = string.Empty;
        }

        private void grid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < gird.Rows.Count)
            {
                DataGridViewRow row = gird.Rows[e.RowIndex];
                maBenh.Text = row.Cells[0].Value.ToString();
                tenBenh.Text = row.Cells[1].Value.ToString();
                temp = row.Cells[0].Value.ToString();
            }
        }
    }
}
