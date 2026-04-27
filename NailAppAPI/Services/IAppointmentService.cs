using NailAppAPI.Models;

namespace NailAppAPI.Services;

public interface IAppointmentService
{
    Task<Appointment?> GetAppointmentByIdAsync(int id);
    Task<IEnumerable<Appointment>> GetUserAppointmentsAsync(int userId);
    Task<IEnumerable<Appointment>> GetAllAppointmentsAsync();
    Task<Appointment> CreateAppointmentAsync(int userId, int serviceId, DateTime appointmentDate, string? notes);
    Task<bool> UpdateAppointmentStatusAsync(int appointmentId, AppointmentStatus status);
    Task<bool> DeleteAppointmentAsync(int appointmentId);
    Task<IEnumerable<DateTime>> GetAvailableTimesAsync(int serviceId, DateTime date);
}
