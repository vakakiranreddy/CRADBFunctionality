using System.ComponentModel.DataAnnotations;

namespace ConferenceRoomBooking.Business.DTOs.Desk
{
    public class DeskAvailabilityRequestDto
    {
        [Required]
        public int LocationId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        public int? BuildingId { get; set; }
        public int? FloorId { get; set; }
    }
}









