using ConferenceRoomBooking.Business.DTOs.Building;
using ConferenceRoomBooking.Business.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ConferenceRoomBooking.API.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BuildingsController : ControllerBase
    {
        private readonly IBuildingService _buildingService;

        public BuildingsController(IBuildingService buildingService)
        {
            _buildingService = buildingService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBuildings()
        {
            try
            {
                var buildings = await _buildingService.GetAllBuildingsAsync();
                return Ok(buildings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving buildings", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBuilding(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { message = "Invalid building ID" });

                var building = await _buildingService.GetBuildingByIdAsync(id);
                return building == null ? NotFound(new { message = "Building not found" }) : Ok(building);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the building", error = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [RequestSizeLimit(5 * 1024 * 1024)] // 5MB limit
        public async Task<IActionResult> CreateBuilding([FromForm] BuildingCreateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var building = await _buildingService.CreateBuildingAsync(dto);
                return CreatedAtAction(nameof(GetBuilding), new { id = building.Id }, building);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the building", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [RequestSizeLimit(5 * 1024 * 1024)] // 5MB limit
        public async Task<IActionResult> UpdateBuilding(int id, [FromForm] BuildingUpdateDto dto)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { message = "Invalid building ID" });

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var building = await _buildingService.UpdateBuildingAsync(id, dto);
                return Ok(building);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the building", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBuilding(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { message = "Invalid building ID" });

                await _buildingService.DeleteBuildingAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the building", error = ex.Message });
            }
        }

        [HttpGet("location/{locationId}")]
        public async Task<IActionResult> GetBuildingsByLocation(int locationId)
        {
            try
            {
                if (locationId <= 0)
                    return BadRequest(new { message = "Invalid location ID" });

                var buildings = await _buildingService.GetBuildingsByLocationIdAsync(locationId);
                return Ok(buildings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving buildings", error = ex.Message });
            }
        }
    }
}

