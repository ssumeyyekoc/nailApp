# Versioning and Release Notes

## Semantic Versioning
Bu proje Semantic Versioning (SemVer) kullanır: MAJOR.MINOR.PATCH

- **MAJOR**: Geriye dönük uyumsuz değişiklikler
- **MINOR**: Geriye dönük uyumlu yeni özellikler  
- **PATCH**: Hata düzeltmeleri

## Release History

### v1.0.0 (2024-01-15)
**Initial Release**

#### Features
- ✅ Temel randevu sistemi
- ✅ Hizmet yönetimi
- ✅ Galeri özelliği
- ✅ Kategori yönetimi
- ✅ Kullanıcı authentication (JWT)
- ✅ Responsive web design
- ✅ Admin paneli (temel)
- ✅ Uygun saat listeleme

#### API Endpoints
- Authentication (login, register, profile)
- Services (CRUD)
- Appointments (CRUD, available times)
- Gallery (CRUD, filtering)
- Categories (CRUD)

#### Known Limitations
- Admin paneli kısıtlı özelliklere sahiptir
- Email/SMS notifikasyonları henüz eklenmedi
- Dosya yükleme sistemi basittir
- Advanced reporting yoktur

#### Breaking Changes
- N/A (İlk sürüm)

---

## Upcoming Features (Planned)

### v1.1.0 (Q2 2024)
- [ ] Email bildirim sistemi
- [ ] SMS bildirim sistemi
- [ ] Müşteri tercihleri saklanması
- [ ] İstatistik dashboard
- [ ] Randevu hatırlatmaları
- [ ] Fiyat promosyonları

### v1.2.0 (Q3 2024)
- [ ] Ödeme entegrasyonu
- [ ] WhatsApp entegrasyonu
- [ ] Çevrimiçi sohbet
- [ ] Müşteri yorumları
- [ ] Randevu rescheduling

### v2.0.0 (Q4 2024)
- [ ] Mobil uygulama (iOS/Android)
- [ ] Sosyal medya sharing
- [ ] Multi-branch desteği
- [ ] Raporlama sistemi (Advanced)
- [ ] API v2.0

---

## Migration Guides

### From v0.9 to v1.0
```sql
-- Veritabanı migration yoktur
-- API endpoint'ler aynıdır
```

---

## Support

### Aktif Support
- v1.0.0 ve üstü: Tam destek
- v1.0.0 altı: Destek yok

### Security Updates
- Kritik güvenlik güncellemeleri yapılır
- Regular security audits planlı

---

## Contribution Guidelines

- Git issue oluştur
- Feature branch aç
- Pull request gönder
- Code review bekleme

---

**Last Updated:** 2024-01-15
