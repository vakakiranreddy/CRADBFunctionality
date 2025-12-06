using ConferenceRoomBooking.Business.DTOs.BookingCheckIn;

namespace ConferenceRoomBooking.Business.Interfaces.IServices
{
    public interface IBookingCheckInService
    {
        Task<BookingCheckInResponseDto> CheckInAsync(int bookingId, int userId);
        Task<bool> CanCheckInAsync(int bookingId);
        Task<BookingCheckInResponseDto> CheckOutAsync(int bookingId, int userId);
        Task<bool> CanCheckOutAsync(int bookingId);
        Task<BookingCheckInResponseDto?> GetCheckInStatusAsync(int bookingId);
        Task<IEnumerable<BookingCheckInResponseDto>> GetUserCheckInHistoryAsync(int userId);
        Task<CheckInStatisticsDto> GetCheckInStatisticsAsync(int userId);
        Task ProcessNoShowBookingsAsync();
        Task ProcessOverdueCheckoutsAsync();
    }

}









