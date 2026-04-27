namespace NailAppAPI.Models;

public class GalleryImage
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public int CategoryId { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsHighlighted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Navigation properties
    public Category? Category { get; set; }
}
