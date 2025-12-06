using ConferenceRoomBooking.Business.DTOs.EventRSVP;
using ConferenceRoomBooking.DataAccess.Enum;
using ConferenceRoomBooking.Business.Interfaces.IServices;
using ConferenceRoomBooking.DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ConferenceRoomBooking.API.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EventRSVPController : ControllerBase
    {
        private readonly IEventRSVPService _eventRSVPService;

        public EventRSVPController(IEventRSVPService eventRSVPService)
        {
            _eventRSVPService = eventRSVPService;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : 0;
        }

        [HttpPost("rsvp")]
        public async Task<IActionResult> AddUserRsvp([FromBody] CreateRsvpDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId <= 0)
                    return Unauthorized(new { message = "Invalid user" });

                var result = await _eventRSVPService.AddUserRsvpAsync(dto.EventId, userId, dto.Status.ToString());
                return result ? Ok(new { message = "RSVP added successfully" }) : BadRequest(new { message = "Failed to add RSVP" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while adding RSVP", error = ex.Message });
            }
        }

        [HttpPut("rsvp")]
        public async Task<IActionResult> UpdateUserRsvp([FromBody] CreateRsvpDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId <= 0)
                    return Unauthorized(new { message = "Invalid user" });

                var result = await _eventRSVPService.UpdateUserRsvpAsync(dto.EventId, userId, dto.Status.ToString());
                return result ? Ok(new { message = "RSVP updated successfully" }) : BadRequest(new { message = "Failed to update RSVP" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating RSVP", error = ex.Message });
            }
        }

        [HttpGet("user-rsvp/{eventId}")]
        public async Task<IActionResult> GetUserRsvp(int eventId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId <= 0)
                    return Unauthorized(new { message = "Invalid user" });

                var rsvp = await _eventRSVPService.GetUserRsvpAsync(eventId, userId);
                if (rsvp == null)
                    return NotFound(new { message = "RSVP not found" });

                var response = new RsvpResponseDto
                {
                    RSVPId = rsvp.RSVPId,
                    EventId = rsvp.EventId,
                    UserId = rsvp.UserId,
                    Status = rsvp.Status,
                    ResponseDate = rsvp.ResponseDate
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving user RSVP", error = ex.Message });
            }
        }

        [HttpGet("event/{eventId}")]
        public async Task<IActionResult> GetRsvpsByEvent(int eventId)
        {
            try
            {
                if (eventId <= 0)
                    return BadRequest(new { message = "Invalid event ID" });

                var rsvps = await _eventRSVPService.GetRsvpsByEventAsync(eventId);
                return Ok(rsvps);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving RSVPs by event", error = ex.Message });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetRsvpsByUser(int userId)
        {
            try
            {
                if (userId <= 0)
                    return BadRequest(new { message = "Invalid user ID" });

                var rsvps = await _eventRSVPService.GetRsvpsByUserAsync(userId);
                return Ok(rsvps);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving RSVPs by user", error = ex.Message });
            }
        }

        [HttpGet("event/{eventId}/interested/count")]
        public async Task<IActionResult> GetInterestedCount(int eventId)
        {
            try
            {
                if (eventId <= 0)
                    return BadRequest(new { message = "Invalid event ID" });

                var count = await _eventRSVPService.GetInterestedCountAsync(eventId);
                return Ok(new { eventId = eventId, interestedCount = count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving interested count", error = ex.Message });
            }
        }

        [HttpGet("event/{eventId}/not-interested/count")]
        public async Task<IActionResult> GetNotInterestedCount(int eventId)
        {
            try
            {
                if (eventId <= 0)
                    return BadRequest(new { message = "Invalid event ID" });

                var count = await _eventRSVPService.GetNotInterestedCountAsync(eventId);
                return Ok(new { eventId = eventId, notInterestedCount = count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving not interested count", error = ex.Message });
            }
        }

        [HttpGet("event/{eventId}/maybe/count")]
        public async Task<IActionResult> GetMaybeCount(int eventId)
        {
            try
            {
                if (eventId <= 0)
                    return BadRequest(new { message = "Invalid event ID" });

                var count = await _eventRSVPService.GetMaybeCountAsync(eventId);
                return Ok(new { eventId = eventId, maybeCount = count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving maybe count", error = ex.Message });
            }
        }
    }
}

