using ConferenceRoomBooking.Business.DTOs.Room;
using ConferenceRoomBooking.Business.Helpers;
using ConferenceRoomBooking.DataAccess.Interfaces.IRepositories;
using ConferenceRoomBooking.Business.Interfaces.IServices;
using ConferenceRoomBooking.DataAccess.Models;

namespace ConferenceRoomBooking.Business.Services
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IResourceRepository _resourceRepository;

        public RoomService(IRoomRepository roomRepository, IResourceRepository resourceRepository)
        {
            _roomRepository = roomRepository;
            _resourceRepository = resourceRepository;
        }

        public async Task<RoomResponseDto> CreateRoomAsync(CreateRoomDto createRoomDto)
        {
            // Validate that the resource exists
            var resource = await _resourceRepository.GetByIdAsync(createRoomDto.ResourceId);
            if (resource == null)
                throw new ArgumentException($"Resource with ID {createRoomDto.ResourceId} does not exist. Please create a resource first.");

            // Check if resource type is Room (1)
            if ((int)resource.ResourceType != 1)
                throw new ArgumentException($"Resource with ID {createRoomDto.ResourceId} is not a Room type");

            // Check if room already exists for this resource
            var existingRoom = await _roomRepository.GetRoomByResourceIdAsync(createRoomDto.ResourceId);
            if (existingRoom != null)
                throw new ArgumentException($"A room already exists for Resource ID {createRoomDto.ResourceId}");

            var room = new Room
            {
                ResourceId = createRoomDto.ResourceId,
                RoomName = createRoomDto.RoomName,
                Capacity = createRoomDto.Capacity,
                HasTV = createRoomDto.HasTV,
                HasWhiteboard = createRoomDto.HasWhiteboard,
                HasWiFi = createRoomDto.HasWiFi,
                HasProjector = createRoomDto.HasProjector,
                HasVideoConference = createRoomDto.HasVideoConference,
                HasAirConditioning = createRoomDto.HasAirConditioning,
                PhoneExtension = createRoomDto.PhoneExtension
            };

            if (createRoomDto.RoomImage != null)
            {
                if (!ImageHelper.IsValidImageFile(createRoomDto.RoomImage))
                    throw new ArgumentException("Invalid image file. Please upload a valid image (JPG, PNG, GIF, BMP) under 5MB");
                room.RoomImage = await ImageHelper.ConvertToByteArrayAsync(createRoomDto.RoomImage);
            }

            var createdRoom = await _roomRepository.AddAsync(room);
            return await MapToResponseDto(createdRoom);
        }

        public async Task<RoomResponseDto?> GetRoomByIdAsync(int roomId)
        {
            var room = await _roomRepository.GetByIdAsync(roomId);
            return room == null ? null : await MapToResponseDto(room);
        }

        public async Task<RoomResponseDto?> GetRoomByResourceIdAsync(int resourceId)
        {
            var room = await _roomRepository.GetRoomByResourceIdAsync(resourceId);
            return room == null ? null : await MapToResponseDto(room);
        }

        public async Task<IEnumerable<RoomResponseDto>> GetAllRoomsAsync()
        {
            var rooms = await _roomRepository.GetAllAsync();
            var tasks = rooms.Select(MapToResponseDto);
            return await Task.WhenAll(tasks);
        }

        public async Task<IEnumerable<RoomResponseDto>> GetRoomsByLocationAsync(int locationId)
        {
            var rooms = await _roomRepository.GetRoomsByLocationAsync(locationId);
            var tasks = rooms.Select(MapToResponseDto);
            return await Task.WhenAll(tasks);
        }

        public async Task<IEnumerable<RoomResponseDto>> GetRoomsByBuildingAsync(int buildingId)
        {
            var rooms = await _roomRepository.GetRoomsByBuildingAsync(buildingId);
            var tasks = rooms.Select(MapToResponseDto);
            return await Task.WhenAll(tasks);
        }

        public async Task<IEnumerable<RoomResponseDto>> GetRoomsByFloorAsync(int floorId)
        {
            var rooms = await _roomRepository.GetRoomsByFloorAsync(floorId);
            var tasks = rooms.Select(MapToResponseDto);
            return await Task.WhenAll(tasks);
        }

        public async Task<IEnumerable<RoomResponseDto>> GetRoomsByCapacityAsync(int minCapacity)
        {
            var rooms = await _roomRepository.GetRoomsByCapacityAsync(minCapacity);
            var tasks = rooms.Select(MapToResponseDto);
            return await Task.WhenAll(tasks);
        }

        public async Task<IEnumerable<RoomResponseDto>> SearchRoomsWithAmenitiesAsync(RoomAmenityFilterDto filterDto)
        {
            var rooms = await _roomRepository.GetRoomsWithAmenitiesAsync(
                filterDto.HasTV ?? false,
                filterDto.HasWhiteboard ?? false,
                filterDto.HasWiFi ?? false,
                filterDto.HasProjector ?? false,
                filterDto.HasVideoConference ?? false,
                filterDto.HasAirConditioning ?? false);
            var tasks = rooms.Select(MapToResponseDto);
            return await Task.WhenAll(tasks);
        }

        public async Task<IEnumerable<RoomResponseDto>> GetAvailableRoomsAsync(RoomAvailabilityRequestDto requestDto)
        {
            var rooms = await _roomRepository.GetAvailableRoomsAsync(
                requestDto.LocationId, requestDto.Date, requestDto.StartTime, requestDto.EndTime);
            var tasks = rooms.Select(MapToResponseDto);
            return await Task.WhenAll(tasks);
        }

        public async Task<RoomResponseDto> UpdateRoomAsync(int roomId, UpdateRoomDto updateRoomDto)
        {
            var room = await _roomRepository.GetByIdAsync(roomId);
            if (room == null) throw new ArgumentException("Room not found");

            if (updateRoomDto.RoomName != null)
                room.RoomName = updateRoomDto.RoomName;

            if (updateRoomDto.Capacity.HasValue)
                room.Capacity = updateRoomDto.Capacity.Value;

            if (updateRoomDto.HasTV.HasValue)
                room.HasTV = updateRoomDto.HasTV.Value;

            if (updateRoomDto.HasWhiteboard.HasValue)
                room.HasWhiteboard = updateRoomDto.HasWhiteboard.Value;

            if (updateRoomDto.HasWiFi.HasValue)
                room.HasWiFi = updateRoomDto.HasWiFi.Value;

            if (updateRoomDto.HasProjector.HasValue)
                room.HasProjector = updateRoomDto.HasProjector.Value;

            if (updateRoomDto.HasVideoConference.HasValue)
                room.HasVideoConference = updateRoomDto.HasVideoConference.Value;

            if (updateRoomDto.HasAirConditioning.HasValue)
                room.HasAirConditioning = updateRoomDto.HasAirConditioning.Value;

            if (updateRoomDto.RoomImage != null)
            {
                if (!ImageHelper.IsValidImageFile(updateRoomDto.RoomImage))
                    throw new ArgumentException("Invalid image file. Please upload a valid image (JPG, PNG, GIF, BMP) under 5MB");
                room.RoomImage = await ImageHelper.ConvertToByteArrayAsync(updateRoomDto.RoomImage);
            }

            await _roomRepository.UpdateAsync(room);
            return await MapToResponseDto(room);
        }

        public async Task<bool> DeleteRoomAsync(int roomId)
        {
            await _roomRepository.DeleteAsync(roomId);
            return true;
        }

       
        private async Task<RoomResponseDto> MapToResponseDto(Room room)
        {
            // If Resource is not loaded, fetch it
            var resource = room.Resource ?? await _resourceRepository.GetResourceWithDetailsAsync(room.ResourceId);

            return new RoomResponseDto
            {
                Id = room.Id,
                RoomId = room.Id, // Map this to Id instead of 0
                ResourceId = room.ResourceId,
                RoomName = room.RoomName,
                Capacity = room.Capacity,
                HasTV = room.HasTV,
                HasWhiteboard = room.HasWhiteboard,
                HasWiFi = room.HasWiFi,
                HasProjector = room.HasProjector,
                HasVideoConference = room.HasVideoConference,
                HasAirConditioning = room.HasAirConditioning,
                PhoneExtension = room.PhoneExtension,
                RoomImage = room.RoomImage,

                // Resource Information
                LocationId = resource?.LocationId ?? 0,
                BuildingId = resource?.BuildingId ?? 0,
                FloorId = resource?.FloorId ?? 0,
                IsUnderMaintenance = resource?.IsUnderMaintenance ?? false,
     
                IsBlocked = resource?.IsBlocked ?? false,
                BlockedFrom = resource?.BlockedFrom,
                BlockedUntil = resource?.BlockedUntil,
                BlockReason = resource?.BlockReason,

                // Location Details
                LocationName = resource?.Location?.Name,
                LocationAddress = resource?.Location?.Address,
                City = resource?.Location?.City,
                BuildingName = resource?.Building?.Name,
                FloorName = resource?.Floor?.FloorName
            };
        }
    }
}








