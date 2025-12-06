using ConferenceRoomBooking.Business.DTOs.Floor;
using ConferenceRoomBooking.Business.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceRoomBooking.API.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FloorController : ControllerBase
    {
        private readonly IFloorService _floorService;

        public FloorController(IFloorService floorService)
        {
            _floorService = floorService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateFloor([FromBody] FloorCreateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var floor = await _floorService.CreateFloorAsync(dto);
                return CreatedAtAction(nameof(GetFloorById), new { id = floor.Id }, floor);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the floor", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateFloor(int id, [FromBody] FloorUpdateDto dto)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { message = "Invalid floor ID" });

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var floor = await _floorService.UpdateFloorAsync(id, dto);
                return Ok(floor);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the floor", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFloorById(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { message = "Invalid floor ID" });

                var floor = await _floorService.GetFloorByIdAsync(id);
                return floor == null ? NotFound(new { message = "Floor not found" }) : Ok(floor);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the floor", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFloors()
        {
            try
            {
                var floors = await _floorService.GetAllFloorsAsync();
                return Ok(floors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving floors", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteFloor(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { message = "Invalid floor ID" });

                await _floorService.DeleteFloorAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the floor", error = ex.Message });
            }
        }

        [HttpGet("building/{buildingId}")]
        public async Task<IActionResult> GetFloorsByBuilding(int buildingId)
        {
            try
            {
                if (buildingId <= 0)
                    return BadRequest(new { message = "Invalid building ID" });

                var floors = await _floorService.GetFloorsByBuildingIdAsync(buildingId);
                return Ok(floors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving floors by building", error = ex.Message });
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchFloors([FromQuery] string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return BadRequest(new { message = "Search term is required" });

                var floors = await _floorService.SearchFloorsAsync(searchTerm);
                return Ok(floors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while searching floors", error = ex.Message });
            }
        }

        [HttpGet("sorted")]
        public async Task<IActionResult> GetFloorsSorted([FromQuery] string sortBy, [FromQuery] bool ascending = true)
        {
            try
            {
                var floors = await _floorService.GetFloorsSortedAsync(sortBy, ascending);
                return Ok(floors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving sorted floors", error = ex.Message });
            }
        }
    }
}

