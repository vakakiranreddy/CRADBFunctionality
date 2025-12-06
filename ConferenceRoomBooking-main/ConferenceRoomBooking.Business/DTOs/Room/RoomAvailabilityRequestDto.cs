using System.ComponentModel.DataAnnotations;

namespace ConferenceRoomBooking.Business.DTOs.Room
{
    public class RoomAvailabilityRequestDto
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
        public int? MinCapacity { get; set; }
    }
}









