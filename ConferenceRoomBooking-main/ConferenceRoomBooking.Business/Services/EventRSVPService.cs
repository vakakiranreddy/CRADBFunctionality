using ConferenceRoomBooking.DataAccess.Enum;
using ConferenceRoomBooking.DataAccess.Interfaces.IRepositories;
using ConferenceRoomBooking.Business.Interfaces.IServices;
using ConferenceRoomBooking.DataAccess.Models;

namespace ConferenceRoomBooking.Business.Services
{
    public class EventRSVPService : IEventRSVPService
    {
        private readonly IEventRSVPRepository _eventRSVPRepository;

        public EventRSVPService(IEventRSVPRepository eventRSVPRepository)
        {
            _eventRSVPRepository = eventRSVPRepository;
        }

        public async Task<bool> AddUserRsvpAsync(int eventId, int userId, string status)
        {
            if (!System.Enum.TryParse<RsvpStatusType>(status, true, out var rsvpStatus))
                return false;

            var existingRsvp = await _eventRSVPRepository.GetUserRsvpAsync(eventId, userId);
            if (existingRsvp != null)
                return await UpdateUserRsvpAsync(eventId, userId, status);

            var rsvp = new EventRSVP
            {
                EventId = eventId,
                UserId = userId,
                Status = rsvpStatus
            };

            await _eventRSVPRepository.AddAsync(rsvp);
            return true;
        }

        public async Task<bool> UpdateUserRsvpAsync(int eventId, int userId, string status)
        {
            if (!System.Enum.TryParse<RsvpStatusType>(status, true, out var rsvpStatus))
                return false;

            var rsvp = await _eventRSVPRepository.GetUserRsvpAsync(eventId, userId);
            if (rsvp == null) return false;

            rsvp.Status = rsvpStatus;
            rsvp.ResponseDate = DateTime.UtcNow;

            await _eventRSVPRepository.UpdateAsync(rsvp);
            return true;
        }

        public async Task<EventRSVP?> GetUserRsvpAsync(int eventId, int userId)
        {
            return await _eventRSVPRepository.GetUserRsvpAsync(eventId, userId);
        }

        public async Task<IEnumerable<EventRSVP>> GetRsvpsByEventAsync(int eventId)
        {
            return await _eventRSVPRepository.GetRsvpsByEventAsync(eventId);
        }

        public async Task<IEnumerable<EventRSVP>> GetRsvpsByUserAsync(int userId)
        {
            return await _eventRSVPRepository.GetRsvpsByUserAsync(userId);
        }

        public async Task<int> GetInterestedCountAsync(int eventId)
        {
            return await _eventRSVPRepository.GetCountByStatusAsync(eventId, RsvpStatusType.Yes);
        }

        public async Task<int> GetNotInterestedCountAsync(int eventId)
        {
            return await _eventRSVPRepository.GetCountByStatusAsync(eventId, RsvpStatusType.No);
        }

        public async Task<int> GetMaybeCountAsync(int eventId)
        {
            return await _eventRSVPRepository.GetCountByStatusAsync(eventId, RsvpStatusType.Maybe);
        }
    }
}








