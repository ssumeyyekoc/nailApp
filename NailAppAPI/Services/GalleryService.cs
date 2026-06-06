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

    public async Task<Gallery?> GetByIdAsync(int id)
    {
        return await _context.Galleries
            .FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<IEnumerable<Gallery>> GetAllAsync()
    {
        return await _context.Galleries
            .OrderByDescending(g => g.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Gallery>> GetByCategoryAsync(int categoryId)
    {
        var strId = categoryId.ToString();
        var items = await _context.Galleries.ToListAsync();
        return items.Where(g => !string.IsNullOrEmpty(g.CategoryIds) && g.CategoryIds.Split(',').Contains(strId)).OrderByDescending(g => g.CreatedAt);
    }

    public async Task<Gallery> CreateAsync(string imageUrl, string? description, string categoryIds)
    {
        var gallery = new Gallery
        {
            ImageUrl = imageUrl,
            Description = description,
            CategoryIds = categoryIds,
            CreatedAt = DateTime.UtcNow
        };

        _context.Galleries.Add(gallery);
        await _context.SaveChangesAsync();
        return gallery;
    }

    public async Task<Gallery?> UpdateAsync(int id, string? imageUrl, string? description, string categoryIds)
    {
        var gallery = await _context.Galleries.FindAsync(id);
        if (gallery == null)
            return null;

        if (!string.IsNullOrEmpty(imageUrl))
        {
            gallery.ImageUrl = imageUrl;
        }
        gallery.Description = description;
        gallery.CategoryIds = categoryIds;

        _context.Galleries.Update(gallery);
        await _context.SaveChangesAsync();
        return gallery;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var gallery = await _context.Galleries.FindAsync(id);
        if (gallery == null)
            return false;

        _context.Galleries.Remove(gallery);
        await _context.SaveChangesAsync();
        return true;
    }
}
