
using System.ComponentModel.DataAnnotations;

namespace NailAppAPI.Models
{
    public class Gallery
    {
        [Key] // Veritabanına "Kimlik numarası kesinlikle budur!" diye emir veriyoruz
        public int Id { get; set; }
        
        public string ImageUrl { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string CategoryIds { get; set; } = string.Empty; // Virgülle ayrılmış kategori ID'leri (örn: "1,2,3")
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}