using Authentication.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using TaskManagerApi.Domain.Entities;
using TaskManagerApi.Domain.Models;

namespace TaskManagerApi.Services
{
    public record class RegisterUserParams(User User, string Password);
    public record class LoginUserParams(string Login, string Password, double RefreshTokenExpiryInDays);
    public interface IAuthService
    {
        public Task<AccessTokenData> LoginUserAsync(LoginUserParams loginParams);
        public Task<User?> GetUserByLoginAsync(string login);
        public Task<AccessTokenData> RefreshTokenAsync(AccessTokenData accessTokenData, double refreshTokenExpiryInDays);
        public Task<IdentityResult> RegisterUserAsync(RegisterUserParams registerParams);
        public Task<List<IdentityError>> UpdateUserAsync(UserUpdateData updateData);
        public Task<User?> GetUserAsync(ClaimsPrincipal principal);
    }
}