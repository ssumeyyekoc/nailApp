using NailAppAPI.Models;

namespace NailAppAPI.Services;

public interface IAuthService
{
    Task<(bool Success, string? Token, string? Message)> LoginAsync(string email, string password);
    Task<(bool Success, string? Message)> RegisterAsync(string email, string password, string firstName, string lastName);
    Task<User?> GetUserByIdAsync(int userId);
    Task LogoutAsync(int userId);
}
