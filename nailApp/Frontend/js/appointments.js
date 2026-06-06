// Appointments Page Logic

document.addEventListener('DOMContentLoaded', async function() {
    const selectedServiceId = sessionStorage.getItem('selectedServiceId');
    
    await loadServicesForForm();
    
    if (selectedServiceId) {
        document.getElementById('service').value = selectedServiceId;
        sessionStorage.removeItem('selectedServiceId');
    }

    document.getElementById('appointmentDate').min = getMinDateString();
    
    const timeSelect = document.getElementById('appointmentTime');
    if (timeSelect) {
        timeSelect.innerHTML = '<option value="">-- Önce Hizmet & Tarih Seçin --</option>';
    }
    
    const form = document.getElementById('appointmentForm');
    if (form) {
        form.addEventListener('submit', handleAppointmentSubmit);
    }

    const serviceSelect = document.getElementById('service');
    if (serviceSelect) {
        serviceSelect.addEventListener('change', handleServiceChange);
    }

    const dateInput = document.getElementById('appointmentDate');
    if (dateInput) {
        dateInput.addEventListener('change', handleDateChange);
    }

    if (currentUser) {
        const isAdmin = currentUser.roles && currentUser.roles.includes('Admin');
        if (isAdmin) {
            const formContainer = document.querySelector('.appointment-form-container');
            if (formContainer) {
                formContainer.innerHTML = `
                    <div class="login-required-card" style="text-align: center; padding: 30px 15px;">
                        <span style="font-size: 3.5rem; display: block; margin-bottom: 20px;">🛡️</span>
                        <h3 style="margin: 0 0 15px; font-size: 1.8rem; font-family: 'Playfair Display', serif; color: var(--dark-color);">Yönetici Yetkisi</h3>
                        <p style="color: var(--text-color); margin-bottom: 30px; font-size: 0.95rem; line-height: 1.7; max-width: 450px; margin-left: auto; margin-right: auto;">
                            Sistem yöneticileri randevu oluşturamazlar. Randevuları izlemek ve yönetmek için lütfen yönetim paneline geçiş yapın.
                        </p>
                        <a href="admin.html" class="btn btn-primary" style="padding: 12px 40px; font-weight: 600;">Yönetim Paneline Git</a>
                    </div>
                `;
            }
            return;
        }

        // Pre-fill user info
        document.getElementById('firstName').value = currentUser.firstName || '';
        document.getElementById('lastName').value = currentUser.lastName || '';
        document.getElementById('email').value = currentUser.email || '';
        document.getElementById('phone').value = currentUser.phoneNumber || '';
        
        // Load appointment history
        loadAppointmentHistory();
    } else {
        const formContainer = document.querySelector('.appointment-form-container');
        if (formContainer) {
            formContainer.innerHTML = `
                <div class="login-required-card" style="text-align: center; padding: 30px 15px;">
                    <span style="font-size: 3.5rem; display: block; margin-bottom: 20px;">✨</span>
                    <h3 style="margin: 0 0 15px; font-size: 1.8rem; font-family: 'Playfair Display', serif; color: var(--dark-color);">Randevu Oluşturun</h3>
                    <p style="color: var(--text-color); margin-bottom: 30px; font-size: 0.95rem; line-height: 1.7; max-width: 450px; margin-left: auto; margin-right: auto;">
                        Nail & Lash Studio kalitesini deneyimlemek için hemen randevu alın. İşlemlerinizi kolayca takip edebilmek için üye girişi yapmanız gerekmektedir.
                    </p>
                    <a href="login.html" class="btn btn-primary" style="padding: 12px 40px; font-weight: 600;">Giriş Yap / Üye Ol</a>
                </div>
            `;
        }
    }
});

async function loadServicesForForm() {
    const services = await loadServices();
    const serviceSelect = document.getElementById('service');
    
    if (!serviceSelect) return;
    
    // Statik eklenmiş seçenekleri temizle (sadece placeholder kalsın)
    serviceSelect.innerHTML = '<option value="">-- Hizmet Seçin --</option>';
    
    services.forEach(service => {
        const option = document.createElement('option');
        option.value = service.id;
        option.textContent = `${service.name} - ₺${service.price} (${service.durationMinutes} dk)`;
        serviceSelect.appendChild(option);
    });
}

async function handleServiceChange() {
    const serviceId = document.getElementById('service').value;
    const dateInput = document.getElementById('appointmentDate');
    const timeSelect = document.getElementById('appointmentTime');
    
    if (!serviceId) {
        timeSelect.innerHTML = '<option value="">-- Saat Seçin --</option>';
        return;
    }
    
    if (serviceId && dateInput.value) {
        await loadAvailableTimes(serviceId, dateInput.value);
    } else {
        timeSelect.innerHTML = '<option value="">-- Tarih Seçin --</option>';
    }
}

