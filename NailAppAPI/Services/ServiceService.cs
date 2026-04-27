using Microsoft.EntityFrameworkCore;
using NailAppAPI.Data;
using NailAppAPI.Models;

namespace NailAppAPI.Services;

public class ServiceService : IServiceService
{
    private readonly AppDbContext _context;

    public ServiceService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Service?> GetServiceByIdAsync(int id)
    {
        return await _context.Services
            .Include(s => s.Category)
            .FirstOrDefaultAsync(s => s.Id == id && s.IsActive);
    }

    public async Task<IEnumerable<Service>> GetAllServicesAsync()
    {
        return await _context.Services
            .Where(s => s.IsActive)
            .Include(s => s.Category)
            .ToListAsync();
    }

    public async Task<IEnumerable<Service>> GetServicesByCategoryAsync(int categoryId)
    {
        return await _context.Services
            .Where(s => s.CategoryId == categoryId && s.IsActive)
            .Include(s => s.Category)
            .ToListAsync();
    }

    public async Task<Service> CreateServiceAsync(string name, string description, decimal price, int durationMinutes, int categoryId)
    {
        var service = new Service
        {
            Name = name,
            Description = description,
            Price = price,
            DurationMinutes = durationMinutes,
            CategoryId = categoryId,
            IsActive = true,
            CreatedAt = DateTime.Now
        };

        _context.Services.Add(service);
        await _context.SaveChangesAsync();
        return service;
    }

    public async Task<bool> UpdateServiceAsync(int id, string name, string description, decimal price, int durationMinutes)
    {
        var service = await _context.Services.FindAsync(id);
        if (service == null)
            return false;

        service.Name = name;
        service.Description = description;
        service.Price = price;
        service.DurationMinutes = durationMinutes;
        service.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteServiceAsync(int id)
    {
        var service = await _context.Services.FindAsync(id);
        if (service == null)
            return false;

        service.IsActive = false;
        service.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();
        return true;
    }
}
