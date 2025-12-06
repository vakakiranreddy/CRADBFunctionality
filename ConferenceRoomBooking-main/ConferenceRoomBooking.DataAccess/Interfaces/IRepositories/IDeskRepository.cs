using ConferenceRoomBooking.DataAccess.Models;

namespace ConferenceRoomBooking.DataAccess.Interfaces.IRepositories
{
    public interface IDeskRepository : IBaseRepository<Desk>
    {
        Task<Desk?> GetDeskByResourceIdAsync(int resourceId);
        Task<IEnumerable<Desk>> GetDesksByLocationAsync(int locationId);
        Task<IEnumerable<Desk>> GetDesksByBuildingAsync(int buildingId);
        Task<IEnumerable<Desk>> GetDesksByFloorAsync(int floorId);
        Task<IEnumerable<Desk>> GetAvailableDesksAsync(int locationId, DateTime date, TimeSpan startTime, TimeSpan endTime);
        Task<bool> DeskExistsAsync(string deskName, int resourceId);
    }

}









