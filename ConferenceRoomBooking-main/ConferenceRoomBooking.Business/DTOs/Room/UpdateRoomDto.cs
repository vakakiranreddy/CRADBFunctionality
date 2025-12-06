using Microsoft.AspNetCore.Http;

namespace ConferenceRoomBooking.Business.DTOs.Room
{
    public class UpdateRoomDto
    {
        public string? RoomName { get; set; }

        public int? Capacity { get; set; }

        public bool? HasTV { get; set; }

        public bool? HasWhiteboard { get; set; }

        public bool? HasWiFi { get; set; }

        public bool? HasProjector { get; set; }

        public bool? HasVideoConference { get; set; }

        public bool? HasAirConditioning { get; set; }

        public IFormFile? RoomImage { get; set; }
    }
}








