// Gallery Page Logic

document.addEventListener('DOMContentLoaded', async function() {
    await loadCategoriesForGalleryFilter();
    await loadGalleryItems();
    setupGalleryFilterButtons();
});

// Kategorileri API'den çekip filtre butonlarını oluştur
async function loadCategoriesForGalleryFilter() {
    const filterSection = document.getElementById('galleryFilterSection');
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

// Galeri öğelerini API'den çek ve göster
async function loadGalleryItems() {
    const container = document.getElementById('galleryGrid');
    if (!container) return;

    const items = await apiCall('/gallery');
    
    container.innerHTML = '';
    
    if (!items || items.length === 0) {
        container.innerHTML = '<p class="no-data">Henüz galeri öğesi bulunmamaktadır.</p>';
        return;
    }

    items.forEach(item => {
        const galleryItem = document.createElement('div');
        galleryItem.className = 'gallery-full-item';
        galleryItem.setAttribute('data-category', item.categoryId);
        
        // API'deki imageUrl backend sunucusundan gelecek
        const imageUrl = `${API_URL.replace('/api', '')}${item.imageUrl}`;
        
        galleryItem.innerHTML = `
            <img src="${imageUrl}" 
                 alt="${item.description || 'Tırnak modeli'}" 
                 onclick="openLightbox('${imageUrl}', '${item.description || ''}')"
                 loading="lazy">
        `;
        
        container.appendChild(galleryItem);
    });
}

// Filtre butonları event delegation
function setupGalleryFilterButtons() {
    const filterSection = document.getElementById('galleryFilterSection');
    if (!filterSection) return;

    filterSection.addEventListener('click', function(e) {
        if (e.target.classList.contains('filter-btn')) {
            filterSection.querySelectorAll('.filter-btn').forEach(btn => btn.classList.remove('active'));
            e.target.classList.add('active');
            
            const categoryId = e.target.getAttribute('data-category');
            filterGalleryItems(categoryId);
        }
    });
}

function filterGalleryItems(categoryId) {
    const allItems = document.querySelectorAll('.gallery-full-item');
    
    allItems.forEach(item => {
        if (categoryId === 'all' || item.getAttribute('data-category') === categoryId) {
            item.style.display = 'block';
        } else {
            item.style.display = 'none';
        }
    });
}

// Lightbox fonksiyonları
function openLightbox(imageUrl, caption) {
    const lightbox = document.getElementById('lightbox');
    const lightboxImg = document.getElementById('lightboxImg');
    const lightboxCaption = document.getElementById('lightboxCaption');
    
    lightbox.style.display = 'block';
    lightboxImg.src = imageUrl;
    lightboxCaption.textContent = caption;
    
    // ESC tuşu ile kapat
    document.addEventListener('keydown', handleEscKey);
}

function closeLightbox() {
    document.getElementById('lightbox').style.display = 'none';
    document.removeEventListener('keydown', handleEscKey);
}

function handleEscKey(e) {
    if (e.key === 'Escape') {
        closeLightbox();
    }
}
