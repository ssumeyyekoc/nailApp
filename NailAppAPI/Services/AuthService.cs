using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using NailAppAPI.Data;
using NailAppAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NailAppAPI.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;
    private readonly AppDbContext _context;

    public AuthService(UserManager<User> userManager, IConfiguration configuration, AppDbContext context)
    {
        _userManager = userManager;
        _configuration = configuration;
        _context = context;
    }

    public async Task<(bool Success, string? Token, string? Message)> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return (false, null, "Kullanıcı bulunamadı.");

        var result = await _userManager.CheckPasswordAsync(user, password);
        if (!result)
            return (false, null, "Şifre hatalı.");

        var token = await GenerateJwtToken(user);
        return (true, token, "Başarıyla giriş yapıldı.");
    }

    public async Task<(bool Success, string? Message)> RegisterAsync(string email, string password, string firstName, string lastName)
    {
        var existingUser = await _userManager.FindByEmailAsync(email);
        if (existingUser != null)
            return (false, "Bu e-posta adresi zaten kayıtlı.");

        var newUser = new User
        {
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            CreatedAt = DateTime.Now
        };

        var result = await _userManager.CreateAsync(newUser, password);
        if (!result.Succeeded)
            return (false, string.Join(", ", result.Errors.Select(e => e.Description)));

        // Assign Customer role
        await _userManager.AddToRoleAsync(newUser, Role.Customer);

        return (true, "Kayıt başarıyla tamamlandı.");
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _userManager.FindByIdAsync(userId.ToString());
    }

    public async Task LogoutAsync(int userId)
    {
        // Token invalidation logic would go here
        await Task.CompletedTask;
    }

    private async Task<string> GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"]!;
        var issuer = jwtSettings["Issuer"]!;
        var expirationStr = jwtSettings["ExpirationMinutes"];
        var expirationMinutes = string.IsNullOrEmpty(expirationStr) ? 60 : int.Parse(expirationStr);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
        };

        // Kullanıcının rollerini JWT token'a ekle
        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<(bool Success, string? Message)> ResetPasswordDirectAsync(string email, string newPassword)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return (false, "Bu e-posta adresine kayıtlı kullanıcı bulunamadı.");

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
        
        if (!result.Succeeded)
            return (false, string.Join(", ", result.Errors.Select(e => e.Description)));

        return (true, "Şifre başarıyla güncellendi.");
    }
}
