// Services Page Logic

document.addEventListener('DOMContentLoaded', async function() {
    await loadCategoriesForFilter();
    await loadServicesList();
    setupFilterButtons();
});

// Kategorileri API'den çekip filtre butonlarını oluştur
async function loadCategoriesForFilter() {
    const filterSection = document.getElementById('filterSection');
    if (!filterSection) return;

    const categories = await apiCall('/categories');
    if (categories && categories.length > 0) {
        categories.forEach(category => {
            const btn = document.createElement('button');
            btn.className = 'filter-btn';
            btn.setAttribute('data-category', category.id);
            btn.textContent = category.name;
            filterSection.appendChild(btn);
        });
    }
}

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

// Filtre butonları event delegation (Çoklu Kategori Seçimi)
function setupFilterButtons() {
    const filterSection = document.getElementById('filterSection');
    if (!filterSection) return;

    filterSection.addEventListener('click', function(e) {
        if (e.target.classList.contains('filter-btn')) {
            const clickedBtn = e.target;
            const categoryId = clickedBtn.getAttribute('data-category');
            const allBtn = filterSection.querySelector('[data-category="all"]');

            if (categoryId === 'all') {
                // "Tümü" seçilirse diğerlerini kapat
                filterSection.querySelectorAll('.filter-btn').forEach(btn => btn.classList.remove('active'));
                allBtn.classList.add('active');
            } else {
                // Diğer kategoriler seçilirse: toggle et, "Tümü" aktifse deaktif yap
                clickedBtn.classList.toggle('active');
                allBtn.classList.remove('active');

                // Eğer aktif olan hiçbir kategori kalmadıysa otomatik "Tümü" seçilsin
                const activeButtons = filterSection.querySelectorAll('.filter-btn.active');
                if (activeButtons.length === 0) {
                    allBtn.classList.add('active');
                }
            }

            // Aktif kategorileri topla ve filtrele
            const activeCats = Array.from(filterSection.querySelectorAll('.filter-btn.active'))
                                    .map(btn => btn.getAttribute('data-category'));
            
            filterServices(activeCats);
        }
    });
}

function filterServices(activeCategories) {
    const allCards = document.querySelectorAll('[data-category]');
    
    allCards.forEach(card => {
        const cardCat = card.getAttribute('data-category');
        if (activeCategories.includes('all') || activeCategories.includes(cardCat)) {
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
