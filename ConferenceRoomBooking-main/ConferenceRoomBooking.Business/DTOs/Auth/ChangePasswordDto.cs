using System.ComponentModel.DataAnnotations;

namespace ConferenceRoomBooking.Business.DTOs.Auth
{
    public class ChangePasswordDto
    {
        [Required]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Current password is required")]
        [StringLength(50, MinimumLength = 6)]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password is required")]
        [StringLength(50, MinimumLength = 8, ErrorMessage = "New password must be at least 8 characters long")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d).+$", ErrorMessage = "New password must contain letters and numbers")]
        public string NewPassword { get; set; } = string.Empty;
    }

}









