using ConferenceRoomBooking.DataAccess.Enum;
using System.ComponentModel.DataAnnotations;

namespace ConferenceRoomBooking.Business.DTOs.Booking
{
    public class CreateBookingDto
    {
        [Required]
        public int ResourceId { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Required]
        public string MeetingName { get; set; }

        [Required]
        public int ParticipantCount { get; set; }

        [MaxLength(500)]
        public string? Purpose { get; set; }

        public bool SendReminder { get; set; } = true;
    }

}









