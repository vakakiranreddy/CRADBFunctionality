using ConferenceRoomBooking.Business.DTOs.UserNotification;
using ConferenceRoomBooking.DataAccess.Enum;
using ConferenceRoomBooking.DataAccess.Interfaces.IRepositories;
using ConferenceRoomBooking.Business.Interfaces.IServices;
using ConferenceRoomBooking.DataAccess.Models;

namespace ConferenceRoomBooking.Business.Services
{
    public class UserNotificationService : IUserNotificationService
    {
        private readonly IUserNotificationRepository _notificationRepository;
        private readonly IUserRepository _userRepository;

        public UserNotificationService(
            IUserNotificationRepository notificationRepository,
            IUserRepository userRepository)
        {
            _notificationRepository = notificationRepository;
            _userRepository = userRepository;
        }

        public async Task SendNotificationAsync(SendNotificationDto dto)
        {
            var user = await _userRepository.GetByIdAsync(dto.UserId);
            if (user == null) throw new ArgumentException("User not found");

            var notification = new UserNotification
            {
                UserId = dto.UserId,
                Title = dto.Title,
                Message = dto.Message,
                Type = dto.Type,
                Status = EmailStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _notificationRepository.AddAsync(notification);
        }

        public async Task<IEnumerable<UserNotification>> GetUserNotificationsAsync(int userId)
        {
            return await _notificationRepository.GetByUserIdAsync(userId);
        }

        public async Task MarkAsReadAsync(int notificationId)
        {
            var notification = await _notificationRepository.GetByIdAsync(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
                await _notificationRepository.UpdateAsync(notification);
            }
        }

        public async Task<IEnumerable<UserNotification>> GetByDateRangeAsync(DateTime from, DateTime to)
        {
            return await _notificationRepository.GetByDateRangeAsync(from, to);
        }
    }
}








