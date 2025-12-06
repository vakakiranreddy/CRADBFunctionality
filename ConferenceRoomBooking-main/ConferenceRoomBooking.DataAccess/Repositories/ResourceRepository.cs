using ConferenceRoomBooking.DataAccess.Data;
using ConferenceRoomBooking.DataAccess.Enum;
using ConferenceRoomBooking.DataAccess.Interfaces.IRepositories;
using ConferenceRoomBooking.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace ConferenceRoomBooking.DataAccess.Repositories
{
    public class ResourceRepository : BaseRepository<Resource>, IResourceRepository
    {
        public ResourceRepository(ConferenceRoomBookingDbContext context) : base(context) { }

        public override async Task<Resource?> GetByIdAsync(int id)
        {
            return await _context.Resources
                .Include(r => r.Location)
                .Include(r => r.Building)
                .Include(r => r.Floor)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        // Override to include navigation properties
        public override async Task<IEnumerable<Resource>> GetAllAsync()
        {
            return await _context.Resources
                .Include(r => r.Location)
                .Include(r => r.Building)
                .Include(r => r.Floor)
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Resource>> GetResourcesByTypeAsync(ResourceType resourceType)
        {
            return await _context.Resources
                .Include(r => r.Location)
                .Include(r => r.Building)
                .Include(r => r.Floor)
                .Where(r => r.ResourceType == resourceType)
                .ToListAsync();
        }

        public async Task<IEnumerable<Resource>> GetResourcesByLocationAsync(int locationId)
        {
            return await _context.Resources
                .Include(r => r.Location)
                .Include(r => r.Building)
                .Include(r => r.Floor)
                .Where(r => r.LocationId == locationId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Resource>> GetResourcesByBuildingAsync(int buildingId)
        {
            return await _context.Resources
                .Include(r => r.Location)
                .Include(r => r.Building)
                .Include(r => r.Floor)
                .Where(r => r.BuildingId == buildingId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Resource>> GetResourcesByFloorAsync(int floorId)
        {
            return await _context.Resources
                .Include(r => r.Location)
                .Include(r => r.Building)
                .Include(r => r.Floor)
                .Where(r => r.FloorId == floorId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Resource>> GetAvailableResourcesAsync(ResourceType resourceType, int locationId, DateTime date, TimeSpan startTime, TimeSpan endTime)
        {
            var startDateTime = date.Date.Add(startTime);
            var endDateTime = date.Date.Add(endTime);
            
            var bookedResourceIds = await _context.Bookings
                .Where(b => b.SessionStatus != SessionStatus.Cancelled &&
                           b.StartTime < endDateTime && b.EndTime > startDateTime)
                .Select(b => b.ResourceId)
                .ToListAsync();

            return await _context.Resources
                .Include(r => r.Location)
                .Include(r => r.Building)
                .Include(r => r.Floor)
                .Where(r => r.ResourceType == resourceType &&
                           r.LocationId == locationId &&
                           !r.IsUnderMaintenance &&
                           !r.IsBlocked &&
                           !bookedResourceIds.Contains(r.Id))
                .ToListAsync();
        }

        public async Task<IEnumerable<Resource>> GetResourcesUnderMaintenanceAsync()
        {
            return await _context.Resources
                .Include(r => r.Location)
                .Include(r => r.Building)
                .Include(r => r.Floor)
                .Where(r => r.IsUnderMaintenance)
                .ToListAsync();
        }

        public async Task<IEnumerable<Resource>> GetBlockedResourcesAsync()
        {
            return await _context.Resources
                .Include(r => r.Location)
                .Include(r => r.Building)
                .Include(r => r.Floor)
                .Where(r => r.IsBlocked)
                .ToListAsync();
        }

        public async Task<Resource?> GetResourceWithDetailsAsync(int resourceId)
        {
            return await _context.Resources
                .Include(r => r.Location)
                .Include(r => r.Building)
                .Include(r => r.Floor)
                .FirstOrDefaultAsync(r => r.Id == resourceId);
        }

        public async Task<bool> IsResourceAvailableAsync(int resourceId, DateTime date, TimeSpan startTime, TimeSpan endTime, int? excludeBookingId = null)
        {
            var resource = await _context.Resources.FindAsync(resourceId);
            if (resource == null || resource.IsUnderMaintenance || resource.IsBlocked)
                return false;

            var startDateTime = date.Date.Add(startTime);
            var endDateTime = date.Date.Add(endTime);

            var query = _context.Bookings
                .Where(b => b.ResourceId == resourceId &&
                           b.SessionStatus != SessionStatus.Cancelled &&
                           b.StartTime < endDateTime && b.EndTime > startDateTime);

            if (excludeBookingId.HasValue)
                query = query.Where(b => b.BookingId != excludeBookingId.Value);

            return !await query.AnyAsync();
        }

        public async Task<bool> UpdateMaintenanceStatusAsync(int resourceId, bool isUnderMaintenance)
        {
            var resource = await _context.Resources.FindAsync(resourceId);
            if (resource == null) return false;

            resource.IsUnderMaintenance = isUnderMaintenance;
            resource.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> BlockResourceAsync(int resourceId, DateTime? blockedFrom, DateTime? blockedUntil, string? blockReason)
        {
            var resource = await _context.Resources.FindAsync(resourceId);
            if (resource == null) return false;

            resource.IsBlocked = true;
            resource.BlockedFrom = blockedFrom;
            resource.BlockedUntil = blockedUntil;
            resource.BlockReason = blockReason;
            resource.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnblockResourceAsync(int resourceId)
        {
            var resource = await _context.Resources.FindAsync(resourceId);
            if (resource == null) return false;

            resource.IsBlocked = false;
            resource.BlockedFrom = null;
            resource.BlockedUntil = null;
            resource.BlockReason = null;
            resource.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}








