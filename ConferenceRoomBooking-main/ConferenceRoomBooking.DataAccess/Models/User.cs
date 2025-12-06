using ConferenceRoomBooking.DataAccess.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConferenceRoomBooking.DataAccess.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string EmployeeId { get; set; }

        [Required]
        [StringLength(50)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "First name must contain only letters.")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Last name must contain only letters.")]
        public string LastName { get; set; }

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(10, MinimumLength = 10)]
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Phone number must be 10 digits and start with 6,7,8, or 9.")]
        public string PhoneNumber { get; set; }

        [Required]
        public byte[] PasswordHash { get; set; } = null!;

        [Required]
        public byte[] PasswordSalt { get; set; } = null!;

        [Required]
        public UserRole Role { get; set; } = UserRole.User;

        public byte[]? ProfileImage { get; set; }

        public int? LocationId { get; set; }

        public int? DepartmentId { get; set; }

        [StringLength(100)]
        public string? Title { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime? LastLoginAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey("LocationId")]
        public Location? Location { get; set; }

        [ForeignKey("DepartmentId")]
        public Department? Department { get; set; }

        public UserBookingStats BookingStats { get; set; } = null!;

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<UserNotification> Notifications { get; set; } = new List<UserNotification>();
        public ICollection<UserOtpVerification> OtpVerifications { get; set; } = new List<UserOtpVerification>();
    }
}









