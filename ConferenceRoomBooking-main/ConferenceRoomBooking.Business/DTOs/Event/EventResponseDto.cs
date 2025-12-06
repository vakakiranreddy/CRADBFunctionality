namespace ConferenceRoomBooking.Business.DTOs.Event
{
    public class EventResponseDto
    {
        public int EventId { get; set; }
        public string EventTitle { get; set; }
        public string? Description { get; set; }

        public int? LocationId { get; set; }
        public string? LocationName { get; set; }

        public int? BuildingId { get; set; }

        public int? FloorId { get; set; }

        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public bool IsActive { get; set; }

        public string? EventImage { get; set; } // Base64 string

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // RSVP info
        public List<int>? InterestedUserIds { get; set; }
        public List<int>? NotInterestedUserIds { get; set; }
        public List<int>? MaybeUserIds { get; set; }
    }

}









