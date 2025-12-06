using ConferenceRoomBooking.DataAccess.Data;
using ConferenceRoomBooking.DataAccess.Enum;
using ConferenceRoomBooking.DataAccess.Interfaces.IRepositories;
using ConferenceRoomBooking.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace ConferenceRoomBooking.DataAccess.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(ConferenceRoomBookingDbContext context) : base(context) { }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetByEmployeeIdAsync(string employeeId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.EmployeeId == employeeId);
        }

        public async Task<IEnumerable<User>> GetByDepartmentAsync(int departmentId)
        {
            return await _context.Users.Where(u => u.DepartmentId == departmentId).ToListAsync();
        }

        public async Task<IEnumerable<User>> GetByLocationAsync(int locationId)
        {
            return await _context.Users.Where(u => u.LocationId == locationId).ToListAsync();
        }

        public async Task<IEnumerable<User>> GetByRoleAsync(UserRole role)
        {
            return await _context.Users.Where(u => u.Role == role).ToListAsync();
        }

        public async Task<IEnumerable<User>> SearchAsync(string keyword)
        {
            return await _context.Users.Where(u => u.FirstName.Contains(keyword) || u.LastName.Contains(keyword) || u.Email.Contains(keyword)).ToListAsync();
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> ExistsByEmployeeIdAsync(string employeeId)
        {
            return await _context.Users.AnyAsync(u => u.EmployeeId == employeeId);
        }
    }
}








