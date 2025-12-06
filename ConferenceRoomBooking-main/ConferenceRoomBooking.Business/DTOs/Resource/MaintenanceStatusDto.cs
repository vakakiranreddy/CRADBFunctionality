using System.ComponentModel.DataAnnotations;

namespace ConferenceRoomBooking.Business.DTOs.Resource
{
    public class MaintenanceStatusDto
    {
        [Required]
        public bool IsUnderMaintenance { get; set; }
    }
}









