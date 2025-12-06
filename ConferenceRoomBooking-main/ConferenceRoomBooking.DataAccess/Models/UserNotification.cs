using ConferenceRoomBooking.DataAccess.Enum;
using ConferenceRoomBooking.DataAccess.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConferenceRoomBooking.DataAccess.Models
{
    public class UserNotification
    {

        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        public int? BookingId { get; set; }

        [Required]
        public NotificationType Type { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        [StringLength(2000)]
        public string Message { get; set; }

        public bool IsRead { get; set; } = false;
        public DateTime? ReadAt { get; set; }

        [Required]
        public EmailStatus Status { get; set; } = EmailStatus.Pending;

        public DateTime? SentAt { get; set; }

        public int? LocationId { get; set; }

        public int? DepartmentId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey("UserId")]
        public User User { get; set; }

        [ForeignKey("BookingId")]
        public Booking Booking { get; set; }

        [ForeignKey("LocationId")]
        public Location Location { get; set; }

        [ForeignKey("DepartmentId")]
        public Department Department { get; set; }
    }
}









