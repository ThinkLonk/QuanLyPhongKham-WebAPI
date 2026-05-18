# =============================================================================
#  Script kiểm thử các endpoint của QLPK Web API
#  Cách chạy:
#      1. Chạy QLPKWebAPI (F5 trong Visual Studio hoặc dotnet run).
#      2. Mở PowerShell tại thư mục này, chạy:
#           .\test-endpoints.ps1
#      Tham số: -Base 'https://localhost:7000' (mặc định) hoặc URL khác.
#  Yêu cầu: PowerShell 7+ (Windows PowerShell 5.1 cũng chạy được).
# =============================================================================
param(
    [string]$Base = 'https://localhost:7000'
)

# Bỏ qua lỗi SSL self-signed (chỉ cho môi trường dev)
if ($PSVersionTable.PSVersion.Major -ge 7) {
    $PSDefaultParameterValues['Invoke-RestMethod:SkipCertificateCheck'] = $true
} else {
    Add-Type @"
        using System.Net;
        using System.Security.Cryptography.X509Certificates;
        public class TrustAllCertsPolicy : ICertificatePolicy {
            public bool CheckValidationResult(ServicePoint srvPoint, X509Certificate cert,
                WebRequest req, int problem) { return true; }
        }
"@
    [System.Net.ServicePointManager]::CertificatePolicy = New-Object TrustAllCertsPolicy
    [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
}

$ErrorActionPreference = 'Continue'
$results = @()

function Test-Endpoint {
    param([string]$Name, [string]$Method, [string]$Url, $Body)
    Write-Host "`n[$Method] $Url" -ForegroundColor Cyan
    try {
        $params = @{
            Uri    = $Url
            Method = $Method
            ContentType = 'application/json'
        }
        if ($Body) { $params.Body = ($Body | ConvertTo-Json -Depth 6) }
        $r = Invoke-RestMethod @params -TimeoutSec 15
        Write-Host "  OK" -ForegroundColor Green
        if ($r) {
            $preview = ($r | ConvertTo-Json -Depth 3 -Compress)
            if ($preview.Length -gt 200) { $preview = $preview.Substring(0,200) + '...' }
            Write-Host "  $preview" -ForegroundColor DarkGray
        }
        $script:results += [pscustomobject]@{ Test=$Name; Status='PASS'; Detail='' }
        return $r
    } catch {
        Write-Host "  FAILED: $($_.Exception.Message)" -ForegroundColor Red
        $script:results += [pscustomobject]@{ Test=$Name; Status='FAIL'; Detail=$_.Exception.Message }
        return $null
    }
}

Write-Host "===== QLPK WEB API TEST RUNNER =====" -ForegroundColor Yellow
Write-Host "Base URL: $Base" -ForegroundColor Yellow

# ---------- 1. AUTH ----------
Test-Endpoint -Name 'Login' -Method POST -Url "$Base/api/auth/login" -Body @{
    userName = 'admin'
    passWord = 'admin'
}

# ---------- 2. BENH NHAN ----------
Test-Endpoint -Name 'GetAllBenhNhan' -Method GET  -Url "$Base/api/benhnhan"
Test-Endpoint -Name 'SearchBenhNhan' -Method GET  -Url "$Base/api/benhnhan/search?keyword=a"

$newBN = @{
    tenBenhNhan = 'Nguyen Van Test'
    ngaySinh    = '2000-01-15'
    diaChi      = '123 Le Loi, Q1'
    cccd        = '079200012345'
    gioiTinh    = 'Nam'
    email       = 'test@example.com'
    isDeleted   = $false
}
$createdBN = Test-Endpoint -Name 'CreateBenhNhan' -Method POST -Url "$Base/api/benhnhan" -Body $newBN

if ($createdBN -and $createdBN.maBenhNhan) {
    $bnId = $createdBN.maBenhNhan
    Test-Endpoint -Name 'LichSuKham' -Method GET -Url "$Base/api/benhnhan/$bnId/lichsu"

    # Cap nhat
    $createdBN.diaChi = '456 Nguyen Hue, Q1'
    Test-Endpoint -Name 'UpdateBenhNhan' -Method PUT -Url "$Base/api/benhnhan/$bnId" -Body $createdBN

    # Xoa mem
    Test-Endpoint -Name 'SoftDeleteBenhNhan' -Method DELETE -Url "$Base/api/benhnhan/$bnId"
}

# ---------- 3. THUOC ----------
Test-Endpoint -Name 'GetAllThuoc'     -Method GET -Url "$Base/api/thuoc"
Test-Endpoint -Name 'ThuocSapHet'     -Method GET -Url "$Base/api/thuoc/saphet?nguong=20"
Test-Endpoint -Name 'ExportThuocXml'  -Method GET -Url "$Base/api/thuoc/export-xml"

# ---------- 4. LICH HEN ----------
Test-Endpoint -Name 'GetAllLichHen'   -Method GET -Url "$Base/api/lichhen"

# ---------- 5. HOA DON ----------
Test-Endpoint -Name 'GetAllHoaDon'    -Method GET -Url "$Base/api/hoadon"
Test-Endpoint -Name 'DoanhThuThang'   -Method GET -Url "$Base/api/hoadon/doanhthu?thang=5&nam=2026"

# ---------- BAO CAO ----------
Write-Host "`n===== TONG HOP =====" -ForegroundColor Yellow
$results | Format-Table -AutoSize
$pass = ($results | Where-Object Status -eq 'PASS').Count
$fail = ($results | Where-Object Status -eq 'FAIL').Count
Write-Host "PASS: $pass | FAIL: $fail" -ForegroundColor $(if ($fail -eq 0) {'Green'} else {'Yellow'})
