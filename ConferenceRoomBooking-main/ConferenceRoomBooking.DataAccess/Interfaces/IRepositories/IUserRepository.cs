using ConferenceRoomBooking.DataAccess.Enum;
using ConferenceRoomBooking.DataAccess.Models;

namespace ConferenceRoomBooking.DataAccess.Interfaces.IRepositories
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByEmployeeIdAsync(string employeeId);

        //Filtering/Querying
        Task<IEnumerable<User>> GetByDepartmentAsync(int departmentId);
        Task<IEnumerable<User>> GetByLocationAsync(int locationId);
        Task<IEnumerable<User>> GetByRoleAsync(UserRole role);
        Task<IEnumerable<User>> SearchAsync(string keyword);

        // Utility Methods
        Task<bool> ExistsByEmailAsync(string email);
        Task<bool> ExistsByEmployeeIdAsync(string employeeId);
    }

}









