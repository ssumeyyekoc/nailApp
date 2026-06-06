namespace NailAppAPI.Models;

public class Announcement
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? BadgeText { get; set; }   // Ör: "%20 İndirim!"
    public string? Emoji { get; set; }       // Ör: "🎉"
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
}
