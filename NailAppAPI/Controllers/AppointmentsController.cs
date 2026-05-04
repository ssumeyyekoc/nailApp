using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NailAppAPI.Models;
using NailAppAPI.Services;

namespace NailAppAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentsController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAppointment(int id)
    {
        var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
        if (appointment == null)
            return NotFound();

        return Ok(appointment);//deneme açıklaması
    }

    [HttpGet("user/{userId}")]
    [Authorize]
    public async Task<IActionResult> GetUserAppointments(int userId)
    {
        var appointments = await _appointmentService.GetUserAppointmentsAsync(userId);
        return Ok(appointments);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllAppointments()
    {
        var appointments = await _appointmentService.GetAllAppointmentsAsync();
        return Ok(appointments);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentRequest request)
    {
        if (request.UserId <= 0 || request.ServiceId <= 0)
            return BadRequest("Geçersiz kullanıcı veya hizmet ID.");

        var appointment = await _appointmentService.CreateAppointmentAsync(
            request.UserId,
            request.ServiceId,
            request.AppointmentDate,
            request.Notes
        );

        return CreatedAtAction(nameof(GetAppointment), new { id = appointment.Id }, appointment);
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateAppointmentStatus(int id, [FromBody] UpdateStatusRequest request)
    {
        var success = await _appointmentService.UpdateAppointmentStatusAsync(id, request.Status);
        if (!success)
            return NotFound();

        return Ok(new { message = "Randevu durumu güncellendi." });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteAppointment(int id)
    {
        var success = await _appointmentService.DeleteAppointmentAsync(id);
        if (!success)
            return NotFound();

        return Ok(new { message = "Randevu silindi." });
    }

    [HttpGet("available-times")]
    public async Task<IActionResult> GetAvailableTimes(int serviceId, DateTime date)
    {
        var times = await _appointmentService.GetAvailableTimesAsync(serviceId, date);
        return Ok(times);
    }
}

public class CreateAppointmentRequest
{
    public int UserId { get; set; }
    public int ServiceId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string? Notes { get; set; }
}

public class UpdateStatusRequest
{
    public AppointmentStatus Status { get; set; }
}
