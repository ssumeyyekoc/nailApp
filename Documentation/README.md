# Tırnak & Kirpik Lifting Randevu Sistemi

## 📌 Proje Hakkında

Bu proje, güzellik salonları için geliştirilmiş web tabanlı bir randevu ve vitrin sistemidir. Kullanıcılar tırnak ve kirpik lifting hizmetlerini inceleyebilir, yapılan çalışmaları galeride görüntüleyebilir ve online randevu oluşturabilir. Salon yöneticileri ise randevuları, hizmetleri ve galeri içeriklerini yönetebilir.

## 🎯 Ana Özellikler

### 👤 Ziyaretçiler / Müşteriler
- ✅ Hizmetleri görüntüleme
- ✅ Galeri (fotoğraf) tarama
- ✅ Online randevu oluşturma
- ✅ Kayıt olma ve giriş yapma
- ✅ Randevu geçmişi görüntüleme

### 🛠️ Admin (Salon Sahibi)
- ✅ Randevuları yönetme (onay/iptal)
- ✅ Hizmet CRUD işlemleri
- ✅ Hizmet fiyat ve süresi ayarlama
- ✅ Galeri görselleri yükleme ve yönetme
- ✅ Kategorileri düzenleme
- ✅ Uygun saatleri otomatik listeleme

## 🛠️ Kullanılan Teknolojiler

### Backend
- **ASP.NET Core 8.0** (MVC / Web API)
- **Entity Framework Core** (ORM)
- **Microsoft SQL Server** (Veritabanı)
- **JWT Authentication** (Token Tabanlı Kimlik Doğrulama)
- **ASP.NET Identity** (Kullanıcı Yönetimi)

### Frontend
- **HTML5**
- **CSS3** (Responsive Design)
- **JavaScript ES6+**
- **Fetch API** (REST API Çağrıları)

## 🏗️ Proje Yapısı

```
nailApp/
├── NailAppAPI/              # Backend (ASP.NET Core)
│   ├── Controllers/         # API Endpoints
│   ├── Services/            # Business Logic
│   ├── Models/              # Entity Models
│   ├── Data/                # Database Context
│   ├── Middleware/          # Custom Middleware
│   ├── Program.cs           # App Configuration
│   └── appsettings.json     # Configuration
├── Frontend/                # Frontend (HTML/CSS/JS)
│   ├── index.html           # Ana Sayfa
│   ├── pages/               # Diğer Sayfalar
│   │   ├── services.html
│   │   ├── appointments.html
│   │   └── login.html
│   ├── css/                 # Stil Dosyaları
│   ├── js/                  # JavaScript Dosyaları
│   └── images/              # Resim Dosyaları
└── Documentation/           # Dokümantasyon
```

## 🗄️ Veritabanı Yapısı

### Temel Tablolar

#### Users
```sql
- Id (PK)
- FirstName
- LastName
- Email
- PhoneNumber
- PasswordHash
- CreatedAt
- UpdatedAt
```

#### Roles
```sql
- Id (PK)
- Name (Admin, Customer, Guest)
- Description
- CreatedAt
```

#### Services
```sql
- Id (PK)
- Name
- Description
- Price
- DurationMinutes
- CategoryId (FK)
- IsActive
- CreatedAt
- UpdatedAt
```

#### Appointments
```sql
- Id (PK)
- UserId (FK)
- ServiceId (FK)
- AppointmentDate
- Status (Pending, Confirmed, Completed, Cancelled)
- Notes
- CreatedAt
- UpdatedAt
```

#### Categories
```sql
- Id (PK)
- Name
- Description
- IsActive
- CreatedAt
```

## 🚀 Kurulum

### Gereksinimler
- .NET 8.0 SDK
- SQL Server (veya LocalDB)
- Node.js (opsiyonel, dosya sunumu için)

### Adım 1: Veritabanı Kurulumu

```bash
cd NailAppAPI

# Migration oluştur
dotnet ef migrations add InitialCreate

# Migration'ı uygula
dotnet ef database update
```

### Adım 2: Backend Başlatma

```bash
cd NailAppAPI

# Bağımlılıkları yükle
dotnet restore

# Uygulamayı çalıştır
dotnet run
```

API şu adreste çalışacaktır: `http://localhost:5000`

### Adım 3: Frontend Başlatma

```bash
# Frontend klasörüne git
cd Frontend

# Basit bir HTTP sunucu başlat (Python)
python -m http.server 8000

# veya Node.js ile
npx http-server -p 8000
```

Frontend şu adreste açılacaktır: `http://localhost:8000`

## 📚 API Endpoints

### Authentication
- `POST /api/auth/register` - Kayıt ol
- `POST /api/auth/login` - Giriş yap
- `POST /api/auth/logout` - Çıkış yap
- `GET /api/auth/profile` - Kullanıcı profili

