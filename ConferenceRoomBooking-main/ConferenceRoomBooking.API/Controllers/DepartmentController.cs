using ConferenceRoomBooking.Business.DTOs.Department;
using ConferenceRoomBooking.Business.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceRoomBooking.API.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateDepartment([FromBody] CreateDepartmentDto dto)
        {
            try
            {
                var result = await _departmentService.CreateDepartmentAsync(dto);
                return result == null ? 
                    BadRequest(new { message = "Department already exists" }) : 
                    Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDepartment(int id)
        {
            try
            {
                var department = await _departmentService.GetDepartmentByIdAsync(id);
                return department == null ? NotFound() : Ok(department);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDepartments()
        {
            try
            {
                var departments = await _departmentService.GetAllDepartmentsAsync();
                return Ok(departments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveDepartments()
        {
            try
            {
                var departments = await _departmentService.GetActiveDepartmentsAsync();
                return Ok(departments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDepartment(int id, [FromBody] UpdateDepartmentDto dto)
        {
            try
            {
                var result = await _departmentService.UpdateDepartmentAsync(id, dto);
                return result == null ? NotFound() : Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            try
            {
                var result = await _departmentService.DeleteDepartmentAsync(id);
                return result ? Ok(new { message = "Department deleted successfully" }) : NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }
    }
}

