# Kurulum ve Konfigürasyon Rehberi

## 📋 Ön Koşullar

### Sistem Gereksinimleri
- **OS:** Windows 10+, macOS 10.14+, Linux (Ubuntu 18.04+)
- **RAM:** En az 4GB (8GB önerilir)
- **Disk:** En az 2GB boş alan

### Yazılım Gereksinimleri
- **.NET 8.0 SDK** (https://dotnet.microsoft.com/download)
- **SQL Server Express** veya **LocalDB**
- **Git** (Projeyi klonlamak için)
- **Visual Studio Code** veya **Visual Studio**

---

## 🚀 Step-by-Step Kurulum

### Adım 1: Projeyi Klonla

```bash
# Terminal/Komut İstemini aç
git clone https://github.com/your-repo/nailApp.git
cd nailApp
```

### Adım 2: Backend Kurulumu

```bash
# Backend klasörüne git
cd NailAppAPI

# Paketleri geri yükle
dotnet restore

# Migrations'ı oluştur
dotnet ef migrations add InitialCreate

# Veritabanını güncelle
dotnet ef database update

# Backend'i çalıştır
dotnet run
```

**Çıktı:**
```
Building...
Built successfully
Starting server on http://localhost:5000
```

### Adım 3: Frontend Kurulumu

**Python 3.x ile:**
```bash
cd Frontend
python -m http.server 8000
```

**Node.js ile:**
```bash
cd Frontend
npx http-server -p 8000
```

**Visual Studio Code Live Server ile:**
1. Frontend klasörüne sağ tıkla
2. "Open with Live Server" seç

---

## ⚙️ Konfigürasyon

### appsettings.json Ayarları

`NailAppAPI/appsettings.json` dosyasını düzenle:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=NailAppDatabase;Trusted_Connection=true;MultipleActiveResultSets=true"
  },
  "JwtSettings": {
    "SecretKey": "your-super-secret-key-change-in-production",
    "Issuer": "NailAppAPI",
    "Audience": "NailAppClient",
    "ExpirationMinutes": 60
  }
}
```

### appsettings.Production.json

Üretim ortamı için:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=your-server;Database=NailAppDB;User Id=sa;Password=your-password;"
  },
  "JwtSettings": {
    "SecretKey": "generate-strong-secret-key",
    "Issuer": "NailAppAPI",
    "Audience": "NailAppClient",
    "ExpirationMinutes": 480
  }
}
```

---

## 🗄️ Veritabanı Kurulumu

### SQL Server LocalDB ile

```bash
# LocalDB instance'ı başlat
sqllocaldb start mssqllocaldb

# Migration oluştur
dotnet ef migrations add InitialCreate

# Migration'ı uygula
dotnet ef database update
```

### SQL Server Express ile

Connection string'i şu şekilde değiştir:

```
Server=.\\SQLEXPRESS;Database=NailAppDB;Trusted_Connection=true;MultipleActiveResultSets=true
```

### Azure SQL Database ile

```
Server=your-server.database.windows.net;Database=NailAppDB;
User Id=your-username;Password=your-password;
```

---

## 🔐 JWT Konfigürasyonu

### Secret Key Oluştur

**PowerShell ile:**
```powershell
[Convert]::ToBase64String([System.Text.Encoding]::UTF8.GetBytes([guid]::NewGuid().ToString() + [guid]::NewGuid().ToString()))
```

**Python ile:**
```python
import base64
import uuid
secret = base64.b64encode(str(uuid.uuid4()).encode()).decode()
print(secret)
```

Bu değeri `appsettings.json` içindeki `JwtSettings:SecretKey`'e yapıştır.

---

## 📱 Frontend Konfigürasyonu

### API_URL Ayarı

`Frontend/js/main.js` dosyasında:

```javascript
// Development
const API_URL = 'http://localhost:5000/api';

// Production
// const API_URL = 'https://api.nailstudio.com/api';
```

---

## 🧪 Test Etme

### Test Kullanıcı Oluştur

Veritabanında yeni kullanıcı oluşturmak için:

```sql
-- SQL sorgusu ile test admin oluştur
-- (Admin GUI'den yapman daha iyi)
```

veya API ile:

```bash
# Yeni kullanıcı kaydı
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@test.com",
    "password": "TestPass123!",
    "firstName": "Salon",
    "lastName": "Yöneticisi"
  }'
```

### Test Randevu Oluştur

