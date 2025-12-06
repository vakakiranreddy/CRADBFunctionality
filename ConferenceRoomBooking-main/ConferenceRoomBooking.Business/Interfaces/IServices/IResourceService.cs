using ConferenceRoomBooking.Business.DTOs.Resource;

namespace ConferenceRoomBooking.Business.Interfaces.IServices
{
    public interface IResourceService
    {
        Task<ResourceResponseDto> CreateResourceAsync(CreateResourceDto createResourceDto);
        Task<ResourceResponseDto?> GetResourceByIdAsync(int resourceId);
        Task<ResourceDetailResponseDto?> GetResourceWithDetailsAsync(int resourceId);
        Task<IEnumerable<ResourceResponseDto>> GetAllResourcesAsync();
        Task<IEnumerable<ResourceResponseDto>> GetResourcesByTypeAsync(ResourceTypeDto typeDto);
        Task<IEnumerable<ResourceResponseDto>> GetResourcesByLocationAsync(int locationId);
        Task<IEnumerable<ResourceResponseDto>> GetResourcesByBuildingAsync(int buildingId);
        Task<IEnumerable<ResourceResponseDto>> GetResourcesByFloorAsync(int floorId);
        Task<IEnumerable<ResourceResponseDto>> GetAvailableResourcesAsync(ResourceAvailabilityRequestDto requestDto);
        Task<IEnumerable<ResourceResponseDto>> GetResourcesUnderMaintenanceAsync();
        Task<IEnumerable<ResourceResponseDto>> GetBlockedResourcesAsync();
        Task<bool> IsResourceAvailableAsync(ResourceAvailabilityCheckDto checkDto);
        Task<ResourceResponseDto> UpdateResourceAsync(int resourceId, UpdateResourceDto updateResourceDto);
        Task<bool> UpdateMaintenanceStatusAsync(int resourceId, MaintenanceStatusDto statusDto);
        Task<bool> BlockResourceAsync(int resourceId, BlockResourceDto blockDto);
        Task<bool> UnblockResourceAsync(int resourceId);
        Task<bool> DeleteResourceAsync(int resourceId);
    }

}









