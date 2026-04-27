using Microsoft.EntityFrameworkCore;
using NailAppAPI.Data;
using NailAppAPI.Models;

namespace NailAppAPI.Services;

public class GalleryService : IGalleryService
{
    private readonly AppDbContext _context;

    public GalleryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<GalleryImage?> GetGalleryImageByIdAsync(int id)
    {
        return await _context.GalleryImages
            .Include(g => g.Category)
            .FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<IEnumerable<GalleryImage>> GetAllGalleryImagesAsync()
    {
        return await _context.GalleryImages
            .Include(g => g.Category)
            .OrderBy(g => g.DisplayOrder)
            .ToListAsync();
    }

    public async Task<IEnumerable<GalleryImage>> GetGalleryImagesByCategoryAsync(int categoryId)
    {
        return await _context.GalleryImages
            .Where(g => g.CategoryId == categoryId)
            .Include(g => g.Category)
            .OrderBy(g => g.DisplayOrder)
            .ToListAsync();
    }

    public async Task<IEnumerable<GalleryImage>> GetHighlightedImagesAsync()
    {
        return await _context.GalleryImages
            .Where(g => g.IsHighlighted)
            .Include(g => g.Category)
            .OrderBy(g => g.DisplayOrder)
            .ToListAsync();
    }

    public async Task<GalleryImage> CreateGalleryImageAsync(string title, string description, string imageUrl, int categoryId)
    {
        var maxOrder = await _context.GalleryImages
            .MaxAsync(g => (int?)g.DisplayOrder) ?? 0;

        var galleryImage = new GalleryImage
        {
            Title = title,
            Description = description,
            ImageUrl = imageUrl,
            CategoryId = categoryId,
            DisplayOrder = maxOrder + 1,
            CreatedAt = DateTime.Now
        };

        _context.GalleryImages.Add(galleryImage);
        await _context.SaveChangesAsync();
        return galleryImage;
    }

    public async Task<bool> UpdateGalleryImageAsync(int id, string title, string description, bool isHighlighted)
    {
        var galleryImage = await _context.GalleryImages.FindAsync(id);
        if (galleryImage == null)
            return false;

        galleryImage.Title = title;
        galleryImage.Description = description;
        galleryImage.IsHighlighted = isHighlighted;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteGalleryImageAsync(int id)
    {
        var galleryImage = await _context.GalleryImages.FindAsync(id);
        if (galleryImage == null)
            return false;

        _context.GalleryImages.Remove(galleryImage);
        await _context.SaveChangesAsync();
        return true;
    }
}
