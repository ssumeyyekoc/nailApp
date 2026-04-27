// Services Page Logic

document.addEventListener('DOMContentLoaded', async function() {
    await loadServicesList();
    setupFilterButtons();
});

async function loadServicesList() {
    const container = document.getElementById('servicesFullGrid');
    if (!container) return;

    const services = await loadServices();
    
    container.innerHTML = '';
    
    if (services.length === 0) {
        container.innerHTML = '<p class="no-data">Hizmet bulunamamıştır.</p>';
        return;
    }

    services.forEach(service => {
        const card = document.createElement('div');
        card.className = 'service-detail-card';
        card.setAttribute('data-category', service.categoryId);
        
        card.innerHTML = `
            <h3>${service.name}</h3>
            <p class="description">${service.description || ''}</p>
            <div class="service-info">
                <span class="price">₺${service.price}</span>
                <span class="duration">⏱️ ${service.durationMinutes} dakika</span>
            </div>
            <button class="btn btn-secondary" onclick="goToAppointment(${service.id})">Randevu Al</button>
        `;
        
        container.appendChild(card);
    });
}

function setupFilterButtons() {
    const filterButtons = document.querySelectorAll('.filter-btn');
    
    filterButtons.forEach(button => {
        button.addEventListener('click', function() {
            // Remove active class from all buttons
            filterButtons.forEach(btn => btn.classList.remove('active'));
            // Add active class to clicked button
            this.classList.add('active');
            
            // Filter services
            const categoryId = this.getAttribute('data-category');
            filterServices(categoryId);
        });
    });
}

function filterServices(categoryId) {
    const allCards = document.querySelectorAll('[data-category]');
    
    allCards.forEach(card => {
        if (categoryId === 'all' || card.getAttribute('data-category') === categoryId) {
            card.style.display = 'block';
        } else {
            card.style.display = 'none';
        }
    });
}

function goToAppointment(serviceId) {
    sessionStorage.setItem('selectedServiceId', serviceId);
    window.location.href = 'appointments.html';
}
