using TaskManagementSystem.Core.Entities;

namespace TaskManagementSystem.Data.Repositories
{
    /// <summary>
    /// Generic repository interface for CRUD operations.
    /// </summary>
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();

        IQueryable<TaskItem> GetAll();

        Task<T> GetByIdAsync(Guid id);

        Task AddAsync(T entity);

        void Update(T entity);

        void Delete(T entity);

        Task SaveChangesAsync();

    }
}