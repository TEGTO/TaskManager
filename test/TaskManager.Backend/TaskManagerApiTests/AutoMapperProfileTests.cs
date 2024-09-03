using Authentication.Models;
using AutoMapper;
using TaskManagerApi.Domain.Dtos.Auth;
using TaskManagerApi.Domain.Dtos.Task;
using TaskManagerApi.Domain.Entities;
using TaskManagerApi.Domain.Models;
using TaskManagerApi.Enums;
using Task = TaskManagerApi.Domain.Entities.Task;

namespace TaskManagerApi.Tests
{
    [TestFixture]
    internal class AutoMapperProfileTests
    {
        private IMapper mapper;

        [SetUp]
        public void SetUp()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
            mapper = config.CreateMapper();
        }

        [Test]
        public void CreateTaskRequestToTask_ValidRequest_MapsCorrectly()
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
            // Act
            var result = mapper.Map<Task>(request);
            // Assert
            Assert.That(result.Title, Is.EqualTo(request.Title));
            Assert.That(result.Description, Is.EqualTo(request.Description));
            Assert.That(result.Status, Is.EqualTo(request.Status));
            Assert.That(result.Priority, Is.EqualTo(request.Priority));
            Assert.That(result.DueDate, Is.EqualTo(request.DueDate));
        }
        [Test]
        public void TaskToTaskResponse_ValidTask_MapsCorrectly()
        {
            // Arrange
            var task = new Task
            {
                Id = Guid.NewGuid(),
                Title = "Task Title",
                Description = "Task Description",
                Status = Enums.TaskStatus.Completed,
                Priority = TaskPriority.High,
                DueDate = DateTime.UtcNow.AddDays(1),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            // Act
            var result = mapper.Map<TaskResponse>(task);
            // Assert
            Assert.That(result.Id, Is.EqualTo(task.Id));
            Assert.That(result.Title, Is.EqualTo(task.Title));
            Assert.That(result.Description, Is.EqualTo(task.Description));
            Assert.That(result.Status, Is.EqualTo(task.Status));
            Assert.That(result.Priority, Is.EqualTo(task.Priority));
            Assert.That(result.CreatedAt, Is.EqualTo(task.CreatedAt));
            Assert.That(result.UpdatedAt, Is.EqualTo(task.UpdatedAt));
        }
        [Test]
        public void UserToUserRegistrationRequest_UserMappedCorrectly()
        {
            //Arrange
            var user = new User
            {
                UserName = "testuser",
                Email = "testuser@example.com",
                RefreshToken = "some-refresh-token",
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1)
            };
            //Act
            var result = mapper.Map<UserRegistrationRequest>(user);
            //Assert
            Assert.That(result.UserName, Is.EqualTo(user.UserName));
            Assert.That(result.Email, Is.EqualTo(user.Email));
        }
        [Test]
        public void UserRegistrationRequestToUser_UserRegistrationRequestMappedCorrectly()
        {
            //Arrange
            var request = new UserRegistrationRequest
            {
                UserName = "testuser",
                Email = "testuser@example.com",
                Password = "Password123",
                ConfirmPassword = "Password123"
            };
            //Act
            var result = mapper.Map<User>(request);
            //Assert
            Assert.That(result.UserName, Is.EqualTo(request.UserName));
            Assert.That(result.Email, Is.EqualTo(request.Email));
        }
        [Test]
        public void AccessTokenDataToAuthToken_AccessTokenDataMappedCorrectly()
        {
            //Arrange
            var accessTokenData = new AccessTokenData
            {
                AccessToken = "access-token",
                RefreshToken = "refresh-token"
            };
            //Act
            var result = mapper.Map<AuthToken>(accessTokenData);
            //Assert
            Assert.That(result.AccessToken, Is.EqualTo(accessTokenData.AccessToken));
            Assert.That(result.RefreshToken, Is.EqualTo(accessTokenData.RefreshToken));
        }
        [Test]
        public void AuthTokenToAccessTokenData_AuthTokenMappedCorrectly()
        {
            //Arrange
            var authToken = new AuthToken
            {
                AccessToken = "access-token",
                RefreshToken = "refresh-token",
                RefreshTokenExpiryDate = DateTime.UtcNow.AddDays(1)
            };
            //Act
            var result = mapper.Map<AccessTokenData>(authToken);
            //Assert
            Assert.That(result.AccessToken, Is.EqualTo(authToken.AccessToken));
            Assert.That(result.RefreshToken, Is.EqualTo(authToken.RefreshToken));
        }
        [Test]
        public void UserUpdateDataRequestToUserUpdateData_UserUpdateDataRequestMappedCorrectly()
        {
            //Arrange
            var request = new UserUpdateDataRequest
            {
                UserName = "testuser",
                OldEmail = "old@example.com",
                NewEmail = "new@example.com",
                OldPassword = "OldPassword123",
                NewPassword = "NewPassword123"
            };
            //Act
            var result = mapper.Map<UserUpdateData>(request);
            //Assert
            Assert.That(result.UserName, Is.EqualTo(request.UserName));
            Assert.That(result.OldEmail, Is.EqualTo(request.OldEmail));
            Assert.That(result.NewEmail, Is.EqualTo(request.NewEmail));
            Assert.That(result.OldPassword, Is.EqualTo(request.OldPassword));
            Assert.That(result.NewPassword, Is.EqualTo(request.NewPassword));
        }
    }
}
