using System.ComponentModel.DataAnnotations;

namespace ConferenceRoomBooking.Business.DTOs.User
{
    public class UserProfileDto
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int? LocationId { get; set; }
        public int? DepartmentId { get; set; }
        public string? Title { get; set; }
        public bool IsActive { get; set; }
        public string? ProfileImage { get; set; } // Base64 string
        public DateTime? LastLoginAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}









