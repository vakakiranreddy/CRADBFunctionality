namespace ConferenceRoomBooking.Business.DTOs.Desk
{
    public class DeskResponseDto
    {
        public int Id { get; set; }
        public int DeskId { get; set; }
        public int ResourceId { get; set; }
        public string DeskName { get; set; }
        public bool HasMonitor { get; set; }
        public bool HasKeyboard { get; set; }
        public bool HasMouse { get; set; }
        public bool HasDockingStation { get; set; }
        public byte[]? DeskImage { get; set; }

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









