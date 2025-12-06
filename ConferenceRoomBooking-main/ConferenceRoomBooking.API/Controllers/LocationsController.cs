using ConferenceRoomBooking.Business.DTOs.Location;
using ConferenceRoomBooking.Business.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ConferenceRoomBooking.API.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LocationsController : ControllerBase
    {
        private readonly ILocationService _locationService;

        public LocationsController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLocations()
        {
            try
            {
                var locations = await _locationService.GetAllLocationsAsync();
                return Ok(locations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving locations", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLocation(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { message = "Invalid location ID" });

                var location = await _locationService.GetLocationByIdAsync(id);
                return location == null ? NotFound(new { message = "Location not found" }) : Ok(location);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the location", error = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateLocation([FromBody] LocationCreateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var location = await _locationService.CreateLocationAsync(dto);
                return CreatedAtAction(nameof(GetLocation), new { id = location.LocationId }, location);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the location", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateLocation(int id, [FromBody] LocationUpdateDto dto)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { message = "Invalid location ID" });

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var location = await _locationService.UpdateLocationAsync(id, dto);
                return Ok(location);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the location", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteLocation(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { message = "Invalid location ID" });

                await _locationService.DeleteLocationAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the location", error = ex.Message });
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchLocations([FromQuery][Required] string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return BadRequest(new { message = "Search term is required" });

                var locations = await _locationService.SearchLocationsAsync(searchTerm);
                return Ok(locations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while searching locations", error = ex.Message });
            }
        }
    }
}

