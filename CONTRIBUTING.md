# Contributing to Nail & Lash Studio Appointment System

Proje geliştirmesine katkıda bulunmak için teşekkürler!

## 🤝 Katkı Adımları

### 1. Projeyi Fork Et
```bash
git clone https://github.com/your-username/nailApp.git
cd nailApp
```

### 2. Feature Branch Oluştur
```bash
git checkout -b feature/your-feature-name
```

### 3. Değişiklikleri Yap
- Kodu yazın
- Testleri çalıştırın
- Kod standardlarını takip edin

### 4. Commit Et
```bash
git add .
git commit -m "Add: Brief description of your changes"
```

**Commit mesajı formatı:**
- `Add:` Yeni özellik
- `Fix:` Hata düzeltme
- `Update:` Güncellemeler
- `Remove:` Kaldırma
- `Docs:` Dokümantasyon

### 5. Push Et
```bash
git push origin feature/your-feature-name
```

### 6. Pull Request Aç
- Clear açıklama yazın
- Hangi issue'yu çözdüğünü belirtin
- Screenshots ekleyin (UI değişiklikleri için)

## 📋 Guideline'lar

### Code Quality
- [ ] Kod temiz ve okunabilir
- [ ] Yorum ve dokümantasyon var
- [ ] Testler geçti
- [ ] Linting hataları yok

### Commit Mesajları
- Başlık 50 karakterden az
- Boş bir satır
- Detaylı açıklama (72 karakterde sarılı)

### Branch Adlandırması
- `feature/new-feature` - Yeni özellik
- `fix/bug-name` - Hata düzeltme
- `docs/update-readme` - Dokümantasyon

### Pull Request
- Tek bir özellik / fix per PR
- Testler geçtiğinden emin ol
- Dokümantasyon güncelle
- Eski kodları kaldır

## 🧪 Testing

```bash
# Backend tests
cd NailAppAPI
dotnet test

# Frontend tests (opsiyonel)
# Jest, Mocha vb. kullanılabilir
```

## 📖 Dokümantasyon

- Yeni özellik eklensen `Documentation/` güncelle
- API değişikliği için `API_DOCUMENTATION.md` güncelle
- Büyük değişiklikler için migration guide yazı

## 🐛 Bug Reporting

1. Bugı kontrol et (var mı zaten?)
2. Minimal reproduction case hazırla
3. Steps to reproduce yaz
4. Expected vs actual behavior
5. Screenshots/logs ekle
6. System info (OS, Browser vb.)

## 💡 Feature Requests

1. Issue template kullan
2. Clear use case açıkla
3. Mockup/wireframe ekle
4. Mevcut alternatifler varsa söyle

## 📞 İletişim

- GitHub Issues: Bug ve feature requests
- Email: ssumeyyekoc@example.com
- Forum: (soon)

## ⭐ Code Style

### C# / Backend
```csharp
// PascalCase for public members
public class UserService
{
    // camelCase for private members
    private IRepository _repository;
    
    // Async methods end with Async
    public async Task<User> GetUserAsync(int id)
    {
        return await _repository.GetAsync(id);
    }
}
```

### JavaScript / Frontend
```javascript
// camelCase for functions and variables
function handleSubmit(event) {
    event.preventDefault();
    // ...
}

// UPPER_CASE for constants
const API_URL = 'http://localhost:5000/api';

// Use async/await
async function fetchData() {
    const response = await fetch(API_URL);
    return response.json();
}
```

### HTML/CSS
```html
<!-- Semantic HTML -->
<section class="appointment-section">
    <h1>Randevu Sistemi</h1>
    <form id="appointmentForm">
        <!-- Form content -->
    </form>
</section>
```

```css
/* kebab-case for CSS classes */
.appointment-form {
    display: grid;
    /* ... */
}

.form-group {
    margin-bottom: 20px;
}
```

## 🔍 Code Review

- Responsive, constructive feedback
- Respect and courtesy
- Approve after feedback resolved
- Timeline: 2-3 iş günü

## ✅ Before Submitting PR

- [ ] Code standards sağlanıyor
- [ ] Tests geçiyor
- [ ] Dokümantasyon güncellendi
- [ ] Breaking changes yok (minor) veya açıklandı
- [ ] Linting hataları yok
- [ ] Build başarılı

## 🎉 Teşekkürler!

Tüm katkılarda bulunanlara teşekkürler. Birlikte harika bir proje yapıyoruz! 🚀

---

**Happy Contributing!** 👋
