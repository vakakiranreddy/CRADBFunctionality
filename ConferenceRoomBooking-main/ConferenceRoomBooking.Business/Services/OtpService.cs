using ConferenceRoomBooking.Business.DTOs.Otp;
using ConferenceRoomBooking.DataAccess.Interfaces.IRepositories;
using ConferenceRoomBooking.Business.Interfaces.IServices;
using ConferenceRoomBooking.DataAccess.Models;

namespace ConferenceRoomBooking.Business.Services
{
    public class OtpService : IOtpService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserOtpVerificationRepository _otpRepository;
        private readonly IEmailService _emailService;

        public OtpService(
            IUserRepository userRepository,
            IUserOtpVerificationRepository otpRepository,
            IEmailService emailService)
        {
            _userRepository = userRepository;
            _otpRepository = otpRepository;
            _emailService = emailService;
        }

        public async Task<bool> SendOtpAsync(SendOtpDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user == null) return false;

            var otp = GenerateOtp();
            var otpRecord = new UserOtpVerification
            {
                UserId = user.Id,
                OtpCode = otp,
                Type = dto.Type,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                CreatedAt = DateTime.UtcNow
            };

            await _otpRepository.AddAsync(otpRecord);

            var subject = dto.Type.ToString() == "ForgotPassword" ? "Password Reset OTP" : "Verification OTP";
            var body = $"Your OTP is: {otp}. It will expire in 10 minutes.";

            return await _emailService.SendEmailAsync(dto.Email, subject, body);
        }

        public async Task<bool> VerifyOtpAsync(VerifyOtpDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user == null) return false;

            var otpRecord = await _otpRepository.GetLatestOtpAsync(user.Id, dto.Type);
            if (otpRecord == null || otpRecord.IsUsed || otpRecord.ExpiresAt < DateTime.UtcNow)
                return false;

            if (otpRecord.OtpCode != dto.Otp)
            {
                await _otpRepository.IncrementAttemptsAsync(otpRecord.Id);
                return false;
            }

            await _otpRepository.MarkAsUsedAsync(otpRecord.Id);
            return true;
        }

        private static string GenerateOtp()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }
}








