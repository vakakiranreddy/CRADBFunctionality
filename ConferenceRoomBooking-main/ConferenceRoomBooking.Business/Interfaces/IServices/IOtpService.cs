using ConferenceRoomBooking.Business.DTOs.Otp;

namespace ConferenceRoomBooking.Business.Interfaces.IServices
{

    public interface IOtpService
    {
        Task<bool> SendOtpAsync(SendOtpDto dto);
        Task<bool> VerifyOtpAsync(VerifyOtpDto dto);
    }
}









