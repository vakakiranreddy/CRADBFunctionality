using Microsoft.AspNetCore.Http;

namespace ConferenceRoomBooking.Business.DTOs.Building
{
    public class BuildingUpdateDto
    {
        public string? Name { get; set; }

        public int? LocationId { get; set; }

        public string? Address { get; set; }

        public int? NumberOfFloors { get; set; }

        public IFormFile? BuildingImage { get; set; }

        public bool? IsActive { get; set; }
    }
}








