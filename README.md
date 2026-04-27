Tırnak & Kirpik Lifting Randevu Sistemi
📌 Proje Hakkında
Bu proje, güzellik salonları için geliştirilmiş web tabanlı bir randevu ve vitrin sistemidir.
Kullanıcılar:
* Tırnak ve kirpik lifting hizmetlerini inceleyebilir
* Yapılan tırnak modellerini görüntüleyebilir
* Online randevu oluşturabilir
Salon yöneticisi:
* Randevuları yönetebilir
* Hizmet ve fiyatları düzenleyebilir
* Galeri içeriklerini güncelleyebilir

  
🛠️ Kullanılan Teknolojiler
Backend
* ASP.NET Core (MVC / Web API)
Frontend
* HTML5
* CSS3
* JavaScript
* (Opsiyonel: React / Vue)
Diğer
* Microsoft SQL Server
* Entity Framework Core
* ASP.NET Identity
Hosting
* IIS / Azure / VPS

  
🏗️ Sistem Mimarisi
Proje katmanlı mimari (N-Tier Architecture) ile geliştirilecektir:
* Presentation Layer (UI)
* Business Layer
* Data Access Layer
Opsiyonel:
* RESTful API desteği


👥 Kullanıcı Rolleri
* 👤 Ziyaretçi (Guest)
* 🧑‍💼 Kayıtlı Kullanıcı (Customer)
* 🛠️ Admin (Salon Sahibi)


⚙️ Fonksiyonel Gereksinimler
🏠 Ana Sayfa
* Hizmet tanıtımları
* Kampanyalar / duyurular
* Öne çıkan tırnak modelleri


💄 Hizmetler
* Tırnak hizmetleri
* Kirpik lifting hizmetleri
* Fiyat ve süre bilgileri


🖼️ Galeri
* Kategori bazlı filtreleme (jel, protez, nail art)
* Admin panelinden resim yükleme
* Lightbox görüntüleme


📅 Randevu Sistemi
* Tarih & saat seçimi
* Uygun saatlerin listelenmesi
* Hizmet seçimi
* Kullanıcı bilgileri (ad, telefon, e-posta)
* Randevu onayı


👤 Kullanıcı İşlemleri
* Kayıt ol / giriş yap
* Şifre sıfırlama
* Randevu geçmişi


⚙️ Admin Paneli
* Dashboard (istatistikler)
* Randevu yönetimi (onay / iptal)
* Hizmet CRUD işlemleri
* Galeri yönetimi
* Kullanıcı yönetimi


🗄️ Veritabanı Tasarımı
Temel Tablolar
* Users
* Roles
* Services
* Appointments
* Gallery
* Categories
📌 Örnek: Appointments Tablosu
AlanAçıklamaIdBirincil anahtarUserIdKullanıcı IDServiceIdHizmet IDDateRandevu tarihiStatusDurum (Onaylı / Beklemede / İptal)


🔐 Güvenlik
* HTTPS zorunludur
* SQL Injection koruması (EF Core)
* Role-based authorization
* Şifre hashleme (ASP.NET Identity)


⚡ Performans
* Sayfa yüklenme süresi < 3 saniye
* Minimum 100 eş zamanlı kullanıcı desteği
* Optimize edilmiş görseller


🎨 UI / UX
* Responsive (mobil uyumlu) tasarım
* Kullanıcı dostu arayüz
* Minimal ve estetik görünüm


🔌 Entegrasyonlar (Opsiyonel)
* SMS bildirim sistemi
* E-posta bildirimleri
* WhatsApp entegrasyonu


🚀 Yayınlama
* Domain & hosting kurulumu
* SSL sertifikası
* CI/CD pipeline (opsiyonel)


🔄 Bakım
* Hata loglama sistemi
* Periyodik yedekleme
* Versiyon güncellemeleri


📬 Katkı
Projeye katkıda bulunmak için:
* Fork oluştur
* Yeni branch aç (feature/...)
* Commit at
* Pull request gönder
📄 Lisans
Bu proje isteğe bağlı olarak lisanslanabilir (MIT, GPL vb.)
