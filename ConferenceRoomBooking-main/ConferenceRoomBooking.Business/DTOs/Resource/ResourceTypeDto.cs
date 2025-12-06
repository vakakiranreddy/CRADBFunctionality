using ConferenceRoomBooking.DataAccess.Enum;
using System.ComponentModel.DataAnnotations;

namespace ConferenceRoomBooking.Business.DTOs.Resource
{
    public class ResourceTypeDto
    {
        [Required]
        public ResourceType ResourceType { get; set; }
    }
}









