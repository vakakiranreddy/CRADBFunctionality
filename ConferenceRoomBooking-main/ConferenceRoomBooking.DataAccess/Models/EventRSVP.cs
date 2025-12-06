using ConferenceRoomBooking.DataAccess.Enum;
using ConferenceRoomBooking.DataAccess.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConferenceRoomBooking.DataAccess.Models
{
    public class EventRSVP
    {
        [Key]
        public int RSVPId { get; set; }

        [Required]
        public int EventId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public RsvpStatusType Status { get; set; }

        public DateTime ResponseDate { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("EventId")]
        public Event? Event { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }
    }
}









