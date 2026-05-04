// API Configuration
const protocol = window.location.protocol;
let hostname = window.location.hostname;

// Handle Codespaces port mapping
if (hostname.includes('-3000.app.github.dev')) {
    hostname = hostname.replace('-3000.app.github.dev', '-5000.app.github.dev');
}

const API_URL = `${protocol}//${hostname}/api`;

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
        if (currentUser) {
            authMenu.innerHTML = `
                <div class="user-menu">
                    <span>👤 ${currentUser.firstName}</span>
                    <a href="#" onclick="logout(); return false;">Çıkış Yap</a>
                </div>
            `;
        } else {
            authMenu.innerHTML = '<a href="pages/login.html">Giriş Yap</a>';
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
            window.location.href = '/pages/login.html';
            return null;
        }

        if (!response.ok) {
            throw new Error(`API error: ${response.status}`);
        }

        return await response.json();
    } catch (error) {
        console.error('API call error:', error);
        return null;
    }
}

// ============ SERVICES FUNCTIONS ============
async function loadServices() {
    const services = await apiCall('/services');
    return services || [];
}

async function getServiceById(serviceId) {
    const service = await apiCall(`/services/${serviceId}`);
    return service;
}

async function getServicesByCategory(categoryId) {
    const services = await apiCall(`/services/category/${categoryId}`);
    return services || [];
}

// ============ GALLERY FUNCTIONS ============
async function loadGalleryImages() {
    const images = await apiCall('/gallery');
    return images || [];
}

async function getGalleryImagesByCategory(categoryId) {
    const images = await apiCall(`/gallery/category/${categoryId}`);
    return images || [];
}

async function getHighlightedImages() {
    const images = await apiCall('/gallery/highlighted');
    return images || [];
}

// ============ APPOINTMENTS FUNCTIONS ============
async function createAppointment(appointmentData) {
    const result = await apiCall('/appointments', 'POST', appointmentData);
    return result;
}

async function getUserAppointments(userId) {
    const appointments = await apiCall(`/appointments/user/${userId}`);
    return appointments || [];
}

async function getAppointmentById(appointmentId) {
    const appointment = await apiCall(`/appointments/${appointmentId}`);
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
    const tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);
    return tomorrow.toISOString().split('T')[0];
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
    window.location.href = '/index.html';
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
