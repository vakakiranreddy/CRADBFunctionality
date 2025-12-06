using ConferenceRoomBooking.DataAccess.Enum;

namespace ConferenceRoomBooking.Business.DTOs.EventRSVP
{
    public class RsvpResponseDto
    {
        public int RSVPId { get; set; }
        public int EventId { get; set; }
        public int UserId { get; set; }
        public RsvpStatusType Status { get; set; }
        public DateTime ResponseDate { get; set; }
    }
}








