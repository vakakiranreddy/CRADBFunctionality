using System.ComponentModel.DataAnnotations;

namespace ConferenceRoomBooking.Business.DTOs.Resource
{
    public class ResourceAvailabilityCheckDto
    {
        [Required]
        public int ResourceId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        public int? ExcludeBookingId { get; set; }
    }
}









