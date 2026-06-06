using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using NailAppAPI.Data;
using NailAppAPI.Models;
using NailAppAPI.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database Configuration (Fallback to SQLite if provider is not SQL Server)
var dbProvider = builder.Configuration["DatabaseProvider"] ?? "Sqlite";
builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (dbProvider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    }
    else
    {
        options.UseSqlite(builder.Configuration.GetConnectionString("SqliteConnection") ?? "Data Source=nailapp.db");
    }
});

// Identity
builder.Services.AddIdentity<User, Role>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = true;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];
var issuer = jwtSettings["Issuer"];
var audience = jwtSettings["Audience"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!))
        };
    });

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Service DI Registrations
builder.Services.AddScoped<IServiceService, ServiceService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IGalleryService, GalleryService>();

var app = builder.Build();

// Global Exception Handler (Güvenlik ve Hata Loglama)
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        var contextFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        if (contextFeature != null)
        {
            Console.WriteLine($"[GLOBAL ERROR LOG]: {contextFeature.Error.Message}\n{contextFeature.Error.StackTrace}");
            await context.Response.WriteAsJsonAsync(new { message = "Sunucuda beklenmedik bir hata oluştu. Lütfen daha sonra tekrar deneyiniz." });
        }
    });
});

var frontendPath = Path.Combine(builder.Environment.ContentRootPath, "..", "Frontend");
var frontendProvider = new PhysicalFileProvider(frontendPath);

app.UseDefaultFiles(new DefaultFilesOptions
{
    FileProvider = frontendProvider,
    RequestPath = string.Empty
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = frontendProvider,
    RequestPath = string.Empty
});

// Configure the HTTP request pipeline
app.UseSwagger();
app.UseSwaggerUI();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowAll");
app.UseStaticFiles(); // wwwroot/uploads içindeki resimleri sunmak için
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Database Migration & Seed Admin User
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    // Admin kullanıcısını seed'le
    var userManager = services.GetRequiredService<UserManager<User>>();
    var roleManager = services.GetRequiredService<RoleManager<Role>>();

    // Rolleri oluştur (eğer yoksa)
    if (!await roleManager.RoleExistsAsync("Admin"))
        await roleManager.CreateAsync(new Role { Name = "Admin", Description = "Sistem yöneticisi" });
    if (!await roleManager.RoleExistsAsync("Customer"))
        await roleManager.CreateAsync(new Role { Name = "Customer", Description = "Kayıtlı müşteri" });

    // Eski admin kullanıcısını temizle
    var oldAdminEmail = "admin@nailstudio.com";
    var oldAdmin = await userManager.FindByEmailAsync(oldAdminEmail);
    if (oldAdmin != null)
    {
        var delRes = await userManager.DeleteAsync(oldAdmin);
        Console.WriteLine($"[SEED LOG] Old admin deleted: {delRes.Succeeded}");
    }

    // Varsayılan admin kullanıcısını oluştur
    var adminEmail = "sumeyye@gmail.com";
    var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
    if (existingAdmin == null)
    {
        var adminUser = new User
        {
            UserName = adminEmail,
            Email = adminEmail,
            FirstName = "Admin",
            LastName = "Yönetici",
            EmailConfirmed = true
        };
        var result = await userManager.CreateAsync(adminUser, "123451!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
            Console.WriteLine("[SEED LOG] New admin created successfully!");
        }
        else
        {
            Console.WriteLine($"[SEED LOG] New admin creation failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
    }
    else
    {
        // Şifresini ve rolünü güncelle
        var token = await userManager.GeneratePasswordResetTokenAsync(existingAdmin);
        var resetRes = await userManager.ResetPasswordAsync(existingAdmin, token, "123451!");
        Console.WriteLine($"[SEED LOG] Existing admin password reset: {resetRes.Succeeded}");
        if (!resetRes.Succeeded)
        {
            Console.WriteLine($"[SEED LOG] Password reset failed: {string.Join(", ", resetRes.Errors.Select(e => e.Description))}");
        }
        if (!await userManager.IsInRoleAsync(existingAdmin, "Admin"))
        {
            await userManager.AddToRoleAsync(existingAdmin, "Admin");
            Console.WriteLine("[SEED LOG] Existing admin added to Admin role.");
        }
    }
}

app.Run("http://localhost:5999");