async function handleDateChange() {
    const serviceId = document.getElementById('service').value;
    const dateValue = document.getElementById('appointmentDate').value;
    const timeSelect = document.getElementById('appointmentTime');
    
    if (!dateValue) {
        timeSelect.innerHTML = '<option value="">-- Saat Seçin --</option>';
        return;
    }
    
    if (serviceId && dateValue) {
        await loadAvailableTimes(serviceId, dateValue);
    } else {
        timeSelect.innerHTML = '<option value="">-- Hizmet Seçin --</option>';
    }
}

async function loadAvailableTimes(serviceId, dateValue) {
    const date = new Date(dateValue).toISOString();
    const result = await apiCall(`/appointments/available-times?serviceId=${serviceId}&date=${date}`);
    
    const timeSelect = document.getElementById('appointmentTime');
    timeSelect.innerHTML = '<option value="">-- Saat Seçin --</option>';
    
    if (result && Array.isArray(result)) {
        result.forEach(timeString => {
            const date = new Date(timeString);
            const time = date.toLocaleTimeString('tr-TR', { hour: '2-digit', minute: '2-digit' });
            
            const option = document.createElement('option');
            option.value = timeString;
            option.textContent = time;
            timeSelect.appendChild(option);
        });
    }
}

async function handleAppointmentSubmit(e) {
    e.preventDefault();

    const serviceId = parseInt(document.getElementById('service').value);
    const appointmentDate = document.getElementById('appointmentTime').value;
    const firstName = document.getElementById('firstName').value;
    const lastName = document.getElementById('lastName').value;
    const email = document.getElementById('email').value;
    const phone = document.getElementById('phone').value;
    const notes = document.getElementById('notes').value;

    if (!serviceId || !appointmentDate) {
        showMessage('Lütfen hizmet ve saat seçiniz.', 'error', 'appointmentMessage');
        return;
    }

    let userId = currentUser?.id;

    // Create appointment
    let appointmentData = {
        userId: userId || 0,
        serviceId: serviceId,
        appointmentDate: appointmentDate,
        notes: notes || ''
    };

    const result = await apiCall('/appointments', 'POST', appointmentData);

    if (result && !result.error) {
        showMessage('Randevunuz başarıyla oluşturulmuştur! Yakında sizinle iletişime geçeceğiz.', 'success', 'appointmentMessage');
        document.getElementById('appointmentForm').reset();
        document.getElementById('appointmentDate').min = getMinDateString();
        // Saat seçim kutusunu sıfırla
        const timeSelect = document.getElementById('appointmentTime');
        if (timeSelect) {
            timeSelect.innerHTML = '<option value="">-- Önce Hizmet & Tarih Seçin --</option>';
        }
        // Randevu geçmişini güncelle
        if (currentUser) {
            loadAppointmentHistory();
        }
    } else {
        showMessage(result?.message || 'Randevu oluşturulurken bir hata oluştu. Lütfen tekrar deneyiniz.', 'error', 'appointmentMessage');
    }
}

async function loadAppointmentHistory() {
    if (!currentUser) return;

    const appointments = await getUserAppointments(currentUser.id);
    const historyContainer = document.getElementById('appointmentHistory');
    const historyList = document.getElementById('historyList');

    if (appointments && appointments.length > 0) {
        historyContainer.style.display = 'block';
        historyList.innerHTML = '';

        appointments.forEach(appointment => {
            const item = document.createElement('div');
            item.className = 'history-item';
            
            const statusClass = getStatusClass(appointment.status);
            const statusLabel = getStatusLabel(appointment.status);
            
            item.innerHTML = `
                <h4>${appointment.service?.name || 'Hizmet'}</h4>
                <p>📅 ${formatDateTime(appointment.appointmentDate)}</p>
                <p>💰 Fiyat: ₺${appointment.service?.price || ''}</p>
                <span class="status ${statusClass}">${statusLabel}</span>
            `;
            
            historyList.appendChild(item);
        });
    }
}

// Add message container to appointments page
document.addEventListener('DOMContentLoaded', function() {
    if (document.getElementById('appointmentForm') && !document.getElementById('appointmentMessage')) {
        const container = document.querySelector('.appointment-form-container');
        if (container) {
            const messageDiv = document.createElement('div');
            messageDiv.id = 'appointmentMessage';
            messageDiv.className = 'auth-message';
            container.parentNode.insertBefore(messageDiv, container);
        }
    }
});
