using ConferenceRoomBooking.DataAccess.Models;

namespace ConferenceRoomBooking.DataAccess.Interfaces.IRepositories
{
    public interface IBookingCheckInRepository : IBaseRepository<BookingCheckIn>
    {
        Task<BookingCheckIn> CheckInAsync(int bookingId);
        Task<BookingCheckIn> CheckOutAsync(int bookingId);
        Task<BookingCheckIn?> GetCheckInByBookingIdAsync(int bookingId);
    }
}









