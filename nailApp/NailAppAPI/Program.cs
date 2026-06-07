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

// Hata Loglama Dizini
var logDirectory = Path.Combine(builder.Environment.ContentRootPath, "logs");
Directory.CreateDirectory(logDirectory);
var logFilePath = Path.Combine(logDirectory, "errors.log");

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
            var logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [ERROR] {contextFeature.Error.Message}\n{contextFeature.Error.StackTrace}\n{new string('-', 80)}\n";
            Console.WriteLine($"[GLOBAL ERROR LOG]: {contextFeature.Error.Message}\n{contextFeature.Error.StackTrace}");
            try { await File.AppendAllTextAsync(logFilePath, logMessage); } catch { /* Log yazılamazsa yoksay */ }
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

    // Seeding gallery images dynamically if database is empty
    if (!db.Galleries.Any())
    {
        var galleryItems = new List<Gallery>
        {
            // Initial 23 Images
            new Gallery { ImageUrl = "/uploads/gallery/05fc194f-3d61-4311-b18a-b3cdcbceed06.jpeg", Description = "Şık Jel Tırnak Tasarımı", CategoryIds = "1,3", CreatedAt = DateTime.UtcNow },
            new Gallery { ImageUrl = "/uploads/gallery/0ea416ce-0024-4ce5-95f2-d6ecd38e59ce.jpeg", Description = "Minimalist Nail Art", CategoryIds = "1,3", CreatedAt = DateTime.UtcNow },
            new Gallery { ImageUrl = "/uploads/gallery/2e5912c6-1e91-4101-8124-216c0395b82e.png", Description = "Protez Tırnak ve Nail Art Uygulaması", CategoryIds = "2,3", CreatedAt = DateTime.UtcNow },
            new Gallery { ImageUrl = "/uploads/gallery/35d29b6b-6b08-451a-b091-967fd89313ed.png", Description = "Manikür ve Nail Art Kombini", CategoryIds = "3,5", CreatedAt = DateTime.UtcNow },
            new Gallery { ImageUrl = "/uploads/gallery/38951a79-11c8-40e1-b08b-a14fa555413d.png", Description = "Doğal Görünümlü Protez Tırnak", CategoryIds = "2", CreatedAt = DateTime.UtcNow },
            new Gallery { ImageUrl = "/uploads/gallery/4a465945-f08e-41ae-841f-1df6e1a990f5.png", Description = "Modern Jel Tırnak Uygulaması", CategoryIds = "1,3", CreatedAt = DateTime.UtcNow },
            new Gallery { ImageUrl = "/uploads/gallery/4cf83459-d332-4854-9a0f-bf8b2e43236f.png", Description = "Hacimli Kirpik Lifting", CategoryIds = "4", CreatedAt = DateTime.UtcNow },
            new Gallery { ImageUrl = "/uploads/gallery/6028f7af-d753-4396-b7dc-585c4ec1ab5f.png", Description = "Mat Protez Tırnak Tasarımı", CategoryIds = "2", CreatedAt = DateTime.UtcNow },
            new Gallery { ImageUrl = "/uploads/gallery/75687327-fa65-47f8-bb11-6af57687979a.png", Description = "Klasik Pedikür & Manikür Bakımı", CategoryIds = "5", CreatedAt = DateTime.UtcNow },
            new Gallery { ImageUrl = "/uploads/gallery/7b72543c-5ec4-4f1f-a7ee-43553bad35aa.png", Description = "Renkli Nail Art Çalışması", CategoryIds = "1,3", CreatedAt = DateTime.UtcNow },
            new Gallery { ImageUrl = "/uploads/gallery/88d7da35-25b6-4b31-a13a-108a5cdf5069.jpeg", Description = "Kirpik Lifting Bakım ve Kıvırma", CategoryIds = "4", CreatedAt = DateTime.UtcNow },
            new Gallery { ImageUrl = "/uploads/gallery/8a4ec80f-4c09-4ebe-ab8b-8472f6e9a641.png", Description = "Işıltılı Protez Tırnak Modeli", CategoryIds = "2,3", CreatedAt = DateTime.UtcNow },
            new Gallery { ImageUrl = "/uploads/gallery/8e6b4006-0456-49cc-9712-27e17a1092f4.png", Description = "Sezon Trendi Jel Tırnaklar", CategoryIds = "1,3", CreatedAt = DateTime.UtcNow },
            new Gallery { ImageUrl = "/uploads/gallery/8edeb9c7-b529-4f45-9817-9e8dc12ff97e.png", Description = "Akrilik Protez Tırnak", CategoryIds = "2", CreatedAt = DateTime.UtcNow },
            new Gallery { ImageUrl = "/uploads/gallery/97eb19e8-1219-4a80-a60f-a83300f7db99.png", Description = "Özel Bakım Manikür", CategoryIds = "5", CreatedAt = DateTime.UtcNow },
            new Gallery { ImageUrl = "/uploads/gallery/aba25623-6c93-4db2-a4b2-b0288edd4685.png", Description = "Çiçek Motifli Nail Art", CategoryIds = "3", CreatedAt = DateTime.UtcNow },
            new Gallery { ImageUrl = "/uploads/gallery/b1142638-0169-4d3e-b730-871ac40e75cc.png", Description = "Sade Jel Tırnak", CategoryIds = "1", CreatedAt = DateTime.UtcNow },
            new Gallery { ImageUrl = "/uploads/gallery/d2e8c75d-c50b-4d6b-8d62-49c7ca7ed081.png", Description = "Geometrik Çizgili Nail Art", CategoryIds = "3", CreatedAt = DateTime.UtcNow },
            new Gallery { ImageUrl = "/uploads/gallery/d6908eba-1144-4336-a8c9-034cfe9c7f4e.png", Description = "Nude Tonlarında Protez Tırnak", CategoryIds = "2", CreatedAt = DateTime.UtcNow },
            new Gallery { ImageUrl = "/uploads/gallery/de4e988a-81a6-417b-bd2f-a3279a313324.png", Description = "Komple El ve Tırnak Bakımı", CategoryIds = "5", CreatedAt = DateTime.UtcNow },
            new Gallery { ImageUrl = "/uploads/gallery/e9cbc782-8cc2-4e47-a759-36ae6d3115d7.png", Description = "Neon Renkli Nail Art Tasarımı", CategoryIds = "3", CreatedAt = DateTime.UtcNow },
            new Gallery { ImageUrl = "/uploads/gallery/fc2527d1-ecfd-4916-858c-66a441ca1632.png", Description = "Glitter Efektli Jel Tırnak", CategoryIds = "1,3", CreatedAt = DateTime.UtcNow },
            new Gallery { ImageUrl = "/uploads/gallery/fd3d576a-39ee-49f0-b55f-35c47dd9966f.png", Description = "Doğal Kirpik Lifting Görünümü", CategoryIds = "4", CreatedAt = DateTime.UtcNow },

            // Restored 34 Images
            new Gallery { ImageUrl = "/uploads/gallery/02ca370b-4fb8-4584-8631-66299333e292.png", Description = "Kirpik Tasarımı", CategoryIds = "4", CreatedAt = DateTime.UtcNow },
            new Gallery { ImageUrl = "/uploads/gallery/097f7758-6d43-4ecd-a4bb-51f3e3c399cb.png", Description = "Protez Tırnak Tasarımı", CategoryIds = "2", CreatedAt = DateTime.UtcNow },
            new Gallery { ImageUrl = "/uploads/gallery/17f8536f-ae0f-4a6c-9a29-1ee0e4a1ef55.png", Description = "Nail Art Sanatı", CategoryIds = "3", CreatedAt = DateTime.UtcNow },
            new Gallery { ImageUrl = "/uploads/gallery/2e826d08-a569-4965-8642-4c916d4b3859.png", Description = "Jel Tırnak Tasarımı", CategoryIds = "1", CreatedAt = DateTime.UtcNow },
            new Gallery { ImageUrl = "/uploads/gallery/327f2406-0e7f-4fda-9109-88188af7b762.png", Description = "Özel Tasarım Protez Tırnak", CategoryIds = "2,3", CreatedAt = DateTime.UtcNow },
            new Gallery { ImageUrl = "/uploads/gallery/345a8a93-6bfe-420d-ae22-baae114e9dbd.png", Description = "Lüks Nail Art Uygulaması", CategoryIds = "3", CreatedAt = DateTime.UtcNow },
            new Gallery { ImageUrl = "/uploads/gallery/3f572a04-d6f8-4487-91e9-027344190f4f.png", Description = "Jel Tırnak Modeli", CategoryIds = "1", CreatedAt = DateTime.UtcNow },
            new Gallery { ImageUrl = "/uploads/gallery/49b60202-9956-4fbc-b183-122e2801729d.png", Description = "Kirpik Lifting Uygulaması", CategoryIds = "4", CreatedAt = DateTime.UtcNow },
            new Gallery { ImageUrl = "/uploads/gallery/545c05b2-be1d-44e1-9e9e-9846975b314f.png", Description = "Özel Süsleme Nail Art", CategoryIds = "3", CreatedAt = DateTime.UtcNow },
            new Gallery { ImageUrl = "/uploads/gallery/5518fa38-0dee-4e0b-8930-27617d7a0581.png", Description = "Manikür ve Kalıcı Oje", CategoryIds = "5", CreatedAt = DateTime.UtcNow },
            new Gallery { ImageUrl = "/uploads/gallery/5548371b-56c7-45c4-98c3-732f3338b936.png", Description = "Doğal Kirpik Görünümü", CategoryIds = "4", CreatedAt = DateTime.UtcNow },
            new Gallery { ImageUrl = "/uploads/gallery/557c4d3c-7162-43c6-80ae-d984eef7852a.png", Description = "Neon Jel Tırnak", CategoryIds = "1", CreatedAt = DateTime.UtcNow },
            new Gallery { ImageUrl = "/uploads/gallery/5c70854c-77da-4ad9-a21f-43daa8b32b50.png", Description = "Protez Tırnak Süsleme", CategoryIds = "2,3", CreatedAt = DateTime.UtcNow },
            new Gallery { ImageUrl = "/uploads/gallery/62084dc9-8adb-4a42-a70e-e3c532453fa3.png", Description = "Kirpik Lifting Bakımı", CategoryIds = "4", CreatedAt = DateTime.UtcNow },
            new Gallery { ImageUrl = "/uploads/gallery/68389693-8fb5-4f32-83eb-fdeb8b9773e2.png", Description = "Şık Manikür ve Pedikür", CategoryIds = "5", CreatedAt = DateTime.UtcNow },
            new Gallery { ImageUrl = "/uploads/gallery/754c8499-09c3-4b3e-b671-1b39d3981a74.png", Description = "Nail Art Çiçek Deseni", CategoryIds = "3", CreatedAt = DateTime.UtcNow },
            new Gallery { ImageUrl = "/uploads/gallery/8d4fcfd2-8a18-4a01-9482-a394a3aa2f8a.png", Description = "Mat Jel Tırnak", CategoryIds = "1", CreatedAt = DateTime.UtcNow },
            new Gallery { ImageUrl = "/uploads/gallery/99e33384-4ce5-464f-ab83-5701fe770351.png", Description = "Renkli Protez Tırnaklar", CategoryIds = "2", CreatedAt = DateTime.UtcNow },
            new Gallery { ImageUrl = "/uploads/gallery/a3011882-e0ef-4c1f-89ea-11ffb1615b5a.png", Description = "Kirpik Lifting ve Boyama", CategoryIds = "4", CreatedAt = DateTime.UtcNow },
            new Gallery { ImageUrl = "/uploads/gallery/a536a3a9-0653-4c6f-abd3-1152fa2732bf.png", Description = "Nail Art Geometrik Desenler", CategoryIds = "3", CreatedAt = DateTime.UtcNow },
            new Gallery { ImageUrl = "/uploads/gallery/b727ba7f-ad09-4951-96e3-87e23367d9e8.png", Description = "Kalıcı Oje ve Manikür", CategoryIds = "5", CreatedAt = DateTime.UtcNow },
            new Gallery { ImageUrl = "/uploads/gallery/ba01ec15-3e93-4659-81d6-d4a2c719a901.png", Description = "Glitter Protez Tırnak", CategoryIds = "2,3", CreatedAt = DateTime.UtcNow },
            new Gallery { ImageUrl = "/uploads/gallery/c458b83f-2cbc-4ffc-bffa-af631c18a5a9.png", Description = "Klasik Manikür Çalışması", CategoryIds = "5", CreatedAt = DateTime.UtcNow },
            new Gallery { ImageUrl = "/uploads/gallery/c7a774cd-f3b6-45f7-8e62-1bd8f9dbd0b2.png", Description = "Profesyonel Kirpik Lifting", CategoryIds = "4", CreatedAt = DateTime.UtcNow },
            new Gallery { ImageUrl = "/uploads/gallery/cc155c21-82ba-4413-930f-2f36e9fa61ae.png", Description = "Şık Nail Art Uygulaması", CategoryIds = "3", CreatedAt = DateTime.UtcNow },
            new Gallery { ImageUrl = "/uploads/gallery/ceb8d67f-9880-4fa8-a76d-14a02c2b6c9f.png", Description = "Kirpik Lifting ve Dolgunlaştırma", CategoryIds = "4", CreatedAt = DateTime.UtcNow },
            new Gallery { ImageUrl = "/uploads/gallery/cfb600fb-000f-428d-8909-5ac0618e30b1.png", Description = "Nail Art Taş Süsleme", CategoryIds = "3", CreatedAt = DateTime.UtcNow },
            new Gallery { ImageUrl = "/uploads/gallery/dcbc4763-ea11-4d09-b003-17b9d102a9f1.png", Description = "Jel Protez Tırnak", CategoryIds = "1,2", CreatedAt = DateTime.UtcNow },
            new Gallery { ImageUrl = "/uploads/gallery/e04a8c09-3eae-4a1f-b42c-e6409003f75e.png", Description = "Ombre Nail Art", CategoryIds = "3", CreatedAt = DateTime.UtcNow },
            new Gallery { ImageUrl = "/uploads/gallery/e22e2498-2303-4363-af1b-3650872bf617.png", Description = "Protez Tırnak ve Taşlı Tasarım", CategoryIds = "2,3", CreatedAt = DateTime.UtcNow },
            new Gallery { ImageUrl = "/uploads/gallery/f3a7b32a-df86-47ef-ac24-f4ad4df78096.png", Description = "Minimal Manikür & Pedikür", CategoryIds = "5", CreatedAt = DateTime.UtcNow },
            new Gallery { ImageUrl = "/uploads/gallery/f5842ae7-40ad-4651-a3ba-997da1f6fb05.png", Description = "Kalıcı Oje Tasarımı", CategoryIds = "5", CreatedAt = DateTime.UtcNow },
            new Gallery { ImageUrl = "/uploads/gallery/fb5d08ba-adbe-451a-9c2a-e953d706ef61.png", Description = "Protez Tırnak Bakımı", CategoryIds = "2", CreatedAt = DateTime.UtcNow },
            new Gallery { ImageUrl = "/uploads/gallery/feeaf964-90ec-41ff-b656-110797569005.png", Description = "Işıltılı Jel Tırnak", CategoryIds = "1", CreatedAt = DateTime.UtcNow }
        };

        db.Galleries.AddRange(galleryItems);
        db.SaveChanges();
        Console.WriteLine("[SEED LOG] Seeding gallery images completed successfully!");
    }
}

app.Run("http://localhost:5999");