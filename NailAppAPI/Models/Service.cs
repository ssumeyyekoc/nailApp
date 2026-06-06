namespace NailAppAPI.Models;

public class Service
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int DurationMinutes { get; set; }
    public string CategoryIds { get; set; } = string.Empty; // Virgülle ayrılmış kategori ID'leri (örn: "1,2,3")
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<Appointment>? Appointments { get; set; }
}
