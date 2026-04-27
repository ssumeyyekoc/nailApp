using Microsoft.EntityFrameworkCore;
using NailAppAPI.Data;
using NailAppAPI.Models;

namespace NailAppAPI.Services;

public class AppointmentService : IAppointmentService
{
    private readonly AppDbContext _context;
    private const int BusinessHoursStart = 9;
    private const int BusinessHoursEnd = 18;

    public AppointmentService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Appointment?> GetAppointmentByIdAsync(int id)
    {
        return await _context.Appointments
            .Include(a => a.User)
            .Include(a => a.Service)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<Appointment>> GetUserAppointmentsAsync(int userId)
    {
        return await _context.Appointments
            .Where(a => a.UserId == userId)
            .Include(a => a.Service)
            .OrderByDescending(a => a.AppointmentDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetAllAppointmentsAsync()
    {
        return await _context.Appointments
            .Include(a => a.User)
            .Include(a => a.Service)
            .OrderByDescending(a => a.AppointmentDate)
            .ToListAsync();
    }

    public async Task<Appointment> CreateAppointmentAsync(int userId, int serviceId, DateTime appointmentDate, string? notes)
    {
        var appointment = new Appointment
        {
            UserId = userId,
            ServiceId = serviceId,
            AppointmentDate = appointmentDate,
            Notes = notes,
            Status = AppointmentStatus.Pending,
            CreatedAt = DateTime.Now
        };

        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();
        return appointment;
    }

    public async Task<bool> UpdateAppointmentStatusAsync(int appointmentId, AppointmentStatus status)
    {
        var appointment = await _context.Appointments.FindAsync(appointmentId);
        if (appointment == null)
            return false;

        appointment.Status = status;
        appointment.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAppointmentAsync(int appointmentId)
    {
        var appointment = await _context.Appointments.FindAsync(appointmentId);
        if (appointment == null)
            return false;

        _context.Appointments.Remove(appointment);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<DateTime>> GetAvailableTimesAsync(int serviceId, DateTime date)
    {
        var service = await _context.Services.FindAsync(serviceId);
        if (service == null)
            return Enumerable.Empty<DateTime>();

        var appointmentsOnDate = await _context.Appointments
            .Where(a => a.ServiceId == serviceId && 
                       a.AppointmentDate.Date == date.Date &&
                       a.Status != AppointmentStatus.Cancelled)
            .ToListAsync();

        var availableTimes = new List<DateTime>();
        var slotDuration = service.DurationMinutes;

        for (int hour = BusinessHoursStart; hour < BusinessHoursEnd; hour++)
        {
            for (int minute = 0; minute < 60; minute += slotDuration)
            {
                var slotTime = new DateTime(date.Year, date.Month, date.Day, hour, minute, 0);
                
                // Check if slot is available
                var isBooked = appointmentsOnDate.Any(a =>
                {
                    var appointmentEnd = a.AppointmentDate.AddMinutes(service.DurationMinutes);
                    var slotEnd = slotTime.AddMinutes(slotDuration);
                    return (slotTime >= a.AppointmentDate && slotTime < appointmentEnd) ||
                           (slotEnd > a.AppointmentDate && slotEnd <= appointmentEnd);
                });

                if (!isBooked)
                    availableTimes.Add(slotTime);
            }
        }

        return availableTimes;
    }
}
