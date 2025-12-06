using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConferenceRoomBooking.DataAccess.Models
{
    public class Floor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int LocationId { get; set; }

        [Required]
        public int BuildingId { get; set; }

        [Required]
        [StringLength(100)]
        public string FloorName { get; set; }

        [Required]
        public int FloorNumber { get; set; }

        public int? NumberOfRooms { get; set; }

        public int? NumberOfDesks { get; set; }

        public byte[]? FloorPlanImage { get; set; }

        public bool IsActive { get; set; } = true;  // ADD THIS

        // Navigation Properties
        [ForeignKey("LocationId")]
        public Location Location { get; set; }

        [ForeignKey("BuildingId")]
        public Building Building { get; set; }

        public ICollection<Resource> Resources { get; set; } = new List<Resource>();
    }
}








