using Microsoft.AspNetCore.Identity;

namespace NailAppAPI.Models;

public class Role : IdentityRole<int>
{
    public const string Admin = "Admin";
    public const string Customer = "Customer";
    public const string Guest = "Guest";

    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
