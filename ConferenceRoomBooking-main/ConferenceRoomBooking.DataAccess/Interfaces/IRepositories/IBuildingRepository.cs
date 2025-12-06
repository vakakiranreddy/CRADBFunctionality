using ConferenceRoomBooking.DataAccess.Models;

namespace ConferenceRoomBooking.DataAccess.Interfaces.IRepositories
{
    public interface IBuildingRepository : IBaseRepository<Building>
    {
        Task<IEnumerable<Building>> GetBuildingsByLocationIdAsync(int locationId);
        Task<IEnumerable<Building>> SearchBuildingsAsync(string searchTerm);
        Task<IEnumerable<Building>> GetBuildingsSortedAsync(string sortBy, bool ascending);
    }
}









