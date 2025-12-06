using Microsoft.AspNetCore.Http;

using System.ComponentModel.DataAnnotations;

namespace ConferenceRoomBooking.Business.DTOs.Location
{
    public class LocationUpdateDto
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
        public IFormFile? LocationImage { get; set; }
        public bool? IsActive { get; set; }
    }
}









