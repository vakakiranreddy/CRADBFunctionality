using ConferenceRoomBooking.Business.DTOs.Desk;
using ConferenceRoomBooking.DataAccess.Interfaces.IRepositories;
using ConferenceRoomBooking.Business.Interfaces.IServices;
using ConferenceRoomBooking.DataAccess.Models;

namespace ConferenceRoomBooking.Business.Services
{
    public class DeskService : IDeskService
    {
        private readonly IDeskRepository _deskRepository;
        private readonly IResourceRepository _resourceRepository;

        public DeskService(IDeskRepository deskRepository, IResourceRepository resourceRepository)
        {
            _deskRepository = deskRepository;
            _resourceRepository = resourceRepository;
        }

        public async Task<DeskResponseDto> CreateDeskAsync(CreateDeskDto createDeskDto)
        {
            var desk = new Desk
            {
                ResourceId = createDeskDto.ResourceId,
                DeskName = createDeskDto.DeskName,
                HasMonitor = createDeskDto.HasMonitor,
                HasKeyboard = createDeskDto.HasKeyboard,
                HasMouse = createDeskDto.HasMouse,
                HasDockingStation = createDeskDto.HasDockingStation
            };

            var createdDesk = await _deskRepository.AddAsync(desk);
            return await MapToResponseDto(createdDesk);
        }

        public async Task<DeskResponseDto?> GetDeskByIdAsync(int deskId)
        {
            var desk = await _deskRepository.GetByIdAsync(deskId);
            return desk == null ? null : await MapToResponseDto(desk);
        }

        public async Task<DeskResponseDto?> GetDeskByResourceIdAsync(int resourceId)
        {
            var desk = await _deskRepository.GetDeskByResourceIdAsync(resourceId);
            return desk == null ? null : await MapToResponseDto(desk);
        }

        public async Task<IEnumerable<DeskResponseDto>> GetAllDesksAsync()
        {
            var desks = await _deskRepository.GetAllAsync();
            var result = new List<DeskResponseDto>();
            foreach (var desk in desks)
            {
                result.Add(await MapToResponseDto(desk));
            }
            return result;
        }

        public async Task<IEnumerable<DeskResponseDto>> GetDesksByLocationAsync(int locationId)
        {
            var desks = await _deskRepository.GetDesksByLocationAsync(locationId);
            var result = new List<DeskResponseDto>();
            foreach (var desk in desks)
            {
                result.Add(await MapToResponseDto(desk));
            }
            return result;
        }

        public async Task<IEnumerable<DeskResponseDto>> GetDesksByBuildingAsync(int buildingId)
        {
            var desks = await _deskRepository.GetDesksByBuildingAsync(buildingId);
            var result = new List<DeskResponseDto>();
            foreach (var desk in desks)
            {
                result.Add(await MapToResponseDto(desk));
            }
            return result;
        }

        public async Task<IEnumerable<DeskResponseDto>> GetDesksByFloorAsync(int floorId)
        {
            var desks = await _deskRepository.GetDesksByFloorAsync(floorId);
            var result = new List<DeskResponseDto>();
            foreach (var desk in desks)
            {
                result.Add(await MapToResponseDto(desk));
            }
            return result;
        }

        public async Task<IEnumerable<DeskResponseDto>> GetAvailableDesksAsync(DeskAvailabilityRequestDto requestDto)
        {
            var desks = await _deskRepository.GetAvailableDesksAsync(
                requestDto.LocationId, requestDto.Date, requestDto.StartTime, requestDto.EndTime);
            var result = new List<DeskResponseDto>();
            foreach (var desk in desks)
            {
                result.Add(await MapToResponseDto(desk));
            }
            return result;
        }

        public async Task<DeskResponseDto> UpdateDeskAsync(int deskId, UpdateDeskDto updateDeskDto)
        {
            var desk = await _deskRepository.GetByIdAsync(deskId);
            if (desk == null) throw new ArgumentException("Desk not found");

            if (updateDeskDto.DeskName != null)
                desk.DeskName = updateDeskDto.DeskName;

            if (updateDeskDto.HasMonitor.HasValue)
                desk.HasMonitor = updateDeskDto.HasMonitor.Value;

            if (updateDeskDto.HasKeyboard.HasValue)
                desk.HasKeyboard = updateDeskDto.HasKeyboard.Value;

            if (updateDeskDto.HasMouse.HasValue)
                desk.HasMouse = updateDeskDto.HasMouse.Value;

            if (updateDeskDto.HasDockingStation.HasValue)
                desk.HasDockingStation = updateDeskDto.HasDockingStation.Value;

            await _deskRepository.UpdateAsync(desk);
            return await MapToResponseDto(desk);
        }

        public async Task<bool> DeleteDeskAsync(int deskId)
        {
            await _deskRepository.DeleteAsync(deskId);
            return true;
        }

        private async Task<DeskResponseDto> MapToResponseDto(Desk desk)
        {
            var resource = await _resourceRepository.GetResourceWithDetailsAsync(desk.ResourceId);

            return new DeskResponseDto
            {
                Id = desk.Id,
                ResourceId = desk.ResourceId,
                DeskName = desk.DeskName,
                HasMonitor = desk.HasMonitor,
                HasKeyboard = desk.HasKeyboard,
                HasMouse = desk.HasMouse,
                HasDockingStation = desk.HasDockingStation,
                LocationName = resource?.Location?.Name,
                BuildingName = resource?.Building?.Name,
                FloorName = resource?.Floor?.FloorName,
                IsUnderMaintenance = resource?.IsUnderMaintenance ?? false,
                IsBlocked = resource?.IsBlocked ?? false,
                LocationId = resource?.LocationId ?? 0,
                BuildingId = resource?.BuildingId ?? 0,
                FloorId = resource?.FloorId ?? 0
            };
        }
    }
}








