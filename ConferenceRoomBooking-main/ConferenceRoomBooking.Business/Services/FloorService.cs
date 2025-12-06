using ConferenceRoomBooking.Business.DTOs.Floor;
using ConferenceRoomBooking.DataAccess.Interfaces.IRepositories;
using ConferenceRoomBooking.Business.Interfaces.IServices;
using ConferenceRoomBooking.DataAccess.Models;

namespace ConferenceRoomBooking.Business.Services
{
    public class FloorService : IFloorService
    {
        private readonly IFloorRepository _floorRepository;

        public FloorService(IFloorRepository floorRepository)
        {
            _floorRepository = floorRepository;
        }

        public async Task<FloorResponseDto> CreateFloorAsync(FloorCreateDto floorCreateDto)
        {
            var floor = new Floor
            {
                FloorName = floorCreateDto.FloorName,
                FloorNumber = floorCreateDto.FloorNumber,
                BuildingId = floorCreateDto.BuildingId,
                LocationId = floorCreateDto.LocationId,
                FloorPlanImage = floorCreateDto.FloorPlanImage
            };

            var createdFloor = await _floorRepository.AddAsync(floor);
            return MapToResponseDto(createdFloor);
        }

        public async Task<FloorResponseDto> UpdateFloorAsync(int floorId, FloorUpdateDto floorUpdateDto)
        {
            var floor = await _floorRepository.GetByIdAsync(floorId);
            if (floor == null) throw new ArgumentException("Floor not found");

            // Only update properties if they are provided (not null)
            if (floorUpdateDto.FloorName != null)
                floor.FloorName = floorUpdateDto.FloorName;

            if (floorUpdateDto.FloorNumber.HasValue)
                floor.FloorNumber = floorUpdateDto.FloorNumber.Value;

            if (floorUpdateDto.BuildingId.HasValue)
                floor.BuildingId = floorUpdateDto.BuildingId.Value;

            if (floorUpdateDto.LocationId.HasValue)
                floor.LocationId = floorUpdateDto.LocationId.Value;

            if (floorUpdateDto.FloorPlanImage != null)
                floor.FloorPlanImage = floorUpdateDto.FloorPlanImage;

            if (floorUpdateDto.IsActive.HasValue)
                floor.IsActive = floorUpdateDto.IsActive.Value;

            await _floorRepository.UpdateAsync(floor);
            return MapToResponseDto(floor);
        }

        public async Task<FloorResponseDto?> GetFloorByIdAsync(int floorId)
        {
            var floor = await _floorRepository.GetByIdAsync(floorId);
            return floor == null ? null : MapToResponseDto(floor);
        }

        public async Task<IEnumerable<FloorResponseDto>> GetAllFloorsAsync()
        {
            var floors = await _floorRepository.GetAllAsync();
            return floors.Select(MapToResponseDto);
        }

        public async Task DeleteFloorAsync(int floorId)
        {
            await _floorRepository.DeleteAsync(floorId);
        }

        public async Task<IEnumerable<FloorResponseDto>> GetFloorsByBuildingIdAsync(int buildingId)
        {
            var floors = await _floorRepository.GetFloorsByBuildingIdAsync(buildingId);
            return floors.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<FloorResponseDto>> SearchFloorsAsync(string searchTerm)
        {
            var floors = await _floorRepository.SearchFloorsAsync(searchTerm);
            return floors.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<FloorResponseDto>> GetFloorsSortedAsync(string sortBy, bool ascending)
        {
            var floors = await _floorRepository.GetFloorsSortedAsync(sortBy, ascending);
            return floors.Select(MapToResponseDto);
        }

        private static FloorResponseDto MapToResponseDto(Floor floor)
        {
            return new FloorResponseDto
            {
                Id = floor.Id,
                FloorName = floor.FloorName,
                FloorNumber = floor.FloorNumber,
                BuildingId = floor.BuildingId,
                BuildingName = floor.Building?.Name,
                LocationId = floor.LocationId,
                LocationName = floor.Location?.Name,
                FloorPlanImage = floor.FloorPlanImage,
                IsActive = floor.IsActive
            };
        }
    }
}








