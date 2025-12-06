using ConferenceRoomBooking.DataAccess.Data;
using ConferenceRoomBooking.DataAccess.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ConferenceRoomBooking.DataAccess.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly ConferenceRoomBookingDbContext _context;
        protected readonly ILogger<BaseRepository<T>> _logger;

        public BaseRepository(ConferenceRoomBookingDbContext context, ILogger<BaseRepository<T>>? logger = null)
        {
            _context = context;
            _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<BaseRepository<T>>.Instance;
        }

        public async Task<T> AddAsync(T entity)
        {
            try
            {
                await _context.Set<T>().AddAsync(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error occurred while adding entity");
                throw new InvalidOperationException("Failed to add entity to database", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while adding entity");
                throw;
            }
        }

        public async virtual Task<T?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<T>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving entity with ID {Id}", id);
                return null;
            }
        }

        public async virtual Task<IEnumerable<T>> GetAllAsync()
        {
            try
            {
                return await _context.Set<T>().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all entities");
                return new List<T>();
            }
        }

        public async Task UpdateAsync(T entity)
        {
            try
            {
                _context.Set<T>().Update(entity);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency error occurred while updating entity");
                throw new InvalidOperationException("Entity was modified by another process", ex);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error occurred while updating entity");
                throw new InvalidOperationException("Failed to update entity in database", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while updating entity");
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var entity = await GetByIdAsync(id);
                if (entity != null)
                {
                    _context.Set<T>().Remove(entity);
                    await _context.SaveChangesAsync();
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error occurred while deleting entity with ID {Id}", id);
                throw new InvalidOperationException($"Failed to delete entity with ID {id}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while deleting entity with ID {Id}", id);
                throw;
            }
        }
    }
}








