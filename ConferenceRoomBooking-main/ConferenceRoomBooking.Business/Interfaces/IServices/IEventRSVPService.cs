using ConferenceRoomBooking.DataAccess.Models;

namespace ConferenceRoomBooking.Business.Interfaces.IServices
{
    public interface IEventRSVPService
    {
        Task<bool> AddUserRsvpAsync(int eventId, int userId, string status);

        Task<bool> UpdateUserRsvpAsync(int eventId, int userId, string status);

        Task<EventRSVP?> GetUserRsvpAsync(int eventId, int userId);

        Task<IEnumerable<EventRSVP>> GetRsvpsByEventAsync(int eventId);

        Task<IEnumerable<EventRSVP>> GetRsvpsByUserAsync(int userId);

        Task<int> GetInterestedCountAsync(int eventId);

        Task<int> GetNotInterestedCountAsync(int eventId);

        Task<int> GetMaybeCountAsync(int eventId);

    }

}









