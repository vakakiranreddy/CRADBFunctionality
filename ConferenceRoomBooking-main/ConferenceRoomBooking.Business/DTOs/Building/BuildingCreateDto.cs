using Microsoft.AspNetCore.Http;

using System.ComponentModel.DataAnnotations;

namespace ConferenceRoomBooking.Business.DTOs.Building
{
    public class BuildingCreateDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public int LocationId { get; set; }

        [Required]
        public string Address { get; set; } = string.Empty;

        public int? NumberOfFloors { get; set; }

        public IFormFile? BuildingImage { get; set; }
    }
}








