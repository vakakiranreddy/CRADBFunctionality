using Microsoft.AspNetCore.Http;

using System.ComponentModel.DataAnnotations;

namespace ConferenceRoomBooking.Business.DTOs.User
{
    public class CreateUserDto
    {
        public IFormFile? ProfileImage { get; set; }

        [Required]
        [StringLength(50)]
        public string EmployeeId { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "First name must contain only letters.")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Last name must contain only letters.")]
        public string LastName { get; set; } = string.Empty;

        public int? LocationId { get; set; }

        public int? DepartmentId { get; set; }

        [StringLength(100)]
        public string? Title { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        

        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

}









