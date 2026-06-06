
using System.ComponentModel.DataAnnotations;

namespace NailAppAPI.Models
{
    public class Gallery
    {
        [Key] // Veritabanına "Kimlik numarası kesinlikle budur!" diye emir veriyoruz
        public int Id { get; set; }
        
        public string ImageUrl { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}