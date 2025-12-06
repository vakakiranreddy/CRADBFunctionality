using ConferenceRoomBooking.Business.DTOs.Building;

namespace ConferenceRoomBooking.Business.Interfaces.IServices
{
    public interface IBuildingService
    {
        Task<BuildingResponseDto> CreateBuildingAsync(BuildingCreateDto buildingCreateDto);
        Task<BuildingResponseDto> UpdateBuildingAsync(int buildingId, BuildingUpdateDto buildingUpdateDto);
        Task<BuildingResponseDto?> GetBuildingByIdAsync(int buildingId);
        Task<IEnumerable<BuildingResponseDto>> GetAllBuildingsAsync();
        Task DeleteBuildingAsync(int buildingId);

        Task<IEnumerable<BuildingResponseDto>> GetBuildingsByLocationIdAsync(int locationId);
        Task<IEnumerable<BuildingResponseDto>> SearchBuildingsAsync(string searchTerm);
        Task<IEnumerable<BuildingResponseDto>> GetBuildingsSortedAsync(string sortBy, bool ascending);
    }
}









