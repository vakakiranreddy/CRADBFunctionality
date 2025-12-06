namespace ConferenceRoomBooking.Business.DTOs.Auth
{
    public class AuthResponseDto
    {
        public int UserId { get; set; }
        public string EmployeeId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public DateTime TokenExpiresAt { get; set; }
    }

}









