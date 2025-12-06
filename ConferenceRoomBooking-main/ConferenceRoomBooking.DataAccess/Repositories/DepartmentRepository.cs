using ConferenceRoomBooking.DataAccess.Data;
using ConferenceRoomBooking.DataAccess.Interfaces.IRepositories;
using ConferenceRoomBooking.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace ConferenceRoomBooking.DataAccess.Repositories
{
    public class DepartmentRepository : BaseRepository<Department>, IDepartmentRepository
    {
        public DepartmentRepository(ConferenceRoomBookingDbContext context) : base(context) { }

        public async Task<Department?> GetByNameAsync(string departmentName)
        {
            return await _context.Departments
                .FirstOrDefaultAsync(d => d.DepartmentName.ToLower() == departmentName.ToLower());
        }

        public async Task<IEnumerable<Department>> GetActiveDepartmentsAsync()
        {
            return await _context.Departments
                .Where(d => d.IsActive)
                .OrderBy(d => d.DepartmentName)
                .ToListAsync();
        }
    }
}








