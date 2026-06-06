// API Configuration
const protocol = window.location.protocol;
let hostname = window.location.hostname;
let API_URL;

if (protocol === 'file:' || !hostname) {
    API_URL = 'http://localhost:5999/api';
} else {
    // Handle Codespaces port mapping
    if (hostname.includes('-3000.app.github.dev')) {
        hostname = hostname.replace('-3000.app.github.dev', '-5000.app.github.dev');
    }
    API_URL = `${protocol}//${hostname}/api`;
}

// Auth State
let currentUser = null;
let authToken = null;

// Initialize on page load
document.addEventListener('DOMContentLoaded', function() {
    loadAuthState();
    updateAuthUI();
});

// ============ AUTH FUNCTIONS ============
function loadAuthState() {
    const token = localStorage.getItem('authToken');
    const user = localStorage.getItem('currentUser');
    
    if (token && user) {
        authToken = token;
        currentUser = JSON.parse(user);
    }
}

function saveAuthState(token, user) {
    authToken = token;
    currentUser = user;
    localStorage.setItem('authToken', token);
    localStorage.setItem('currentUser', JSON.stringify(user));
}

function clearAuthState() {
    authToken = null;
    currentUser = null;
    localStorage.removeItem('authToken');
    localStorage.removeItem('currentUser');
}

function updateAuthUI() {
    const authMenu = document.getElementById('authMenu');
    
    if (authMenu) {
        const isPageFolder = window.location.pathname.toLowerCase().includes('/pages/');
        const prefix = isPageFolder ? '' : 'pages/';
        
        if (currentUser) {
            const isAdmin = currentUser.roles && currentUser.roles.includes('Admin');
            const adminLink = isAdmin ? `<a href="${prefix}admin.html" style="margin-right: 15px; font-weight: 600; color: var(--primary-color);">⚙️ Yönetim</a>` : '';
            authMenu.innerHTML = `
                <div class="user-menu">
                    ${adminLink}
                    <span>👤 ${currentUser.firstName}</span>
                    <a href="#" onclick="logout(); return false;">Çıkış Yap</a>
                </div>
            `;
        } else {
            authMenu.innerHTML = `<a href="${prefix}login.html">Giriş Yap</a>`;
        }
    }
}

// ============ API FUNCTIONS ============
async function apiCall(endpoint, method = 'GET', data = null) {
    const options = {
        method: method,
        headers: {
            'Content-Type': 'application/json'
        }
    };

    if (authToken) {
        options.headers['Authorization'] = `Bearer ${authToken}`;
    }

    if (data) {
        options.body = JSON.stringify(data);
    }

    try {
        const response = await fetch(`${API_URL}${endpoint}`, options);
        
        if (response.status === 401) {
            clearAuthState();
            updateAuthUI();
            const isPageFolder = window.location.pathname.toLowerCase().includes('/pages/');
            window.location.href = isPageFolder ? 'login.html' : 'pages/login.html';
            return null;
        }

        // Hata durumunda mesajı okumaya çalış
        if (!response.ok) {
            let errorMsg = `Hata oluştu (Kod: ${response.status})`;
            try {
                const errData = await response.json();
                errorMsg = errData.message || errData.errors || errData;
                if (typeof errorMsg === 'object') {
                    errorMsg = JSON.stringify(errorMsg);
                }
            } catch (jsonErr) {
                try {
                    errorMsg = await response.text();
                } catch (textErr) {}
            }
            return { error: true, message: errorMsg };
        }

        return await response.json();
    } catch (error) {
        console.error('API call error:', error);
        return { error: true, message: 'Sunucuya bağlanılamadı. Lütfen internetinizi veya API durumunu kontrol edin.' };
    }
}

// ============ SERVICES FUNCTIONS ============
async function loadServices() {
    const services = await apiCall('/services');
    if (services && services.error) return [];
    return services || [];
}

async function getServiceById(serviceId) {
    const service = await apiCall(`/services/${serviceId}`);
    if (service && service.error) return null;
    return service;
}

async function getServicesByCategory(categoryId) {
    const services = await apiCall(`/services/category/${categoryId}`);
    if (services && services.error) return [];
    return services || [];
}

// ============ APPOINTMENTS FUNCTIONS ============
async function createAppointment(appointmentData) {
    const result = await apiCall('/appointments', 'POST', appointmentData);
    return result;
}

async function getUserAppointments(userId) {
    const appointments = await apiCall(`/appointments/user/${userId}`);
    if (appointments && appointments.error) return [];
    return appointments || [];
}

async function getAppointmentById(appointmentId) {
    const appointment = await apiCall(`/appointments/${appointmentId}`);
    if (appointment && appointment.error) return null;
    return appointment;
}

// ============ UTILITY FUNCTIONS ============
function formatDate(date) {
    return new Date(date).toLocaleDateString('tr-TR');
}

function formatTime(date) {
    return new Date(date).toLocaleTimeString('tr-TR', { hour: '2-digit', minute: '2-digit' });
}

function formatDateTime(date) {
    return `${formatDate(date)} ${formatTime(date)}`;
}

function getMinDateString() {
    const today = new Date();
    const year = today.getFullYear();
    const month = String(today.getMonth() + 1).padStart(2, '0');
    const day = String(today.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
}

function toggleAuthForm() {
    const loginForm = document.getElementById('loginForm');
    const registerForm = document.getElementById('registerForm');
    
    if (loginForm && registerForm) {
        loginForm.classList.toggle('active');
        registerForm.classList.toggle('active');
    }
}

async function logout() {
    await apiCall('/auth/logout', 'POST');
    clearAuthState();
    updateAuthUI();
    const isPageFolder = window.location.pathname.toLowerCase().includes('/pages/');
    window.location.href = isPageFolder ? '../index.html' : 'index.html';
}

// ============ UI HELPERS ============
function showMessage(message, type = 'info', elementId = 'authMessage') {
    const messageElement = document.getElementById(elementId);
    if (messageElement) {
        messageElement.textContent = message;
        messageElement.className = `auth-message ${type}`;
        
        if (type !== 'info') {
            setTimeout(() => {
                messageElement.className = 'auth-message';
            }, 5000);
        }
    }
}

function getStatusLabel(status) {
    const labels = {
        0: 'Beklemede',
        1: 'Onaylandı',
        2: 'Tamamlandı',
        3: 'İptal Edildi'
    };
    return labels[status] || 'Bilinmiyor';
}

function getStatusClass(status) {
    const classes = {
        0: 'pending',
        1: 'confirmed',
        2: 'completed',
        3: 'cancelled'
    };
    return classes[status] || '';
}
