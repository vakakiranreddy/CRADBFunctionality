using ConferenceRoomBooking.Business.DTOs.Desk;
using ConferenceRoomBooking.Business.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceRoomBooking.API.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DeskController : ControllerBase
    {
        private readonly IDeskService _deskService;

        public DeskController(IDeskService deskService)
        {
            _deskService = deskService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateDesk([FromBody] CreateDeskDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var desk = await _deskService.CreateDeskAsync(dto);
                return CreatedAtAction(nameof(GetDeskById), new { id = desk.ResourceId }, desk);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the desk", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDeskById(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { message = "Invalid desk ID" });

                var desk = await _deskService.GetDeskByIdAsync(id);
                return desk == null ? NotFound(new { message = "Desk not found" }) : Ok(desk);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the desk", error = ex.Message });
            }
        }

        [HttpGet("resource/{resourceId}")]
        public async Task<IActionResult> GetDeskByResourceId(int resourceId)
        {
            try
            {
                if (resourceId <= 0)
                    return BadRequest(new { message = "Invalid resource ID" });

                var desk = await _deskService.GetDeskByResourceIdAsync(resourceId);
                return desk == null ? NotFound(new { message = "Desk not found for the given resource" }) : Ok(desk);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the desk by resource ID", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDesks()
        {
            try
            {
                var desks = await _deskService.GetAllDesksAsync();
                return Ok(desks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving desks", error = ex.Message });
            }
        }

        [HttpGet("location/{locationId}")]
        public async Task<IActionResult> GetDesksByLocation(int locationId)
        {
            try
            {
                if (locationId <= 0)
                    return BadRequest(new { message = "Invalid location ID" });

                var desks = await _deskService.GetDesksByLocationAsync(locationId);
                return Ok(desks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving desks by location", error = ex.Message });
            }
        }

        [HttpGet("building/{buildingId}")]
        public async Task<IActionResult> GetDesksByBuilding(int buildingId)
        {
            try
            {
                if (buildingId <= 0)
                    return BadRequest(new { message = "Invalid building ID" });

                var desks = await _deskService.GetDesksByBuildingAsync(buildingId);
                return Ok(desks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving desks by building", error = ex.Message });
            }
        }

        [HttpGet("floor/{floorId}")]
        public async Task<IActionResult> GetDesksByFloor(int floorId)
        {
            try
            {
                if (floorId <= 0)
                    return BadRequest(new { message = "Invalid floor ID" });

                var desks = await _deskService.GetDesksByFloorAsync(floorId);
                return Ok(desks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving desks by floor", error = ex.Message });
            }
        }

        [HttpPost("available")]
        public async Task<IActionResult> GetAvailableDesks([FromBody] DeskAvailabilityRequestDto requestDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var desks = await _deskService.GetAvailableDesksAsync(requestDto);
                return Ok(desks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving available desks", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateDesk(int id, [FromBody] UpdateDeskDto dto)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { message = "Invalid desk ID" });

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var desk = await _deskService.UpdateDeskAsync(id, dto);
                return Ok(desk);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the desk", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteDesk(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { message = "Invalid desk ID" });

                var result = await _deskService.DeleteDeskAsync(id);
                return result ? NoContent() : NotFound(new { message = "Desk not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the desk", error = ex.Message });
            }
        }
    }
}

