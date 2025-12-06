using ConferenceRoomBooking.DataAccess.Data;
using ConferenceRoomBooking.DataAccess.Enum;
using ConferenceRoomBooking.DataAccess.Interfaces.IRepositories;
using ConferenceRoomBooking.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace ConferenceRoomBooking.DataAccess.Repositories
{
    public class UserNotificationRepository : BaseRepository<UserNotification>, IUserNotificationRepository
    {
        public UserNotificationRepository(ConferenceRoomBookingDbContext context) : base(context) { }

        public async Task<IEnumerable<UserNotification>> GetByUserIdAsync(int userId)
        {
            return await _context.UserNotifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserNotification>> GetByStatusAsync(EmailStatus status)
        {
            return await _context.UserNotifications
                .Where(n => n.Status == status)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserNotification>> GetByDateRangeAsync(DateTime from, DateTime to)
        {
            return await _context.UserNotifications
                .Where(n => n.CreatedAt >= from && n.CreatedAt <= to)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserNotification>> GetPendingNotificationsAsync()
        {
            return await _context.UserNotifications
                .Where(n => n.Status == EmailStatus.Pending)
                .ToListAsync();
        }
    }
}








