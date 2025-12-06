using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConferenceRoomBooking.DataAccess.Models
{
    public class Desk
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ResourceId { get; set; }

        [Required]
        [StringLength(100)]
        public string DeskName { get; set; }

        public bool HasMonitor { get; set; } = false;
        public bool HasKeyboard { get; set; } = false;
        public bool HasMouse { get; set; } = false;
        public bool HasDockingStation { get; set; } = false;

        public byte[]? DeskImage { get; set; }

        // Navigation Properties
        [ForeignKey("ResourceId")]
        public Resource Resource { get; set; }
    }
}









