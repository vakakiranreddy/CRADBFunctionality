using ConferenceRoomBooking.DataAccess.Enum;
using System.ComponentModel.DataAnnotations;

namespace ConferenceRoomBooking.Business.DTOs.EventRSVP
{
    public class CreateRsvpDto
    {
        [Required]
        public int EventId { get; set; }

        [Required]
        public RsvpStatusType Status { get; set; }
    }
}








