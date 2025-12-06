using ConferenceRoomBooking.Business.DTOs.Desk;

namespace ConferenceRoomBooking.Business.Interfaces.IServices
{
    public interface IDeskService
    {
        Task<DeskResponseDto> CreateDeskAsync(CreateDeskDto createDeskDto);
        Task<DeskResponseDto?> GetDeskByIdAsync(int deskId);
        Task<DeskResponseDto?> GetDeskByResourceIdAsync(int resourceId);
        Task<IEnumerable<DeskResponseDto>> GetAllDesksAsync();
        Task<IEnumerable<DeskResponseDto>> GetDesksByLocationAsync(int locationId);
        Task<IEnumerable<DeskResponseDto>> GetDesksByBuildingAsync(int buildingId);
        Task<IEnumerable<DeskResponseDto>> GetDesksByFloorAsync(int floorId);
        Task<IEnumerable<DeskResponseDto>> GetAvailableDesksAsync(DeskAvailabilityRequestDto requestDto);
        Task<DeskResponseDto> UpdateDeskAsync(int deskId, UpdateDeskDto updateDeskDto);
        Task<bool> DeleteDeskAsync(int deskId);
    }

}









