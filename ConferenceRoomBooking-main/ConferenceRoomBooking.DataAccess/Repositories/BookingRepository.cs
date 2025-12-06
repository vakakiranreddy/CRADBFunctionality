using ConferenceRoomBooking.DataAccess.Data;
using ConferenceRoomBooking.DataAccess.Enum;
using ConferenceRoomBooking.DataAccess.Interfaces.IRepositories;
using ConferenceRoomBooking.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace ConferenceRoomBooking.DataAccess.Repositories
{
    public class BookingRepository : BaseRepository<Booking>, IBookingRepository
    {
        public BookingRepository(ConferenceRoomBookingDbContext context) : base(context) { }
        
        public override async Task<Booking?> GetByIdAsync(int id)
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Resource)
                    .ThenInclude(r => r.Room)
                .Include(b => b.Resource)
                    .ThenInclude(r => r.Desk)
                .FirstOrDefaultAsync(b => b.BookingId == id);
        }

        public async Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(int userId)
        {
            return await _context.Bookings
                .Include(b => b.Resource)
                .Include(b => b.User)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetBookingsByResourceIdAsync(int resourceId)
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Where(b => b.ResourceId == resourceId)
                .OrderBy(b => b.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetBookingsByStatusAsync(SessionStatus status)
        {
            return await _context.Bookings
                .Include(b => b.Resource)
                .Include(b => b.User)
                .Where(b => b.SessionStatus == status)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetBookingsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Bookings
                .Include(b => b.Resource)
                .Include(b => b.User)
                .Where(b => b.StartTime >= startDate && b.StartTime <= endDate)
                .OrderBy(b => b.StartTime)
                .ToListAsync();
        }

        public async Task<bool> IsResourceAvailableAsync(int resourceId, DateTime startTime, DateTime endTime, int? excludeBookingId = null)
        {
            var query = _context.Bookings
                .Where(b => b.ResourceId == resourceId &&
                           b.SessionStatus != SessionStatus.Cancelled &&
                           ((b.StartTime < endTime && b.EndTime > startTime)));

            if (excludeBookingId.HasValue)
                query = query.Where(b => b.BookingId != excludeBookingId.Value);

            return !await query.AnyAsync();
        }

        public async Task<bool> CancelBookingAsync(int bookingId, string cancellationReason)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking == null) return false;

            booking.SessionStatus = SessionStatus.Cancelled;
            booking.CancellationReason = cancellationReason;
            booking.CancelledAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Booking>> GetNoShowBookingsAsync()
        {
            var currentTime = DateTime.UtcNow;
            return await _context.Bookings
                .Include(b => b.Resource)
                .Include(b => b.User)
                .Where(b => b.SessionStatus == SessionStatus.Reserved &&
                           b.EndTime <= currentTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetOverdueBookingsAsync()
        {
            var currentTime = DateTime.UtcNow;
            return await _context.Bookings
                .Include(b => b.Resource)
                .Include(b => b.User)
                .Where(b => b.SessionStatus == SessionStatus.CheckedIn &&
                           b.EndTime < currentTime)
                .ToListAsync();
        }
    }
}








