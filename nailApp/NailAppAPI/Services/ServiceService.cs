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
            .FirstOrDefaultAsync(s => s.Id == id && s.IsActive);
    }

    public async Task<IEnumerable<Service>> GetAllServicesAsync()
    {
        return await _context.Services
            .Where(s => s.IsActive)
            .ToListAsync();
    }

    public async Task<IEnumerable<Service>> GetServicesByCategoryAsync(int categoryId)
    {
        var strId = categoryId.ToString();
        var services = await _context.Services.Where(s => s.IsActive).ToListAsync();
        return services.Where(s => !string.IsNullOrEmpty(s.CategoryIds) && s.CategoryIds.Split(',').Contains(strId));
    }

    public async Task<Service> CreateServiceAsync(string name, string description, decimal price, int durationMinutes, string categoryIds)
    {
        var service = new Service
        {
            Name = name,
            Description = description,
            Price = price,
            DurationMinutes = durationMinutes,
            CategoryIds = categoryIds,
            IsActive = true,
            CreatedAt = DateTime.Now
        };

        _context.Services.Add(service);
        await _context.SaveChangesAsync();
        return service;
    }

    public async Task<bool> UpdateServiceAsync(int id, string name, string description, decimal price, int durationMinutes, string categoryIds)
    {
        var service = await _context.Services.FindAsync(id);
        if (service == null)
            return false;

        service.Name = name;
        service.Description = description;
        service.Price = price;
        service.DurationMinutes = durationMinutes;
        service.CategoryIds = categoryIds;
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
