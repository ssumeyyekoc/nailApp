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
        galleryItem.setAttribute('data-category', item.categoryIds || '');
        
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

// Filtre butonları event delegation (Çoklu Kategori Seçimi)
function setupGalleryFilterButtons() {
    const filterSection = document.getElementById('galleryFilterSection');
    if (!filterSection) return;

    filterSection.addEventListener('click', function(e) {
        if (e.target.classList.contains('filter-btn')) {
            const clickedBtn = e.target;
            const categoryId = clickedBtn.getAttribute('data-category');
            const allBtn = filterSection.querySelector('[data-category="all"]');

            if (categoryId === 'all') {
                // "Tümü" seçilirse diğer tüm kategorileri kaldır, sadece "Tümü" kalsın
                filterSection.querySelectorAll('.filter-btn').forEach(btn => btn.classList.remove('active'));
                allBtn.classList.add('active');
            } else {
                // Özel kategori tıklandığında: aktif durumunu değiştir, "Tümü" aktifse deaktif yap
                clickedBtn.classList.toggle('active');
                allBtn.classList.remove('active');

                // Eğer aktif olan hiçbir kategori kalmadıysa otomatik "Tümü" seçilsin
                const activeButtons = filterSection.querySelectorAll('.filter-btn.active');
                if (activeButtons.length === 0) {
                    allBtn.classList.add('active');
                }
            }

            // Seçilen tüm aktif kategorilerin ID'lerini diziye dönüştür
            const activeCats = Array.from(filterSection.querySelectorAll('.filter-btn.active'))
                                    .map(btn => btn.getAttribute('data-category'));
            
            filterGalleryItems(activeCats);
        }
    });
}

function filterGalleryItems(activeCategories) {
    const allItems = document.querySelectorAll('.gallery-full-item');
    
    allItems.forEach(item => {
        const itemCat = item.getAttribute('data-category'); // örn: "1,2" veya "5"
        if (activeCategories.includes('all')) {
            item.style.display = 'block';
            return;
        }

        const itemCatsArray = itemCat ? itemCat.split(',') : [];
        const hasMatch = itemCatsArray.some(cat => activeCategories.includes(cat));

        if (hasMatch) {
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

// Lightbox kapatma
function closeLightbox() {
    document.getElementById('lightbox').style.display = 'none';
    document.removeEventListener('keydown', handleEscKey);
}

function handleEscKey(e) {
    if (e.key === 'Escape') {
        closeLightbox();
    }
}
