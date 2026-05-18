# Hệ thống Quản lý Phòng khám (QLPK)

> Đồ án môn học **Lập trình cơ sở dữ liệu** – Trường Đại học Mở TP.HCM.
> Vận dụng kiến thức tổng hợp: **T-SQL**, **ADO.NET**, **LINQ**, **Entity Framework Core 8**, **ASP.NET Core 8 Web API**.

[![.NET](https://img.shields.io/badge/.NET-8.0-blueviolet)]()
[![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.7.2-blue)]()
[![SQL Server](https://img.shields.io/badge/SQL%20Server-2019%2B-red)]()
[![License](https://img.shields.io/badge/License-Academic-green)]()

**Repository (Public):** <https://github.com/ThinkLonk/QuanLyPhongKham-WebAPI>

---

## 1. Giới thiệu

Phần mềm hỗ trợ tin học hoá toàn bộ quy trình của một phòng khám đa khoa: tiếp nhận bệnh nhân, đặt và theo dõi lịch hẹn, khám bệnh, kê toa thuốc, lập hoá đơn thanh toán, và lập báo cáo doanh thu / sử dụng thuốc.

Hệ thống được tổ chức theo **kiến trúc 3 lớp + DTO** và cung cấp song song hai cách truy cập dữ liệu:

1. **WinForms client** (ADO.NET + Stored Procedure / View) – sản phẩm chính, có thêm tầng `Services` gọi Web API qua `HttpClient` (fallback BUS/DAL khi API ngoại tuyến).
2. **Web API** (Entity Framework Core 8 + ASP.NET Core 8) – sẵn sàng cho client web/mobile.

---

## 2. Phiên bản phần mềm sử dụng

| Thành phần | Phiên bản | Ghi chú |
|---|---|---|
| **Hệ điều hành** | Windows 10/11 64-bit | Đã thử nghiệm trên Win 11 |
| **Visual Studio** | 2022 (17.11) | Có thể dùng VS 2019 trở lên |
| **.NET Framework** | 4.7.2 | Cho `GUI_QLPK`, `QLPKBUS`, `QLPKDAL`, `QLPKDTO` |
| **.NET SDK** | 8.0 (LTS) | Cho `QLPKEF`, `QLPKWebAPI` |
| **C# Language** | 12 | Hỗ trợ implicit usings, nullable references, collection expressions |
| **SQL Server** | 2019 / 2022 (Developer Edition) | Đã chạy thử trên LocalDB |
| **SQL Server Management Studio (SSMS)** | 19 | Quản trị CSDL |
| **Entity Framework Core** | 8.0.10 | `Microsoft.EntityFrameworkCore.SqlServer` |
| **Swashbuckle.AspNetCore** | 6.5.0 | Swagger UI |
| **Guna.UI2.WinForms** | 2.0.4.6 | Thư viện UI hiện đại |
| **iText7** | 9.2.0 | Xuất hoá đơn PDF |
| **BouncyCastle.Cryptography** | 2.4.0 | Phụ thuộc của iText7 |
| **Newtonsoft.Json** | 13.0.1 | Phục vụ tương tác JSON |
| **Microsoft.Extensions.Logging** | 5.0.0 | Logging |

---

## 3. Cấu trúc dự án

```
QuanLyPhongKham/
├── QuanLyPhongKham.sln               # Solution chung (6 project)
├── script.sql                        # Toàn bộ CSDL: tables, view, function, SP, trigger
├── PK.bak                            # Backup database mẫu
├── README.md                         # File này
│
├── GUI_QLPK/                         # [Presentation Layer] – WinForms + Guna.UI2
│   ├── Login.cs, QLPMMain.cs, Home.cs
│   ├── QuanLyBenhNhan.cs, KeToa.cs, LapHoaDon.cs
│   ├── BaoCaoDoanhThu.cs, BaoCaoSuDungThuoc.cs
│   ├── Services/                     # API client (gọi Web API + fallback BUS/DAL)
│   │   ├── ApiClient.cs              # HttpClient + Newtonsoft.Json
│   │   ├── ApiContracts.cs           # DTO trao đổi với Web API
│   │   ├── AuthService.cs, BenhNhanService.cs
│   │   ├── HoaDonService.cs, LichHenService.cs
│   │   └── ThuocService.cs
│   ├── App.config                    # ConnectionString + ApiBaseUrl + ApiEnabled
│   └── packages.config               # Phụ thuộc NuGet
│
├── QLPKBUS/                          # [Business Logic Layer]
│   └── *BUS.cs                       # BenhNhanBUS, HoadonBUS, …
│
├── QLPKDAL/                          # [Data Access Layer] – ADO.NET
│   └── *DAL.cs                       # SqlConnection / SqlCommand / SqlDataReader
│
├── QLPKDTO/                          # [Data Transfer Object]
│   └── *DTO.cs                       # POCO truyền dữ liệu giữa các lớp
│
├── QLPKEF/                           # [Entity Framework Core 8]
│   ├── Models/                       # 13 entity class
│   ├── Context/QLPKDbContext.cs      # DbContext + DbSet + Fluent API
│   ├── Repositories/                 # LINQ to Entities
│   └── Samples/LinqToXmlExamples.cs  # LINQ to XML
│
├── QLPKWebAPI/                       # [ASP.NET Core 8 Web API – RESTful JSON]
│   ├── Program.cs                    # Cấu hình DI + Swagger + CORS
│   ├── appsettings.json              # ConnectionStrings:Default
│   ├── Properties/launchSettings.json
│   └── Controllers/                  # BenhNhan / HoaDon / Thuoc / LichHen / Auth
│
└── BaoCao/                           # Báo cáo LaTeX + PDF
    ├── BaoCao_QLPK.tex
    └── BaoCao_QLPK.pdf
```

---

## 4. Hướng dẫn cài đặt

### 4.1. Phục hồi cơ sở dữ liệu

Có hai cách:

**Cách 1 – Từ script:** Mở SSMS, kết nối tới SQL Server, mở `script.sql` và chạy.

```sql
-- Tạo CSDL trống
CREATE DATABASE QLPK;
GO
USE QLPK;
GO
-- Chạy toàn bộ nội dung file script.sql
```

**Cách 2 – Restore từ backup:**

```sql
RESTORE DATABASE QLPK FROM DISK = N'D:\QuanLyPhongKham\PK.bak'
WITH MOVE 'QLPK'     TO 'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\QLPK.mdf',
     MOVE 'QLPK_log' TO 'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\QLPK_log.ldf';
```

### 4.2. Chạy ứng dụng WinForms

1. Mở `QuanLyPhongKham.sln` bằng Visual Studio 2022.
2. Sửa chuỗi kết nối trong `GUI_QLPK/App.config` nếu cần.
3. Đặt **GUI_QLPK** làm Startup Project.
4. Build → Run (F5).

```xml
<appSettings>
  <add key="ConnectionString"
       value="Data Source=localhost;Initial Catalog=QLPK;Integrated Security=True"/>
  <!-- Tuỳ chọn: bật/tắt việc gọi Web API qua thư mục Services -->
  <add key="ApiBaseUrl" value="http://localhost:5000/"/>
  <add key="ApiEnabled" value="true"/>
</appSettings>
```

> WinForms client ưu tiên gọi Web API thông qua `GUI_QLPK/Services/*Service.cs`; nếu Web API tắt hoặc lỗi mạng, các service tự động *fallback* về tầng BUS/DAL truyền thống nên ứng dụng vẫn chạy được mà không cần Web API.

### 4.3. Chạy Web API

```bash
cd QLPKWebAPI
dotnet restore
dotnet run
```

Mở Swagger UI tại: <https://localhost:7000/swagger>

### 4.4. Sử dụng Entity Framework

```bash
cd QLPKEF
# Code First – tạo migration:
dotnet ef migrations add InitialCreate
dotnet ef database update

# Database First – scaffold lại entity:
dotnet ef dbcontext scaffold \
  "Server=localhost;Database=QLPK;Trusted_Connection=True;TrustServerCertificate=True" \
  Microsoft.EntityFrameworkCore.SqlServer \
  --output-dir Models --context QLPKDbContext --force
```

---

## 5. Tài khoản mẫu

| Username | Password | Vai trò |
|---|---|---|
| `admin` | `123` | Quản trị |
| `bacsi1` | `123` | Bác sĩ |
| `bacsi2` | `123` | Bác sĩ |
| `thungan1` | `123` | Thu ngân |
| `dieuduong1` | `123` | Điều dưỡng |

> Mật khẩu hiện lưu plaintext, **dự kiến cập nhật hash BCrypt** trong giai đoạn 2.

---

## 6. Các kỹ thuật đã triển khai

### 6.1. Lập trình CSDL (T-SQL)

- **13 View**: `v_DanhSachBenhNhan`, `v_DanhSachThuoc`, `v_DanhSachLichHen`, `v_ChiTietPhieuKham`, `v_DanhSachHoaDon`, …
- **2 Function**:
  - `f_TinhTienThuoc(@maPKB)` – Scalar Function.
  - `f_LichSuKhamBenh(@maBenhNhan)` – Table-Valued Function.
- **25 Stored Procedure**: CRUD cho từng entity + nghiệp vụ tổng hợp.
- **4 Trigger**:
  - `trg_CapNhatSoLuongThuocTrongKho` – kiểm tra & trừ kho khi kê thuốc.
  - `trg_HoanTraSoLuongThuoc` – hoàn trả kho khi xoá thuốc khỏi đơn.
  - `trg_KiemTraNgayDatLich` – chặn đặt lịch trong quá khứ.
  - `trg_ChanXoaTaiKhoan` – `INSTEAD OF DELETE` chống xoá tài khoản có lịch sử.
- **2 Stored Procedure dùng Transaction**: `sp_LapHoaDonThanhToan`, `sp_TaoPhieuKhamVaToaThuoc`.

### 6.2. Kiến trúc 3 lớp + DTO

| Tầng | Project | Vai trò |
|---|---|---|
| Presentation | `GUI_QLPK` | WinForms + Guna.UI2 |
| Business Logic | `QLPKBUS` | Kiểm tra ràng buộc, LINQ to Objects |
| Data Access | `QLPKDAL` | ADO.NET (SqlConnection / SqlCommand / SqlDataReader / DataSet) |
| Data Transfer Object | `QLPKDTO` | POCO chia sẻ giữa các lớp |
| Web API Client | `GUI_QLPK/Services` | `HttpClient` gọi `QLPKWebAPI` với fallback xuống BUS/DAL |

### 6.3. ADO.NET (cả mô hình kết nối và phi kết nối)

- **Kết nối:** `SqlConnection`, `SqlCommand`, `SqlDataReader`, `SqlParameter` – gọi View và Stored Procedure.
- **Phi kết nối:** `SqlDataAdapter` + `DataSet` / `DataTable` cho các form báo cáo.

### 6.4. LINQ

- **LINQ to Objects:** Lọc / gom nhóm / sắp xếp danh sách DTO ở tầng BUS, GUI (báo cáo doanh thu).
- **LINQ to XML:** Xuất / nhập danh mục thuốc dạng XML – xem `QLPKEF/Samples/LinqToXmlExamples.cs`.
- **LINQ to Entities:** Toàn bộ truy vấn trong `QLPKEF/Repositories/`.

### 6.5. Entity Framework Core 8

- **Code First:** Entity + DataAnnotations + Fluent API trong `QLPKDbContext.OnModelCreating` (composite key, unique index, check constraint, default value, cấu hình `OnDelete`).
- **Database First:** Có thể scaffold lại bằng `dotnet ef dbcontext scaffold`.
- **DbContext / DbSet:** 14 `DbSet<T>` ứng với 14 bảng nghiệp vụ.
- **Migrations:** Hỗ trợ `dotnet ef migrations add`.
- **Gọi Stored Procedure qua EF:** `_db.Database.ExecuteSqlInterpolatedAsync($"EXEC sp_LapHoaDonThanhToan @maPKB={maPKB}, @maTaiKhoan={maTaiKhoan}")`.

### 6.6. Web API – RESTful JSON

| Method | Endpoint | Mô tả |
|---|---|---|
| `POST` | `/api/auth/login` | Đăng nhập |
| `GET` | `/api/benhnhan` | Danh sách bệnh nhân |
| `GET` | `/api/benhnhan/search?keyword=` | Tìm kiếm |
| `GET` | `/api/benhnhan/{id}/lichsu` | Lịch sử khám |
| `POST` | `/api/benhnhan` | Thêm |
| `PUT` | `/api/benhnhan/{id}` | Cập nhật |
| `DELETE` | `/api/benhnhan/{id}` | Xoá mềm |
| `GET` | `/api/hoadon` | Danh sách hoá đơn |
| `GET` | `/api/hoadon/doanhthu?thang=&nam=` | Báo cáo doanh thu |
| `POST` | `/api/hoadon/lap` | Lập hoá đơn (gọi SP có Transaction) |
| `GET` | `/api/thuoc` | Danh sách thuốc |
| `GET` | `/api/thuoc/saphet?nguong=` | Cảnh báo tồn kho thấp |
| `GET` | `/api/thuoc/export-xml` | Xuất XML (LINQ to XML) |
| `POST` | `/api/thuoc` | Thêm |
| `GET` | `/api/lichhen` | Danh sách lịch hẹn |
| `POST` | `/api/lichhen` | Tạo lịch hẹn |
| `PUT` | `/api/lichhen/{id}/trangthai` | Cập nhật trạng thái |

---

## 7. Báo cáo

Xem báo cáo chi tiết tại [`BaoCao/BaoCao_QLPK.pdf`](BaoCao/BaoCao_QLPK.pdf). Mã nguồn LaTeX ở `BaoCao/BaoCao_QLPK.tex`.

Biên dịch lại báo cáo:

```bash
cd BaoCao
xelatex BaoCao_QLPK.tex
xelatex BaoCao_QLPK.tex   # chạy 2 lần để cập nhật mục lục
```

---

## 8. Nhóm thực hiện

> **Giảng viên hướng dẫn:** ThS. Phạm Hoàng An
> **Trường:** Đại học Mở TP. Hồ Chí Minh – Khoa Công nghệ Thông tin

| Họ và tên | MSSV | Vai trò |
|---|---|---|
| Châu Ngọc Sơn | 2351010184 | Phân tích & thiết kế CSDL, T-SQL (View / Function / Stored Procedure / Trigger / Transaction) |
| Đỗ Phú Điền | 2351010048 | ADO.NET (DAL kết nối + phi kết nối), QLPKDTO, Entity Framework Core 8 (Code First + Database First) |
| Huỳnh Nhật Thiên Long | 2351010113 | QLPKBUS + LINQ to Objects/XML, QLPKWebAPI (RESTful + JSON), WinForms (Guna UI) + xuất PDF + gửi mail, viết báo cáo LaTeX |

---

## 9. Tài liệu tham khảo

- [Microsoft Learn – T-SQL reference](https://learn.microsoft.com/en-us/sql/t-sql/)
- [Microsoft Learn – ADO.NET overview](https://learn.microsoft.com/en-us/dotnet/framework/data/adonet/)
- [Microsoft Learn – LINQ](https://learn.microsoft.com/en-us/dotnet/csharp/linq/)
- [Microsoft Learn – EF Core](https://learn.microsoft.com/en-us/ef/core/)
- [Microsoft Learn – ASP.NET Core Web API](https://learn.microsoft.com/en-us/aspnet/core/web-api/)
- [Microsoft Learn – DML Triggers](https://learn.microsoft.com/en-us/sql/relational-databases/triggers/dml-triggers)
- [Swashbuckle – Swagger for ASP.NET Core](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)
- [Guna.UI2 Docs](https://docs.gunaui.com/)
- [iText 7 Knowledge Base](https://kb.itextpdf.com/)

---

## 10. Giấy phép

Đồ án phục vụ mục đích học tập, không có giá trị thương mại.
