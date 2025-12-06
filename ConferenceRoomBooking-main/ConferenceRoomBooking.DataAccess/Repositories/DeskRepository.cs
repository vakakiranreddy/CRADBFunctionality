using ConferenceRoomBooking.DataAccess.Data;
using ConferenceRoomBooking.DataAccess.Enum;
using ConferenceRoomBooking.DataAccess.Interfaces.IRepositories;
using ConferenceRoomBooking.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace ConferenceRoomBooking.DataAccess.Repositories
{
    public class DeskRepository : BaseRepository<Desk>, IDeskRepository
    {
        public DeskRepository(ConferenceRoomBookingDbContext context) : base(context) { }

        public async Task<Desk?> GetDeskByResourceIdAsync(int resourceId)
        {
            return await _context.Desks
                .Include(d => d.Resource)
                .ThenInclude(r => r.Location)
                .Include(d => d.Resource)
                .ThenInclude(r => r.Building)
                .Include(d => d.Resource)
                .ThenInclude(r => r.Floor)
                .FirstOrDefaultAsync(d => d.ResourceId == resourceId);
        }

        public async Task<IEnumerable<Desk>> GetDesksByLocationAsync(int locationId)
        {
            return await _context.Desks
                .Include(d => d.Resource)
                .Where(d => d.Resource.LocationId == locationId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Desk>> GetDesksByBuildingAsync(int buildingId)
        {
            return await _context.Desks
                .Include(d => d.Resource)
                .Where(d => d.Resource.BuildingId == buildingId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Desk>> GetDesksByFloorAsync(int floorId)
        {
            return await _context.Desks
                .Include(d => d.Resource)
                .Where(d => d.Resource.FloorId == floorId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Desk>> GetAvailableDesksAsync(int locationId, DateTime date, TimeSpan startTime, TimeSpan endTime)
        {
            var startDateTime = date.Date.Add(startTime);
            var endDateTime = date.Date.Add(endTime);
            
            var bookedResourceIds = await _context.Bookings
                .Where(b => b.SessionStatus != SessionStatus.Cancelled &&
                           b.StartTime < endDateTime && b.EndTime > startDateTime)
                .Select(b => b.ResourceId)
                .ToListAsync();

            return await _context.Desks
                .Include(d => d.Resource)
                .Where(d => d.Resource.LocationId == locationId &&
                           d.Resource.ResourceType == ResourceType.Desk &&
                           !d.Resource.IsUnderMaintenance &&
                           !d.Resource.IsBlocked &&
                           !bookedResourceIds.Contains(d.ResourceId))
                .ToListAsync();
        }

        public async Task<bool> DeskExistsAsync(string deskName, int resourceId)
        {
            return await _context.Desks
                .AnyAsync(d => d.DeskName == deskName && d.ResourceId != resourceId);
        }
    }
}








