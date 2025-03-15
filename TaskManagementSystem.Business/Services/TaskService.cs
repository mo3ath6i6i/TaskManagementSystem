using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Business.Interfaces;
using TaskManagementSystem.Core.DTOs;
using TaskManagementSystem.Core.Entities;
using TaskManagementSystem.Core.Enums;
using TaskManagementSystem.Data.Repositories;
using TaskStatus = TaskManagementSystem.Core.Enums.TaskStatus;

namespace TaskManagementSystem.Business.Services
{
    /// <summary>
    /// Provides business logic for managing tasks.
    /// </summary>
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;

        private readonly IMapper _mapper;

        public TaskService(ITaskRepository taskRepository, IMapper mapper)
        {
            _taskRepository = taskRepository;
            _mapper = mapper;
        }

        private IQueryable<TaskItem> RetrieveTasks(string userId, bool isAdmin)
        {
            return isAdmin ? _taskRepository.GetAll() : _taskRepository.GetAll().Where(t => t.UserId == userId);
        }

        private IQueryable<TaskItem> OrderTasks(IQueryable<TaskItem> tasks, string orderBy)
        {
            return orderBy switch
            {
                "Priority" => tasks.OrderBy(t => t.Priority),
                "DueDate" => tasks.OrderBy(t => t.DueDate),
                _ => tasks.OrderBy(t => t.CreatedDate)
            };
        }

        private async Task<IEnumerable<TaskDto>> PaginateTasksAsync(IQueryable<TaskItem> tasks, int page, int pageSize)
        {
            var pagedTasks = await tasks.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return _mapper.Map<IEnumerable<TaskDto>>(pagedTasks);
        }

        /// <summary>
        /// Retrieves paginated tasks with optional ordering.
        /// </summary>
        public async Task<IEnumerable<TaskDto>> GetTasksAsync(string userId, bool isAdmin, int page, int pageSize, string orderBy = "CreatedDate")
        {
            var tasks = RetrieveTasks(userId, isAdmin);
            tasks = OrderTasks(tasks, orderBy);
            return await PaginateTasksAsync(tasks, page, pageSize);
        }

        /// <summary>
        /// Retrieves a single task by id ensuring proper authorization.
        /// </summary>
        public async Task<TaskDto> GetTaskByIdAsync(Guid id, string userId, bool isAdmin)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null || (!isAdmin && task.UserId != userId))
                return null;

            return _mapper.Map<TaskDto>(task);
        }

        /// <summary>
        /// Creates a new task for the given user.
        /// </summary>
        public async Task<TaskDto> CreateTaskAsync(CreateTaskDto createTaskDto, string userId)
        {
            var newTask = _mapper.Map<TaskItem>(createTaskDto);
            newTask.UserId = userId;
            await _taskRepository.AddAsync(newTask);
            await _taskRepository.SaveChangesAsync();

            return _mapper.Map<TaskDto>(newTask);
        }

        /// <summary>
        /// Updates an existing task after validating user access.
        /// </summary>
        public async Task<bool> UpdateTaskAsync(Guid id, UpdateTaskDto updateTaskDto, string userId, bool isAdmin)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null || (!isAdmin && task.UserId != userId))
                return false;

            _mapper.Map(updateTaskDto, task);
            task.UpdatedDate = DateTime.UtcNow;

            _taskRepository.Update(task);
            await _taskRepository.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Deletes a task if the user is authorized.
        /// </summary>
        public async Task<bool> DeleteTaskAsync(Guid id, string userId, bool isAdmin)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null || (!isAdmin && task.UserId != userId))
                return false;

            _taskRepository.Delete(task);
            await _taskRepository.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Searches tasks using the stored procedure integration.
        /// </summary>
        public async Task<IEnumerable<TaskDto>> SearchTasksAsync(string title, string status, string priority)
        {
            TaskStatus? taskStatus = null;
            TaskPriority? taskPriority = null;

            if (Enum.TryParse<TaskStatus>(status, true, out var parsedStatus))
                taskStatus = parsedStatus;
            if (Enum.TryParse<TaskPriority>(priority, true, out var parsedPriority))
                taskPriority = parsedPriority;

            var tasks = await _taskRepository.SearchTasksAsync(title, taskStatus, taskPriority);

            return _mapper.Map<IEnumerable<TaskDto>>(tasks);
        }
    }
}