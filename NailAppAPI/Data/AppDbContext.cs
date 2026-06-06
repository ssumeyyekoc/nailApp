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
    public DbSet<Gallery> Galleries { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 1. Rolleri Seed'le
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Admin", NormalizedName = "ADMIN", Description = "Sistem yöneticisi", ConcurrencyStamp = "admin-stamp" },
            new Role { Id = 2, Name = "Customer", NormalizedName = "CUSTOMER", Description = "Kayıtlı müşteri", ConcurrencyStamp = "customer-stamp" }
        );

        // 2. Kategorileri Seed'le
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Jel Tırnak", Description = "Jel tırnak hizmetleri" },
            new Category { Id = 2, Name = "Protez Tırnak", Description = "Protez tırnak hizmetleri" },
            new Category { Id = 3, Name = "Nail Art", Description = "Nail art tasarımları" },
            new Category { Id = 4, Name = "Kirpik Lifting", Description = "Kirpik lifting hizmetleri" },
            new Category { Id = 5, Name = "Manikür & Pedikür", Description = "Manikür ve pedikür hizmetleri" }
        );

        // 3. Hizmetleri Seed'le
        modelBuilder.Entity<Service>().HasData(
            new Service { Id = 1, Name = "Kalıcı Oje", Description = "Uzun ömürlü kalıcı oje uygulaması", Price = 400, DurationMinutes = 45, CategoryId = 5 },
            new Service { Id = 2, Name = "Protez Tırnak", Description = "Doğal görünümlü protez tırnak tasarımı", Price = 800, DurationMinutes = 90, CategoryId = 2 },
            new Service { Id = 3, Name = "Manikür", Description = "Klasik manikür bakımı", Price = 300, DurationMinutes = 30, CategoryId = 5 },
            new Service { Id = 4, Name = "Jel Tırnak Uygulaması", Description = "Yüksek kaliteli jel malzemeleri ile uzun ömürlü uygulama", Price = 500, DurationMinutes = 60, CategoryId = 1 },
            new Service { Id = 5, Name = "Nail Art Tasarım", Description = "Özel nail art tasarımları ve süsleme", Price = 600, DurationMinutes = 75, CategoryId = 3 },
            new Service { Id = 6, Name = "Kirpik Lifting", Description = "Kirpiklerinizi kıvırma ve hacimlendirme", Price = 350, DurationMinutes = 45, CategoryId = 4 }
        );

        // 4. Service İlişkileri
        modelBuilder.Entity<Service>().HasKey(s => s.Id);
        
        modelBuilder.Entity<Service>()
            .HasOne(s => s.Category)
            .WithMany(c => c.Services)
            .HasForeignKey(s => s.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        // 5. Appointment İlişkileri
        modelBuilder.Entity<Appointment>().HasKey(a => a.Id);
        
        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.User)
            .WithMany(u => u.Appointments)
            .HasForeignKey(a => a.UserId);

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Service)
            .WithMany(s => s.Appointments)
            .HasForeignKey(a => a.ServiceId);

        // 6. Gallery İlişkileri
        modelBuilder.Entity<Models.Gallery>().HasKey(g => g.Id);
        
        modelBuilder.Entity<Models.Gallery>()
            .HasOne(g => g.Category)
            .WithMany()
            .HasForeignKey(g => g.CategoryId);
    }
}