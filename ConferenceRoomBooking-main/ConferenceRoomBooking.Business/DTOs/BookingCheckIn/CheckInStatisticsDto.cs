namespace ConferenceRoomBooking.Business.DTOs.BookingCheckIn
{
    public class CheckInStatisticsDto
    {
        public int TotalBookings { get; set; }
        public int CheckedInBookings { get; set; }
        public int NoShowBookings { get; set; }
        public double CheckInRate { get; set; }
        public int TotalCheckIns { get; set; }
        public int TotalNoShows { get; set; }
        public double NoShowRate { get; set; }
        public double AverageCheckInDuration { get; set; }
        public int OnTimeCheckIns { get; set; }
        public int LateCheckIns { get; set; }
    }

}









