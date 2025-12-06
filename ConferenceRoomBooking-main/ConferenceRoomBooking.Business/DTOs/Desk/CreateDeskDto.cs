using System.ComponentModel.DataAnnotations;

namespace ConferenceRoomBooking.Business.DTOs.Desk
{
    public class CreateDeskDto
    {
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
    }
}









