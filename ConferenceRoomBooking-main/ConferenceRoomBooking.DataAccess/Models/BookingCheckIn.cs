
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConferenceRoomBooking.DataAccess.Models
{
    public class BookingCheckIn
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int BookingId { get; set; }

        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        
        public bool IsCheckedIn { get; set; } = false;
        public bool IsCheckedOut { get; set; } = false;

        public TimeSpan? ActualDuration { get; set; }

        [ForeignKey("BookingId")]
        public Booking Booking { get; set; }
    }

}









