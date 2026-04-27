// Appointments Page Logic

document.addEventListener('DOMContentLoaded', async function() {
    const selectedServiceId = sessionStorage.getItem('selectedServiceId');
    
    await loadServicesForForm();
    
    if (selectedServiceId) {
        document.getElementById('service').value = selectedServiceId;
        sessionStorage.removeItem('selectedServiceId');
    }

    document.getElementById('appointmentDate').min = getMinDateString();
    
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
        // Pre-fill user info
        document.getElementById('firstName').value = currentUser.firstName || '';
        document.getElementById('lastName').value = currentUser.lastName || '';
        document.getElementById('email').value = currentUser.email || '';
        document.getElementById('phone').value = currentUser.phoneNumber || '';
        
        // Load appointment history
        loadAppointmentHistory();
    }
});

async function loadServicesForForm() {
    const services = await loadServices();
    const serviceSelect = document.getElementById('service');
    
    if (!serviceSelect) return;
    
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
    
    if (serviceId && dateInput.value) {
        await loadAvailableTimes(serviceId, dateInput.value);
    }
}

async function handleDateChange() {
    const serviceId = document.getElementById('service').value;
    const dateValue = document.getElementById('appointmentDate').value;
    
    if (serviceId && dateValue) {
        await loadAvailableTimes(serviceId, dateValue);
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

    if (result) {
        showMessage('Randevunuz başarıyla oluşturulmuştur! Yakında size iletişime geçeceğiz.', 'success', 'appointmentMessage');
        document.getElementById('appointmentForm').reset();
        document.getElementById('appointmentDate').min = getMinDateString();
    } else {
        showMessage('Randevu oluşturulurken bir hata oluştu. Lütfen tekrar deneyiniz.', 'error', 'appointmentMessage');
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
