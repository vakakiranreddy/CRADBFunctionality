using ConferenceRoomBooking.DataAccess.Data;
using ConferenceRoomBooking.DataAccess.Enum;
using ConferenceRoomBooking.DataAccess.Interfaces.IRepositories;
using ConferenceRoomBooking.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace ConferenceRoomBooking.DataAccess.Repositories
{
    public class RoomRepository : BaseRepository<Room>, IRoomRepository
    {
        public RoomRepository(ConferenceRoomBookingDbContext context) : base(context) { }

        // Override to include navigation properties
        public override async Task<Room?> GetByIdAsync(int id)
        {
            return await _context.Rooms
                .Include(r => r.Resource)
                    .ThenInclude(res => res.Location)
                .Include(r => r.Resource)
                    .ThenInclude(res => res.Building)
                .Include(r => r.Resource)
                    .ThenInclude(res => res.Floor)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        // Override to include navigation properties
        public override async Task<IEnumerable<Room>> GetAllAsync()
        {
            return await _context.Rooms
                .Include(r => r.Resource)
                    .ThenInclude(res => res.Location)
                .Include(r => r.Resource)
                    .ThenInclude(res => res.Building)
                .Include(r => r.Resource)
                    .ThenInclude(res => res.Floor)
                .OrderBy(r => r.RoomName)
                .ToListAsync();
        }

        public async Task<Room?> GetRoomByResourceIdAsync(int resourceId)
        {
            return await _context.Rooms
                .Include(r => r.Resource)
                    .ThenInclude(res => res.Location)
                .Include(r => r.Resource)
                    .ThenInclude(res => res.Building)
                .Include(r => r.Resource)
                    .ThenInclude(res => res.Floor)
                .FirstOrDefaultAsync(r => r.ResourceId == resourceId);
        }

        public async Task<IEnumerable<Room>> GetRoomsByLocationAsync(int locationId)
        {
            return await _context.Rooms
                .Include(r => r.Resource)
                .Where(r => r.Resource.LocationId == locationId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Room>> GetRoomsByBuildingAsync(int buildingId)
        {
            return await _context.Rooms
                .Include(r => r.Resource)
                .Where(r => r.Resource.BuildingId == buildingId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Room>> GetRoomsByFloorAsync(int floorId)
        {
            return await _context.Rooms
                .Include(r => r.Resource)
                .Where(r => r.Resource.FloorId == floorId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Room>> GetRoomsByCapacityAsync(int minCapacity)
        {
            return await _context.Rooms
                .Include(r => r.Resource)
                .Where(r => r.Capacity >= minCapacity)
                .ToListAsync();
        }

        public async Task<IEnumerable<Room>> GetRoomsWithAmenitiesAsync(bool tv, bool whiteboard, bool wifi, bool projector, bool videoConference, bool airConditioning)
        {
            return await _context.Rooms
                .Include(r => r.Resource)
                .Where(r => (!tv || r.HasTV) &&
                           (!whiteboard || r.HasWhiteboard) &&
                           (!wifi || r.HasWiFi) &&
                           (!projector || r.HasProjector) &&
                           (!videoConference || r.HasVideoConference) &&
                           (!airConditioning || r.HasAirConditioning))
                .ToListAsync();
        }

        public async Task<IEnumerable<Room>> GetAvailableRoomsAsync(int locationId, DateTime date, TimeSpan startTime, TimeSpan endTime)
        {
            var startDateTime = date.Date.Add(startTime);
            var endDateTime = date.Date.Add(endTime);
            
            var bookedResourceIds = await _context.Bookings
                .Where(b => b.SessionStatus != SessionStatus.Cancelled &&
                           b.StartTime < endDateTime && b.EndTime > startDateTime)
                .Select(b => b.ResourceId)
                .ToListAsync();

            return await _context.Rooms
                .Include(r => r.Resource)
                .Where(r => r.Resource.LocationId == locationId &&
                           r.Resource.ResourceType == ResourceType.Room &&
                           !r.Resource.IsUnderMaintenance &&
                           !r.Resource.IsBlocked &&
                           !bookedResourceIds.Contains(r.ResourceId))
                .ToListAsync();
        }

        public async Task<bool> RoomExistsAsync(string roomName, int resourceId)
        {
            return await _context.Rooms
                .AnyAsync(r => r.RoomName == roomName && r.ResourceId != resourceId);
        }
    }
}








