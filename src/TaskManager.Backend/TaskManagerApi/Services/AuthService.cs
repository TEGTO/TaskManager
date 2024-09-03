using Authentication.Models;
using Authentication.Services;
using Metalama.Attributes;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using TaskManagerApi.Domain.Entities;
using TaskManagerApi.Domain.Models;
using Task = System.Threading.Tasks.Task;

namespace TaskManagerApi.Services
{
    public partial class AuthService : IAuthService
    {
        private readonly UserManager<User> userManager;
        private readonly ITokenHandler tokenHandler;

        public AuthService(UserManager<User> userManager, ITokenHandler tokenHandler)
        {
            this.userManager = userManager;
            this.tokenHandler = tokenHandler;
        }
        #region IAuthService Members

        [Log]
        public async Task<IdentityResult> RegisterUserAsync(RegisterUserParams registerParams)
        {
            return await userManager.CreateAsync(registerParams.User, registerParams.Password);
        }
        [Log]
        public async Task<AccessTokenData> LoginUserAsync(LoginUserParams loginParams)
        {
            var user = await GetUserByLoginAsync(loginParams.Login);
            if (user == null || !await userManager.CheckPasswordAsync(user, loginParams.Password))
            {
                throw new UnauthorizedAccessException("Invalid authentication. Login or email address is not correct.");
            }
            var tokenData = CreateNewTokenData(user, loginParams.RefreshTokenExpiryInDays);
            await SetRefreshToken(user, tokenData);
            return tokenData;
        }
        [Log]
        public async Task<User?> GetUserAsync(ClaimsPrincipal principal)
        {
            var name = principal.FindFirstValue(ClaimTypes.Name);
            return string.IsNullOrEmpty(name) ? null : await GetUserByLoginAsync(name);
        }
        [Log]
        public async Task<User?> GetUserByLoginAsync(string login)
        {
            var user = await userManager.FindByEmailAsync(login);
            user = user == null ? await userManager.FindByNameAsync(login) : user;
            return user;
        }
        [Log]
        public async Task<List<IdentityError>> UpdateUserAsync(UserUpdateData updateData)
        {
            var user = await userManager.FindByEmailAsync(updateData.OldEmail);
            List<IdentityError> identityErrors = new List<IdentityError>();

            if (!string.IsNullOrEmpty(updateData.UserName))
            {
                var result = await userManager.SetUserNameAsync(user, updateData.UserName);
                identityErrors.AddRange(result.Errors);
            }

            if (!string.IsNullOrEmpty(updateData.NewEmail) && !updateData.NewEmail.Equals(updateData.OldEmail))
            {
                var token = await userManager.GenerateChangeEmailTokenAsync(user, updateData.NewEmail);
                var result = await userManager.ChangeEmailAsync(user, updateData.NewEmail, token);
                identityErrors.AddRange(result.Errors);
            }

            if (!string.IsNullOrEmpty(updateData.NewPassword))
            {
                var result = await userManager.ChangePasswordAsync(user, updateData.OldPassword, updateData.NewPassword);
                identityErrors.AddRange(result.Errors);
            }

            return RemoveDuplicates(identityErrors);
        }
        [Log]
        public async Task<AccessTokenData> RefreshTokenAsync(AccessTokenData accessTokenData, double refreshTokenExpiryInDays)
        {
            var principal = tokenHandler.GetPrincipalFromExpiredToken(accessTokenData.AccessToken);
            var user = await userManager.FindByNameAsync(principal.Identity.Name);

            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid authentication. AccessToken is not valid.");
            }

            if (user.RefreshToken != accessTokenData.RefreshToken || user.RefreshTokenExpiryTime < DateTime.UtcNow)
            {
                throw new InvalidDataException("Refresh token is not valid!");
            }

            var tokenData = CreateNewTokenData(user, refreshTokenExpiryInDays);
            await SetRefreshToken(user, tokenData);
            return tokenData;
        }

        #endregion

        #region Private Helpers
        private AccessTokenData CreateNewTokenData(User user, double refreshTokenExpiryInDays)
        {
            var tokenData = tokenHandler.CreateToken(user);
            tokenData.RefreshTokenExpiryDate = DateTime.UtcNow.AddDays(refreshTokenExpiryInDays);
            return tokenData;
        }
        private async Task SetRefreshToken(User user, AccessTokenData accessTokenData)
        {
            user.RefreshToken = accessTokenData.RefreshToken;
            user.RefreshTokenExpiryTime = accessTokenData.RefreshTokenExpiryDate;
            await userManager.UpdateAsync(user);
        }
        private List<IdentityError> RemoveDuplicates(List<IdentityError> identityErrors)
        {
            identityErrors = identityErrors
            .GroupBy(e => e.Description)
            .Select(g => g.First())
            .ToList();
            return identityErrors;
        }
        #endregion
    }
}