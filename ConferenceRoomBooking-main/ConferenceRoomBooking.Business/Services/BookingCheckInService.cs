using ConferenceRoomBooking.Business.DTOs.BookingCheckIn;
using ConferenceRoomBooking.DataAccess.Enum;
using ConferenceRoomBooking.DataAccess.Interfaces.IRepositories;
using ConferenceRoomBooking.Business.Interfaces.IServices;
using ConferenceRoomBooking.DataAccess.Models;

namespace ConferenceRoomBooking.Business.Services
{
    public class BookingCheckInService : IBookingCheckInService
    {
        private readonly IBookingCheckInRepository _checkInRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IUserNotificationService _notificationService;
        private readonly IResourceRepository _resourceRepository;

        public BookingCheckInService(
            IBookingCheckInRepository checkInRepository,
            IBookingRepository bookingRepository,
            IUserNotificationService notificationService,
            IResourceRepository resourceRepository)
        {
            _checkInRepository = checkInRepository;
            _bookingRepository = bookingRepository;
            _notificationService = notificationService;
            _resourceRepository = resourceRepository;
        }

        public async Task<BookingCheckInResponseDto> CheckInAsync(int bookingId, int userId)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking == null || booking.UserId != userId)
                throw new ArgumentException("Booking not found or unauthorized");

            // Validate if check-in is allowed
            if (!await CanCheckInAsync(bookingId))
                throw new InvalidOperationException("Check-in is not allowed at this time. Please check the booking time window.");

            var checkIn = await _checkInRepository.CheckInAsync(bookingId);
            
            // Send check-in confirmation email
            await SendCheckInNotificationAsync(booking);
            
