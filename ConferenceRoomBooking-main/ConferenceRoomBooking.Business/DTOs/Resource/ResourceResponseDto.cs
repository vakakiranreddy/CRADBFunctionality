using ConferenceRoomBooking.DataAccess.Enum;

namespace ConferenceRoomBooking.Business.DTOs.Resource
{
    public class ResourceResponseDto
    {
        public int Id { get; set; }
        public int ResourceId { get; set; }
        public string Name { get; set; }
        public ResourceType ResourceType { get; set; }
        public int LocationId { get; set; }
        public int BuildingId { get; set; }
        public int FloorId { get; set; }
        public bool IsUnderMaintenance { get; set; }
        public bool IsBlocked { get; set; }
        
        public bool IsActive { get; set; }
        public DateTime? BlockedFrom { get; set; }
        public DateTime? BlockedUntil { get; set; }
        public string? BlockReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Location Information
        public string? LocationName { get; set; }
        public string? LocationAddress { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }

        // Building Information
        public string? BuildingName { get; set; }

        // Floor Information
        public string? FloorName { get; set; }
    }
}









