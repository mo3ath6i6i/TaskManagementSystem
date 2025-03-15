using TaskManagementSystem.Core.DTOs;

namespace TaskManagementSystem.Business.Interfaces
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskDto>> GetTasksAsync(string userId, bool isAdmin, int page, int pageSize, string orderBy = "CreatedDate");

        Task<TaskDto> GetTaskByIdAsync(Guid id, string userId, bool isAdmin);

        Task<TaskDto> CreateTaskAsync(CreateTaskDto createTaskDto, string userId);

        Task<bool> UpdateTaskAsync(Guid id, UpdateTaskDto updateTaskDto, string userId, bool isAdmin);

        Task<bool> DeleteTaskAsync(Guid id, string userId, bool isAdmin);

        Task<IEnumerable<TaskDto>> SearchTasksAsync(string title, string status, string priority);
    }
}