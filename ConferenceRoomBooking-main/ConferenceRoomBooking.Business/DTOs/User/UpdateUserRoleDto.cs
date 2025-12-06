using ConferenceRoomBooking.DataAccess.Enum;
using System.ComponentModel.DataAnnotations;

namespace ConferenceRoomBooking.Business.DTOs.User
{
    public class UpdateUserRoleDto
    {
        [Required]
        public UserRole Role { get; set; }
    }
}








