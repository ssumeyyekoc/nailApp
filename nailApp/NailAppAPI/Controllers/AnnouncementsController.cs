using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NailAppAPI.Data;
using NailAppAPI.Models;

namespace NailAppAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnnouncementsController : ControllerBase
{
    private readonly AppDbContext _context;

    public AnnouncementsController(AppDbContext context)
    {
        _context = context;
    }

    // GET /api/announcements — Aktif duyuruları getir (herkes erişebilir)
    [HttpGet]
    public async Task<IActionResult> GetActive()
    {
        var announcements = await _context.Announcements
            .Where(a => a.IsActive)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
        return Ok(announcements);
    }

    // GET /api/announcements/all — Tüm duyuruları getir (admin)
    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var announcements = await _context.Announcements
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
        return Ok(announcements);
    }

    // POST /api/announcements — Yeni duyuru oluştur (admin)
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] AnnouncementRequest request)
    {
        var announcement = new Announcement
        {
            Title = request.Title,
            Content = request.Content,
            BadgeText = request.BadgeText,
            Emoji = request.Emoji,
            IsActive = request.IsActive,
            CreatedAt = DateTime.Now
        };

        _context.Announcements.Add(announcement);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetActive), new { id = announcement.Id }, announcement);
    }

    // PUT /api/announcements/{id} — Duyuru güncelle (admin)
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] AnnouncementRequest request)
    {
        var announcement = await _context.Announcements.FindAsync(id);
        if (announcement == null) return NotFound();

        announcement.Title = request.Title;
        announcement.Content = request.Content;
        announcement.BadgeText = request.BadgeText;
        announcement.Emoji = request.Emoji;
        announcement.IsActive = request.IsActive;
        announcement.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();
        return Ok(announcement);
    }

    // DELETE /api/announcements/{id} — Duyuru sil (admin)
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var announcement = await _context.Announcements.FindAsync(id);
        if (announcement == null) return NotFound();

        _context.Announcements.Remove(announcement);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Duyuru silindi." });
    }
}

public class AnnouncementRequest
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? BadgeText { get; set; }
    public string? Emoji { get; set; }
    public bool IsActive { get; set; } = true;
}
