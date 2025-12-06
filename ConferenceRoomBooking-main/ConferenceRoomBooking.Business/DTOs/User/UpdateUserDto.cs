using Microsoft.AspNetCore.Http;

using System.ComponentModel.DataAnnotations;

namespace ConferenceRoomBooking.Business.DTOs.User
{
    public class UpdateUserDto
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
        [StringLength(10, MinimumLength = 10)]
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Phone number must be 10 digits and start with 6,7,8, or 9.")]
        public string PhoneNumber { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

}









