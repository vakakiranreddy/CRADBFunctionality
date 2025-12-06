using ConferenceRoomBooking.DataAccess.Enum;
using ConferenceRoomBooking.DataAccess.Models;

namespace ConferenceRoomBooking.DataAccess.Interfaces.IRepositories
{
    public interface IUserNotificationRepository : IBaseRepository<UserNotification>
    {
        Task<IEnumerable<UserNotification>> GetByUserIdAsync(int userId);
        Task<IEnumerable<UserNotification>> GetByStatusAsync(EmailStatus status);
        Task<IEnumerable<UserNotification>> GetByDateRangeAsync(DateTime from, DateTime to);
        Task<IEnumerable<UserNotification>> GetPendingNotificationsAsync();
    }
}









