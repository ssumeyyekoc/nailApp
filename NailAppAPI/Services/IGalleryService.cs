using NailAppAPI.Models;

namespace NailAppAPI.Services;

public interface IGalleryService
{
    Task<GalleryImage?> GetGalleryImageByIdAsync(int id);
    Task<IEnumerable<GalleryImage>> GetAllGalleryImagesAsync();
    Task<IEnumerable<GalleryImage>> GetGalleryImagesByCategoryAsync(int categoryId);
    Task<IEnumerable<GalleryImage>> GetHighlightedImagesAsync();
    Task<GalleryImage> CreateGalleryImageAsync(string title, string description, string imageUrl, int categoryId);
    Task<bool> UpdateGalleryImageAsync(int id, string title, string description, bool isHighlighted);
    Task<bool> DeleteGalleryImageAsync(int id);
}
