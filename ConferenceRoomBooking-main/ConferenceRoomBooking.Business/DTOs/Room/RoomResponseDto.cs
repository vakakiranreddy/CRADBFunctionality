namespace ConferenceRoomBooking.Business.DTOs.Room
{
    public class RoomResponseDto
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public int ResourceId { get; set; }
        public string RoomName { get; set; }
        public int Capacity { get; set; }
        public bool HasTV { get; set; }
        public bool HasWhiteboard { get; set; }
        public bool HasWiFi { get; set; }
        public bool HasProjector { get; set; }
        public bool HasVideoConference { get; set; }
        public bool HasAirConditioning { get; set; }
        public string? PhoneExtension { get; set; }
        public byte[]? RoomImage { get; set; }

        // Resource Information
        public int LocationId { get; set; }
        public int BuildingId { get; set; }
        public int FloorId { get; set; }
        public bool IsUnderMaintenance { get; set; }
    
        public bool IsBlocked { get; set; }
        public DateTime? BlockedFrom { get; set; }
        public DateTime? BlockedUntil { get; set; }
        public string? BlockReason { get; set; }
        public int? MinBookingDuration { get; set; }
        public int? MaxBookingDuration { get; set; }

        // Location Details
        public string? LocationName { get; set; }
        public string? LocationAddress { get; set; }
        public string? City { get; set; }
        public string? BuildingName { get; set; }
        public string? FloorName { get; set; }

    }
}









