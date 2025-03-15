using AutoMapper;
using Moq;
using TaskManagementSystem.Business.Mappings;
using TaskManagementSystem.Business.Services;
using TaskManagementSystem.Core.DTOs;
using TaskManagementSystem.Core.Entities;
using TaskManagementSystem.Core.Enums;
using TaskManagementSystem.Data.Repositories;
using TaskStatus = TaskManagementSystem.Core.Enums.TaskStatus;

namespace TaskManagementSystem.Tests.Services
{
    public class TaskServiceTests
    {
        public const string UserId = "user1";

        public const string TaskTitle = "Test Task";

        public const string NewTaskTitle = "New Task";

        public const string UpdatedTaskTitle = "Updated Task";

        public const string TaskDescription = "Task Description";

        public const string UpdatedTaskDescription = "Updated Description";

        public static readonly DateTime DueDate = DateTime.UtcNow.AddDays(1);

        public static readonly DateTime UpdatedDueDate = DateTime.UtcNow.AddDays(2);

        public static readonly TaskPriority TaskPriority = TaskPriority.Medium;

        public static readonly TaskPriority UpdatedTaskPriority = TaskPriority.High;

        public static readonly TaskStatus TaskStatus = TaskStatus.Pending;

        public static readonly TaskStatus UpdatedTaskStatus = TaskStatus.Completed;

        private readonly Mock<ITaskRepository> _taskRepositoryMock;

        private readonly TaskService _taskService;

        private readonly IMapper _mapper;

        public TaskServiceTests()
        {
            _taskRepositoryMock = new Mock<ITaskRepository>();

            var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
            _mapper = config.CreateMapper();

            _taskService = new TaskService(_taskRepositoryMock.Object, _mapper);
        }

        [Fact]
        public async Task GetTasksAsync_ShouldReturnTasks_WhenCalled()
        {
            // Arrange
            var tasks = new List<TaskItem>
            {
                new TaskItem { Id = Guid.NewGuid(), Title = TaskTitle, UserId = UserId, CreatedDate = DateTime.UtcNow }
            };
            _taskRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(tasks);

            // Act
            var result = await _taskService.GetTasksAsync(UserId, true, 1, 10);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetTaskByIdAsync_ShouldReturnTask_WhenTaskExists()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var task = new TaskItem { Id = taskId, Title = TaskTitle, UserId = UserId };
            _taskRepositoryMock.Setup(repo => repo.GetByIdAsync(taskId)).ReturnsAsync(task);

            // Act
            var result = await _taskService.GetTaskByIdAsync(taskId, UserId, true);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(taskId, result.Id);
        }

        [Fact]
        public async Task CreateTaskAsync_ShouldReturnCreatedTask_WhenCalled()
        {
            // Arrange
            var createTaskDto = new CreateTaskDto { Title = NewTaskTitle, Description = TaskDescription, Priority = TaskPriority, DueDate = DueDate };

            // Act
            var result = await _taskService.CreateTaskAsync(createTaskDto, UserId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(NewTaskTitle, result.Title);
        }

        [Fact]
        public async Task UpdateTaskAsync_ShouldReturnTrue_WhenTaskIsUpdated()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var updateTaskDto = new UpdateTaskDto { Title = UpdatedTaskTitle, Description = UpdatedTaskDescription, Status = UpdatedTaskStatus, Priority = UpdatedTaskPriority, DueDate = UpdatedDueDate };
            var task = new TaskItem { Id = taskId, UserId = UserId };
            _taskRepositoryMock.Setup(repo => repo.GetByIdAsync(taskId)).ReturnsAsync(task);

            // Act
            var result = await _taskService.UpdateTaskAsync(taskId, updateTaskDto, UserId, true);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteTaskAsync_ShouldReturnTrue_WhenTaskIsDeleted()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var task = new TaskItem { Id = taskId, UserId = UserId };
            _taskRepositoryMock.Setup(repo => repo.GetByIdAsync(taskId)).ReturnsAsync(task);

            // Act
            var result = await _taskService.DeleteTaskAsync(taskId, UserId, true);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task SearchTasksAsync_ShouldReturnTasks_WhenCalled()
        {
            // Arrange
            var tasks = new List<TaskItem>
            {
                new TaskItem { Id = Guid.NewGuid(), Title = TaskTitle, Status = TaskStatus, Priority = TaskPriority }
            };
            _taskRepositoryMock.Setup(repo => repo.SearchTasksAsync(It.IsAny<string>(), It.IsAny<TaskStatus?>(), It.IsAny<TaskPriority?>())).ReturnsAsync(tasks);

            // Act
            var result = await _taskService.SearchTasksAsync("Test", "Pending", "Medium");

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetTasksAsync_ShouldReturnPaginatedTasks_WhenCalled()
        {
            // Arrange
            var tasks = new List<TaskItem>
            {
                new TaskItem { Id = Guid.NewGuid(), Title = TaskTitle, UserId = UserId, CreatedDate = DateTime.UtcNow },
                new TaskItem { Id = Guid.NewGuid(), Title = TaskTitle, UserId = UserId, CreatedDate = DateTime.UtcNow }
            }.AsQueryable();

            _taskRepositoryMock.Setup(repo => repo.GetAll()).Returns(tasks);

            // Act
            var result = await _taskService.GetTasksAsync(UserId, true, 1, 1);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetTasksAsync_ShouldReturnEmpty_WhenNoTasksExist()
        {
            // Arrange
            var tasks = new List<TaskItem>().AsQueryable();
            _taskRepositoryMock.Setup(repo => repo.GetAll()).Returns(tasks);

            // Act
            var result = await _taskService.GetTasksAsync(UserId, true, 1, 1);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetTasksAsync_ShouldReturnEmpty_WhenPageNumberIsInvalid()
        {
            // Arrange
            var tasks = new List<TaskItem>
            {
                new TaskItem { Id = Guid.NewGuid(), Title = TaskTitle, UserId = UserId, CreatedDate = DateTime.UtcNow }
            }.AsQueryable();

            _taskRepositoryMock.Setup(repo => repo.GetAll()).Returns(tasks);

            // Act
            var result = await _taskService.GetTasksAsync(UserId, true, 2, 1);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}