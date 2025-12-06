using ConferenceRoomBooking.DataAccess.Enum;
using ConferenceRoomBooking.DataAccess.Models;

namespace ConferenceRoomBooking.DataAccess.Interfaces.IRepositories
{
    public interface IBroadcastNotificationRepository : IBaseRepository<BroadcastNotification>
    {
        Task<IEnumerable<BroadcastNotification>> GetPendingBroadcastsAsync();
        Task<IEnumerable<BroadcastNotification>> GetByStatusAsync(EmailStatus status);
        Task<IEnumerable<BroadcastNotification>> GetByDateRangeAsync(DateTime from, DateTime to);
        Task<IEnumerable<BroadcastNotification>> GetByDepartmentIdAsync(int departmentId);
        Task<IEnumerable<BroadcastNotification>> GetByLocationIdAsync(int locationId);

    }
}