            return await MapToResponseDtoAsync(checkIn, booking);
        }

        public async Task<bool> CanCheckInAsync(int bookingId)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking == null) return false;

            var now = DateTime.UtcNow;
            var checkInWindow = TimeSpan.FromMinutes(15);

            return booking.SessionStatus == SessionStatus.Reserved &&
                   now >= booking.StartTime.Subtract(checkInWindow) &&
                   now <= booking.EndTime;
        }

        public async Task<BookingCheckInResponseDto> CheckOutAsync(int bookingId, int userId)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking == null || booking.UserId != userId)
                throw new ArgumentException("Booking not found or unauthorized");

            // Validate if check-out is allowed
            if (!await CanCheckOutAsync(bookingId))
                throw new InvalidOperationException("Check-out is not allowed. User must be checked in first.");

            var checkIn = await _checkInRepository.CheckOutAsync(bookingId);
            
            // Send check-out confirmation email
            await SendCheckOutNotificationAsync(booking);
            
            return await MapToResponseDtoAsync(checkIn, booking);
        }

        public async Task<bool> CanCheckOutAsync(int bookingId)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            return booking?.SessionStatus == SessionStatus.CheckedIn;
        }

        public async Task<BookingCheckInResponseDto?> GetCheckInStatusAsync(int bookingId)
        {
            var checkIn = await _checkInRepository.GetCheckInByBookingIdAsync(bookingId);
            if (checkIn == null) return null;
            
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            return await MapToResponseDtoAsync(checkIn, booking);
        }

        public async Task<IEnumerable<BookingCheckInResponseDto>> GetUserCheckInHistoryAsync(int userId)
        {
            var bookings = await _bookingRepository.GetBookingsByUserIdAsync(userId);
            var checkIns = new List<BookingCheckInResponseDto>();

            foreach (var booking in bookings)
            {
                var checkIn = await _checkInRepository.GetCheckInByBookingIdAsync(booking.BookingId);
                if (checkIn != null)
                {
                    checkIns.Add(await MapToResponseDtoAsync(checkIn, booking));
                }
            }

            return checkIns.OrderByDescending(c => c.CheckInTime);
        }

        public async Task<CheckInStatisticsDto> GetCheckInStatisticsAsync(int userId)
        {
            var bookings = await _bookingRepository.GetBookingsByUserIdAsync(userId);
            var totalBookings = bookings.Count();
            var checkedInBookings = bookings.Count(b => b.SessionStatus == SessionStatus.CheckedIn || b.SessionStatus == SessionStatus.Completed);
            var noShowBookings = bookings.Count(b => b.SessionStatus == SessionStatus.NoShow);

            return new CheckInStatisticsDto
            {
                TotalBookings = totalBookings,
                CheckedInBookings = checkedInBookings,
                NoShowBookings = noShowBookings,
                CheckInRate = totalBookings > 0 ? (double)checkedInBookings / totalBookings * 100 : 0
            };
        }

        public async Task ProcessNoShowBookingsAsync()
        {
            var noShowBookings = await _bookingRepository.GetNoShowBookingsAsync();
            foreach (var booking in noShowBookings)
            {
                booking.SessionStatus = SessionStatus.NoShow;
                await _bookingRepository.UpdateAsync(booking);
            }
        }

        public async Task ProcessOverdueCheckoutsAsync()
        {
            var overdueBookings = await _bookingRepository.GetOverdueBookingsAsync();
            foreach (var booking in overdueBookings)
            {
                var checkIn = await _checkInRepository.GetCheckInByBookingIdAsync(booking.BookingId);
                if (checkIn != null && !checkIn.IsCheckedOut)
                {
                    await _checkInRepository.CheckOutAsync(booking.BookingId);
                }
            }
        }

        private async Task SendCheckInNotificationAsync(Booking booking)
        {
            var resource = await _resourceRepository.GetByIdAsync(booking.ResourceId);
            var subject = "Check-in Successful";
            var body = $@"You have successfully checked in!

Details:
Meeting: {booking.MeetingName}
Resource: {resource?.Name}
Check-in Time: {DateTime.UtcNow:yyyy-MM-dd HH:mm}

Enjoy your meeting!";
            
            await _notificationService.SendNotificationAsync(new DTOs.UserNotification.SendNotificationDto
            {
                UserId = booking.UserId,
                Title = subject,
                Message = body,
                Type = ConferenceRoomBooking.DataAccess.Enum.NotificationType.CheckIn
            });
        }

        private async Task SendCheckOutNotificationAsync(Booking booking)
        {
            var resource = await _resourceRepository.GetByIdAsync(booking.ResourceId);
            var subject = "Check-out Successful";
            var body = $@"You have successfully checked out!

Details:
Meeting: {booking.MeetingName}
Resource: {resource?.Name}
Check-out Time: {DateTime.UtcNow:yyyy-MM-dd HH:mm}

Thank you for using our booking system!";
            
            await _notificationService.SendNotificationAsync(new DTOs.UserNotification.SendNotificationDto
            {
                UserId = booking.UserId,
                Title = subject,
                Message = body,
                Type = ConferenceRoomBooking.DataAccess.Enum.NotificationType.CheckOut
            });
        }

        private async Task<BookingCheckInResponseDto> MapToResponseDtoAsync(BookingCheckIn checkIn, Booking? booking = null)
        {
            if (booking == null)
                booking = await _bookingRepository.GetByIdAsync(checkIn.BookingId);
            
            var resource = await _resourceRepository.GetByIdAsync(booking.ResourceId);
            
            // Get resource name from Room or Desk
            string resourceName = "";
            if (booking.ResourceType == ResourceType.Room && resource?.Room != null)
                resourceName = resource.Room.RoomName;
            else if (booking.ResourceType == ResourceType.Desk && resource?.Desk != null)
                resourceName = resource.Desk.DeskName;
            
            return new BookingCheckInResponseDto
            {
                Id = checkIn.Id,
                BookingId = checkIn.BookingId,
                CheckInTime = checkIn.CheckInTime,
                CheckOutTime = checkIn.CheckOutTime,
                IsCheckedIn = checkIn.IsCheckedIn,
                IsCheckedOut = checkIn.IsCheckedOut,
                IsNoShow = booking.SessionStatus == SessionStatus.NoShow,
                UserName = $"{booking.User?.FirstName} {booking.User?.LastName}".Trim(),
                ResourceName = resourceName,
                Duration = checkIn.ActualDuration
            };
        }
    }
}








