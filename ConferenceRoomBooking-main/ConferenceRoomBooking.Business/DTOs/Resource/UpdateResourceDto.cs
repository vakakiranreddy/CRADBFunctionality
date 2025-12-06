using System.ComponentModel.DataAnnotations;

namespace ConferenceRoomBooking.Business.DTOs.Resource
{
    public class UpdateResourceDto
    {
        public string? Name { get; set; }
        public int? LocationId { get; set; }
        public int? BuildingId { get; set; }
        public int? FloorId { get; set; }
        public bool? IsUnderMaintenance { get; set; }

        public bool? IsActive { get; set; }

        public DateTime? BlockedFrom { get; set; }
        public DateTime? BlockedUntil { get; set; }

        [MaxLength(500)]
        public string? BlockReason { get; set; }
    }
}









