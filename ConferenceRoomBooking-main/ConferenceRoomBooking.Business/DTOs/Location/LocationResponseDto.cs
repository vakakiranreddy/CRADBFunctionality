namespace ConferenceRoomBooking.Business.DTOs.Location
{
    public class LocationResponseDto
    {
        public int LocationId { get; set; }
       
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string? LocationImage { get; set; } // Base64 string
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}









