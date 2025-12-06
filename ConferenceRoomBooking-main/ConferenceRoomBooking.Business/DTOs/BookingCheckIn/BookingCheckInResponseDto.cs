namespace ConferenceRoomBooking.Business.DTOs.BookingCheckIn
{
    public class BookingCheckInResponseDto
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public bool IsCheckedIn { get; set; }
        public bool IsCheckedOut { get; set; }
        public bool IsNoShow { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string ResourceName { get; set; } = string.Empty;
        public TimeSpan? Duration { get; set; }
    }

}









