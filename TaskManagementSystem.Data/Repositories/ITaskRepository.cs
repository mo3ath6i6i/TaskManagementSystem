using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementSystem.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagementSystem.Core.Enums;
using TaskStatus = TaskManagementSystem.Core.Enums.TaskStatus;

namespace TaskManagementSystem.Data.Repositories
{
    public interface ITaskRepository : IRepository<TaskItem>
    {
        /// <summary>
        /// Searches tasks based on title, status, and priority using a stored procedure.
        /// </summary>
        Task<IEnumerable<TaskItem>> SearchTasksAsync(string title, TaskStatus? status, TaskManagementSystem.Core.Enums.TaskPriority? priority);

        /// <summary>
        /// Gets tasks by status for a specific user role using stored procedure.
        /// </summary>
        Task<IEnumerable<TaskItem>> GetTasksByStatusAsync(TaskStatus status, string userId, bool isAdmin);

        /// <summary>
        /// Retrieves all tasks that are due today using stored procedure.
        /// </summary>
        Task<IEnumerable<TaskItem>> GetTasksDueTodayAsync();

        /// <summary>
        /// Returns task counts per user (Admin only) using stored procedure.
        /// </summary>
        Task<IEnumerable<(string UserId, int TaskCount)>> GetUserTaskCountsAsync();
    }
}
