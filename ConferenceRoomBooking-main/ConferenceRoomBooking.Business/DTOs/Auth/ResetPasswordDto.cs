using System.ComponentModel.DataAnnotations;

namespace ConferenceRoomBooking.Business.DTOs.Auth
{
    public class ResetPasswordDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "OTP is required")]
        [StringLength(6, MinimumLength = 4, ErrorMessage = "OTP must be 4–6 digits")]
        public string Otp { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password is required")]
        [StringLength(50, MinimumLength = 8, ErrorMessage = "Password must be between 8–50 characters")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d).+$", ErrorMessage = "Password must contain letters and numbers")]
        public string NewPassword { get; set; } = string.Empty;
    }

}









