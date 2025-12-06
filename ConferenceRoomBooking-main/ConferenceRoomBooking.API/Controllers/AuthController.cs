using ConferenceRoomBooking.Business.DTOs.Auth;
using ConferenceRoomBooking.Business.DTOs.Otp;
using ConferenceRoomBooking.DataAccess.Enum;
using ConferenceRoomBooking.Business.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceRoomBooking.API.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            return result == null ? Unauthorized("Invalid credentials") : Ok(result);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
        {
            try
            {
                await _authService.GenerateAndSendOtpAsync(email, OtpType.ForgotPassword);
                return Ok(new { message = "OTP sent to your email" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto dto)
        {
            var isValid = await _authService.VerifyOtpAsync(dto.Email, dto.Otp, OtpType.ForgotPassword);
            return isValid ? Ok(new { message = "OTP verified successfully" }) : BadRequest("Invalid or expired OTP");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var otpValid = await _authService.VerifyOtpAsync(dto.Email, dto.Otp, OtpType.ForgotPassword);
            if (!otpValid) return BadRequest("Invalid or expired OTP");

            var result = await _authService.ResetPasswordAsync(dto.Email, dto.NewPassword);
            return result ? Ok(new { message = "Password reset successfully" }) : BadRequest("Failed to reset password");
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var result = await _authService.ChangePasswordAsync(dto.UserId, dto.CurrentPassword, dto.NewPassword);
            return result ? Ok(new { message = "Password changed successfully" }) : BadRequest("Invalid current password");
        }
    }
}

