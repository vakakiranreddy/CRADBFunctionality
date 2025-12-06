using ConferenceRoomBooking.DataAccess.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConferenceRoomBooking.DataAccess.Models
{
    public class BroadcastNotification
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        [StringLength(2000)]
        public string Message { get; set; }

        [Required]
        public BroadcastNotificationType Type { get; set; }

        [Required]
        public EmailStatus Status { get; set; } = EmailStatus.Pending;

        public DateTime? SentAt { get; set; }

        public int? TargetLocationId { get; set; }
        public int? TargetDepartmentId { get; set; }
        public UserRole? TargetRole { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey("TargetLocationId")]
        public Location? Location { get; set; }

        [ForeignKey("TargetDepartmentId")]
        public Department? Department { get; set; }
    }
}









