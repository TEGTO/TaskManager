﻿using Authentication.Models;
using Authentication.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using TaskManagerApi.Domain.Entities;
using TaskManagerApi.Domain.Models;
using Task = System.Threading.Tasks.Task;

namespace TaskManagerApi.Services.Tests
{
    [TestFixture]
    internal class AuthServiceTests
    {
        private const int EXPIRY_IN_DAYS = 7;

        private Mock<UserManager<User>> userManagerMock;
        private Mock<ITokenHandler> tokenHandlerMock;
        private Mock<ILogger<AuthService>> mockLogger;
        private AuthService authService;

        [SetUp]
        public void SetUp()
        {
            var userStoreMock = new Mock<IUserStore<User>>();
            userManagerMock = new Mock<UserManager<User>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            tokenHandlerMock = new Mock<ITokenHandler>();
            mockLogger = new Mock<ILogger<AuthService>>();
            authService = new AuthService(userManagerMock.Object, tokenHandlerMock.Object, mockLogger.Object);
        }

        [Test]
        public async Task RegisterUserAsync_UserAndPasswordProvided_IdentityResultReturned()
        {
            //Arrange
            var user = new User { UserName = "testuser", Email = "testuser@example.com" };
            var password = "Password123";
            var identityResult = IdentityResult.Success;
            userManagerMock.Setup(x => x.CreateAsync(user, password)).ReturnsAsync(identityResult);
            //Act
            var result = await authService.RegisterUserAsync(new RegisterUserParams(user, password));
            //Assert
            Assert.That(result, Is.EqualTo(identityResult));
        }
        [Test]
        public async Task LoginUserAsync_ValidLoginAndPassword_TokenReturned()
        {
            //Arrange
            var user = new User { UserName = "testuser", Email = "testuser@example.com" };
            var password = "Password123";
            var login = "testuser";
            var tokenData = new AccessTokenData { AccessToken = "token", RefreshToken = "refreshToken" };
            userManagerMock.Setup(x => x.FindByEmailAsync(login)).ReturnsAsync((User)null);
            userManagerMock.Setup(x => x.FindByNameAsync(login)).ReturnsAsync(user);
            userManagerMock.Setup(x => x.CheckPasswordAsync(user, password)).ReturnsAsync(true);
            tokenHandlerMock.Setup(x => x.CreateToken(user)).Returns(tokenData);
            userManagerMock.Setup(x => x.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);
            //Act
            var result = await authService.LoginUserAsync(new LoginUserParams(login, password, EXPIRY_IN_DAYS));
            //Assert
            Assert.That(result, Is.EqualTo(tokenData));
            Assert.That(result.RefreshTokenExpiryDate, Is.GreaterThan(DateTime.UtcNow.AddDays(EXPIRY_IN_DAYS - 1)));
        }
        [Test]
        public async Task GetUserByLoginAsync_LoginProvided_UserReturned()
        {
            //Arrange
            var login = "testuser";
            var user = new User { UserName = login, Email = "testuser@example.com" };
            userManagerMock.Setup(x => x.FindByEmailAsync(login)).ReturnsAsync((User)null);
            userManagerMock.Setup(x => x.FindByNameAsync(login)).ReturnsAsync(user);
            //Act
            var result = await authService.GetUserByLoginAsync(login);
            //Assert
            Assert.That(result, Is.EqualTo(user));
        }
        [Test]
        public async Task UpdateUserAsync_ValidUpdateData_IdentityErrorsReturned()
        {
            //Arrange
            var updateData = new UserUpdateData
            {
                UserName = "newuser",
                OldEmail = "old@example.com",
                NewEmail = "new@example.com",
                OldPassword = "OldPassword123",
                NewPassword = "NewPassword123"
            };
            var user = new User { UserName = "olduser", Email = updateData.OldEmail };
            userManagerMock.Setup(x => x.FindByEmailAsync(updateData.OldEmail)).ReturnsAsync(user);
            userManagerMock.Setup(x => x.SetUserNameAsync(user, updateData.UserName)).ReturnsAsync(IdentityResult.Success);
            userManagerMock.Setup(x => x.GenerateChangeEmailTokenAsync(user, updateData.NewEmail)).ReturnsAsync("token");
            userManagerMock.Setup(x => x.ChangeEmailAsync(user, updateData.NewEmail, "token")).ReturnsAsync(IdentityResult.Success);
            userManagerMock.Setup(x => x.ChangePasswordAsync(user, updateData.OldPassword, updateData.NewPassword)).ReturnsAsync(IdentityResult.Success);
            //Act
            var result = await authService.UpdateUserAsync(updateData);
            //Assert
            Assert.IsEmpty(result);
        }
        [Test]
        public async Task RefreshTokenAsync_ValidTokenData_TokenReturned()
        {
            //Arrange
            var accessTokenData = new AccessTokenData { AccessToken = "accessToken", RefreshToken = "refreshToken" };
            var user = new User { UserName = "testuser", RefreshToken = "refreshToken", RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1) };
            var principal = new Mock<ClaimsPrincipal>();
            var identity = new Mock<ClaimsIdentity>();
            principal.Setup(x => x.Identity).Returns(identity.Object);
            identity.Setup(x => x.Name).Returns(user.UserName);
            tokenHandlerMock.Setup(x => x.GetPrincipalFromExpiredToken(accessTokenData.AccessToken)).Returns(principal.Object);
            userManagerMock.Setup(x => x.FindByNameAsync(user.UserName)).ReturnsAsync(user);
            tokenHandlerMock.Setup(x => x.CreateToken(user)).Returns(accessTokenData);
            //Act
            var result = await authService.RefreshTokenAsync(accessTokenData, EXPIRY_IN_DAYS);
            //Assert
            Assert.That(result, Is.EqualTo(accessTokenData));
            Assert.That(result.RefreshTokenExpiryDate, Is.GreaterThan(DateTime.UtcNow.AddDays(EXPIRY_IN_DAYS - 1)));
        }
    }
}