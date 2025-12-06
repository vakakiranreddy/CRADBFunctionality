using ConferenceRoomBooking.DataAccess.Enum;

namespace ConferenceRoomBooking.Business.DTOs.Booking
{
    public class BookingResponseDto
    {
        public int BookingId { get; set; }

        public int Id { get; set; }

        public int UserId { get; set; }

        public string? UserName { get; set; }

        public int ResourceId { get; set; }

        public string? ResourceName { get; set; }

        public ResourceType ResourceType { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public SessionStatus Status { get; set; }

        public SessionStatus SessionStatus { get; set; }

        public string? MeetingName { get; set; }

        public string? Purpose { get; set; }

        public int? ParticipantCount { get; set; }

        public string? LocationName { get; set; }

        public string? BuildingName { get; set; }

        public string? FloorName { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public DateTime? CancelledAt { get; set; }

        public string? CancellationReason { get; set; }
    }
}








