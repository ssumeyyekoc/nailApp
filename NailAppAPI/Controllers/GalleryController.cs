using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NailAppAPI.Services;

namespace NailAppAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GalleryController : ControllerBase
{
    private readonly IGalleryService _galleryService;

    public GalleryController(IGalleryService galleryService)
    {
        _galleryService = galleryService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetGalleryImage(int id)
    {
        var image = await _galleryService.GetGalleryImageByIdAsync(id);
        if (image == null)
            return NotFound();

        return Ok(image);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllGalleryImages()
    {
        var images = await _galleryService.GetAllGalleryImagesAsync();
        return Ok(images);
    }

    [HttpGet("category/{categoryId}")]
    public async Task<IActionResult> GetGalleryImagesByCategory(int categoryId)
    {
        var images = await _galleryService.GetGalleryImagesByCategoryAsync(categoryId);
        return Ok(images);
    }

    [HttpGet("highlighted")]
    public async Task<IActionResult> GetHighlightedImages()
    {
        var images = await _galleryService.GetHighlightedImagesAsync();
        return Ok(images);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateGalleryImage([FromBody] CreateGalleryImageRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ImageUrl))
            return BadRequest("Görüntü URL'i gereklidir.");

        var image = await _galleryService.CreateGalleryImageAsync(
            request.Title ?? "",
            request.Description ?? "",
            request.ImageUrl,
            request.CategoryId
        );

        return CreatedAtAction(nameof(GetGalleryImage), new { id = image.Id }, image);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateGalleryImage(int id, [FromBody] UpdateGalleryImageRequest request)
    {
        var success = await _galleryService.UpdateGalleryImageAsync(
            id,
            request.Title ?? "",
            request.Description ?? "",
            request.IsHighlighted
        );

        if (!success)
            return NotFound();

        return Ok(new { message = "Galeri görüntüsü güncellendi." });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteGalleryImage(int id)
    {
        var success = await _galleryService.DeleteGalleryImageAsync(id);
        if (!success)
            return NotFound();

        return Ok(new { message = "Galeri görüntüsü silindi." });
    }
}

public class CreateGalleryImageRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public int CategoryId { get; set; }
}

public class UpdateGalleryImageRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public bool IsHighlighted { get; set; }
}
