using ConferenceRoomBooking.DataAccess.Enum;
using ConferenceRoomBooking.DataAccess.Models;

namespace ConferenceRoomBooking.DataAccess.Interfaces.IRepositories
{
    public interface IUserOtpVerificationRepository : IBaseRepository<UserOtpVerification>
    {
        Task<UserOtpVerification?> GetLatestOtpAsync(int userId, OtpType type);
        Task MarkAsUsedAsync(int otpId);
        Task IncrementAttemptsAsync(int otpId);
    }
}









