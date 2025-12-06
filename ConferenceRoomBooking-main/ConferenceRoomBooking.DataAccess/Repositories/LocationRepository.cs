using ConferenceRoomBooking.DataAccess.Data;
using ConferenceRoomBooking.DataAccess.Interfaces.IRepositories;
using ConferenceRoomBooking.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace ConferenceRoomBooking.DataAccess.Repositories
{
    public class LocationRepository : BaseRepository<Location>, ILocationRepository
    {
        public LocationRepository(ConferenceRoomBookingDbContext context) : base(context) { }

        public async Task<IEnumerable<Location>> SearchLocationsAsync(string searchTerm)
        {
            return await _context.Locations
                .Where(l => l.Name.Contains(searchTerm) || l.Address.Contains(searchTerm) || l.City.Contains(searchTerm))
                .ToListAsync();
        }

        public async Task<IEnumerable<Location>> GetLocationsSortedAsync(string sortBy, bool ascending)
        {
            var query = _context.Locations.AsQueryable();
            query = sortBy.ToLower() switch
            {
                "name" => ascending ? query.OrderBy(l => l.Name) : query.OrderByDescending(l => l.Name),
                "city" => ascending ? query.OrderBy(l => l.City) : query.OrderByDescending(l => l.City),
                _ => query.OrderBy(l => l.Name)
            };
            return await query.ToListAsync();
        }

        public async Task<int> GetBuildingCountAsync(int locationId)
        {
            return await _context.Buildings.CountAsync(b => b.LocationId == locationId);
        }
    }
}








