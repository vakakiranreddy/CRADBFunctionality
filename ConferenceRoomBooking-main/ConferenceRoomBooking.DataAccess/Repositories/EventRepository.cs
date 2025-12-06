using ConferenceRoomBooking.DataAccess.Data;
using ConferenceRoomBooking.DataAccess.Enum;
using ConferenceRoomBooking.DataAccess.Interfaces.IRepositories;
using ConferenceRoomBooking.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace ConferenceRoomBooking.DataAccess.Repositories
{
    public class EventRepository : BaseRepository<Event>, IEventRepository
    {
        public EventRepository(ConferenceRoomBookingDbContext context) : base(context) { }

        public async Task<IEnumerable<Event>> SearchEventsAsync(string keyword)
        {
            return await _context.Events
                .Include(e => e.Location)
                .Where(e => e.EventName.Contains(keyword) || e.Description.Contains(keyword))
                .ToListAsync();
        }

        public async Task<IEnumerable<Event>> FilterEventsByLocationNameAsync(string locationName)
        {
            return await _context.Events
                .Include(e => e.Location)
                .Where(e => e.Location.Name.Contains(locationName))
                .ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetUpcomingEventsAsync()
        {
            return await _context.Events
                .Include(e => e.Location)
                .Where(e => e.Date >= DateTime.UtcNow.Date)
                .OrderBy(e => e.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetPastEventsAsync()
        {
            return await _context.Events
                .Include(e => e.Location)
                .Where(e => e.Date < DateTime.UtcNow.Date)
                .OrderByDescending(e => e.Date)
                .ToListAsync();
        }

        public async Task<int> GetTotalEventCountAsync()
        {
            return await _context.Events.CountAsync();
        }

        public async Task<int> GetEventParticipantCountAsync(int eventId)
        {
            return await _context.EventRSVPs
                .CountAsync(r => r.EventId == eventId && r.Status == RsvpStatusType.Yes);
        }
    }
}








