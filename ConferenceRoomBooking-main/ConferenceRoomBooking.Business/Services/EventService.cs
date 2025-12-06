using ConferenceRoomBooking.Business.DTOs.Event;
using ConferenceRoomBooking.Business.Helpers;
using ConferenceRoomBooking.DataAccess.Interfaces.IRepositories;
using ConferenceRoomBooking.Business.Interfaces.IServices;
using ConferenceRoomBooking.DataAccess.Models;
using Microsoft.Extensions.Logging;

namespace ConferenceRoomBooking.Business.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly ILogger<EventService> _logger;

        public EventService(IEventRepository eventRepository, ILogger<EventService> logger)
        {
            _eventRepository = eventRepository;
            _logger = logger;
        }

        public async Task<EventResponseDto> CreateEventAsync(CreateEventDto dto)
        {
            if (dto.StartTime >= dto.EndTime)
                throw new ArgumentException("Start time must be before end time");

            byte[]? eventImageBytes = null;
            if (dto.EventImage != null)
            {
                if (!ImageHelper.IsValidImageFile(dto.EventImage))
                    throw new ArgumentException("Invalid image file. Please upload JPG, PNG, or GIF under 2MB");
                
                eventImageBytes = await ImageHelper.ConvertToByteArrayAsync(dto.EventImage);
            }

            var eventEntity = new Event
            {
                EventName = dto.EventTitle,
                Description = dto.Description,
                LocationId = dto.LocationId,
                BuildingId = dto.BuildingId,
                FloorId = dto.FloorId,
                Date = dto.Date,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                EventImage = eventImageBytes,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createdEvent = await _eventRepository.AddAsync(eventEntity);
            return MapToResponseDto(createdEvent);
        }

        public async Task<EventResponseDto> UpdateEventAsync(int eventId, UpdateEventDto dto)
        {
            var eventEntity = await _eventRepository.GetByIdAsync(eventId);
            if (eventEntity == null) throw new ArgumentException("Event not found");

            eventEntity.EventName = dto.EventTitle;
            eventEntity.Description = dto.Description;
            eventEntity.LocationId = dto.LocationId;
            eventEntity.BuildingId = dto.BuildingId;
            eventEntity.FloorId = dto.FloorId;
            eventEntity.Date = dto.Date;
            eventEntity.StartTime = dto.StartTime;
            eventEntity.EndTime = dto.EndTime;
            
            if (dto.EventImage != null)
            {
                if (!ImageHelper.IsValidImageFile(dto.EventImage))
                    throw new ArgumentException("Invalid image file");
                eventEntity.EventImage = await ImageHelper.ConvertToByteArrayAsync(dto.EventImage);
            }
            
            eventEntity.UpdatedAt = DateTime.UtcNow;

            await _eventRepository.UpdateAsync(eventEntity);
            return MapToResponseDto(eventEntity);
        }

        public async Task<bool> DeleteEventAsync(int eventId)
        {
            var eventEntity = await _eventRepository.GetByIdAsync(eventId);
            if (eventEntity == null) return false;

            await _eventRepository.DeleteAsync(eventId);
            return true;
        }

        public async Task<IEnumerable<EventResponseDto>> GetAllEventsAsync()
        {
            var events = await _eventRepository.GetAllAsync();
            return events.Select(MapToResponseDto);
        }

        public async Task<EventResponseDto?> GetEventByIdAsync(int eventId)
        {
            var eventEntity = await _eventRepository.GetByIdAsync(eventId);
            return eventEntity == null ? null : MapToResponseDto(eventEntity);
        }

        public async Task<IEnumerable<EventResponseDto>> SearchEventsAsync(string keyword)
        {
            var events = await _eventRepository.SearchEventsAsync(keyword);
            return events.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<EventResponseDto>> FilterEventsByLocationNameAsync(string locationName)
        {
            var events = await _eventRepository.FilterEventsByLocationNameAsync(locationName);
            return events.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<EventResponseDto>> GetUpcomingEventsAsync()
        {
            var events = await _eventRepository.GetUpcomingEventsAsync();
            return events.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<EventResponseDto>> GetPastEventsAsync()
        {
            var events = await _eventRepository.GetPastEventsAsync();
            return events.Select(MapToResponseDto);
        }

        public async Task<int> GetTotalEventCountAsync()
        {
            return await _eventRepository.GetTotalEventCountAsync();
        }

        public async Task<int> GetEventParticipantCountAsync(int eventId)
        {
            return await _eventRepository.GetEventParticipantCountAsync(eventId);
        }

        private static EventResponseDto MapToResponseDto(Event eventEntity)
        {
            return new EventResponseDto
            {
                EventId = eventEntity.EventId,
                EventTitle = eventEntity.EventName,
                Description = eventEntity.Description,
                LocationId = eventEntity.LocationId,
                LocationName = eventEntity.Location?.Name,
                BuildingId = eventEntity.BuildingId,
                FloorId = eventEntity.FloorId,
                Date = eventEntity.Date,
                StartTime = eventEntity.StartTime,
                EndTime = eventEntity.EndTime,
                IsActive = eventEntity.IsActive,
                EventImage = ImageHelper.ConvertToBase64String(eventEntity.EventImage),
                CreatedAt = eventEntity.CreatedAt,
                UpdatedAt = eventEntity.UpdatedAt
            };
        }
    }
}








