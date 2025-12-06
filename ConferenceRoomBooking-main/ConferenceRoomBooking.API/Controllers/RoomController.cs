using ConferenceRoomBooking.Business.DTOs.Room;
using ConferenceRoomBooking.Business.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceRoomBooking.API.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [RequestSizeLimit(5 * 1024 * 1024)] // 5MB limit
        public async Task<IActionResult> CreateRoom([FromForm] CreateRoomDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var room = await _roomService.CreateRoomAsync(dto);
                return CreatedAtAction(nameof(GetRoomById), new { id = room.RoomId }, room);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the room", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoomById(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { message = "Invalid room ID" });

                var room = await _roomService.GetRoomByIdAsync(id);
                return room == null ? NotFound(new { message = "Room not found" }) : Ok(room);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the room", error = ex.Message });
            }
        }

        [HttpGet("resource/{resourceId}")]
        public async Task<IActionResult> GetRoomByResourceId(int resourceId)
        {
            try
            {
                if (resourceId <= 0)
                    return BadRequest(new { message = "Invalid resource ID" });

                var room = await _roomService.GetRoomByResourceIdAsync(resourceId);
                return room == null ? NotFound(new { message = "Room not found for the given resource" }) : Ok(room);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the room by resource ID", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRooms()
        {
            try
            {
                var rooms = await _roomService.GetAllRoomsAsync();
                return Ok(rooms);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving rooms", error = ex.Message });
            }
        }

        [HttpGet("location/{locationId}")]
        public async Task<IActionResult> GetRoomsByLocation(int locationId)
        {
            try
            {
                if (locationId <= 0)
                    return BadRequest(new { message = "Invalid location ID" });

                var rooms = await _roomService.GetRoomsByLocationAsync(locationId);
                return Ok(rooms);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving rooms by location", error = ex.Message });
            }
        }

        [HttpGet("building/{buildingId}")]
        public async Task<IActionResult> GetRoomsByBuilding(int buildingId)
        {
            try
            {
                if (buildingId <= 0)
                    return BadRequest(new { message = "Invalid building ID" });

                var rooms = await _roomService.GetRoomsByBuildingAsync(buildingId);
                return Ok(rooms);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving rooms by building", error = ex.Message });
            }
        }

        [HttpGet("floor/{floorId}")]
        public async Task<IActionResult> GetRoomsByFloor(int floorId)
        {
            try
            {
                if (floorId <= 0)
                    return BadRequest(new { message = "Invalid floor ID" });

                var rooms = await _roomService.GetRoomsByFloorAsync(floorId);
                return Ok(rooms);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving rooms by floor", error = ex.Message });
            }
        }

        [HttpGet("capacity/{minCapacity}")]
        public async Task<IActionResult> GetRoomsByCapacity(int minCapacity)
        {
            try
            {
                if (minCapacity <= 0)
                    return BadRequest(new { message = "Invalid capacity" });

                var rooms = await _roomService.GetRoomsByCapacityAsync(minCapacity);
                return Ok(rooms);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving rooms by capacity", error = ex.Message });
            }
        }

        [HttpPost("search-amenities")]
        public async Task<IActionResult> SearchRoomsWithAmenities([FromBody] RoomAmenityFilterDto filterDto)
        {
            try
            {
                var rooms = await _roomService.SearchRoomsWithAmenitiesAsync(filterDto);
                return Ok(rooms);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while searching rooms with amenities", error = ex.Message });
            }
        }

        [HttpPost("available")]
        public async Task<IActionResult> GetAvailableRooms([FromBody] RoomAvailabilityRequestDto requestDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var rooms = await _roomService.GetAvailableRoomsAsync(requestDto);
                return Ok(rooms);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving available rooms", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [RequestSizeLimit(5 * 1024 * 1024)] // 5MB limit
        public async Task<IActionResult> UpdateRoom(int id, [FromForm] UpdateRoomDto dto)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { message = "Invalid room ID" });

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var room = await _roomService.UpdateRoomAsync(id, dto);
                return Ok(room);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the room", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { message = "Invalid room ID" });

                var result = await _roomService.DeleteRoomAsync(id);
                return result ? NoContent() : NotFound(new { message = "Room not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the room", error = ex.Message });
            }
        }
    }
}

