using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConferenceRoomBooking.DataAccess.Models
{
    public class UserBookingStats
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int UserId { get; set; }
        
        public int TotalBookings { get; set; }
        public int CompletedBookings { get; set; }
        public int CancelledBookings { get; set; }
        public int NoShowBookings { get; set; }
        
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;
    }
}









