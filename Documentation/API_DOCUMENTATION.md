# API Dokümantasyonu

## Base URL
```
Development: http://localhost:5000/api
Production: https://api.nailstudio.com/api
```

## Authentication

Tüm korumalı endpoint'ler için JWT token kullanılır.

### Token Gönderme
```
Authorization: Bearer {token}
```

---

## Auth Endpoints

### 1. Kayıt Ol
**Endpoint:** `POST /auth/register`

**Request:**
```json
{
  "email": "user@example.com",
  "password": "SecurePass123!",
  "firstName": "Ayşe",
  "lastName": "Yılmaz"
}
```

**Response:** `200 OK`
```json
{
  "message": "Kayıt başarıyla tamamlandı."
}
```

### 2. Giriş Yap
**Endpoint:** `POST /auth/login`

**Request:**
```json
{
  "email": "user@example.com",
  "password": "SecurePass123!"
}
```

**Response:** `200 OK`
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "message": "Başarıyla giriş yapıldı."
}
```

### 3. Profil Bilgisi
**Endpoint:** `GET /auth/profile`
**Kimlik Doğrulama:** ✅ Gerekli

**Response:** `200 OK`
```json
{
  "id": 1,
  "email": "user@example.com",
  "firstName": "Ayşe",
  "lastName": "Yılmaz",
  "phoneNumber": "+90 555 123 4567"
}
```

---

## Services Endpoints

### 1. Tüm Hizmetleri Getir
**Endpoint:** `GET /services`

**Response:** `200 OK`
```json
[
  {
    "id": 1,
    "name": "Jel Tırnak",
    "description": "Yüksek kaliteli jel uygulaması",
    "price": 200,
    "durationMinutes": 60,
    "categoryId": 1,
    "isActive": true,
    "createdAt": "2024-01-15T10:30:00"
  }
]
```

### 2. Hizmet Detayı
**Endpoint:** `GET /services/{id}`

**Response:** `200 OK`
```json
{
  "id": 1,
  "name": "Jel Tırnak",
  "description": "Yüksek kaliteli jel uygulaması",
  "price": 200,
  "durationMinutes": 60,
  "categoryId": 1,
  "isActive": true,
  "category": {
    "id": 1,
    "name": "Jel"
  }
}
```

### 3. Kategoriye Göre Hizmetler
**Endpoint:** `GET /services/category/{categoryId}`

**Response:** `200 OK`
```json
[
  {
    "id": 1,
    "name": "Jel Tırnak",
    "price": 200,
    "durationMinutes": 60
  }
]
```

### 4. Yeni Hizmet Oluştur
**Endpoint:** `POST /services`
**Kimlik Doğrulama:** ✅ Admin

**Request:**
```json
{
  "name": "Jel Tırnak",
  "description": "Yüksek kaliteli jel uygulaması",
  "price": 200,
  "durationMinutes": 60,
  "categoryId": 1
}
```

**Response:** `201 Created`

### 5. Hizmet Güncelle
**Endpoint:** `PUT /services/{id}`
**Kimlik Doğrulama:** ✅ Admin

### 6. Hizmet Sil
**Endpoint:** `DELETE /services/{id}`
**Kimlik Doğrulama:** ✅ Admin

---

## Appointments Endpoints

### 1. Uygun Saatleri Getir
**Endpoint:** `GET /appointments/available-times?serviceId={id}&date={date}`

**Query Parameters:**
- `serviceId` (int) - Hizmet ID'si
- `date` (date) - Randevu tarihi (ISO 8601 format)

**Response:** `200 OK`
```json
[
  "2024-01-20T09:00:00",
  "2024-01-20T10:00:00",
  "2024-01-20T11:00:00"
]
```

### 2. Randevu Oluştur
**Endpoint:** `POST /appointments`
**Kimlik Doğrulama:** ✅ Gerekli

**Request:**
```json
{
  "userId": 1,
  "serviceId": 1,
  "appointmentDate": "2024-01-20T10:00:00",
  "notes": "Çift taraf lütfen"
}
```

**Response:** `201 Created`
```json
{
  "id": 1,
  "userId": 1,
  "serviceId": 1,
  "appointmentDate": "2024-01-20T10:00:00",
  "status": 0,
  "notes": "Çift taraf lütfen",
  "createdAt": "2024-01-15T10:30:00"
}
```

### 3. Kullanıcının Randevuları
**Endpoint:** `GET /appointments/user/{userId}`
**Kimlik Doğrulama:** ✅ Gerekli

### 4. Tüm Randevular
**Endpoint:** `GET /appointments`
**Kimlik Doğrulama:** ✅ Admin

### 5. Randevu Durumunu Güncelle
**Endpoint:** `PUT /appointments/{id}/status`
**Kimlik Doğrulama:** ✅ Admin

**Request:**
```json
{
  "status": 1
}
```

**Status Kodları:**
- 0 = Beklemede (Pending)
- 1 = Onaylandı (Confirmed)
- 2 = Tamamlandı (Completed)
- 3 = İptal Edildi (Cancelled)

---

## Categories Endpoints

### 1. Tüm Kategoriler
**Endpoint:** `GET /categories`

**Response:** `200 OK`
```json
[
  {
    "id": 1,
    "name": "Jel",
    "description": "Jel tırnak hizmetleri",
    "isActive": true
  }
]
```

### 2. Kategori Oluştur
**Endpoint:** `POST /categories`
**Kimlik Doğrulama:** ✅ Admin

---

## Error Responses

### 400 Bad Request
```json
{
  "error": "Geçersiz istek",
  "message": "Required field is missing"
}
```

### 401 Unauthorized
```json
{
  "error": "Unauthorized",
  "message": "Token geçersiz veya süresi dolmuş"
}
```

### 403 Forbidden
```json
{
  "error": "Forbidden",
  "message": "Bu işlem için yetkiniz yok"
}
```

### 404 Not Found
```json
{
  "error": "Not Found",
  "message": "Kaynak bulunamadı"
}
```

### 500 Internal Server Error
```json
{
  "error": "Server Error",
  "message": "Beklenmeyen bir hata oluştu"
}
```

---

## Rate Limiting

ApiRate limitin mevcuttur:
- 100 requests per minute per IP

---

## Versioning

Mevcut API versiyonu: **v1.0**

---

**Son Güncelleme:** 2024
