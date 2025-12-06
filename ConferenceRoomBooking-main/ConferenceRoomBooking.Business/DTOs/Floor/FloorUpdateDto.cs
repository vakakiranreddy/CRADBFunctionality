namespace ConferenceRoomBooking.Business.DTOs.Floor
{
    public class FloorUpdateDto
    {
        public string? FloorName { get; set; }

        public int? FloorNumber { get; set; }

        public int? BuildingId { get; set; }

        public int? LocationId { get; set; }

        public byte[]? FloorPlanImage { get; set; }

        public bool? IsActive { get; set; }
    }
}








