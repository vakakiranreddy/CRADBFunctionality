using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConferenceRoomBooking.DataAccess.Models
{
    public class Room
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ResourceId { get; set; }

        [Required]
        [StringLength(100)]
        public string RoomName { get; set; }

        public int Capacity { get; set; }

        public bool HasTV { get; set; } = false;

        public bool HasWhiteboard { get; set; } = false;

        public bool HasWiFi { get; set; } = false;

        public bool HasProjector { get; set; } = false;

        public bool HasVideoConference { get; set; } = false;

        public bool HasAirConditioning { get; set; } = false;

        [StringLength(20)]
        public string? PhoneExtension { get; set; }

        public byte[]? RoomImage { get; set; }

        // Navigation Properties
        [ForeignKey("ResourceId")]
        public Resource Resource { get; set; }
    }
}









