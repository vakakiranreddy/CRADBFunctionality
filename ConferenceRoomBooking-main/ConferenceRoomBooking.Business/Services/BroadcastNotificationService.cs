using ConferenceRoomBooking.Business.DTOs.BroadCastNotification;
using ConferenceRoomBooking.DataAccess.Enum;
using ConferenceRoomBooking.DataAccess.Interfaces.IRepositories;
using ConferenceRoomBooking.Business.Interfaces.IServices;
using ConferenceRoomBooking.DataAccess.Models;
using Microsoft.Extensions.Logging;

namespace ConferenceRoomBooking.Business.Services
{
    public class BroadcastNotificationService : IBroadcastNotificationService
    {
        private readonly IBroadcastNotificationRepository _broadcastRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserNotificationService _notificationService;
        private readonly ILogger<BroadcastNotificationService> _logger;

        public BroadcastNotificationService(
            IBroadcastNotificationRepository broadcastRepository,
            IUserRepository userRepository,
            IUserNotificationService notificationService,
            ILogger<BroadcastNotificationService> logger)
        {
            _broadcastRepository = broadcastRepository;
            _userRepository = userRepository;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task<BroadcastNotification> CreateBroadcastAsync(SendBroadcastDto dto)
        {
            var broadcast = new BroadcastNotification
            {
                Title = dto.Title,
                Message = dto.Message,
                Type = dto.Type,
                TargetLocationId = dto.TargetLocationId,
                TargetDepartmentId = dto.TargetDepartmentId,
                TargetRole = dto.TargetRole,
                Status = EmailStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            return await _broadcastRepository.AddAsync(broadcast);
        }

        public async Task<BroadcastNotification> UpdateBroadcastAsync(int id, SendBroadcastDto dto)
        {
            var broadcast = await _broadcastRepository.GetByIdAsync(id);
            if (broadcast == null) throw new ArgumentException("Broadcast notification not found");

            broadcast.Title = dto.Title;
            broadcast.Message = dto.Message;
            broadcast.Type = dto.Type;
            broadcast.TargetLocationId = dto.TargetLocationId;
            broadcast.TargetDepartmentId = dto.TargetDepartmentId;
            broadcast.TargetRole = dto.TargetRole;

            await _broadcastRepository.UpdateAsync(broadcast);
            return broadcast;
        }

        public async Task<bool> DeleteBroadcastAsync(int id)
        {
            var broadcast = await _broadcastRepository.GetByIdAsync(id);
            if (broadcast == null) return false;

            await _broadcastRepository.DeleteAsync(id);
            return true;
        }

        public async Task<BroadcastNotification> GetBroadcastByIdAsync(int id)
        {
            return await _broadcastRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<BroadcastNotification>> GetAllBroadcastsAsync()
        {
            return await _broadcastRepository.GetAllAsync();
        }

        public async Task<IEnumerable<BroadcastNotification>> GetByDateRangeAsync(DateTime from, DateTime to)
        {
            return await _broadcastRepository.GetByDateRangeAsync(from, to);
        }

        public async Task<IEnumerable<BroadcastNotification>> GetByDepartmentIdAsync(int departmentId)
        {
            return await _broadcastRepository.GetByDepartmentIdAsync(departmentId);
        }

        public async Task<IEnumerable<BroadcastNotification>> GetByLocationIdAsync(int locationId)
        {
            return await _broadcastRepository.GetByLocationIdAsync(locationId);
        }

        public async Task<IEnumerable<BroadcastNotification>> GetByRoleAsync(UserRole role)
        {
            var allBroadcasts = await _broadcastRepository.GetAllAsync();
            return allBroadcasts.Where(b => b.TargetRole == role);
        }

        public async Task ProcessPendingBroadcastsAsync()
        {
            try
            {
                var pendingBroadcasts = await _broadcastRepository.GetPendingBroadcastsAsync();
                _logger.LogInformation("Processing {Count} pending broadcast notifications", pendingBroadcasts.Count());

                foreach (var broadcast in pendingBroadcasts)
                {
                    try
                    {
                        await SendBroadcastToTargetUsersAsync(broadcast);
                        
                        broadcast.Status = EmailStatus.Sent;
                        broadcast.SentAt = DateTime.UtcNow;
                        await _broadcastRepository.UpdateAsync(broadcast);
                        _logger.LogInformation("Successfully sent broadcast notification {BroadcastId}", broadcast.Id);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send broadcast notification {BroadcastId}", broadcast.Id);
                        broadcast.Status = EmailStatus.Failed;
                        await _broadcastRepository.UpdateAsync(broadcast);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing pending broadcast notifications");
                throw new InvalidOperationException("Failed to process pending broadcast notifications", ex);
            }
        }

        private async Task SendBroadcastToTargetUsersAsync(BroadcastNotification broadcast)
        {
            IEnumerable<User> targetUsers;

            if (broadcast.TargetLocationId.HasValue)
            {
                targetUsers = await _userRepository.GetByLocationAsync(broadcast.TargetLocationId.Value);
            }
            else if (broadcast.TargetDepartmentId.HasValue)
            {
                targetUsers = await _userRepository.GetByDepartmentAsync(broadcast.TargetDepartmentId.Value);
            }
            else if (broadcast.TargetRole.HasValue)
            {
                targetUsers = await _userRepository.GetByRoleAsync(broadcast.TargetRole.Value);
            }
            else
            {
                targetUsers = await _userRepository.GetAllAsync();
            }

            var activeUsers = targetUsers.Where(u => u.IsActive);

            foreach (var user in activeUsers)
            {
                await _notificationService.SendNotificationAsync(new DTOs.UserNotification.SendNotificationDto
                {
                    UserId = user.Id,
                    Title = broadcast.Title,
                    Message = broadcast.Message,
                    Type = MapBroadcastTypeToNotificationType(broadcast.Type)
                });
            }
        }

        private static NotificationType MapBroadcastTypeToNotificationType(BroadcastNotificationType broadcastType)
        {
            return broadcastType switch
            {
                BroadcastNotificationType.EventAnnouncement => NotificationType.Welcome,
                BroadcastNotificationType.MaintenanceAlert => NotificationType.MaintenanceAlert,
                BroadcastNotificationType.SystemUpdate => NotificationType.MaintenanceAlert,
                BroadcastNotificationType.PolicyUpdate => NotificationType.MaintenanceAlert,
                _ => NotificationType.MaintenanceAlert
            };
        }
    }
}








