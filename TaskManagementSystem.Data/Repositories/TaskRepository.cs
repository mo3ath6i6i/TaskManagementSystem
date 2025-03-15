using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using TaskManagementSystem.Core.Configurations;
using TaskManagementSystem.Core.Entities;
using TaskManagementSystem.Core.Enums;
using TaskStatus = TaskManagementSystem.Core.Enums.TaskStatus;

namespace TaskManagementSystem.Data.Repositories
{
    /// <summary>
    /// Implementation of task-specific repository operations.
    /// </summary>
    public class TaskRepository : ITaskRepository
    {
        private readonly AppDbContext _context;

        private readonly IMemoryCache _cache;

        private readonly CacheSettings _cacheSettings;

        public TaskRepository(AppDbContext context, IMemoryCache cache, IOptions<CacheSettings> cacheSettings)
        {
            _context = context;
            _cache = cache;
            _cacheSettings = cacheSettings.Value;
        }

        public async Task<IEnumerable<TaskItem>> GetAllAsync()
        {
            const string cacheKey = "GetAllTasks";

            if (!_cache.TryGetValue(cacheKey, out IEnumerable<TaskItem> tasks))
            {
                tasks = await _context.Tasks.AsNoTracking().ToListAsync();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(_cacheSettings.SlidingExpirationMinutes));
                _cache.Set(cacheKey, tasks, cacheEntryOptions);
            }

            return tasks ?? Enumerable.Empty<TaskItem>();
        }

        public IQueryable<TaskItem> GetAll()
        {
            return _context.Tasks.AsNoTracking();
        }

        public async Task<TaskItem> GetByIdAsync(Guid id)
        {
            string cacheKey = $"GetTaskById_{id}";

            if (!_cache.TryGetValue(cacheKey, out TaskItem task))
            {
                task = await _context.Tasks.FindAsync(id);

                if (task != null)
                {
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(_cacheSettings.SlidingExpirationMinutes));
                    _cache.Set(cacheKey, task, cacheEntryOptions);
                }
            }

            return task!;
        }

        public async Task AddAsync(TaskItem entity)
        {
            await _context.Tasks.AddAsync(entity);
            _cache.Remove("GetAllTasks"); // Invalidate cache
        }

        public void Update(TaskItem entity)
        {
            _context.Tasks.Update(entity);
            _cache.Remove("GetAllTasks"); // Invalidate cache
            _cache.Remove($"GetTaskById_{entity.Id}"); // Invalidate cache
        }

        public void Delete(TaskItem entity)
        {
            _context.Tasks.Remove(entity);
            _cache.Remove("GetAllTasks"); // Invalidate cache
            _cache.Remove($"GetTaskById_{entity.Id}"); // Invalidate cache
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<TaskItem>> SearchTasksAsync(string title, TaskStatus? status, TaskPriority? priority)
        {
            string cacheKey = $"SearchTasks_{title}_{status}_{priority}";

            if (!_cache.TryGetValue(cacheKey, out IEnumerable<TaskItem> tasks))
            {
                tasks = await _context.Tasks
                    .FromSqlRaw("EXEC usp_SearchTasks @p0, @p1, @p2", title, (int?)status, (int?)priority)
                    .ToListAsync();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(_cacheSettings.SlidingExpirationMinutes));
                _cache.Set(cacheKey, tasks, cacheEntryOptions);
            }

            return tasks ?? Enumerable.Empty<TaskItem>();
        }

        public async Task<IEnumerable<TaskItem>> GetTasksByStatusAsync(TaskStatus status, string userId, bool isAdmin)
        {
            string cacheKey = $"GetTasksByStatus_{status}_{userId}_{isAdmin}";

            if (!_cache.TryGetValue(cacheKey, out IEnumerable<TaskItem> tasks))
            {
                tasks = await _context.Tasks
                    .FromSqlRaw("EXEC GetTasksByStatus @Status, @UserId, @IsAdmin",
                        new SqlParameter("@Status", (int)status),
                        new SqlParameter("@UserId", userId),
                        new SqlParameter("@IsAdmin", isAdmin))
                    .AsNoTracking()
                    .ToListAsync();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(_cacheSettings.SlidingExpirationMinutes));
                _cache.Set(cacheKey, tasks, cacheEntryOptions);
            }

            return tasks ?? Enumerable.Empty<TaskItem>();
        }

        public async Task<IEnumerable<TaskItem>> GetTasksDueTodayAsync()
        {
            const string cacheKey = "GetTasksDueToday";

            if (!_cache.TryGetValue(cacheKey, out IEnumerable<TaskItem> tasks))
            {
                tasks = await _context.Tasks
                    .FromSqlRaw("EXEC usp_GetTasksDueToday")
                    .ToListAsync();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(_cacheSettings.SlidingExpirationMinutes));
                _cache.Set(cacheKey, tasks, cacheEntryOptions);
            }

            return tasks ?? Enumerable.Empty<TaskItem>();
        }

        public async Task<IEnumerable<(string UserId, int TaskCount)>> GetUserTaskCountsAsync()
        {
            const string cacheKey = "GetUserTaskCounts";

            if (!_cache.TryGetValue(cacheKey, out IEnumerable<(string UserId, int TaskCount)> results))
            {
                var rawResults = await _context.Database.ExecuteSqlRawAsync("EXEC usp_GetUserTaskCounts");
                // Note: In a real implementation, you'd map the results properly using a keyless entity or Dapper.
                // This is a placeholder to indicate the use of a stored procedure.
                results = new List<(string UserId, int TaskCount)>();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(_cacheSettings.SlidingExpirationMinutes));
                _cache.Set(cacheKey, results, cacheEntryOptions);
            }

            return results ?? Enumerable.Empty<(string UserId, int TaskCount)>();
        }
    }
}