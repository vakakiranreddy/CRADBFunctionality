namespace ConferenceRoomBooking.Business.DTOs.Resource
{
    public class ResourceDetailResponseDto : ResourceResponseDto
    {
        // Room details (if ResourceType is Room)
        public int? RoomId { get; set; }
        public string? RoomName { get; set; }
        public int? Capacity { get; set; }
        public bool? HasTV { get; set; }
        public bool? HasWhiteboard { get; set; }
        public bool? HasWiFi { get; set; }
        public bool? HasProjector { get; set; }
        public bool? HasVideoConference { get; set; }
        public bool? HasAirConditioning { get; set; }
        public string? PhoneExtension { get; set; }
        public byte[]? RoomImage { get; set; }

        // Desk details (if ResourceType is Desk)
        public int? DeskId { get; set; }
        public string? DeskName { get; set; }
        public bool? HasMonitor { get; set; }
        public bool? HasKeyboard { get; set; }
        public bool? HasMouse { get; set; }
        public bool? HasDockingStation { get; set; }
        public byte[]? DeskImage { get; set; }
    }
}









