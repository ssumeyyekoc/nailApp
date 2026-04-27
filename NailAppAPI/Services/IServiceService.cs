using NailAppAPI.Models;

namespace NailAppAPI.Services;

public interface IServiceService
{
    Task<Service?> GetServiceByIdAsync(int id);
    Task<IEnumerable<Service>> GetAllServicesAsync();
    Task<IEnumerable<Service>> GetServicesByCategoryAsync(int categoryId);
    Task<Service> CreateServiceAsync(string name, string description, decimal price, int durationMinutes, int categoryId);
    Task<bool> UpdateServiceAsync(int id, string name, string description, decimal price, int durationMinutes);
    Task<bool> DeleteServiceAsync(int id);
}
