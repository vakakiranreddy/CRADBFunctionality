using ConferenceRoomBooking.DataAccess.Models;

namespace ConferenceRoomBooking.DataAccess.Interfaces.IRepositories
{
    public interface IRoomRepository : IBaseRepository<Room>
    {
        Task<Room?> GetRoomByResourceIdAsync(int resourceId);
        Task<IEnumerable<Room>> GetRoomsByLocationAsync(int locationId);
        Task<IEnumerable<Room>> GetRoomsByBuildingAsync(int buildingId);
        Task<IEnumerable<Room>> GetRoomsByFloorAsync(int floorId);
        Task<IEnumerable<Room>> GetRoomsByCapacityAsync(int minCapacity);
        Task<IEnumerable<Room>> GetRoomsWithAmenitiesAsync(bool tv, bool whiteboard, bool wifi, bool projector, bool videoConference, bool airConditioning);
        Task<IEnumerable<Room>> GetAvailableRoomsAsync(int locationId, DateTime date, TimeSpan startTime, TimeSpan endTime);
        Task<bool> RoomExistsAsync(string roomName, int resourceId);
    }

}









