using ConferenceRoomBooking.DataAccess.Data;
using ConferenceRoomBooking.DataAccess.Interfaces.IRepositories;
using ConferenceRoomBooking.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace ConferenceRoomBooking.DataAccess.Repositories
{
    public class FloorRepository : BaseRepository<Floor>, IFloorRepository
    {
        public FloorRepository(ConferenceRoomBookingDbContext context) : base(context) { }

        // Override to include navigation properties
        public override async Task<Floor?> GetByIdAsync(int id)
        {
            return await _context.Floors
                .Include(f => f.Building)
                .Include(f => f.Location)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        // Override to include navigation properties
        public override async Task<IEnumerable<Floor>> GetAllAsync()
        {
            return await _context.Floors
                .Include(f => f.Building)
                .Include(f => f.Location)
                .OrderBy(f => f.FloorNumber)
                .ToListAsync();
        }

        public async Task<IEnumerable<Floor>> GetFloorsByBuildingIdAsync(int buildingId)
        {
            return await _context.Floors
                .Include(f => f.Building)
                .Include(f => f.Location)
                .Where(f => f.BuildingId == buildingId)
                .OrderBy(f => f.FloorNumber)
                .ToListAsync();
        }

        public async Task<IEnumerable<Floor>> SearchFloorsAsync(string searchTerm)
        {
            return await _context.Floors
                .Include(f => f.Building)
                .Include(f => f.Location)
                .Where(f => f.FloorName.Contains(searchTerm) || f.Building.Name.Contains(searchTerm))
                .ToListAsync();
        }

        public async Task<IEnumerable<Floor>> GetFloorsSortedAsync(string sortBy, bool ascending)
        {
            var query = _context.Floors
                .Include(f => f.Building)
                .Include(f => f.Location)
                .AsQueryable();

            query = sortBy.ToLower() switch
            {
                "name" => ascending ? query.OrderBy(f => f.FloorName) : query.OrderByDescending(f => f.FloorName),
                "number" => ascending ? query.OrderBy(f => f.FloorNumber) : query.OrderByDescending(f => f.FloorNumber),
                "building" => ascending ? query.OrderBy(f => f.Building.Name) : query.OrderByDescending(f => f.Building.Name),
                _ => query.OrderBy(f => f.FloorNumber)
            };

            return await query.ToListAsync();
        }
    }
}








