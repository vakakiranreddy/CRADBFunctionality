using ConferenceRoomBooking.DataAccess.Data;
using ConferenceRoomBooking.DataAccess.Interfaces.IRepositories;
using ConferenceRoomBooking.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace ConferenceRoomBooking.DataAccess.Repositories
{
    public class BuildingRepository : BaseRepository<Building>, IBuildingRepository
    {
        public BuildingRepository(ConferenceRoomBookingDbContext context) : base(context) { }

        public override async Task<Building?> GetByIdAsync(int id)
        {
            return await _context.Buildings
                .Include(b => b.Location)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        // Override to include navigation properties
        public override async Task<IEnumerable<Building>> GetAllAsync()
        {
            return await _context.Buildings
                .Include(b => b.Location)
                .OrderBy(b => b.Name)
                .ToListAsync();
        }
        public async Task<IEnumerable<Building>> GetBuildingsByLocationIdAsync(int locationId)
        {
            return await _context.Buildings
                .Include(b => b.Location)
                .Where(b => b.LocationId == locationId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Building>> SearchBuildingsAsync(string searchTerm)
        {
            return await _context.Buildings
                .Include(b => b.Location)
                .Where(b => b.Name.Contains(searchTerm) || b.Address.Contains(searchTerm))
                .ToListAsync();
        }

        public async Task<IEnumerable<Building>> GetBuildingsSortedAsync(string sortBy, bool ascending)
        {
            var query = _context.Buildings.Include(b => b.Location).AsQueryable();
            query = sortBy.ToLower() switch
            {
                "name" => ascending ? query.OrderBy(b => b.Name) : query.OrderByDescending(b => b.Name),
                "location" => ascending ? query.OrderBy(b => b.Location.Name) : query.OrderByDescending(b => b.Location.Name),
                _ => query.OrderBy(b => b.Name)
            };
            return await query.ToListAsync();
        }
    }
}








