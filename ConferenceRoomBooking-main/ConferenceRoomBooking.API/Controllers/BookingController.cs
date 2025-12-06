using ConferenceRoomBooking.Business.DTOs.Booking;
using ConferenceRoomBooking.DataAccess.Enum;
using ConferenceRoomBooking.Business.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ConferenceRoomBooking.API.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : 0;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = GetCurrentUserId();
                if (userId <= 0)
                    return Unauthorized(new { message = "Invalid user" });

                var result = await _bookingService.BookResource(dto, userId);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the booking", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBooking(int id)
        {
            var result = await _bookingService.GetBookingByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllBookings()
        {
            var result = await _bookingService.GetAllBookingsAsync();
            return Ok(result);
        }

        [HttpGet("my-bookings")]
        public async Task<IActionResult> GetMyBookings()
        {
            var userId = GetCurrentUserId();
            if (userId <= 0)
                return Unauthorized(new { message = "Invalid user" });

            var result = await _bookingService.GetUserBookingsAsync(userId);
            return Ok(result);
        }

        [HttpGet("user/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserBookings(int userId)
        {
            var result = await _bookingService.GetUserBookingsAsync(userId);
            return Ok(result);
        }

        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetBookingsByStatus(SessionStatus status)
        {
            var result = await _bookingService.GetBookingsByStatusAsync(status);
            return Ok(result);
        }

        [HttpGet("calendar")]
        public async Task<IActionResult> GetCalendarBookings([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var result = await _bookingService.GetBookingsForCalendarAsync(startDate, endDate);
            return Ok(result);
        }

        [HttpGet("resource/{resourceId}")]
        public async Task<IActionResult> GetResourceBookings(int resourceId, [FromQuery] DateTime? date = null)
        {
            var result = await _bookingService.GetBookingsByResourceIdAsync(resourceId, date);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelBooking(int id, [FromBody] string reason)
        {
            var userId = GetCurrentUserId();
            if (userId <= 0)
                return Unauthorized(new { message = "Invalid user" });

            var result = await _bookingService.CancelBookingAsync(id, userId, reason);
            return result ? Ok(new { message = "Booking cancelled successfully" }) : BadRequest("Failed to cancel booking");
        }

        [HttpGet("alternative-slots")]
        public async Task<IActionResult> GetAlternativeSlots([FromQuery] int resourceId, [FromQuery] DateTime date, [FromQuery] DateTime startTime, [FromQuery] DateTime endTime)
        {
            var result = await _bookingService.GetAlternativeTimeSlotsAsync(resourceId, date, startTime, endTime);
            return Ok(result);
        }

        [HttpGet("analytics")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAnalytics([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null, [FromQuery] int? locationId = null)
        {
            var result = await _bookingService.GetBookingAnalyticsAsync(startDate, endDate, locationId);
            return Ok(result);
        }
    }
}

