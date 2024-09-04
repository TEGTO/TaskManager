using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;
using Shared.Repositories;
using TaskManagerApi.Data;
using Task = System.Threading.Tasks.Task;
using UserTask = TaskManagerApi.Domain.Entities.Task;

namespace TaskManagerApi.Services.Tests
{
    [TestFixture]
    internal class TaskServiceTests
    {
        private Mock<IDatabaseRepository<TaskManagerDbContext>> databaseRepositoryMock;
        private Mock<ILogger<TaskService>> loggerMock;
        private TaskService taskService;
        private CancellationToken cancellationToken;

        [SetUp]
        public void SetUp()
        {
            databaseRepositoryMock = new Mock<IDatabaseRepository<TaskManagerDbContext>>();
            loggerMock = new Mock<ILogger<TaskService>>();
            taskService = new TaskService(databaseRepositoryMock.Object, loggerMock.Object);
            cancellationToken = new CancellationToken();
        }
        private static Mock<DbSet<T>> GetDbSetMock<T>(IQueryable<T> data) where T : class
        {
            return data.BuildMockDbSet();
        }

        [Test]
        public async Task CreateTaskAsync_ValidTask_TaskCreated()
        {
            // Arrange
            var task = new UserTask { Id = Guid.NewGuid(), Title = "Test Task" };
            databaseRepositoryMock.Setup(repo => repo.AddAsync(task, cancellationToken)).ReturnsAsync(task);
            // Act
            var result = await taskService.CreateTaskAsync(task, cancellationToken);
            // Assert
            Assert.That(result, Is.EqualTo(task));
            databaseRepositoryMock.Verify(repo => repo.AddAsync(task, cancellationToken), Times.Once);
        }
        [Test]
        public async Task GetTasksByUserIdAsync_ValidRequest_ReturnsFilteredTasks()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var tasks = new List<UserTask>
            {
                new UserTask { Id = Guid.NewGuid(), UserId = userId, Title = "Task 1" },
                new UserTask { Id = Guid.NewGuid(), UserId = userId, Title = "Task 2" },
            };

            var dbSetMock = GetDbSetMock(tasks.AsQueryable());

            var request = new GetTasksByUserIdParams(userId, 1, 10);
            databaseRepositoryMock.Setup(repo => repo.GetQueryableAsync<UserTask>(cancellationToken))
                .ReturnsAsync(dbSetMock.Object);
            // Act
            var result = await taskService.GetTasksByUserIdAsync(request, cancellationToken);
            // Assert
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.IsTrue(result.All(t => t.UserId == userId));
            databaseRepositoryMock.Verify(repo => repo.GetQueryableAsync<UserTask>(cancellationToken), Times.Once);
        }
        [Test]
        public async Task GetTaskByIdAsync_ValidId_TaskReturned()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var task = new UserTask { Id = taskId, UserId = userId, Title = "Test Task" };
            var tasks = new List<UserTask> { task }.AsQueryable();

            var dbSetMock = GetDbSetMock(tasks.AsQueryable());

            databaseRepositoryMock.Setup(repo => repo.GetQueryableAsync<UserTask>(cancellationToken))
                .ReturnsAsync(dbSetMock.Object);
            // Act
            var param = new UserTaskParams(userId, taskId);
            var result = await taskService.GetTaskByIdAsync(param, cancellationToken);
            // Assert
            Assert.That(result, Is.EqualTo(task));
            databaseRepositoryMock.Verify(repo => repo.GetQueryableAsync<UserTask>(cancellationToken), Times.Once);
        }

        [Test]
        public async Task UpdateTaskAsync_ValidTask_TaskUpdated()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var existingTask = new UserTask { Id = taskId, UserId = userId, Title = "Old Task" };
            var updatedTask = new UserTask { Id = taskId, UserId = userId, Title = "Updated Task" };
            var tasks = new List<UserTask> { existingTask }.AsQueryable();

            var dbSetMock = GetDbSetMock(tasks.AsQueryable());

            databaseRepositoryMock.Setup(repo => repo.GetQueryableAsync<UserTask>(cancellationToken))
                .ReturnsAsync(dbSetMock.Object);
            databaseRepositoryMock.Setup(repo => repo.UpdateAsync(existingTask, cancellationToken))
                .Returns(Task.CompletedTask);
            // Act
            var param = new UserTaskParams(userId, taskId);
            await taskService.UpdateTaskAsync(param, updatedTask, cancellationToken);
            // Assert
            databaseRepositoryMock.Verify(repo => repo.GetQueryableAsync<UserTask>(cancellationToken), Times.Once);
            databaseRepositoryMock.Verify(repo => repo.UpdateAsync(existingTask, cancellationToken), Times.Once);
            Assert.That(existingTask.Title, Is.EqualTo(updatedTask.Title));
        }

        [Test]
        public async Task DeleteTaskByIdAsync_ValidId_TaskDeleted()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var task = new UserTask { Id = taskId, UserId = userId, Title = "Test Task" };
            var tasks = new List<UserTask> { task }.AsQueryable();

            var dbSetMock = GetDbSetMock(tasks.AsQueryable());

            databaseRepositoryMock.Setup(repo => repo.GetQueryableAsync<UserTask>(cancellationToken))
                .ReturnsAsync(dbSetMock.Object);
            databaseRepositoryMock.Setup(repo => repo.DeleteAsync(task, cancellationToken))
                .Returns(Task.CompletedTask);
            // Act
            var param = new UserTaskParams(userId, taskId);
            await taskService.DeleteTaskByIdAsync(param, cancellationToken);
            // Assert
            databaseRepositoryMock.Verify(repo => repo.GetQueryableAsync<UserTask>(cancellationToken), Times.Once);
            databaseRepositoryMock.Verify(repo => repo.DeleteAsync(task, cancellationToken), Times.Once);
        }
    }
}