using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NailAppAPI.Services;

namespace NailAppAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GalleryController : ControllerBase
{
    private readonly IGalleryService _galleryService;
    private readonly IWebHostEnvironment _environment;

    public GalleryController(IGalleryService galleryService, IWebHostEnvironment environment)
    {
        _galleryService = galleryService;
        _environment = environment;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _galleryService.GetAllAsync();
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _galleryService.GetByIdAsync(id);
        if (item == null)
            return NotFound();

        return Ok(item);
    }

    [HttpGet("category/{categoryId}")]
    public async Task<IActionResult> GetByCategory(int categoryId)
    {
        var items = await _galleryService.GetByCategoryAsync(categoryId);
        return Ok(items);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Upload([FromForm] GalleryUploadRequest request)
    {
        if (request.Image == null || request.Image.Length == 0)
            return BadRequest("Resim dosyası gereklidir.");

        // Resim dosyasını kaydet
        var uploadsFolder = Path.Combine(_environment.ContentRootPath, "wwwroot", "uploads", "gallery");
        Directory.CreateDirectory(uploadsFolder);

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(request.Image.FileName)}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await request.Image.CopyToAsync(stream);
        }

        var imageUrl = $"/uploads/gallery/{fileName}";
        var gallery = await _galleryService.CreateAsync(imageUrl, request.Description, request.CategoryId);

        return CreatedAtAction(nameof(GetById), new { id = gallery.Id }, gallery);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        // Önce galeri öğesini bul (dosyayı da silmek için)
        var item = await _galleryService.GetByIdAsync(id);
        if (item == null)
            return NotFound();

        // Dosyayı sil
        var filePath = Path.Combine(_environment.ContentRootPath, "wwwroot", item.ImageUrl.TrimStart('/'));
        if (System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);
        }

        var success = await _galleryService.DeleteAsync(id);
        if (!success)
            return NotFound();

        return Ok(new { message = "Galeri öğesi silindi." });
    }
}

public class GalleryUploadRequest
{
    public IFormFile? Image { get; set; }
    public string? Description { get; set; }
    public int CategoryId { get; set; }
}
