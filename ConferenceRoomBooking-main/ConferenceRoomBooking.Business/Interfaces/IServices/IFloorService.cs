using ConferenceRoomBooking.Business.DTOs.Floor;

namespace ConferenceRoomBooking.Business.Interfaces.IServices
{
    public interface IFloorService
    {
        Task<FloorResponseDto> CreateFloorAsync(FloorCreateDto floorCreateDto);
        Task<FloorResponseDto> UpdateFloorAsync(int floorId, FloorUpdateDto floorUpdateDto);
        Task<FloorResponseDto?> GetFloorByIdAsync(int floorId);
        Task<IEnumerable<FloorResponseDto>> GetAllFloorsAsync();
        Task DeleteFloorAsync(int floorId);

        Task<IEnumerable<FloorResponseDto>> GetFloorsByBuildingIdAsync(int buildingId);
        Task<IEnumerable<FloorResponseDto>> SearchFloorsAsync(string searchTerm);
        Task<IEnumerable<FloorResponseDto>> GetFloorsSortedAsync(string sortBy, bool ascending);
    }
}









