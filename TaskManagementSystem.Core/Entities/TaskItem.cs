using System.ComponentModel.DataAnnotations;
using TaskManagementSystem.Core.Enums;
using TaskStatus = TaskManagementSystem.Core.Enums.TaskStatus;

namespace TaskManagementSystem.Core.Entities
{
    public class TaskItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [MaxLength(200)]
        public string Title { get; set; }

        public string Description { get; set; }

        public TaskStatus Status { get; set; } = TaskStatus.Pending;

        public TaskPriority Priority { get; set; } = TaskPriority.Medium;

        public DateTime DueDate { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

        public string UserId { get; set; }
    }
}