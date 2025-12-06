using ConferenceRoomBooking.DataAccess.Enum;

namespace ConferenceRoomBooking.Business.DTOs.Otp
{
    public class VerifyOtpDto
    {
        public string Email { get; set; } = string.Empty;
        public string Otp { get; set; } = string.Empty;
        public OtpType Type { get; set; }
    }
}









