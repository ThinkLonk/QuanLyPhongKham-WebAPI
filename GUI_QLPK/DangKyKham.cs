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
    public partial class DangKyKham : Form
    {
        LichHenService lhBus = new LichHenService();
        BenhNhanService bnBus = new BenhNhanService();

        private int madd;
        private int stt;
        taiKhoanBUS tkBus = new taiKhoanBUS();
        loaiTaiKhoanBUS ltkBus = new loaiTaiKhoanBUS();
        lichHenDTO lh = new lichHenDTO();
        public DangKyKham(int mataikhoan)
        {
            madd = mataikhoan;
            InitializeComponent();
            load_combobox_mabn(); // load mã bệnh nhân
            load_combobox_bacsi(); // load bác sĩ
            load_combobox_giokham();
            ngaykham.Text = DateTime.Now.ToString();
            gird.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            load_Gird(); 
            load_data();
            
        }
        public void load_data()
        {
            malichhen.Text = "Tự động";
            hoten.Text = "";
            mabenhnhan.Text = "";
        }
        private void load_combobox_giokham()
        {
            giokham.Items.Clear();
            //các khung giờ nửa tiếng 08:00–17:00
            int hour;
            for (hour = 8; hour <= 17; hour++)
            {
                giokham.Items.Add(string.Format("{0:D2}:00", hour));
                giokham.Items.Add(string.Format("{0:D2}:30", hour));
            }
            giokham.SelectedIndex = -1;
        }

        //load dữ liệu bệnh nhân
        public void load_combobox_mabn()
        {
            bnBus = new BenhNhanService();
            List<BenhNhanDTO> listBenhNhan = bnBus.select();
            this.loadData_Vao_comboboxbn(listBenhNhan);
        }
        // load dữ liệu mã bệnh nhân vào combobox
        private void loadData_Vao_comboboxbn(List<BenhNhanDTO> listBenhNhan)
        {
            if (listBenhNhan == null)
            {
                System.Windows.Forms.MessageBox.Show("Có lỗi khi lấy thông tin bệnh nhân từ DB", "Result", System.Windows.Forms.MessageBoxButtons.OKCancel, System.Windows.Forms.MessageBoxIcon.Error);
                return;
            }
            foreach (BenhNhanDTO bn in listBenhNhan)
            {
                mabenhnhan.Items.Add(bn.MaBN.ToString());
            }
        }
        //load dữ liệu bác sĩ
        public void load_combobox_bacsi()
        {
            List<loaiTaiKhoanDTO> listTaiKhoan = ltkBus.select();
            loaiTaiKhoanDTO bacSiRole = null;
            if (listTaiKhoan == null)
            {
                System.Windows.Forms.MessageBox.Show("Có lỗi khi lấy thông tin bác sĩ từ DB", "Result", System.Windows.Forms.MessageBoxButtons.OKCancel, System.Windows.Forms.MessageBoxIcon.Error);
                return;
            }
            foreach (loaiTaiKhoanDTO role in listTaiKhoan)
            {
                if (role.TenLoaiTaiKhoan.Equals("Bác sĩ", StringComparison.OrdinalIgnoreCase))
                {
                    bacSiRole = role;
                    break;
                }
            }
            List<taiKhoanDTO> allUsers = tkBus.select();
            List<taiKhoanDTO> listBacSi = new List<taiKhoanDTO>(); //lọc ra danh sách bác sĩ
            foreach (taiKhoanDTO user in allUsers)
            {
                if (user.MaLoai == bacSiRole.MaRole)
                {
                    listBacSi.Add(user);
                }
            }
            loadData_Vao_comboboxtenbs(listBacSi);

        }
        private void loadData_Vao_comboboxtenbs(List<taiKhoanDTO> listBacSi)
        {
            bacsi.DataSource = listBacSi;
            bacsi.ValueMember = "MaTK";  
            bacsi.DisplayMember = "Name";   
            bacsi.SelectedIndex = -1;      
        }
        private void load_ten(List<BenhNhanDTO> listBenhNhan, int mabn)
        {
            if (listBenhNhan == null)
            {
                System.Windows.Forms.MessageBox.Show("Có lỗi khi lấy thông tin từ DB", "Result", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                return;
            }
            foreach (BenhNhanDTO bn in listBenhNhan)
            {
                if (bn.MaBN == mabn)
                {
                    hoten.Text = bn.TenBN;

                }
            }
        }
        //thay đổi lựa chọn cho mã bệnh nhân
        private void mabenhnhan_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mabenhnhan.SelectedIndex < 0) return;
            //lấy mã đã được chọn
            int selectedMaBN = int.Parse(mabenhnhan.SelectedItem.ToString());
            //lấy danh sách bệnh nhân
            List<BenhNhanDTO> listBenhNhan = bnBus.select();
            load_ten(listBenhNhan, selectedMaBN);
        }

        private void btn_dangky_Click(object sender, EventArgs e)
        {
            
            if (string.IsNullOrEmpty(mabenhnhan.Text) || bacsi.SelectedIndex < 0)
{
                MessageBox.Show("Vui lòng điền đủ thông tin", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                
                return;
            }
            DateTime ngay = ngaykham.Value.Date;
            TimeSpan gio; //pare từ combobox sang time
            if (!TimeSpan.TryParse(giokham.SelectedItem.ToString(), out gio))
            {
                MessageBox.Show("Giờ khám không hợp lệ.", "Thông báo",MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DateTime lichhen = ngay + gio;
            if (lichhen < DateTime.Now)
            {
                MessageBox.Show("Bạn đã chọn ngày/giờ trong quá khứ. Vui lòng chọn lại!",
                                "Ngày hẹn không hợp lệ",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }
            //Kiểm tra trùng lịch theo bác sĩ + ngày giờ
            List<lichHenDTO> dsTrongNgay = lhBus.selectByDate(ngay); //lịch hẹn trong ngày
            int maBacSiChon = int.Parse(bacsi.SelectedValue.ToString());
            foreach (lichHenDTO x in dsTrongNgay)
            {
                if (x.MaTaiKhoan == maBacSiChon && x.NgayHen.TimeOfDay == gio )
                {
                    MessageBox.Show("Khung giờ này bác sĩ đã có lịch. Vui lòng chọn giờ khác.",
                                    "Trùng lịch",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                    return;
                }
            }
            lichHenDTO lh = new lichHenDTO();
            lh.MaDieuDuong = madd;
            lh.MaBenhNhan = int.Parse(mabenhnhan.Text);
            lh.MaTaiKhoan = int.Parse(bacsi.SelectedValue.ToString());
            lh.NgayHen = lichhen;
            lh.TrangThai = "Chờ khám";
            bool kq = lhBus.them(lh);
            if (kq == true)
            {
                System.Windows.Forms.MessageBox.Show("Đăng ký lịch hẹn thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                load_data();
                load_Gird();
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Đăng ký lịch hẹn thất bại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

       
    
        private void load_Gird()
        {
            List<lichHenDTO> listLichHen = lhBus.select();
            List<taiKhoanDTO> listTaiKhoan = tkBus.select();
            int stt = 1;
            DataTable table = new DataTable();
            table.Columns.Add("Số thứ tự", typeof(int));
            table.Columns.Add("Mã lịch hẹn", typeof(int));
            table.Columns.Add("Mã bệnh nhân", typeof(int));
            table.Columns.Add("Tên bác sĩ", typeof(string));
            table.Columns.Add("Tên điều dưỡng đăng kí", typeof(string));
            table.Columns.Add("Ngày hẹn", typeof(string));
            table.Columns.Add("Trạng thái", typeof(string));



            foreach (lichHenDTO lh in listLichHen)
            {

                var taiKhoan = listTaiKhoan
           .FirstOrDefault(x => x.MaTK== lh.MaTaiKhoan);

                // tìm tên điều dưỡng
                var dieuDuong = listTaiKhoan
                    .FirstOrDefault(x => x.MaTK == lh.MaDieuDuong);
                DataRow row = table.NewRow();

                row["Số thứ tự"] = stt;
                row["Mã lịch hẹn"] = lh.MaLichHen;
                row["Mã bệnh nhân"] = lh.MaBenhNhan;
                row["Tên bác sĩ"] = taiKhoan != null ? taiKhoan.Name : "";
                row["Tên điều dưỡng đăng kí"] = dieuDuong != null ? dieuDuong.Name : "";
                row["Ngày hẹn"] = lh.NgayHen.ToString("yyyy-MM-dd");
                row["Trạng thái"] = lh.TrangThai;

                table.Rows.Add(row);
                stt++;
            }

            gird.DataSource =  table.DefaultView;
            gird.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
        private void gird_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < gird.Rows.Count)
            {
                DataGridViewRow row = gird.Rows[e.RowIndex];
                mabenhnhan.Text = row.Cells[2].Value.ToString();
                hoten.Text = row.Cells[3].Value.ToString();
                malichhen.Text = row.Cells[1].Value.ToString();
                bacsi.Text = row.Cells[4].Value.ToString();
                string ngay = row.Cells[5].Value.ToString();    // "dd/MM/yyyy"
                string gio = row.Cells[6].Value.ToString();     // "HH:mm"
                if (DateTime.TryParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime dt))
                {
                    ngaykham.Value = dt;
                }
                else
                {
                    MessageBox.Show("Invalid date or time format.");
                }
                // Chọn giờ đúng trong combobox
                int idx = giokham.FindStringExact(gio); //tìm
                giokham.SelectedIndex = idx; // = -1 nếu không có

            }
        }

        private void btn_xoa_Click(object sender, EventArgs e)
        {
            lh.MaLichHen = int.Parse(malichhen.Text);
            bool kq = lhBus.xoa(lh);
            if (!kq)
            {
                MessageBox.Show("Xóa lịch hẹn thất bại. Vui lòng kiểm tra lại dữ liệu.","Thông báo",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            }
            else
            {
                MessageBox.Show("Xóa lịch hẹn thành công.","Thông báo",MessageBoxButtons.OK,MessageBoxIcon.Information);
                load_data();
                load_Gird();
            }
        }
    }
}