```bash
# Giriş yap
TOKEN=$(curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@test.com",
    "password": "TestPass123!"
  }' | jq -r '.token')

# Randevu oluştur
curl -X POST http://localhost:5000/api/appointments \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{
    "userId": 1,
    "serviceId": 1,
    "appointmentDate": "2024-01-20T10:00:00",
    "notes": "Test randevu"
  }'
```

---

## 📦 Paket Yönetimi

### NuGet Paketleri Güncelle

```bash
dotnet package restore
dotnet package update
```

### npm Paketleri (opsiyonel)

```bash
npm list --outdated
npm update
```

---

## 🐛 Sorun Giderme

### Port Zaten Kullanımda

```bash
# Port 5000'ı kullanan süreci bul
# Windows
netstat -ano | findstr :5000

# Linux/macOS
lsof -i :5000

# Süreci sonlandır
# Windows
taskkill /PID {process_id} /F

# Linux/macOS
kill -9 {process_id}
```

### Veritabanı Bağlantı Hatası

```bash
# Connection string'i kontrol et
# LocalDB çalışıyor mı?
sqllocaldb info mssqllocaldb

# Connection string'i test et
sqlConnectionString = "Server=(localdb)\\mssqllocaldb;Database=NailAppDatabase;Trusted_Connection=true;"
```

### CORS Hatası

`Program.cs`'de CORS'u etkinleştir:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

app.UseCors("AllowAll");
```

### HTTPS Sertifikası

```bash
# ASP.NET Core geliştirme sertifikası oluştur
dotnet dev-certs https --clean
dotnet dev-certs https --trust
```

---

## 🚀 Üretim Yayınlaması

### Step 1: Publish Et

```bash
cd NailAppAPI

# Release build oluştur
dotnet publish -c Release -o ./publish
```

### Step 2: IIS'ye Deploy Et

1. IIS Manager açın
2. "Add Website" tıkla
3. Site adı: "NailStudio"
4. Physical Path: publish klasörü yolu
5. Port: 80 (HTTP) veya 443 (HTTPS)

### Step 3: Azure'a Deploy Et

```bash
# Azure CLI ile giriş yap
az login

# App Service oluştur
az appservice plan create --name NailStudioPlan --resource-group myResourceGroup --sku FREE

# Web App oluştur
az webapp create --resource-group myResourceGroup --plan NailStudioPlan --name nailstudio-app

# Deploy et
dotnet publish -c Release
cd publish
zip -r ../publish.zip .
az webapp deployment source config-zip --resource-group myResourceGroup --name nailstudio-app --src-path ../publish.zip
```

---

## 📊 Monitoring

### Logları Kontrol Et

```bash
# Local loglar
tail -f logs/error.log

# Application Insights (Azure)
# https://portal.azure.com
```

### Performans Monitoring

```bash
# CPU ve RAM kullanımı
# Windows Task Manager
# Linux: htop, top

# Veritabanı performansı
# SQL Server Management Studio
```

---

## 🔄 Yedekleme

### Veritabanı Yedeklemesi

```sql
-- SQL Server'da
BACKUP DATABASE [NailAppDatabase] 
TO DISK = 'C:\\Backups\\NailApp_backup.bak'
```

### Otomatik Yedekleme Script'i

```bash
#!/bin/bash
# backup.sh - Linux/macOS

DATE=$(date +%Y%m%d_%H%M%S)
BACKUP_DIR="/backups/nailapp"

mkdir -p $BACKUP_DIR

# Veritabanı yedekle
sqlcmd -S (localdb)\\mssqllocaldb -Q "BACKUP DATABASE NailAppDatabase TO DISK = '$BACKUP_DIR/nailapp_$DATE.bak'"

# Eski yedekleri sil (30 günden eski)
find $BACKUP_DIR -name "*.bak" -mtime +30 -delete
```

---

## ✅ Kontrol Listesi

- [ ] .NET 8.0 SDK kuruluyor
- [ ] SQL Server/LocalDB kuruluyor
- [ ] Proje klonlanıyor
- [ ] NuGet paketleri geri yükleniyor
- [ ] Migrations çalıştırılıyor
- [ ] appsettings.json yapılandırılıyor
- [ ] Backend başlatılıyor
- [ ] Frontend başlatılıyor
- [ ] Login testi yapılıyor
- [ ] Randevu testi yapılıyor

---

**Son Güncelleme:** 2024
