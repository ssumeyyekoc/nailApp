using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using NailAppAPI.Models;

namespace NailAppAPI.Data;

public class AppDbContext : IdentityDbContext<User, Role, int>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Service> Services { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Appointment> Appointments { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 1. ÖNCE ÖRNEK BİR KATEGORİ OLUŞTURUYORUZ (Foreign Key Hatasını Çözmek İçin)
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Tırnak Bakımı" }
        );

        // 2. HİZMETLERİ BU KATEGORİYE BAĞLIYORUZ (CategoryId = 1 yaptık)
        modelBuilder.Entity<Service>().HasData(
            new Service { Id = 1, Name = "Kalıcı Oje", Price = 400, CategoryId = 1 },
            new Service { Id = 2, Name = "Protez Tırnak", Price = 800, CategoryId = 1 },
            new Service { Id = 3, Name = "Manikür", Price = 300, CategoryId = 1 }
        );

        // 3. Service İlişkileri
        modelBuilder.Entity<Service>().HasKey(s => s.Id);
        
        modelBuilder.Entity<Service>()
            .HasOne(s => s.Category)
            .WithMany(c => c.Services)
            .HasForeignKey(s => s.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        // 4. Appointment İlişkileri
        modelBuilder.Entity<Appointment>().HasKey(a => a.Id);
        
        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.User)
            .WithMany(u => u.Appointments);
    }
}