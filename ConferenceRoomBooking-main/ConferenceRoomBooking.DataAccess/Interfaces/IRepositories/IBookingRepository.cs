using ConferenceRoomBooking.DataAccess.Enum;
using ConferenceRoomBooking.DataAccess.Models;

namespace ConferenceRoomBooking.DataAccess.Interfaces.IRepositories
{
    public interface IBookingRepository : IBaseRepository<Booking>
    {
        Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(int userId);
        Task<IEnumerable<Booking>> GetBookingsByResourceIdAsync(int resourceId);
        Task<IEnumerable<Booking>> GetBookingsByStatusAsync(SessionStatus status);
        Task<IEnumerable<Booking>> GetBookingsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<bool> IsResourceAvailableAsync(int resourceId, DateTime startTime, DateTime endTime, int? excludeBookingId = null);
        Task<bool> CancelBookingAsync(int bookingId, string cancellationReason);
        Task<IEnumerable<Booking>> GetNoShowBookingsAsync();
        Task<IEnumerable<Booking>> GetOverdueBookingsAsync();
    }
}









