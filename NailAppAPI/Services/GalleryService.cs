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
            .Include(g => g.Category)
            .FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<IEnumerable<Gallery>> GetAllAsync()
    {
        return await _context.Galleries
            .Include(g => g.Category)
            .OrderByDescending(g => g.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Gallery>> GetByCategoryAsync(int categoryId)
    {
        return await _context.Galleries
            .Where(g => g.CategoryId == categoryId)
            .Include(g => g.Category)
            .OrderByDescending(g => g.CreatedAt)
            .ToListAsync();
    }

    public async Task<Gallery> CreateAsync(string imageUrl, string? description, int categoryId)
    {
        var gallery = new Gallery
        {
            ImageUrl = imageUrl,
            Description = description,
            CategoryId = categoryId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Galleries.Add(gallery);
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
