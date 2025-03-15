using TaskManagementSystem.Core.Enums;
using TaskStatus = TaskManagementSystem.Core.Enums.TaskStatus;

namespace TaskManagementSystem.Core.DTOs
{
    public class UpdateTaskDto
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public TaskStatus Status { get; set; }

        public TaskPriority Priority { get; set; }

        public DateTime DueDate { get; set; }
    }
}