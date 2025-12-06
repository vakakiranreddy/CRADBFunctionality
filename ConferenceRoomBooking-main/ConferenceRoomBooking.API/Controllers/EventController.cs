using ConferenceRoomBooking.Business.DTOs.Event;
using ConferenceRoomBooking.Business.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceRoomBooking.API.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [RequestSizeLimit(3 * 1024 * 1024)] // 3MB limit
        public async Task<IActionResult> CreateEvent([FromForm] CreateEventDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var eventResponse = await _eventService.CreateEventAsync(dto);
                return CreatedAtAction(nameof(GetEventById), new { id = eventResponse.EventId }, eventResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the event", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [RequestSizeLimit(3 * 1024 * 1024)] // 3MB limit
        public async Task<IActionResult> UpdateEvent(int id, [FromForm] UpdateEventDto dto)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { message = "Invalid event ID" });

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var eventResponse = await _eventService.UpdateEventAsync(id, dto);
                return Ok(eventResponse);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the event", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { message = "Invalid event ID" });

                var result = await _eventService.DeleteEventAsync(id);
                return result ? NoContent() : NotFound(new { message = "Event not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the event", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventById(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { message = "Invalid event ID" });

                var eventResponse = await _eventService.GetEventByIdAsync(id);
                return eventResponse == null ? NotFound(new { message = "Event not found" }) : Ok(eventResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the event", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEvents()
        {
            try
            {
                var events = await _eventService.GetAllEventsAsync();
                return Ok(events);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving events", error = ex.Message });
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchEvents([FromQuery] string keyword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                    return BadRequest(new { message = "Keyword is required" });

                var events = await _eventService.SearchEventsAsync(keyword);
                return Ok(events);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while searching events", error = ex.Message });
            }
        }

        [HttpGet("filter/location")]
        public async Task<IActionResult> FilterEventsByLocationName([FromQuery] string locationName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(locationName))
                    return BadRequest(new { message = "Location name is required" });

                var events = await _eventService.FilterEventsByLocationNameAsync(locationName);
                return Ok(events);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while filtering events by location", error = ex.Message });
            }
        }

        [HttpGet("upcoming")]
        public async Task<IActionResult> GetUpcomingEvents()
        {
            try
            {
                var events = await _eventService.GetUpcomingEventsAsync();
                return Ok(events);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving upcoming events", error = ex.Message });
            }
        }

        [HttpGet("past")]
        public async Task<IActionResult> GetPastEvents()
        {
            try
            {
                var events = await _eventService.GetPastEventsAsync();
                return Ok(events);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving past events", error = ex.Message });
            }
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetTotalEventCount()
        {
            try
            {
                var count = await _eventService.GetTotalEventCountAsync();
                return Ok(new { totalEvents = count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving event count", error = ex.Message });
            }
        }

        [HttpGet("{id}/participants/count")]
        public async Task<IActionResult> GetEventParticipantCount(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { message = "Invalid event ID" });

                var count = await _eventService.GetEventParticipantCountAsync(id);
                return Ok(new { eventId = id, participantCount = count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving participant count", error = ex.Message });
            }
        }
    }
}

