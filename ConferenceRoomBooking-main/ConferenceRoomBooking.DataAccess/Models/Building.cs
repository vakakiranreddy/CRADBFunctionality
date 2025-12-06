using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;

namespace ConferenceRoomBooking.DataAccess.Models
{
    public class Building
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(200)]
        public string Address { get; set; }

        [Required]
        public int LocationId { get; set; }

        public int? NumberOfFloors { get; set; }  // ADD THIS

        public byte[]? BuildingImage { get; set; }  // ADD THIS

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey("LocationId")]
        public Location Location { get; set; }

        public ICollection<Floor> Floors { get; set; } = new List<Floor>();

        public ICollection<Resource> Resources { get; set; } = new List<Resource>();
    }
}








