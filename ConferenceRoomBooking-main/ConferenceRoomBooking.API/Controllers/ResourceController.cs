using ConferenceRoomBooking.Business.DTOs.Resource;
using ConferenceRoomBooking.Business.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceRoomBooking.API.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ResourceController : ControllerBase
    {
        private readonly IResourceService _resourceService;

        public ResourceController(IResourceService resourceService)
        {
            _resourceService = resourceService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateResource([FromBody] CreateResourceDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var resource = await _resourceService.CreateResourceAsync(dto);
                return CreatedAtAction(nameof(GetResourceById), new { id = resource.ResourceId }, resource);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the resource", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetResourceById(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { message = "Invalid resource ID" });

                var resource = await _resourceService.GetResourceByIdAsync(id);
                return resource == null ? NotFound(new { message = "Resource not found" }) : Ok(resource);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the resource", error = ex.Message });
            }
        }

        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetResourceWithDetails(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { message = "Invalid resource ID" });

                var resource = await _resourceService.GetResourceWithDetailsAsync(id);
                return resource == null ? NotFound(new { message = "Resource not found" }) : Ok(resource);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving resource details", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllResources()
        {
            try
            {
                var resources = await _resourceService.GetAllResourcesAsync();
                return Ok(resources);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving resources", error = ex.Message });
            }
        }

        [HttpGet("type")]
        public async Task<IActionResult> GetResourcesByType([FromQuery] ResourceTypeDto typeDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var resources = await _resourceService.GetResourcesByTypeAsync(typeDto);
                return Ok(resources);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving resources by type", error = ex.Message });
            }
        }

        [HttpGet("location/{locationId}")]
        public async Task<IActionResult> GetResourcesByLocation(int locationId)
        {
            try
            {
                if (locationId <= 0)
                    return BadRequest(new { message = "Invalid location ID" });

                var resources = await _resourceService.GetResourcesByLocationAsync(locationId);
                return Ok(resources);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving resources by location", error = ex.Message });
            }
        }

        [HttpGet("building/{buildingId}")]
        public async Task<IActionResult> GetResourcesByBuilding(int buildingId)
        {
            try
            {
                if (buildingId <= 0)
                    return BadRequest(new { message = "Invalid building ID" });

                var resources = await _resourceService.GetResourcesByBuildingAsync(buildingId);
                return Ok(resources);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving resources by building", error = ex.Message });
            }
        }

        [HttpGet("floor/{floorId}")]
        public async Task<IActionResult> GetResourcesByFloor(int floorId)
        {
            try
            {
                if (floorId <= 0)
                    return BadRequest(new { message = "Invalid floor ID" });

                var resources = await _resourceService.GetResourcesByFloorAsync(floorId);
                return Ok(resources);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving resources by floor", error = ex.Message });
            }
        }

        [HttpPost("available")]
        public async Task<IActionResult> GetAvailableResources([FromBody] ResourceAvailabilityRequestDto requestDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var resources = await _resourceService.GetAvailableResourcesAsync(requestDto);
                return Ok(resources);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving available resources", error = ex.Message });
            }
        }

        [HttpGet("maintenance")]
        public async Task<IActionResult> GetResourcesUnderMaintenance()
        {
            try
            {
                var resources = await _resourceService.GetResourcesUnderMaintenanceAsync();
                return Ok(resources);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving resources under maintenance", error = ex.Message });
            }
        }

        [HttpGet("blocked")]
        public async Task<IActionResult> GetBlockedResources()
        {
            try
            {
                var resources = await _resourceService.GetBlockedResourcesAsync();
                return Ok(resources);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving blocked resources", error = ex.Message });
            }
        }

        [HttpPost("check-availability")]
        public async Task<IActionResult> CheckResourceAvailability([FromBody] ResourceAvailabilityCheckDto checkDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var isAvailable = await _resourceService.IsResourceAvailableAsync(checkDto);
                return Ok(new { isAvailable });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while checking resource availability", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateResource(int id, [FromBody] UpdateResourceDto dto)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { message = "Invalid resource ID" });

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var resource = await _resourceService.UpdateResourceAsync(id, dto);
                return Ok(resource);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the resource", error = ex.Message });
            }
        }

        [HttpPut("{id}/maintenance")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateMaintenanceStatus(int id, [FromBody] MaintenanceStatusDto statusDto)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { message = "Invalid resource ID" });

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _resourceService.UpdateMaintenanceStatusAsync(id, statusDto);
                return result ? Ok(new { message = "Maintenance status updated successfully" }) : NotFound(new { message = "Resource not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating maintenance status", error = ex.Message });
            }
        }

        [HttpPost("{id}/block")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> BlockResource(int id, [FromBody] BlockResourceDto blockDto)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { message = "Invalid resource ID" });

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _resourceService.BlockResourceAsync(id, blockDto);
                return result ? Ok(new { message = "Resource blocked successfully" }) : NotFound(new { message = "Resource not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while blocking the resource", error = ex.Message });
            }
        }

        [HttpPost("{id}/unblock")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UnblockResource(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { message = "Invalid resource ID" });

                var result = await _resourceService.UnblockResourceAsync(id);
                return result ? Ok(new { message = "Resource unblocked successfully" }) : NotFound(new { message = "Resource not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while unblocking the resource", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteResource(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { message = "Invalid resource ID" });

                var result = await _resourceService.DeleteResourceAsync(id);
                return result ? NoContent() : NotFound(new { message = "Resource not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the resource", error = ex.Message });
            }
        }
    }
}

