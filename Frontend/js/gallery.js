// Gallery Page Logic

document.addEventListener('DOMContentLoaded', async function() {
    await loadGallery();
    setupFilterButtons();
    setupLightbox();
});

async function loadGallery() {
    const container = document.getElementById('galleryFullGrid');
    if (!container) return;

    const images = await loadGalleryImages();
    
    container.innerHTML = '';
    
    if (images.length === 0) {
        container.innerHTML = '<p class="no-data">Galeri görüntüsü bulunamamıştır.</p>';
        return;
    }

    images.forEach(image => {
        const item = document.createElement('div');
        item.className = 'gallery-full-item';
        item.setAttribute('data-category', image.categoryId);
        
        item.innerHTML = `
            <img src="${image.imageUrl}" alt="${image.title}" onclick="openLightbox('${image.imageUrl}', '${image.title || 'Galeri Görüntüsü'}')">
        `;
        
        container.appendChild(item);
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
            
            // Filter gallery
            const categoryId = this.getAttribute('data-category');
            filterGallery(categoryId);
        });
    });
}

function filterGallery(categoryId) {
    const allItems = document.querySelectorAll('[data-category]');
    
    allItems.forEach(item => {
        if (categoryId === 'all' || item.getAttribute('data-category') === categoryId) {
            item.style.display = 'block';
        } else {
            item.style.display = 'none';
        }
    });
}

// ============ LIGHTBOX ============
function setupLightbox() {
    const lightbox = document.getElementById('lightbox');
    const closeBtn = document.querySelector('.close');
    
    if (closeBtn) {
        closeBtn.addEventListener('click', closeLightbox);
    }
    
    if (lightbox) {
        lightbox.addEventListener('click', function(e) {
            if (e.target === lightbox) {
                closeLightbox();
            }
        });
    }
    
    // Keyboard navigation
    document.addEventListener('keydown', function(e) {
        if (e.key === 'Escape') {
            closeLightbox();
        }
    });
}

function openLightbox(imageSrc, caption) {
    const lightbox = document.getElementById('lightbox');
    const lightboxImg = document.getElementById('lightboxImg');
    const lightboxCaption = document.getElementById('lightboxCaption');
    
    lightbox.style.display = 'block';
    lightboxImg.src = imageSrc;
    lightboxCaption.textContent = caption;
}

function closeLightbox() {
    const lightbox = document.getElementById('lightbox');
    lightbox.style.display = 'none';
}
