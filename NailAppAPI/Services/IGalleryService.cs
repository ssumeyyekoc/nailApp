using NailAppAPI.Models;

namespace NailAppAPI.Services;

public interface IGalleryService
{
    Task<Gallery?> GetByIdAsync(int id);
    Task<IEnumerable<Gallery>> GetAllAsync();
    Task<IEnumerable<Gallery>> GetByCategoryAsync(int categoryId);
    Task<Gallery> CreateAsync(string imageUrl, string? description, int categoryId);
    Task<Gallery?> UpdateAsync(int id, string? imageUrl, string? description, int categoryId);
    Task<bool> DeleteAsync(int id);
}
