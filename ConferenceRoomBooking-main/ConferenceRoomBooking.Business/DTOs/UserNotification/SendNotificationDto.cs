using ConferenceRoomBooking.DataAccess.Enum;
using System.ComponentModel.DataAnnotations;

namespace ConferenceRoomBooking.Business.DTOs.UserNotification
{
    public class SendNotificationDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Message { get; set; } = string.Empty;

        [Required]
        public NotificationType Type { get; set; }

        public int? BookingId { get; set; }
    }
}








