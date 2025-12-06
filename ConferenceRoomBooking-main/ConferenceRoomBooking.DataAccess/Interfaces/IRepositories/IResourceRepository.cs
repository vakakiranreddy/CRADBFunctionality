using ConferenceRoomBooking.DataAccess.Enum;
using ConferenceRoomBooking.DataAccess.Models;

namespace ConferenceRoomBooking.DataAccess.Interfaces.IRepositories
{
    public interface IResourceRepository : IBaseRepository<Resource>
    {
        Task<IEnumerable<Resource>> GetResourcesByTypeAsync(ResourceType resourceType);
        Task<IEnumerable<Resource>> GetResourcesByLocationAsync(int locationId);
        Task<IEnumerable<Resource>> GetResourcesByBuildingAsync(int buildingId);
        Task<IEnumerable<Resource>> GetResourcesByFloorAsync(int floorId);
        Task<IEnumerable<Resource>> GetAvailableResourcesAsync(ResourceType resourceType, int locationId, DateTime date, TimeSpan startTime, TimeSpan endTime);
        Task<IEnumerable<Resource>> GetResourcesUnderMaintenanceAsync();
        Task<IEnumerable<Resource>> GetBlockedResourcesAsync();
        Task<Resource?> GetResourceWithDetailsAsync(int resourceId);
        Task<bool> IsResourceAvailableAsync(int resourceId, DateTime date, TimeSpan startTime, TimeSpan endTime, int? excludeBookingId = null);
        Task<bool> UpdateMaintenanceStatusAsync(int resourceId, bool isUnderMaintenance);
        Task<bool> BlockResourceAsync(int resourceId, DateTime? blockedFrom, DateTime? blockedUntil, string? blockReason);
        Task<bool> UnblockResourceAsync(int resourceId);
    }

}









