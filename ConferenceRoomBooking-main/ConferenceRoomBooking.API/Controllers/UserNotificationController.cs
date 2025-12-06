using ConferenceRoomBooking.Business.DTOs.UserNotification;
using ConferenceRoomBooking.Business.Interfaces.IServices;
using ConferenceRoomBooking.DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceRoomBooking.API.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserNotificationController : ControllerBase
    {
        private readonly IUserNotificationService _notificationService;

        public UserNotificationController(IUserNotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SendNotification([FromBody] SendNotificationDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                await _notificationService.SendNotificationAsync(dto);
                return Ok(new { message = "Notification sent successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while sending the notification", error = ex.Message });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserNotifications(int userId)
        {
            try
            {
                if (userId <= 0)
                    return BadRequest(new { message = "Invalid user ID" });

                var notifications = await _notificationService.GetUserNotificationsAsync(userId);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving user notifications", error = ex.Message });
            }
        }

        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { message = "Invalid notification ID" });

                await _notificationService.MarkAsReadAsync(id);
                return Ok(new { message = "Notification marked as read" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while marking notification as read", error = ex.Message });
            }
        }

        [HttpGet("date-range")]
        public async Task<IActionResult> GetByDateRange([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            try
            {
                var notifications = await _notificationService.GetByDateRangeAsync(from, to);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving notifications by date range", error = ex.Message });
            }
        }
    }
}

