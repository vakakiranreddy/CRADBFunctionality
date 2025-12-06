using System.ComponentModel.DataAnnotations;

namespace ConferenceRoomBooking.Business.DTOs.Department
{
    public class CreateDepartmentDto
    {
        [Required]
        [StringLength(100)]
        public string DepartmentName { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }
    }
}








