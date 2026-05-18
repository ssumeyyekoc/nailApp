using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NailAppAPI.Services;

using System.ComponentModel.DataAnnotations; // Bu satırı dosyanın en üstündeki using'lerin arasına da eklemeyi unutma!



namespace NailAppAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServicesController : ControllerBase
{
    private readonly IServiceService _serviceService;

    public ServicesController(IServiceService serviceService)
    {
        _serviceService = serviceService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetService(int id)
    {
        var service = await _serviceService.GetServiceByIdAsync(id);
        if (service == null)
            return NotFound();

        return Ok(service);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllServices()
    {
        var services = await _serviceService.GetAllServicesAsync();
        return Ok(services);
    }

    [HttpGet("category/{categoryId}")]
    public async Task<IActionResult> GetServicesByCategory(int categoryId)
    {
        var services = await _serviceService.GetServicesByCategoryAsync(categoryId);
        return Ok(services);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateService([FromBody] CreateServiceRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest("Hizmet adı gereklidir.");

        var service = await _serviceService.CreateServiceAsync(
            request.Name,
            request.Description ?? "",
            request.Price,
            request.DurationMinutes,
            request.CategoryId
        );

        return CreatedAtAction(nameof(GetService), new { id = service.Id }, service);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateService(int id, [FromBody] UpdateServiceRequest request)
    {
        var success = await _serviceService.UpdateServiceAsync(
            id,
            request.Name ?? "",
            request.Description ?? "",
            request.Price,
            request.DurationMinutes
        );

        if (!success)
            return NotFound();

        return Ok(new { message = "Hizmet güncellendi." });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteService(int id)
    {
        var success = await _serviceService.DeleteServiceAsync(id);
        if (!success)
            return NotFound();

        return Ok(new { message = "Hizmet silindi." });
    }
}


public class CreateServiceRequest
{
    [Required(ErrorMessage = "Hizmet adı boş bırakılamaz.")]
    [MaxLength(100, ErrorMessage = "Hizmet adı en fazla 100 karakter olabilir.")]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required(ErrorMessage = "Fiyat bilgisi zorunludur.")]
    [Range(0.1, 10000, ErrorMessage = "Geçerli bir fiyat giriniz (0'dan büyük olmalı).")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Süre bilgisi zorunludur.")]
    [Range(1, 300, ErrorMessage = "Hizmet süresi 1 ile 300 dakika arasında olmalıdır.")]
    public int DurationMinutes { get; set; }

    [Required(ErrorMessage = "Kategori seçimi zorunludur.")]
    public int CategoryId { get; set; }
}

public class UpdateServiceRequest
{
    [Required(ErrorMessage = "Hizmet adı boş bırakılamaz.")]
    [MaxLength(100, ErrorMessage = "Hizmet adı en fazla 100 karakter olabilir.")]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required(ErrorMessage = "Fiyat bilgisi zorunludur.")]
    [Range(0.1, 10000, ErrorMessage = "Geçerli bir fiyat giriniz.")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Süre bilgisi zorunludur.")]
    [Range(1, 300, ErrorMessage = "Hizmet süresi 1 ile 300 dakika arasında olmalıdır.")]
    public int DurationMinutes { get; set; }
}
