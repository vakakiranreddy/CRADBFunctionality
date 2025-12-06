using ConferenceRoomBooking.DataAccess.Enum;
using System.ComponentModel.DataAnnotations;

namespace ConferenceRoomBooking.Business.DTOs.Resource
{
    public class CreateResourceDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public ResourceType ResourceType { get; set; }

        [Required]
        public int BuildingId { get; set; }

        [Required]
        public int FloorId { get; set; }

        public bool IsUnderMaintenance { get; set; } = false;

        public DateTime? BlockedFrom { get; set; }
        public DateTime? BlockedUntil { get; set; }

        [MaxLength(500)]
        public string? BlockReason { get; set; }
    }
}









