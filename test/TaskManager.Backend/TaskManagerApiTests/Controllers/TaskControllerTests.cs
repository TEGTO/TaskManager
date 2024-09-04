using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using TaskManagerApi.Domain.Dtos.Task;
using TaskManagerApi.Domain.Entities;
using TaskManagerApi.Enums;
using TaskManagerApi.Services;
using Task = System.Threading.Tasks.Task;
using UserTask = TaskManagerApi.Domain.Entities.Task;

namespace TaskManagerApi.Controllers.Tests
{
    [TestFixture]
    internal class TaskControllerTests
    {
        private Mock<ITaskService> mockTaskService;
        private Mock<IAuthService> mockAuthService;
        private Mock<IMapper> mockMapper;
        private Mock<ILogger<TaskController>> mockLogger;
        private TaskController controller;

        [SetUp]
        public void SetUp()
        {
            mockTaskService = new Mock<ITaskService>();
            mockAuthService = new Mock<IAuthService>();
            mockMapper = new Mock<IMapper>();
            mockLogger = new Mock<ILogger<TaskController>>();
            controller = new TaskController(mockTaskService.Object, mockAuthService.Object, mockMapper.Object, mockLogger.Object);
        }

        [Test]
        public async Task CreateTask_ValidRequest_ReturnsCreatedResult()
        {
            // Arrange
            var request = new CreateTaskRequest
            {
                Title = "Task Title",
                Description = "Task Description",
                Status = Enums.TaskStatus.Pending,
                Priority = TaskPriority.Medium,
                DueDate = DateTime.UtcNow.AddDays(1)
            };
            var userTask = new UserTask { Id = Guid.NewGuid() };
            mockMapper.Setup(m => m.Map<UserTask>(request)).Returns(userTask);
            mockAuthService.Setup(a => a.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User { Id = Guid.NewGuid() });
            mockTaskService.Setup(ts => ts.CreateTaskAsync(It.IsAny<UserTask>(), It.IsAny<CancellationToken>())).ReturnsAsync(userTask);
            // Act
            var result = await controller.CreateTask(request, CancellationToken.None);
            // Assert
            Assert.IsInstanceOf<CreatedResult>(result.Result);
            var createdResult = result.Result as CreatedResult;
            Assert.That(createdResult.Location, Is.EqualTo($"/tasks/{userTask.Id}"));
            mockTaskService.Verify(ts => ts.CreateTaskAsync(It.IsAny<UserTask>(), It.IsAny<CancellationToken>()), Times.Once);
            mockAuthService.Verify(ts => ts.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
        }
        [Test]
        public async Task GetTasks_ValidParams_ReturnsOkResult()
        {
            // Arrange
            var tasks = new List<UserTask> { new UserTask { Id = Guid.NewGuid(), Title = "Task 1" } };
            mockAuthService.Setup(a => a.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User { Id = Guid.NewGuid() });
            mockTaskService.Setup(ts => ts.GetTasksByUserIdAsync(It.IsAny<GetTasksByUserIdParams>(), It.IsAny<CancellationToken>())).ReturnsAsync(tasks);
            mockMapper.Setup(m => m.Map<TaskResponse>(It.IsAny<UserTask>())).Returns(new TaskResponse { Id = Guid.NewGuid(), Title = "Task 1" });
            // Act
            var result = await controller.GetTasks(1, 10, null, null, null, CancellationToken.None);
            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.IsInstanceOf<IEnumerable<TaskResponse>>(okResult.Value);
            mockTaskService.Verify(ts => ts.GetTasksByUserIdAsync(It.IsAny<GetTasksByUserIdParams>(), It.IsAny<CancellationToken>()), Times.Once);
            mockAuthService.Verify(ts => ts.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
        }
        [Test]
        public async Task GetTaskById_ValidId_ReturnsOkResult()
        {
            // Arrange
            var task = new UserTask { Id = Guid.NewGuid() };
            mockAuthService.Setup(a => a.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User { Id = Guid.NewGuid() });
            mockTaskService.Setup(ts => ts.GetTaskByIdAsync(It.IsAny<UserTaskParams>(), It.IsAny<CancellationToken>())).ReturnsAsync(task);
            mockMapper.Setup(m => m.Map<TaskResponse>(It.IsAny<UserTask>())).Returns(new TaskResponse { Id = task.Id });
            // Act
            var result = await controller.GetTaskById(task.Id, CancellationToken.None);
            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.IsInstanceOf<TaskResponse>(okResult.Value);
            Assert.That(((TaskResponse)okResult.Value).Id, Is.EqualTo(task.Id));
            mockTaskService.Verify(ts => ts.GetTaskByIdAsync(It.IsAny<UserTaskParams>(), It.IsAny<CancellationToken>()), Times.Once);
            mockAuthService.Verify(ts => ts.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
        }
        [Test]
        public async Task UpdateTask_ValidId_UpdatesTask()
        {
            // Arrange
            var request = new UpdateTaskRequest { Title = "Updated Task", Description = "Updated Description" };
            var task = new UserTask { Id = Guid.NewGuid() };
            mockMapper.Setup(m => m.Map<UserTask>(request)).Returns(task);
            mockAuthService.Setup(a => a.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User { Id = Guid.NewGuid() });
            // Act
            var result = await controller.UpdateTask(task.Id, request, CancellationToken.None);
            // Assert
            Assert.IsInstanceOf<OkResult>(result);
            mockTaskService.Verify(ts => ts.UpdateTaskAsync(It.IsAny<UserTaskParams>(), It.IsAny<UserTask>(), It.IsAny<CancellationToken>()), Times.Once);
            mockAuthService.Verify(ts => ts.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Exactly(2));
        }
        [Test]
        public async Task DeleteTaskById_ValidId_DeletesTask()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            mockAuthService.Setup(a => a.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User { Id = Guid.NewGuid() });
            mockTaskService.Setup(ts => ts.DeleteTaskByIdAsync(It.IsAny<UserTaskParams>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            // Act
            var result = await controller.DeleteTaskById(taskId, CancellationToken.None);
            // Assert
            Assert.IsInstanceOf<OkResult>(result);
            mockTaskService.Verify(ts => ts.DeleteTaskByIdAsync(It.IsAny<UserTaskParams>(), It.IsAny<CancellationToken>()), Times.Once);
            mockAuthService.Verify(ts => ts.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
        }
    }
}