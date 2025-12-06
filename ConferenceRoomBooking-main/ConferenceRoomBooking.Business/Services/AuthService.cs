using ConferenceRoomBooking.Business.DTOs.Auth;
using ConferenceRoomBooking.DataAccess.Enum;
using ConferenceRoomBooking.DataAccess.Interfaces.IRepositories;
using ConferenceRoomBooking.Business.Interfaces.IServices;
using ConferenceRoomBooking.DataAccess.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ConferenceRoomBooking.Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserOtpVerificationRepository _otpRepository;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public AuthService(
            IUserRepository userRepository,
            IUserOtpVerificationRepository otpRepository,
            IEmailService emailService,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _otpRepository = otpRepository;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user == null || !user.IsActive) return null;

            if (!VerifyPasswordHash(dto.Password, user.PasswordHash, user.PasswordSalt))
                return null;

            user.LastLoginAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);

            var token = await GenerateJwtTokenAsync(user);

            return new AuthResponseDto
            {
                Token = token,
                UserId = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role.ToString()
            };
        }

        public async Task<bool> ValidateUserCredentialsAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null || !user.IsActive) return false;

            return VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt);
        }

        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            if (!VerifyPasswordHash(currentPassword, user.PasswordHash, user.PasswordSalt))
                return false;

            CreatePasswordHash(newPassword, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> ResetPasswordAsync(string email, string newPassword)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) return false;

            CreatePasswordHash(newPassword, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task GenerateAndSendOtpAsync(string email, OtpType type)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) throw new ArgumentException("User not found");

            var otp = GenerateOtp();
            var otpRecord = new UserOtpVerification
            {
                UserId = user.Id,
                OtpCode = otp,
                Type = type,
                ExpiresAt = DateTime.UtcNow.AddMinutes(2880),
                CreatedAt = DateTime.UtcNow
            };

            await _otpRepository.AddAsync(otpRecord);

            var subject = type == OtpType.ForgotPassword ? "Password Reset OTP" : "Verification OTP";
            var body = $"Your OTP is: {otp}. It will expire in 10 minutes.";
            
            await _emailService.SendEmailAsync(email, subject, body);
        }

        public async Task<bool> VerifyOtpAsync(string email, string otp, OtpType type)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) return false;

            var otpRecord = await _otpRepository.GetLatestOtpAsync(user.Id, type);
            if (otpRecord == null || otpRecord.IsUsed || otpRecord.ExpiresAt < DateTime.UtcNow)
                return false;

            if (otpRecord.OtpCode != otp)
            {
                await _otpRepository.IncrementAttemptsAsync(otpRecord.Id);
                return false;
            }

            await _otpRepository.MarkAsUsedAsync(otpRecord.Id);
            return true;
        }

        public async Task<string> GenerateJwtTokenAsync(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "your-secret-key"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            using var hmac = new HMACSHA512(storedSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(storedHash);
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        private static string GenerateOtp()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }
}








