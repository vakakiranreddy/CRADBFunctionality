using ConferenceRoomBooking.Business.DTOs.User;

namespace ConferenceRoomBooking.Business.Interfaces.IServices
{
    public interface IUserService
    {
        // User Management (Admin-only operations)
        Task<UserResponseDto?> GetUserByIdAsync(int id);
        Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();
        Task<IEnumerable<UserResponseDto>> GetUsersByLocationAsync(int locationId);
        Task<IEnumerable<UserResponseDto>> GetUsersByDepartmentAsync(int departmentId);
        Task<IEnumerable<UserResponseDto>> SearchUsersAsync(string keyword);
        Task<UserResponseDto> CreateUserAsync(CreateUserDto dto);   // Admin creates new employee
        Task UpdateUserAsync(int id, UpdateUserDto dto);
        Task UpdateUserRoleAsync(int id, UpdateUserRoleDto dto);   // Admin-only role update
        Task DeleteUserAsync(int userId);

        // Employee Profile (Self-access section)
        Task<UserProfileDto?> GetProfileAsync(int userId);

        Task UpdateProfileAsync(int userId, UpdateProfileDto dto);
        Task UploadProfileImageAsync(int userId, Microsoft.AspNetCore.Http.IFormFile image);

        // Validation Utilities
        Task<bool> IsEmailAvailableAsync(string email);
        Task<bool> IsEmployeeIdAvailableAsync(string employeeId);
    }

}









