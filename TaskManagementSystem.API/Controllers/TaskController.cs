using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagementSystem.Business.Interfaces;
using TaskManagementSystem.Core.DTOs;

namespace TaskManagementSystem.API.Controllers
{
    /// <summary>
    /// API endpoints for managing tasks.
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        /// <summary>
        /// Retrieves paginated list of tasks with optional ordering.
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin,RegularUser")]
        public async Task<IActionResult> GetTasks([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string orderBy = "CreatedDate")
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var isAdmin = User.IsInRole("Admin");
                var tasks = await _taskService.GetTasksAsync(userId, isAdmin, page, pageSize, orderBy);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving tasks.", details = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves a task by its identifier.
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,RegularUser")]
        public async Task<IActionResult> GetTask(Guid id)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var isAdmin = User.IsInRole("Admin");
                var task = await _taskService.GetTaskByIdAsync(id, userId, isAdmin);
                if (task == null)
                    return NotFound(new { message = "Task not found." });
                return Ok(task);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the task.", details = ex.Message });
            }
        }

        /// <summary>
        /// Creates a new task.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,RegularUser")]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto createTaskDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var task = await _taskService.CreateTaskAsync(createTaskDto, userId);
                return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the task.", details = ex.Message });
            }
        }

        /// <summary>
        /// Updates an existing task.
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,RegularUser")]
        public async Task<IActionResult> UpdateTask(Guid id, [FromBody] UpdateTaskDto updateTaskDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var isAdmin = User.IsInRole("Admin");
                var result = await _taskService.UpdateTaskAsync(id, updateTaskDto, userId, isAdmin);
                if (!result)
                    return NotFound(new { message = "Task not found." });
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the task.", details = ex.Message });
            }
        }

        /// <summary>
        /// Deletes a task.
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,RegularUser")]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var isAdmin = User.IsInRole("Admin");
                var result = await _taskService.DeleteTaskAsync(id, userId, isAdmin);
                if (!result)
                    return NotFound(new { message = "Task not found." });
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the task.", details = ex.Message });
            }
        }

        /// <summary>
        /// Searches tasks based on query parameters.
        /// </summary>
        [HttpGet("search")]
        [Authorize(Roles = "Admin,RegularUser")]
        public async Task<IActionResult> SearchTasks([FromQuery] string? title = null, [FromQuery] string? status = null, [FromQuery] string? priority = null)
        {
            try
            {
                var tasks = await _taskService.SearchTasksAsync(title, status, priority);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while searching for tasks.", details = ex.Message });
            }
        }
    }
}