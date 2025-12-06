using ConferenceRoomBooking.DataAccess.Data;
using ConferenceRoomBooking.DataAccess.Enum;
using ConferenceRoomBooking.DataAccess.Interfaces.IRepositories;
using ConferenceRoomBooking.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace ConferenceRoomBooking.DataAccess.Repositories
{
    public class BroadcastNotificationRepository : BaseRepository<BroadcastNotification>, IBroadcastNotificationRepository
    {
        public BroadcastNotificationRepository(ConferenceRoomBookingDbContext context) : base(context) { }

        public async Task<IEnumerable<BroadcastNotification>> GetPendingBroadcastsAsync()
        {
            return await _context.BroadcastNotifications
                .Where(b => b.Status == EmailStatus.Pending)
                .ToListAsync();
        }

        public async Task<IEnumerable<BroadcastNotification>> GetByStatusAsync(EmailStatus status)
        {
            return await _context.BroadcastNotifications
                .Where(b => b.Status == status)
                .ToListAsync();
        }

        public async Task<IEnumerable<BroadcastNotification>> GetByDateRangeAsync(DateTime from, DateTime to)
        {
            return await _context.BroadcastNotifications
                .Where(b => b.CreatedAt >= from && b.CreatedAt <= to)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<BroadcastNotification>> GetByDepartmentIdAsync(int departmentId)
        {
            return await _context.BroadcastNotifications
                .Where(b => b.TargetDepartmentId == departmentId)
                .ToListAsync();
        }

        public async Task<IEnumerable<BroadcastNotification>> GetByLocationIdAsync(int locationId)
        {
            return await _context.BroadcastNotifications
                .Where(b => b.TargetLocationId == locationId)
                .ToListAsync();
        }
    }
}








