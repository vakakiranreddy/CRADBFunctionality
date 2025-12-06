using ConferenceRoomBooking.DataAccess.Models;

namespace ConferenceRoomBooking.DataAccess.Interfaces.IRepositories
{
    public interface IDepartmentRepository : IBaseRepository<Department>
    {
        Task<Department?> GetByNameAsync(string departmentName);
        Task<IEnumerable<Department>> GetActiveDepartmentsAsync();
    }
}








