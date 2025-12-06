using ConferenceRoomBooking.Business.DTOs.BroadCastNotification;
using ConferenceRoomBooking.DataAccess.Enum;
using ConferenceRoomBooking.Business.Interfaces.IServices;
using ConferenceRoomBooking.DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceRoomBooking.API.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BroadcastNotificationController : ControllerBase
    {
        private readonly IBroadcastNotificationService _broadcastService;

        public BroadcastNotificationController(IBroadcastNotificationService broadcastService)
        {
            _broadcastService = broadcastService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateBroadcast([FromBody] SendBroadcastDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var broadcast = await _broadcastService.CreateBroadcastAsync(dto);
                return CreatedAtAction(nameof(GetBroadcastById), new { id = broadcast.Id }, broadcast);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the broadcast notification", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateBroadcast(int id, [FromBody] SendBroadcastDto dto)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { message = "Invalid broadcast ID" });

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var broadcast = await _broadcastService.UpdateBroadcastAsync(id, dto);
                return Ok(broadcast);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the broadcast notification", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBroadcast(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { message = "Invalid broadcast ID" });

                var result = await _broadcastService.DeleteBroadcastAsync(id);
                return result ? NoContent() : NotFound(new { message = "Broadcast notification not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the broadcast notification", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBroadcastById(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { message = "Invalid broadcast ID" });

                var broadcast = await _broadcastService.GetBroadcastByIdAsync(id);
                return broadcast == null ? NotFound(new { message = "Broadcast notification not found" }) : Ok(broadcast);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the broadcast notification", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBroadcasts()
        {
            try
            {
                var broadcasts = await _broadcastService.GetAllBroadcastsAsync();
                return Ok(broadcasts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving broadcast notifications", error = ex.Message });
            }
        }

        [HttpGet("date-range")]
        public async Task<IActionResult> GetByDateRange([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            try
            {
                var broadcasts = await _broadcastService.GetByDateRangeAsync(from, to);
                return Ok(broadcasts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving broadcast notifications by date range", error = ex.Message });
            }
        }

        [HttpGet("department/{departmentId}")]
        public async Task<IActionResult> GetByDepartment(int departmentId)
        {
            try
            {
                if (departmentId <= 0)
                    return BadRequest(new { message = "Invalid department ID" });

                var broadcasts = await _broadcastService.GetByDepartmentIdAsync(departmentId);
                return Ok(broadcasts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving broadcast notifications by department", error = ex.Message });
            }
        }

        [HttpGet("location/{locationId}")]
        public async Task<IActionResult> GetByLocation(int locationId)
        {
            try
            {
                if (locationId <= 0)
                    return BadRequest(new { message = "Invalid location ID" });

                var broadcasts = await _broadcastService.GetByLocationIdAsync(locationId);
                return Ok(broadcasts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving broadcast notifications by location", error = ex.Message });
            }
        }

        [HttpGet("role/{role}")]
        public async Task<IActionResult> GetByRole(UserRole role)
        {
            try
            {
                var broadcasts = await _broadcastService.GetByRoleAsync(role);
                return Ok(broadcasts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving broadcast notifications by role", error = ex.Message });
            }
        }

        [HttpPost("process-pending")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ProcessPendingBroadcasts()
        {
            try
            {
                await _broadcastService.ProcessPendingBroadcastsAsync();
                return Ok(new { message = "Pending broadcasts processed successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing pending broadcasts", error = ex.Message });
            }
        }
    }
}

