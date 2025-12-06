
using ConferenceRoomBooking.DataAccess.Enum;
using ConferenceRoomBooking.DataAccess.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;

namespace ConferenceRoomBooking.DataAccess.Models
{
    public class Resource
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public ResourceType ResourceType { get; set; }

        [Required]
        public int LocationId { get; set; }

        [Required]
        public int BuildingId { get; set; }

        [Required]
        public int FloorId { get; set; }

        public bool IsUnderMaintenance { get; set; } = false;
        public bool IsBlocked { get; set; } = false;
        public bool IsActive { get; set; } = true;

        public DateTime? BlockedFrom { get; set; }
        public DateTime? BlockedUntil { get; set; }

        [MaxLength(500)]
        public string? BlockReason { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey("LocationId")]
        public Location Location { get; set; }

        [ForeignKey("BuildingId")]
        public Building Building { get; set; }

        [ForeignKey("FloorId")]
        public Floor Floor { get; set; }

        public Room Room { get; set; }
        public Desk Desk { get; set; }
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}









