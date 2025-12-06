using ConferenceRoomBooking.Business.DTOs.Location;
using ConferenceRoomBooking.Business.Helpers;
using ConferenceRoomBooking.DataAccess.Interfaces.IRepositories;
using ConferenceRoomBooking.Business.Interfaces.IServices;
using ConferenceRoomBooking.DataAccess.Models;

namespace ConferenceRoomBooking.Business.Services
{
    public class LocationService : ILocationService
    {
        private readonly ILocationRepository _locationRepository;

        public LocationService(ILocationRepository locationRepository)
        {
            _locationRepository = locationRepository;
        }

        public async Task<LocationResponseDto> CreateLocationAsync(LocationCreateDto locationCreateDto)
        {
            var location = new Location
            {
                Name = locationCreateDto.Name,
                Address = locationCreateDto.Address,
                City = locationCreateDto.City,
                State = locationCreateDto.State,
                Country = locationCreateDto.Country,
                PostalCode = locationCreateDto.PostalCode,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            if (locationCreateDto.LocationImage != null)
            {
                if (!ImageHelper.IsValidImageFile(locationCreateDto.LocationImage))
                    throw new ArgumentException("Invalid image file. Please upload JPG, PNG, or GIF under 2MB");
                location.LocationImage = await ImageHelper.ConvertToByteArrayAsync(locationCreateDto.LocationImage);
            }

            var createdLocation = await _locationRepository.AddAsync(location);
            return MapToResponseDto(createdLocation);
        }

        public async Task<LocationResponseDto> UpdateLocationAsync(int locationId, LocationUpdateDto locationUpdateDto)
        {
            var location = await _locationRepository.GetByIdAsync(locationId);
            if (location == null) throw new ArgumentException("Location not found");

            if (locationUpdateDto.Name != null)
                location.Name = locationUpdateDto.Name;
            if (locationUpdateDto.Address != null)
                location.Address = locationUpdateDto.Address;
            if (locationUpdateDto.City != null)
                location.City = locationUpdateDto.City;
            if (locationUpdateDto.State != null)
                location.State = locationUpdateDto.State;
            if (locationUpdateDto.Country != null)
                location.Country = locationUpdateDto.Country;
            if (locationUpdateDto.PostalCode != null)
                location.PostalCode = locationUpdateDto.PostalCode;
            
            if (locationUpdateDto.LocationImage != null)
            {
                if (!ImageHelper.IsValidImageFile(locationUpdateDto.LocationImage))
                    throw new ArgumentException("Invalid image file. Please upload JPG, PNG, or GIF under 2MB");
                location.LocationImage = await ImageHelper.ConvertToByteArrayAsync(locationUpdateDto.LocationImage);
            }
            
            if (locationUpdateDto.IsActive.HasValue)
                location.IsActive = locationUpdateDto.IsActive.Value;
            
            location.UpdatedAt = DateTime.UtcNow;

            await _locationRepository.UpdateAsync(location);
            return MapToResponseDto(location);
        }

        public async Task<LocationResponseDto?> GetLocationByIdAsync(int locationId)
        {
            var location = await _locationRepository.GetByIdAsync(locationId);
            return location == null ? null : MapToResponseDto(location);
        }

        public async Task<IEnumerable<LocationResponseDto>> GetAllLocationsAsync()
        {
            var locations = await _locationRepository.GetAllAsync();
            return locations.Select(MapToResponseDto);
        }

        public async Task DeleteLocationAsync(int locationId)
        {
            await _locationRepository.DeleteAsync(locationId);
        }

        public async Task<IEnumerable<LocationResponseDto>> SearchLocationsAsync(string searchTerm)
        {
            var locations = await _locationRepository.SearchLocationsAsync(searchTerm);
            return locations.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<LocationResponseDto>> GetLocationsSortedAsync(string sortBy, bool ascending)
        {
            var locations = await _locationRepository.GetLocationsSortedAsync(sortBy, ascending);
            return locations.Select(MapToResponseDto);
        }

        public async Task<int> GetBuildingCountAsync(int locationId)
        {
            return await _locationRepository.GetBuildingCountAsync(locationId);
        }

        private static LocationResponseDto MapToResponseDto(Location location)
        {
            return new LocationResponseDto
            {
                LocationId = location.LocationId,
                Name = location.Name,
                Address = location.Address,
                City = location.City,
                State = location.State,
                Country = location.Country,
                PostalCode = location.PostalCode,
                LocationImage = ImageHelper.ConvertToBase64String(location.LocationImage),
                IsActive = location.IsActive,
                CreatedAt = location.CreatedAt,
                UpdatedAt = location.UpdatedAt
            };
        }
    }
}








