using ConferenceRoomBooking.Business.DTOs.User;
using ConferenceRoomBooking.Business.Helpers;
using ConferenceRoomBooking.DataAccess.Interfaces.IRepositories;
using ConferenceRoomBooking.Business.Interfaces.IServices;
using ConferenceRoomBooking.DataAccess.Models;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace ConferenceRoomBooking.Business.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserNotificationService _notificationService;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, IUserNotificationService notificationService, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task<UserResponseDto?> GetUserByIdAsync(int id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                return user == null ? null : MapToResponseDto(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user with ID {UserId}", id);
                throw new InvalidOperationException($"Failed to retrieve user with ID {id}", ex);
            }
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<UserResponseDto>> GetUsersByLocationAsync(int locationId)
        {
            var users = await _userRepository.GetByLocationAsync(locationId);
            return users.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<UserResponseDto>> GetUsersByDepartmentAsync(int departmentId)
        {
            var users = await _userRepository.GetByDepartmentAsync(departmentId);
            return users.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<UserResponseDto>> SearchUsersAsync(string keyword)
        {
            var users = await _userRepository.SearchAsync(keyword);
            return users.Select(MapToResponseDto);
        }

        public async Task<UserResponseDto> CreateUserAsync(CreateUserDto dto)
        {
            CreatePasswordHash(dto.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var user = new User
            {
                EmployeeId = dto.EmployeeId,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                LocationId = dto.LocationId,
                DepartmentId = dto.DepartmentId,
                Title = dto.Title,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            if (dto.ProfileImage != null)
            {
                if (!ImageHelper.IsValidImageFile(dto.ProfileImage))
                {
                    _logger.LogWarning("Invalid image file uploaded for new user");
                    throw new ArgumentException("Invalid image file. Please upload a valid image (JPG, PNG, GIF, BMP) under 5MB");
                }
                user.ProfileImage = await ImageHelper.ConvertToByteArrayAsync(dto.ProfileImage);
            }

            var createdUser = await _userRepository.AddAsync(user);
            
            // Send welcome email with credentials
            await SendWelcomeEmailAsync(createdUser, dto.Password);
            
            return MapToResponseDto(createdUser);
        }

        private async Task SendWelcomeEmailAsync(User user, string password)
        {
            var subject = "Welcome to Conference Room Booking System";
            var body = $@"Dear {user.FirstName} {user.LastName},

Welcome to the Conference Room Booking System!

Your login credentials:
Email: {user.Email}
Password: {password}

Please login and change your password for security.

Best regards,
Conference Room Booking Team";
            
            await _notificationService.SendNotificationAsync(new DTOs.UserNotification.SendNotificationDto
            {
                UserId = user.Id,
                Title = subject,
                Message = body,
                Type = ConferenceRoomBooking.DataAccess.Enum.NotificationType.Welcome
            });
        }

        public async Task UpdateUserAsync(int id, UpdateUserDto dto)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("Attempted to update non-existent user with ID {UserId}", id);
                    throw new ArgumentException($"User with ID {id} not found");
                }

                user.FirstName = dto.FirstName;
                user.LastName = dto.LastName;
                user.Email = dto.Email;
                user.PhoneNumber = dto.PhoneNumber;
                user.LocationId = dto.LocationId;
                user.DepartmentId = dto.DepartmentId;
                user.Title = dto.Title;
                user.IsActive = dto.IsActive;

                if (dto.ProfileImage != null)
                {
                    if (!ImageHelper.IsValidImageFile(dto.ProfileImage))
                    {
                        _logger.LogWarning("Invalid image file uploaded for user {UserId}", id);
                        throw new ArgumentException("Invalid image file. Please upload a valid image (JPG, PNG, GIF, BMP) under 5MB");
                    }
                    user.ProfileImage = await ImageHelper.ConvertToByteArrayAsync(dto.ProfileImage);
                }

                user.UpdatedAt = DateTime.UtcNow;

                await _userRepository.UpdateAsync(user);
                _logger.LogInformation("Successfully updated user with ID {UserId}", id);
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user with ID {UserId}", id);
                throw new InvalidOperationException($"Failed to update user with ID {id}", ex);
            }
        }

        public async Task UpdateUserRoleAsync(int id, UpdateUserRoleDto dto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) throw new ArgumentException("User not found");

            user.Role = dto.Role;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
        }

        public async Task DeleteUserAsync(int userId)
        {
            await _userRepository.DeleteAsync(userId);
        }

        public async Task<UserProfileDto?> GetProfileAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            return user == null ? null : MapToProfileDto(user);
        }

        public async Task UpdateProfileAsync(int userId, UpdateProfileDto dto)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("Attempted to update profile for non-existent user with ID {UserId}", userId);
                    throw new ArgumentException($"User with ID {userId} not found");
                }

                user.FirstName = dto.FirstName;
                user.LastName = dto.LastName;
                user.PhoneNumber = dto.PhoneNumber;
                user.Title = dto.Title;
                
                if (dto.ProfileImage != null)
                {
                    if (!ImageHelper.IsValidImageFile(dto.ProfileImage))
                    {
                        _logger.LogWarning("Invalid image file uploaded for user {UserId}", userId);
                        throw new ArgumentException("Invalid image file. Please upload a valid image (JPG, PNG, GIF, BMP) under 5MB");
                    }
                    user.ProfileImage = await ImageHelper.ConvertToByteArrayAsync(dto.ProfileImage);
                }
                
                user.UpdatedAt = DateTime.UtcNow;

                await _userRepository.UpdateAsync(user);
                _logger.LogInformation("Successfully updated profile for user with ID {UserId}", userId);
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile for user with ID {UserId}", userId);
                throw new InvalidOperationException($"Failed to update profile for user with ID {userId}", ex);
            }
        }

        public async Task<bool> IsEmailAvailableAsync(string email)
        {
            return !await _userRepository.ExistsByEmailAsync(email);
        }

        public async Task<bool> IsEmployeeIdAvailableAsync(string employeeId)
        {
            return !await _userRepository.ExistsByEmployeeIdAsync(employeeId);
        }

        private static UserResponseDto MapToResponseDto(User user)
        {
            return new UserResponseDto
            {
                Id = user.Id,
                EmployeeId = user.EmployeeId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                LocationId = user.LocationId,
                DepartmentId = user.DepartmentId,
                Title = user.Title,
                Role = user.Role.ToString(),
                IsActive = user.IsActive,
                LastLoginAt = user.LastLoginAt,
                CreatedAt = user.CreatedAt
            };
        }

        private static UserProfileDto MapToProfileDto(User user)
        {
            return new UserProfileDto
            {
                Id = user.Id,
                EmployeeId = user.EmployeeId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                LocationId = user.LocationId,
                DepartmentId = user.DepartmentId,
                Title = user.Title,
                Role= user.Role.ToString(),
                IsActive = user.IsActive,
                ProfileImage = ImageHelper.ConvertToBase64String(user.ProfileImage),
                LastLoginAt = user.LastLoginAt,
                CreatedAt = user.CreatedAt
            };
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
    }
}








