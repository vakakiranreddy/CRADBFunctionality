  using ConferenceRoomBooking.Business.DTOs.Booking;
using ConferenceRoomBooking.DataAccess.Enum;
using ConferenceRoomBooking.Business.Helpers;
using ConferenceRoomBooking.DataAccess.Interfaces.IRepositories;
using ConferenceRoomBooking.Business.Interfaces.IServices;
using ConferenceRoomBooking.DataAccess.Models;

namespace ConferenceRoomBooking.Business.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IResourceRepository _resourceRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserNotificationService _notificationService;

        public BookingService(
            IBookingRepository bookingRepository,
            IResourceRepository resourceRepository,
            IUserRepository userRepository,
            IUserNotificationService notificationService)
        {
            _bookingRepository = bookingRepository;
            _resourceRepository = resourceRepository;
            _userRepository = userRepository;
            _notificationService = notificationService;
        }

        public async Task<BookingResponseDto> BookResource(CreateBookingDto bookingDto, int userId)
        {
            // Basic validation
            if (bookingDto.StartTime >= bookingDto.EndTime)
                throw new ArgumentException("Start time must be before end time");
            
            if (bookingDto.StartTime < DateTime.Now.AddMinutes(-5))
                throw new ArgumentException("Cannot book in the past");

            var resource = await _resourceRepository.GetByIdAsync(bookingDto.ResourceId);
            if (resource == null) throw new ArgumentException("Resource not found");
            if (resource.IsUnderMaintenance || resource.IsBlocked) throw new ArgumentException($"Resource {resource.Name} is currently unavailable");

            // Convert times to UTC for storage (handles different DateTime kinds)
            var startTimeUtc = DateTimeHelper.ConvertIstToUtcForStorage(bookingDto.StartTime);
            var endTimeUtc = DateTimeHelper.ConvertIstToUtcForStorage(bookingDto.EndTime);

            var isAvailable = await _bookingRepository.IsResourceAvailableAsync(
                bookingDto.ResourceId, startTimeUtc, endTimeUtc);
            
            if (!isAvailable) throw new InvalidOperationException("Resource is not available for the selected time slot");

            var booking = new Booking
            {
                UserId = userId,
                ResourceId = bookingDto.ResourceId,
                ResourceType = resource.ResourceType, // Fetch from Resource
                MeetingName = bookingDto.MeetingName,
                StartTime = startTimeUtc,
                EndTime = endTimeUtc,
                ParticipantCount = bookingDto.ParticipantCount,
                SessionStatus = SessionStatus.Reserved
            };

            var createdBooking = await _bookingRepository.AddAsync(booking);
            
            // Send booking confirmation email
            await SendBookingConfirmationAsync(createdBooking);
            
            return await MapToResponseDto(createdBooking);
        }

        public async Task<BookingResponseDto?> GetBookingByIdAsync(int bookingId)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            return booking == null ? null : await MapToResponseDto(booking);
        }

        public async Task<IEnumerable<BookingResponseDto>> GetAllBookingsAsync()
        {
            var bookings = await _bookingRepository.GetAllAsync();
            var tasks = bookings.Select(MapToResponseDto);
            return await Task.WhenAll(tasks);
        }

        public async Task<IEnumerable<BookingResponseDto>> GetUserBookingsAsync(int userId)
        {
            var bookings = await _bookingRepository.GetBookingsByUserIdAsync(userId);
            var tasks = bookings.Select(MapToResponseDto);
            return await Task.WhenAll(tasks);
        }

        public async Task<IEnumerable<BookingResponseDto>> GetBookingsByStatusAsync(SessionStatus status)
        {
            var bookings = await _bookingRepository.GetBookingsByStatusAsync(status);
            var tasks = bookings.Select(MapToResponseDto);
            return await Task.WhenAll(tasks);
        }

        public async Task<IEnumerable<BookingResponseDto>> GetBookingsForCalendarAsync(DateTime startDate, DateTime endDate)
        {
            var bookings = await _bookingRepository.GetBookingsByDateRangeAsync(startDate, endDate);
            var tasks = bookings.Select(MapToResponseDto);
            return await Task.WhenAll(tasks);
        }

        public async Task<IEnumerable<BookingResponseDto>> GetBookingsByResourceIdAsync(int resourceId, DateTime? date = null)
        {
            var bookings = await _bookingRepository.GetBookingsByResourceIdAsync(resourceId);
            if (date.HasValue)
                bookings = bookings.Where(b => DateTimeHelper.ConvertUtcToIst(b.StartTime).Date == date.Value.Date);
            
            var tasks = bookings.Select(MapToResponseDto);
            return await Task.WhenAll(tasks);
        }

        public async Task<bool> CancelBookingAsync(int bookingId, int userId, string reason)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking == null || booking.UserId != userId) return false;

            var result = await _bookingRepository.CancelBookingAsync(bookingId, reason);
            
            if (result)
            {
                await SendBookingCancellationAsync(booking, reason);
            }
            
            return result;
        }

        public async Task<bool> ValidateBookingAvailabilityAsync(int resourceId, DateTime startTime, DateTime endTime, int? excludeBookingId = null)
        {
            var startTimeUtc = DateTimeHelper.ConvertToUtcForStorage(startTime);
            var endTimeUtc = DateTimeHelper.ConvertToUtcForStorage(endTime);
            return await _bookingRepository.IsResourceAvailableAsync(resourceId, startTimeUtc, endTimeUtc, excludeBookingId);
        }

        public async Task<AlternativeTimeSlotsDto> GetAlternativeTimeSlotsAsync(int resourceId, DateTime date, DateTime startTime, DateTime endTime)
        {
            var bookings = await _bookingRepository.GetBookingsByResourceIdAsync(resourceId);
            var dayBookings = bookings.Where(b => DateTimeHelper.ConvertUtcToIst(b.StartTime).Date == date.Date && b.SessionStatus != SessionStatus.Cancelled)
                                    .OrderBy(b => b.StartTime).ToList();

            var alternatives = new List<TimeSlotDto>();
            var workingStart = date.Date.AddHours(9); // 9 AM IST
            var workingEnd = date.Date.AddHours(19);   // 7 PM IST
            var duration = endTime - startTime;

            for (var time = workingStart; time.Add(duration) <= workingEnd; time = time.AddMinutes(30))
            {
                var slotEnd = time.Add(duration);
                var timeUtc = DateTimeHelper.ConvertToUtcForStorage(time);
                var slotEndUtc = DateTimeHelper.ConvertToUtcForStorage(slotEnd);
                
                var hasConflict = dayBookings.Any(b => b.StartTime < slotEndUtc && b.EndTime > timeUtc);
                
                if (!hasConflict)
                {
                    alternatives.Add(new TimeSlotDto { StartTime = time, EndTime = slotEnd });
                }
            }

            return new AlternativeTimeSlotsDto { AlternativeSlots = alternatives };
        }

        public async Task<BookingAnalyticsDto> GetBookingAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null, int? locationId = null)
        {
            var start = startDate ?? DateTime.UtcNow.AddDays(-30);
            var end = endDate ?? DateTime.UtcNow;
            
            var bookings = await _bookingRepository.GetBookingsByDateRangeAsync(start, end);
            
            return new BookingAnalyticsDto
            {
                TotalBookings = bookings.Count(),
                CompletedBookings = bookings.Count(b => b.SessionStatus == SessionStatus.Completed),
                CancelledBookings = bookings.Count(b => b.SessionStatus == SessionStatus.Cancelled),
                NoShowBookings = bookings.Count(b => b.SessionStatus == SessionStatus.NoShow)
            };
        }

        public async Task<IEnumerable<BookingResponseDto>> GetUpcomingBookingsAsync(int userId, int hours = 24)
        {
            var bookings = await _bookingRepository.GetBookingsByUserIdAsync(userId);
            var currentIst = DateTimeHelper.GetCurrentIstTime();
            var upcoming = bookings.Where(b => 
                DateTimeHelper.ConvertUtcToIst(b.StartTime) >= currentIst && 
                b.SessionStatus == SessionStatus.Reserved &&
                DateTimeHelper.ConvertUtcToIst(b.StartTime) <= currentIst.AddHours(hours));
            
            var tasks = upcoming.Select(MapToResponseDto);
            return await Task.WhenAll(tasks);
        }

        public async Task<IEnumerable<BookingResponseDto>> GetBookingHistoryAsync(int userId)
        {
            var bookings = await _bookingRepository.GetBookingsByUserIdAsync(userId);
            var currentIst = DateTimeHelper.GetCurrentIstTime();
            var history = bookings.Where(b => DateTimeHelper.ConvertUtcToIst(b.EndTime) < currentIst || 
                                            b.SessionStatus == SessionStatus.Completed ||
                                            b.SessionStatus == SessionStatus.Cancelled);
            
            var tasks = history.Select(MapToResponseDto);
            return await Task.WhenAll(tasks);
        }

        public async Task<bool> HasConflictingBookingsAsync(int userId, DateTime startTime, DateTime endTime)
        {
            var userBookings = await _bookingRepository.GetBookingsByUserIdAsync(userId);
            var startTimeUtc = DateTimeHelper.ConvertToUtcForStorage(startTime);
            var endTimeUtc = DateTimeHelper.ConvertToUtcForStorage(endTime);
            
            return userBookings.Any(b => 
                b.SessionStatus != SessionStatus.Cancelled &&
                b.StartTime < endTimeUtc && b.EndTime > startTimeUtc);
        }

        private async Task<BookingResponseDto> MapToResponseDto(Booking booking)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(booking.UserId);
                var resource = await _resourceRepository.GetByIdAsync(booking.ResourceId);

                return new BookingResponseDto
                {
                    BookingId = booking.BookingId,
                    UserId = booking.UserId,
                    UserName = user != null ? $"{user.FirstName} {user.LastName}" : "Unknown User",
                    ResourceId = booking.ResourceId,
                    ResourceName = resource?.Name ?? "Unknown Resource",
                    ResourceType = booking.ResourceType,
                    MeetingName = booking.MeetingName,
                    StartTime = DateTimeHelper.ConvertUtcToIst(booking.StartTime),
                    EndTime = DateTimeHelper.ConvertUtcToIst(booking.EndTime),
                    ParticipantCount = booking.ParticipantCount,
                    SessionStatus = booking.SessionStatus,
                    CancellationReason = booking.CancellationReason,
                    CancelledAt = booking.CancelledAt
                };
            }
            catch (Exception)
            {
                return new BookingResponseDto
                {
                    BookingId = booking.BookingId,
                    UserId = booking.UserId,
                    UserName = "Unknown User",
                    ResourceId = booking.ResourceId,
                    ResourceName = "Unknown Resource",
                    ResourceType = booking.ResourceType,
                    MeetingName = booking.MeetingName,
                    StartTime = DateTimeHelper.ConvertUtcToIst(booking.StartTime),
                    EndTime = DateTimeHelper.ConvertUtcToIst(booking.EndTime),
                    ParticipantCount = booking.ParticipantCount,
                    SessionStatus = booking.SessionStatus,
                    CancellationReason = booking.CancellationReason,
                    CancelledAt = booking.CancelledAt
                };
            }
        }

        private async Task SendBookingConfirmationAsync(Booking booking)
        {
            var resource = await _resourceRepository.GetByIdAsync(booking.ResourceId);
            var subject = "Booking Confirmation";
            var startTimeIst = DateTimeHelper.ConvertUtcToIst(booking.StartTime);
            var endTimeIst = DateTimeHelper.ConvertUtcToIst(booking.EndTime);
            var body = $@"Your booking has been confirmed!

Details:
Meeting: {booking.MeetingName}
Resource: {resource?.Name}
Date: {startTimeIst:yyyy-MM-dd}
Time: {startTimeIst:HH:mm} - {endTimeIst:HH:mm} IST
Participants: {booking.ParticipantCount}

Please check-in on time.";
            
            await _notificationService.SendNotificationAsync(new DTOs.UserNotification.SendNotificationDto
            {
                UserId = booking.UserId,
                Title = subject,
                Message = body,
                Type = ConferenceRoomBooking.DataAccess.Enum.NotificationType.BookingConfirmation
            });
        }

        private async Task SendBookingCancellationAsync(Booking booking, string reason)
        {
            var resource = await _resourceRepository.GetByIdAsync(booking.ResourceId);
            var subject = "Booking Cancelled";
            var startTimeIst = DateTimeHelper.ConvertUtcToIst(booking.StartTime);
            var endTimeIst = DateTimeHelper.ConvertUtcToIst(booking.EndTime);
            var body = $@"Your booking has been cancelled.

Details:
Meeting: {booking.MeetingName}
Resource: {resource?.Name}
Date: {startTimeIst:yyyy-MM-dd}
Time: {startTimeIst:HH:mm} - {endTimeIst:HH:mm} IST
Reason: {reason}";
            
            await _notificationService.SendNotificationAsync(new DTOs.UserNotification.SendNotificationDto
            {
                UserId = booking.UserId,
                Title = subject,
                Message = body,
                Type = ConferenceRoomBooking.DataAccess.Enum.NotificationType.BookingCancellation
            });
        }
    }
}








