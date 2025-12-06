using ConferenceRoomBooking.Business.DTOs.Building;
using ConferenceRoomBooking.Business.Helpers;
using ConferenceRoomBooking.DataAccess.Interfaces.IRepositories;
using ConferenceRoomBooking.Business.Interfaces.IServices;
using ConferenceRoomBooking.DataAccess.Models;

namespace ConferenceRoomBooking.Business.Services
{
    public class BuildingService : IBuildingService
    {
        private readonly IBuildingRepository _buildingRepository;

        public BuildingService(IBuildingRepository buildingRepository)
        {
            _buildingRepository = buildingRepository;
        }

        public async Task<BuildingResponseDto> CreateBuildingAsync(BuildingCreateDto buildingCreateDto)
        {
            var building = new Building
            {
                Name = buildingCreateDto.Name,
                LocationId = buildingCreateDto.LocationId,
                Address = buildingCreateDto.Address,
                NumberOfFloors = buildingCreateDto.NumberOfFloors,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            if (buildingCreateDto.BuildingImage != null)
            {
                if (!ImageHelper.IsValidImageFile(buildingCreateDto.BuildingImage))
                    throw new ArgumentException("Invalid image file. Please upload JPG, PNG, or GIF under 2MB");
                building.BuildingImage = await ImageHelper.ConvertToByteArrayAsync(buildingCreateDto.BuildingImage);
            }

            var createdBuilding = await _buildingRepository.AddAsync(building);
            return MapToResponseDto(createdBuilding);
        }

        public async Task<BuildingResponseDto> UpdateBuildingAsync(int buildingId, BuildingUpdateDto buildingUpdateDto)
        {
            var building = await _buildingRepository.GetByIdAsync(buildingId);
            if (building == null) throw new ArgumentException("Building not found");

            if (buildingUpdateDto.Name != null)
                building.Name = buildingUpdateDto.Name;

            if (buildingUpdateDto.LocationId.HasValue)
                building.LocationId = buildingUpdateDto.LocationId.Value;

            if (buildingUpdateDto.Address != null)
                building.Address = buildingUpdateDto.Address;

            if (buildingUpdateDto.NumberOfFloors.HasValue)
                building.NumberOfFloors = buildingUpdateDto.NumberOfFloors.Value;

            if (buildingUpdateDto.BuildingImage != null)
            {
                if (!ImageHelper.IsValidImageFile(buildingUpdateDto.BuildingImage))
                    throw new ArgumentException("Invalid image file. Please upload JPG, PNG, or GIF under 2MB");
                building.BuildingImage = await ImageHelper.ConvertToByteArrayAsync(buildingUpdateDto.BuildingImage);
            }

            if (buildingUpdateDto.IsActive.HasValue)
                building.IsActive = buildingUpdateDto.IsActive.Value;

            building.UpdatedAt = DateTime.UtcNow;

            await _buildingRepository.UpdateAsync(building);
            return MapToResponseDto(building);
        }

        public async Task<BuildingResponseDto?> GetBuildingByIdAsync(int buildingId)
        {
            var building = await _buildingRepository.GetByIdAsync(buildingId);
            return building == null ? null : MapToResponseDto(building);
        }

        public async Task<IEnumerable<BuildingResponseDto>> GetAllBuildingsAsync()
        {
            var buildings = await _buildingRepository.GetAllAsync();
            return buildings.Select(MapToResponseDto);
        }

        public async Task DeleteBuildingAsync(int buildingId)
        {
            await _buildingRepository.DeleteAsync(buildingId);
        }

        public async Task<IEnumerable<BuildingResponseDto>> GetBuildingsByLocationIdAsync(int locationId)
        {
            var buildings = await _buildingRepository.GetBuildingsByLocationIdAsync(locationId);
            return buildings.Select(MapToResponseDto);
        }

        // ADD THESE TWO METHODS IF YOUR INTERFACE REQUIRES THEM
        public async Task<IEnumerable<BuildingResponseDto>> SearchBuildingsAsync(string searchTerm)
        {
            var buildings = await _buildingRepository.SearchBuildingsAsync(searchTerm);
            return buildings.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<BuildingResponseDto>> GetBuildingsSortedAsync(string sortBy, bool ascending)
        {
            var buildings = await _buildingRepository.GetBuildingsSortedAsync(sortBy, ascending);
            return buildings.Select(MapToResponseDto);
        }

        private static BuildingResponseDto MapToResponseDto(Building building)
        {
            return new BuildingResponseDto
            {
                Id = building.Id,
                Name = building.Name,
                LocationId = building.LocationId,
                LocationName = building.Location?.Name,
                Address = building.Address,
                NumberOfFloors = building.NumberOfFloors,
                BuildingImage = ImageHelper.ConvertToBase64String(building.BuildingImage),
                IsActive = building.IsActive,
                CreatedAt = building.CreatedAt,
                UpdatedAt = building.UpdatedAt
            };
        }
    }
}








