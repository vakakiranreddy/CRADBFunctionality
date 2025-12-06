using ConferenceRoomBooking.Business.DTOs.User;
using ConferenceRoomBooking.Business.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceRoomBooking.API.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { message = "Invalid user ID" });

                var user = await _userService.GetUserByIdAsync(id);
                return user == null ? NotFound(new { message = $"User with ID {id} not found" }) : Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user with ID {UserId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the user", error = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [RequestSizeLimit(5 * 1024 * 1024)] // 5MB limit
        public async Task<IActionResult> CreateUser([FromForm] CreateUserDto dto)
        {
            var user = await _userService.CreateUserAsync(dto);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        [HttpPut("{id}")]
        [RequestSizeLimit(5 * 1024 * 1024)] // 5MB limit
        public async Task<IActionResult> UpdateUser(int id, [FromForm] UpdateUserDto dto)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { message = "Invalid user ID" });

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Get current user ID from claims
                var currentUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
                var currentUserRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
                
                // Allow admin to update any user, or user to update their own profile
                if (currentUserRole != "Admin" && currentUserId != id)
                {
                    _logger.LogWarning("User {CurrentUserId} attempted to update user {TargetUserId} without permission", currentUserId, id);
                    return Forbid("You can only update your own profile");
                }

                await _userService.UpdateUserAsync(id, dto);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument when updating user {UserId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user with ID {UserId}", id);
                return StatusCode(500, new { message = "An error occurred while updating the user", error = ex.Message });
            }
        }

        [HttpPut("{id}/role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUserRole(int id, [FromBody] UpdateUserRoleDto dto)
        {
            await _userService.UpdateUserRoleAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _userService.DeleteUserAsync(id);
            return NoContent();
        }

        [HttpGet("search")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SearchUsers([FromQuery] string keyword)
        {
            var users = await _userService.SearchUsersAsync(keyword);
            return Ok(users);
        }

        [HttpGet("location/{locationId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsersByLocation(int locationId)
        {
            var users = await _userService.GetUsersByLocationAsync(locationId);
            return Ok(users);
        }

        [HttpGet("department/{departmentId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsersByDepartment(int departmentId)
        {
            var users = await _userService.GetUsersByDepartmentAsync(departmentId);
            return Ok(users);
        }

        [HttpGet("profile/{userId}")]
        public async Task<IActionResult> GetProfile(int userId)
        {
            var profile = await _userService.GetProfileAsync(userId);
            return profile == null ? NotFound() : Ok(profile);
        }

        [HttpPut("profile/{userId}")]
        [RequestSizeLimit(3 * 1024 * 1024)] // 3MB limit
        public async Task<IActionResult> UpdateProfile(int userId, [FromForm] UpdateProfileDto dto)
        {
            try
            {
                if (userId <= 0)
                    return BadRequest(new { message = "Invalid user ID" });

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                await _userService.UpdateProfileAsync(userId, dto);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument when updating profile for user {UserId}", userId);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile for user with ID {UserId}", userId);
                return StatusCode(500, new { message = "An error occurred while updating the profile", error = ex.Message });
            }
        }
    }
}

