  using ConferenceRoomBooking.DataAccess.Enum;
using ConferenceRoomBooking.Business.Helpers;
using ConferenceRoomBooking.DataAccess.Interfaces.IRepositories;
using ConferenceRoomBooking.Business.Interfaces.IServices;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ConferenceRoomBooking.Business.Services
{
    public class NotificationBackgroundService : BackgroundService, INotificationBackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<NotificationBackgroundService> _logger;

        public NotificationBackgroundService(IServiceProvider serviceProvider, ILogger<NotificationBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessPendingNotificationsAsync();
                    await ProcessPendingBroadcastsAsync();
                    await SendEntryRemindersAsync();
                    await SendExitRemindersAsync();
                    await SendOverdueRemindersAsync();
                    await SendNoCheckInRemindersAsync();
                    await SendNoCheckOutRemindersAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in background notification service");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private async Task ProcessPendingNotificationsAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var notificationRepository = scope.ServiceProvider.GetRequiredService<IUserNotificationRepository>();
            var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            var pendingNotifications = await notificationRepository.GetPendingNotificationsAsync();

            foreach (var notification in pendingNotifications)
            {
                var user = await userRepository.GetByIdAsync(notification.UserId);
                if (user != null)
                {
                    var emailSent = await emailService.SendEmailAsync(user.Email, notification.Title, notification.Message, false);
                    notification.Status = emailSent ? EmailStatus.Sent : EmailStatus.Failed;
                    notification.SentAt = emailSent ? DateTime.UtcNow : null;
                    await notificationRepository.UpdateAsync(notification);
                }
            }
        }

        public async Task SendEntryRemindersAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var bookingRepo = scope.ServiceProvider.GetRequiredService<IBookingRepository>();
            var userRepo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            var bookings = await bookingRepo.GetBookingsByStatusAsync(SessionStatus.Reserved);
            var upcomingBookings = bookings.Where(b => 
                !b.EntryReminderSent &&
                DateTimeHelper.IsWithinNotificationWindow(b.StartTime, 15)); // 15 minutes before start

            foreach (var booking in upcomingBookings)
            {
                var user = await userRepo.GetByIdAsync(booking.UserId);
                if (user != null)
                {
                    var startTimeIst = DateTimeHelper.ConvertUtcToIst(booking.StartTime);
                    var subject = "Booking Reminder - Check-in Required";
                    var body = $"Your booking '{booking.MeetingName}' starts at {startTimeIst:HH:mm} IST. Please check-in within 15 minutes.";
                    
                    await emailService.SendEmailAsync(user.Email, subject, body);
                    booking.EntryReminderSent = true;
                    await bookingRepo.UpdateAsync(booking);
                }
            }
        }

        public async Task SendExitRemindersAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var bookingRepo = scope.ServiceProvider.GetRequiredService<IBookingRepository>();
            var userRepo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            var bookings = await bookingRepo.GetBookingsByStatusAsync(SessionStatus.CheckedIn);
            var endingBookings = bookings.Where(b => 
                !b.ExitReminderSent &&
                DateTimeHelper.IsWithinNotificationWindow(b.EndTime, 10)); // 10 minutes before end

            foreach (var booking in endingBookings)
            {
                var user = await userRepo.GetByIdAsync(booking.UserId);
                if (user != null)
                {
                    var endTimeIst = DateTimeHelper.ConvertUtcToIst(booking.EndTime);
                    var subject = "Booking Ending Soon - Check-out Required";
                    var body = $"Your booking '{booking.MeetingName}' ends at {endTimeIst:HH:mm} IST. Please check-out before leaving.";
                    
                    await emailService.SendEmailAsync(user.Email, subject, body);
                    booking.ExitReminderSent = true;
                    await bookingRepo.UpdateAsync(booking);
                }
            }
        }

        public async Task SendOverdueRemindersAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var bookingRepo = scope.ServiceProvider.GetRequiredService<IBookingRepository>();
            var userRepo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            var bookings = await bookingRepo.GetBookingsByStatusAsync(SessionStatus.CheckedIn);
            var overdueBookings = bookings.Where(b => 
                !b.OverdueRemainderSent &&
                DateTimeHelper.IsBookingOverdue(b.EndTime));

            foreach (var booking in overdueBookings)
            {
                var user = await userRepo.GetByIdAsync(booking.UserId);
                if (user != null)
                {
                    var subject = "Overdue Booking - Please Check-out";
                    var body = $"Your booking '{booking.MeetingName}' has exceeded the end time. Please check-out immediately.";
                    
                    await emailService.SendEmailAsync(user.Email, subject, body);
                    booking.OverdueRemainderSent = true;
                    await bookingRepo.UpdateAsync(booking);
                }
            }
        }

        public async Task SendNoCheckInRemindersAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var bookingRepo = scope.ServiceProvider.GetRequiredService<IBookingRepository>();
            var userRepo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            var currentIst = DateTimeHelper.GetCurrentIstTime();
            var bookings = await bookingRepo.GetBookingsByStatusAsync(SessionStatus.Reserved);
            var missedBookings = bookings.Where(b => 
                DateTimeHelper.ConvertUtcToIst(b.StartTime).AddMinutes(15) < currentIst); // 15 minutes past start time

            foreach (var booking in missedBookings)
            {
                var user = await userRepo.GetByIdAsync(booking.UserId);
                if (user != null)
                {
                    var startTimeIst = DateTimeHelper.ConvertUtcToIst(booking.StartTime);
                    var subject = "Missed Booking - No Check-in Detected";
                    var body = $"You missed your booking '{booking.MeetingName}' scheduled at {startTimeIst:HH:mm} IST. The booking has been marked as no-show.";
                    
                    await emailService.SendEmailAsync(user.Email, subject, body);
                }

                booking.SessionStatus = SessionStatus.NoShow;
                await bookingRepo.UpdateAsync(booking);
            }
        }

        public async Task SendNoCheckOutRemindersAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var checkInService = scope.ServiceProvider.GetRequiredService<IBookingCheckInService>();

            await checkInService.ProcessOverdueCheckoutsAsync();
        }

        private async Task ProcessPendingBroadcastsAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var broadcastService = scope.ServiceProvider.GetRequiredService<IBroadcastNotificationService>();

            await broadcastService.ProcessPendingBroadcastsAsync();
        }
    }
}








