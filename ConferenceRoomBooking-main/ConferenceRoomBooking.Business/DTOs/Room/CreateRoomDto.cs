using Microsoft.AspNetCore.Http;

using System.ComponentModel.DataAnnotations;

namespace ConferenceRoomBooking.Business.DTOs.Room
{
    public class CreateRoomDto
    {
        [Required]
        public int ResourceId { get; set; }

        [Required]
        [StringLength(100)]
        public string RoomName { get; set; }

        [Required]
        [Range(1, 1000)]
        public int Capacity { get; set; }

        public bool HasTV { get; set; } = false;
        public bool HasWhiteboard { get; set; } = false;
        public bool HasWiFi { get; set; } = false;
        public bool HasProjector { get; set; } = false;
        public bool HasVideoConference { get; set; } = false;
        public bool HasAirConditioning { get; set; } = false;

        [StringLength(20)]
        public string? PhoneExtension { get; set; }

        public IFormFile? RoomImage { get; set; }
    }
}









