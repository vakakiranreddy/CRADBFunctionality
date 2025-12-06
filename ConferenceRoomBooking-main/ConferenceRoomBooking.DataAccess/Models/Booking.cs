using ConferenceRoomBooking.DataAccess.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConferenceRoomBooking.DataAccess.Models
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int ResourceId { get; set; }

        [Required]
        public ResourceType ResourceType { get; set; }

        [Required]
        [MaxLength(200)]
        public string MeetingName { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        public int? ParticipantCount { get; set; }

        [Required]
        public SessionStatus SessionStatus { get; set; } = SessionStatus.Reserved;

        [Required]
        public bool CheckInReminder { get; set; } = false;

        [Required]
        public bool CheckOutReminder { get; set; } = false;

        [Required]
        public bool EntryReminderSent { get; set; } = false;

        [Required]
        public bool ExitReminderSent { get; set; } = false;

        [Required]
        public bool OverdueRemainderSent { get; set; } = false;

        [MaxLength(500)]
        public string? CancellationReason { get; set; }

        public DateTime? CancelledAt { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [ForeignKey("ResourceId")]
        public Resource Resource { get; set; }

        public BookingCheckIn? CheckIn { get; set; }

        public ICollection<UserNotification> Notifications { get; set; }
    }
}








