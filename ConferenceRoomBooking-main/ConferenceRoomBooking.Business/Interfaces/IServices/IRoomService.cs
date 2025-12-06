using ConferenceRoomBooking.Business.DTOs.Room;

namespace ConferenceRoomBooking.Business.Interfaces.IServices
{
    public interface IRoomService
    {
        Task<RoomResponseDto> CreateRoomAsync(CreateRoomDto createRoomDto);
        Task<RoomResponseDto?> GetRoomByIdAsync(int roomId);
        Task<RoomResponseDto?> GetRoomByResourceIdAsync(int resourceId);
        Task<IEnumerable<RoomResponseDto>> GetAllRoomsAsync();
        Task<IEnumerable<RoomResponseDto>> GetRoomsByLocationAsync(int locationId);
        Task<IEnumerable<RoomResponseDto>> GetRoomsByBuildingAsync(int buildingId);
        Task<IEnumerable<RoomResponseDto>> GetRoomsByFloorAsync(int floorId);
        Task<IEnumerable<RoomResponseDto>> GetRoomsByCapacityAsync(int minCapacity);
        Task<IEnumerable<RoomResponseDto>> SearchRoomsWithAmenitiesAsync(RoomAmenityFilterDto filterDto);
        Task<IEnumerable<RoomResponseDto>> GetAvailableRoomsAsync(RoomAvailabilityRequestDto requestDto);
        Task<RoomResponseDto> UpdateRoomAsync(int roomId, UpdateRoomDto updateRoomDto);
        Task<bool> DeleteRoomAsync(int roomId);
    }

}









