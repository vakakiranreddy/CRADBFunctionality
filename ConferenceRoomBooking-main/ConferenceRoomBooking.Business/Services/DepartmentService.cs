using ConferenceRoomBooking.Business.DTOs.Department;
using ConferenceRoomBooking.DataAccess.Interfaces.IRepositories;
using ConferenceRoomBooking.Business.Interfaces.IServices;
using ConferenceRoomBooking.DataAccess.Models;

namespace ConferenceRoomBooking.Business.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;

        public DepartmentService(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        public async Task<DepartmentResponseDto?> CreateDepartmentAsync(CreateDepartmentDto dto)
        {
            var existingDepartment = await _departmentRepository.GetByNameAsync(dto.DepartmentName);
            if (existingDepartment != null)
                return null;

            var department = new Department
            {
                DepartmentName = dto.DepartmentName,
                Description = dto.Description
            };

            await _departmentRepository.AddAsync(department);

            return MapToResponseDto(department);
        }

        public async Task<DepartmentResponseDto?> GetDepartmentByIdAsync(int id)
        {
            var department = await _departmentRepository.GetByIdAsync(id);
            return department == null ? null : MapToResponseDto(department);
        }

        public async Task<IEnumerable<DepartmentResponseDto>> GetAllDepartmentsAsync()
        {
            var departments = await _departmentRepository.GetAllAsync();
            return departments.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<DepartmentResponseDto>> GetActiveDepartmentsAsync()
        {
            var departments = await _departmentRepository.GetActiveDepartmentsAsync();
            return departments.Select(MapToResponseDto);
        }

        public async Task<DepartmentResponseDto?> UpdateDepartmentAsync(int id, UpdateDepartmentDto dto)
        {
            var department = await _departmentRepository.GetByIdAsync(id);
            if (department == null) return null;

            department.DepartmentName = dto.DepartmentName;
            department.Description = dto.Description;
            department.IsActive = dto.IsActive;
            department.UpdatedAt = DateTime.UtcNow;

            await _departmentRepository.UpdateAsync(department);
            return MapToResponseDto(department);
        }

        public async Task<bool> DeleteDepartmentAsync(int id)
        {
            var department = await _departmentRepository.GetByIdAsync(id);
            if (department == null) return false;

            await _departmentRepository.DeleteAsync(id);
            return true;
        }

        private static DepartmentResponseDto MapToResponseDto(Department department)
        {
            return new DepartmentResponseDto
            {
                DepartmentId = department.DepartmentId,
                DepartmentName = department.DepartmentName,
                Description = department.Description,
                IsActive = department.IsActive,
                CreatedAt = department.CreatedAt,
                UpdatedAt = department.UpdatedAt
            };
        }
    }
}








