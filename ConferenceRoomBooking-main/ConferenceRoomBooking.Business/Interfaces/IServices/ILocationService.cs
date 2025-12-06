using ConferenceRoomBooking.Business.DTOs.Location;

namespace ConferenceRoomBooking.Business.Interfaces.IServices
{
    public interface ILocationService
    {
        Task<LocationResponseDto> CreateLocationAsync(LocationCreateDto locationCreateDto);
        Task<LocationResponseDto> UpdateLocationAsync(int locationId, LocationUpdateDto locationUpdateDto);
        Task<LocationResponseDto?> GetLocationByIdAsync(int locationId);
        Task<IEnumerable<LocationResponseDto>> GetAllLocationsAsync();
        Task DeleteLocationAsync(int locationId);

        Task<IEnumerable<LocationResponseDto>> SearchLocationsAsync(string searchTerm);
        Task<IEnumerable<LocationResponseDto>> GetLocationsSortedAsync(string sortBy, bool ascending);
        Task<int> GetBuildingCountAsync(int locationId);
    }
}









