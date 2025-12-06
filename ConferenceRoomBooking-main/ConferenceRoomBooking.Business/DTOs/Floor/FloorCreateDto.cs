using System.ComponentModel.DataAnnotations;

namespace ConferenceRoomBooking.Business.DTOs.Floor
{
    public class FloorCreateDto
    {
        [Required]
        public int LocationId { get; set; }

        [Required]
        public int BuildingId { get; set; }

        [Required]
        public string FloorName { get; set; } = string.Empty;

        [Required]
        public int FloorNumber { get; set; }

        public byte[]? FloorPlanImage { get; set; }
    }
}








