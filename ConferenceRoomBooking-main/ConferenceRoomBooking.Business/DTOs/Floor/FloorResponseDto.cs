namespace ConferenceRoomBooking.Business.DTOs.Floor
{
    public class FloorResponseDto
    {
        public int Id { get; set; }

        public string FloorName { get; set; } = string.Empty;

        public int FloorNumber { get; set; }

        public int BuildingId { get; set; }

        public string? BuildingName { get; set; }

        public int LocationId { get; set; }

        public string? LocationName { get; set; }

        public byte[]? FloorPlanImage { get; set; }

        public bool IsActive { get; set; }
    }
}








