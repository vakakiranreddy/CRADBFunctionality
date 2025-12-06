using ConferenceRoomBooking.DataAccess.Enum;
using ConferenceRoomBooking.DataAccess.Models;

namespace ConferenceRoomBooking.DataAccess.Interfaces.IRepositories
{
    public interface IEventRSVPRepository : IBaseRepository<EventRSVP>
    {
        Task<EventRSVP?> GetUserRsvpAsync(int eventId, int userId);

        Task<IEnumerable<EventRSVP>> GetRsvpsByEventAsync(int eventId);

        Task<IEnumerable<EventRSVP>> GetRsvpsByUserAsync(int userId);

        Task<int> GetCountByStatusAsync(int eventId, RsvpStatusType status);
    }

}









