using GUI_QLPK;
using GUI_QLPM;
using QLPKBUS;
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
    public partial class QLPMMain : Form
    {
        public string mataikhoan;
        public string ma;
        taiKhoanBUS tkBUS = new taiKhoanBUS();
        public taiKhoanDTO tk = new taiKhoanDTO();
        loaiTaiKhoanBUS loaitkBUS = new loaiTaiKhoanBUS();
        loaiTaiKhoanDTO loaitk = new loaiTaiKhoanDTO();

        public QLPMMain(string mataikhoanLogin)
        {
            
            mataikhoan = mataikhoanLogin;
            List<taiKhoanDTO> listTk = tkBUS.select();
            List<loaiTaiKhoanDTO> listLoaiTk = loaitkBUS.select();
            InitializeComponent();
            foreach(taiKhoanDTO taiKhoan in listTk)
            {
                
                if (taiKhoan.MaTK == mataikhoanLogin)
                {
                    tentaikhoandangnhat.Text = taiKhoan.Name;
                    foreach(loaiTaiKhoanDTO loaiTaiKhoan in listLoaiTk)
                    {
                        if(loaiTaiKhoan.MaRole == taiKhoan.MaLoai)
                        {
                            txtChucvu.Text = loaiTaiKhoan.TenLoaiTaiKhoan;
                        }
                    }
                    ma = taiKhoan.MaLoai;
                    if (taiKhoan.MaLoai == "1")
                    {
                        btnDanhMuc.Visible = false;
                        btnHoaDon.Visible = false;
                        btn_BaoCao.Visible = false;
                        lichkham.Visible = false;
                    }
                    else
                    {
                        if (taiKhoan.MaLoai == "2")
                        {
                            phieukham.Visible = false;
                            btnDanhMuc.Visible = false;
                            btn_BaoCao.Visible = false;
                            lichkham.Visible = false;
                            dsLichkham.Visible = false;
                        }
                        else if (taiKhoan.MaLoai == "3")
                        {
                            phieukham.Visible = false;
                            btnHoaDon.Visible = false;
                            btn_BaoCao.Visible = false;
                            btn_qlAcc.Visible = false;
                            btn_qlThuoc.Visible = false;
                            btn_qlLoaiBenh.Visible = false;
                            btn_qlDV.Visible = false;
                            //dsLichkham.Visible = false;
                        }
                        else
                        {
                            phieukham.Visible = false;
                            dangkykham.Enabled = false ;
                            btnLHD.Enabled = false ;
                        }
                    }
                }
            }
        }
        private void QLPMMain_Load(object sender, EventArgs e)
        {
            guna2ShadowForm1.SetShadowForm(this);
            label_Val.Text = "Home";
            container(new Home());
            customSubMenu();
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            label_Val.Text = "Home";
            container(new Home());

        }

        //hàm container để hiển thị form con vào panel
        private void container(object _form)
        {
            if (guna2Panel_container.Controls.Count > 0) guna2Panel_container.Controls.Clear();
            Form fm = _form as Form;
            fm.TopLevel = false;
            fm.FormBorderStyle = FormBorderStyle.None;
            fm.Dock = DockStyle.Fill;
            guna2Panel_container.Controls.Add(fm);
            guna2Panel_container.Tag = fm;
            fm.Show();
        }
        private void btnTraCuu_Click(object sender, EventArgs e)
        {
            label_Val.Text = "Tra cứu bệnh nhân";
            container(new TraCuuBenhNhan());
            
        }

        private void customSubMenu()
        {
            PansubMenuHoaDon.Visible = false;
            subMenuDanhMuc.Visible = false;
            PanSubBaoCao.Visible = false;
            subLichKham.Visible = false;
        }
        private void hideSubMenu()
        {
            if (PansubMenuHoaDon.Visible == true)
            {
                PansubMenuHoaDon.Visible = false;
            }
            if (subMenuDanhMuc.Visible == true)
            {
                subMenuDanhMuc.Visible = false;
            }
            if (PanSubBaoCao.Visible == true)
            {
                PanSubBaoCao.Visible = false;
            }
            if (subLichKham.Visible == true)
            {
                subLichKham.Visible = false;
            }
        }
        private void showSubMenu(Panel subMenu)
        {
            if (subMenu.Visible == false)
            {
                hideSubMenu();
                subMenu.Visible = true;
            }
            else
            {
                subMenu.Visible = false;
            }
        }
        //nút chính
        private void btnHoaDon_Click(object sender, EventArgs e)
        {
            label_Val.Text = "Hóa Dơn";
            showSubMenu(PansubMenuHoaDon);
        }

        private void btnDanhMuc_Click(object sender, EventArgs e)
        {
            label_Val.Text = "Danh Mục";
            showSubMenu(subMenuDanhMuc);
        }

        //nút phụ 
        private void btnLHD_Click(object sender, EventArgs e)
        {
            hideSubMenu();
            label_Val.Text = "Lập hóa đơn";
            container(new LapHoaDon(mataikhoan));

        }

        private void btnDSHD_Click(object sender, EventArgs e)
        {
            hideSubMenu();
            label_Val.Text = "Danh sách hóa đơn";
            container(new DanhSachHoaDon());
        }

        private void btn_qlThuoc_Click(object sender, EventArgs e)
        {
            hideSubMenu();
            label_Val.Text = "Quản lý thuốc";
            container(new QuanLyThuoc());
        }

        private void btn_qlLoaiBenh_Click(object sender, EventArgs e)
        {
            hideSubMenu();
            label_Val.Text = "Quản lý loại bệnh";
            container(new QuanLyLoaiBenh());
        }

        private void btn_qlBN_Click(object sender, EventArgs e)
        {
            hideSubMenu();
            label_Val.Text = "Quản lý bệnh nhân";
            container(new QuanLyBenhNhan(ma));
        }

        private void btn_qlDV_Click(object sender, EventArgs e)
        {
            hideSubMenu();
            label_Val.Text = "Quản lý dịch vụ";
            container(new QuanLyDichVu());
        }

        private void btn_qlAcc_Click(object sender, EventArgs e)
        {
            hideSubMenu();
            label_Val.Text = "Quản lý tài khoản";
            container(new QuanLyTaiKhoan());
        }
        //Nút chính
        private void btn_BaoCao_Click(object sender, EventArgs e)
        {
            label_Val.Text = "Báo Cáo";
            showSubMenu(PanSubBaoCao);
        }

        private void btn_DThu_Click(object sender, EventArgs e)
        {
            hideSubMenu();
            container(new BaoCaoDoanhThu());
        }

        private void btn_sdThuoc_Click(object sender, EventArgs e)
        {
            hideSubMenu();
            container(new BaoCaoSuDungThuoc());
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            label_Val.Text = "Phiếu khám bệnh";
            container(new ThemPhieuKhamBenh(mataikhoan));
        }

        private void btn_out_Click(object sender, EventArgs e)
        {
            this.Close();
            Login lg = new Login();
            lg.Show();
        }

        private void btnDSBenhNhan_Click(object sender, EventArgs e)
        {
            label_Val.Text = "Danh sách bệnh nhân";
            container(new DanhSachBenhNhan());
        }

        private void dangkykham_Click(object sender, EventArgs e)
        {
            hideSubMenu();
            label_Val.Text = "Đăng ký khám bệnh";
            container(new DangKyKham(mataikhoan));
        }

        private void dsLichkham_Click(object sender, EventArgs e)
        {
            hideSubMenu();
            label_Val.Text = "Danh sách lịch khám";
            container(new DSLaySoKham());
        }
        private void lichkham_Click(object sender, EventArgs e)
        {
            label_Val.Text = "Lịch khám bệnh";
            showSubMenu(subLichKham);
        }

        private void qlNhacLich_Click(object sender, EventArgs e)
        {
            hideSubMenu();
            container(new QuanLyNhacHen());
        }
    }
}
