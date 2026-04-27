using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NailAppAPI.Models;

namespace NailAppAPI.Data;

public class AppDbContext : IdentityDbContext<User, Role, int>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) 
        : base(options)
    {
    }

    public DbSet<Service> Services { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Appointment> Appointments { get; set; } = null!;
    public DbSet<GalleryImage> GalleryImages { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Category configuration
        modelBuilder.Entity<Category>()
            .HasKey(c => c.Id);

        // Service configuration
        modelBuilder.Entity<Service>()
            .HasKey(s => s.Id);
        modelBuilder.Entity<Service>()
            .HasOne(s => s.Category)
            .WithMany(c => c.Services)
            .HasForeignKey(s => s.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        // Appointment configuration
        modelBuilder.Entity<Appointment>()
            .HasKey(a => a.Id);
        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.User)
            .WithMany(u => u.Appointments)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Service)
            .WithMany(s => s.Appointments)
            .HasForeignKey(a => a.ServiceId)
            .OnDelete(DeleteBehavior.Cascade);

        // GalleryImage configuration
        modelBuilder.Entity<GalleryImage>()
            .HasKey(g => g.Id);
        modelBuilder.Entity<GalleryImage>()
            .HasOne(g => g.Category)
            .WithMany(c => c.GalleryImages)
            .HasForeignKey(g => g.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        // Seed default roles
        var roles = new[]
        {
            new Role 
            { 
                Id = 1, 
                Name = Role.Admin, 
                NormalizedName = Role.Admin.ToUpper(),
                Description = "Administrator"
            },
            new Role 
            { 
                Id = 2, 
                Name = Role.Customer, 
                NormalizedName = Role.Customer.ToUpper(),
                Description = "Registered Customer"
            },
            new Role 
            { 
                Id = 3, 
                Name = Role.Guest, 
                NormalizedName = Role.Guest.ToUpper(),
                Description = "Guest User"
            }
        };

        modelBuilder.Entity<Role>().HasData(roles);

        // Seed default categories
        var categories = new[]
        {
            new Category { Id = 1, Name = "Jel", Description = "Jel tırnak hizmetleri", IsActive = true },
            new Category { Id = 2, Name = "Protez", Description = "Protez tırnak hizmetleri", IsActive = true },
            new Category { Id = 3, Name = "Nail Art", Description = "Nail Art tasarımları", IsActive = true },
            new Category { Id = 4, Name = "Kirpik Lifting", Description = "Kirpik lifting hizmetleri", IsActive = true }
        };

        modelBuilder.Entity<Category>().HasData(categories);
    }
}