### Services
- `GET /api/services` - Tüm hizmetler
- `GET /api/services/{id}` - Hizmet detayı
- `GET /api/services/category/{categoryId}` - Kategoriye göre hizmetler
- `POST /api/services` - Yeni hizmet (Admin)
- `PUT /api/services/{id}` - Hizmet güncelle (Admin)
- `DELETE /api/services/{id}` - Hizmet sil (Admin)

### Appointments
- `GET /api/appointments` - Tüm randevular (Admin)
- `GET /api/appointments/{id}` - Randevu detayı
- `GET /api/appointments/user/{userId}` - Kullanıcının randevuları
- `GET /api/appointments/available-times` - Uygun saatler
- `POST /api/appointments` - Randevu oluştur
- `PUT /api/appointments/{id}/status` - Randevu durumu güncelle (Admin)
- `DELETE /api/appointments/{id}` - Randevu sil (Admin)

### Categories
- `GET /api/categories` - Tüm kategoriler
- `GET /api/categories/{id}` - Kategori detayı
- `POST /api/categories` - Yeni kategori (Admin)
- `PUT /api/categories/{id}` - Kategori güncelle (Admin)
- `DELETE /api/categories/{id}` - Kategori sil (Admin)

## 🔐 Güvenlik

- ✅ HTTPS (Üretim ortamında zorunlu)
- ✅ SQL Injection koruması (Entity Framework Core)
- ✅ JWT Token tabanlı authentication
- ✅ Role-based authorization
- ✅ Şifre hashleme (ASP.NET Identity)
- ✅ CORS yapılandırması

## ⚡ Performans Hedefleri

- ✅ Sayfa yüklenme süresi < 3 saniye
- ✅ Minimum 100 eş zamanlı kullanıcı desteği
- ✅ Optimize edilmiş görseller (WebP format)
- ✅ Veritabanı indexing ve caching

## 📱 Responsive Design

- ✅ Desktop (1920px+)
- ✅ Tablet (768px - 1024px)
- ✅ Mobile (320px - 767px)
- ✅ Touch device optimizations

## 🎨 UI/UX Özellikleri

- Minimal ve estetik tasarım
- Kullanıcı dostu arayüz
- Smooth animasyonlar
- Lightbox galeri görüntüleme
- Form validasyonu
- Responsive loading states

## 🚀 Yayınlama

### Adım 1: Domain ve Hosting
1. Domain adı satın al
2. Hosting sağlayıcısında ASP.NET desteğini kontrol et
3. Veritabanı hosting seçeneğini seç

### Adım 2: SSL Sertifikası
```bash
# Let's Encrypt ile ücretsiz SSL
# IIS Manager veya kontrol panelinden yapılandır
```

### Adım 3: Deploy Etme

**IIS'e Deploy:**
1. NailAppAPI'yi publish et: `dotnet publish -c Release`
2. IIS'de yeni site oluştur
3. Yayınlanan dosyaları site dizinine kopyala

**Azure'a Deploy:**
```bash
dotnet publish -c Release
# Azure App Service üzerine yükle
```

## 📋 Konfigürasyon

### appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=your_server;Database=NailAppDB;Trusted_Connection=true;"
  },
  "JwtSettings": {
    "SecretKey": "your-secret-key-here",
    "Issuer": "NailAppAPI",
    "Audience": "NailAppClient",
    "ExpirationMinutes": 60
  }
}
```

## 🐛 Hata Düzeltme

Hata logları şu dosyada tutulur:
- `logs/error.log`

## 📞 İletişim ve Destek

- 📧 Email: ssumeyyekoc@example.com
- 📞 Telefon: +90 XXX XXX XX XX
- 🌐 Website: www.nailstudio.com

## 📄 Lisans

Bu proje MIT Lisansı altında yayınlanmıştır. Detaylar için LICENSE dosyasına bakınız.

## 🤝 Katkı

Katkı yapmak istiyorsanız:
1. Projeyi fork et
2. Feature branch oluştur (`git checkout -b feature/amazing-feature`)
3. Değişiklikleri commit et (`git commit -m 'Add amazing feature'`)
4. Branch'i push et (`git push origin feature/amazing-feature`)
5. Pull request aç

## 📝 Versiyon Tarihi

### v1.0.0 (2024)
- İlk sürüm yayınlandı
- Temel özellikler (Randevu, Hizmet, Galeri)
- Authentication sistemi
- Admin paneli

## ⚙️ Bakım

- Düzenli güvenlik güncellemeleri
- Periyodik veritabanı yedeklemesi
- Log dosyaları takibi
- Sistem performans kontrolü

---

**Son Güncelleme:** 2024
**Geliştirici:** Nail & Lash Studio Team
