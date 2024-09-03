using Authentication.Models;
using AutoMapper;
using Metalama.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos;
using System.Net;
using TaskManagerApi.Domain.Dtos.Auth;
using TaskManagerApi.Domain.Entities;
using TaskManagerApi.Domain.Models;
using TaskManagerApi.Services;

namespace TaskManagerApi.Controllers
{
    [Route("users")]
    [ApiController]
    public partial class AuthController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IAuthService authService;
        private readonly IConfiguration configuration;
        private readonly double expiryInDays;

        public AuthController(IMapper mapper, IAuthService authService, IConfiguration configuration)
        {
            this.mapper = mapper;
            this.authService = authService;
            this.configuration = configuration;
            expiryInDays = double.Parse(configuration[Configuration.AUTH_REFRESH_TOKEN_EXPIRY_IN_DAYS]!);
        }

        #region Endpoints

        [Log]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid client request");
            }

            var user = mapper.Map<User>(request);
            var registerParams = new RegisterUserParams(user, request.Password);

            var result = await authService.RegisterUserAsync(registerParams);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToArray();
                return BadRequest(new ResponseError
                {
                    StatusCode = ((int)HttpStatusCode.BadRequest).ToString(),
                    Messages = errors
                });
            }
            return Created($"", null);
        }
        [HttpPost("login")]
        public async Task<ActionResult<UserAuthenticationResponse>> Login([FromBody] UserAuthenticationRequest request)
        {
            var loginParams = new LoginUserParams(request.Login, request.Password, expiryInDays);
            var token = await authService.LoginUserAsync(loginParams);
            var tokenDto = mapper.Map<AuthToken>(token);

            var user = await authService.GetUserByLoginAsync(request.Login);

            var response = new UserAuthenticationResponse()
            {
                AuthToken = tokenDto,
                UserName = user.UserName,
                Email = user.Email
            };
            return Ok(response);
        }
        [Authorize]
        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] UserUpdateDataRequest request)
        {
            UserUpdateData updateData = mapper.Map<UserUpdateData>(request);

            var identityErrors = await authService.UpdateUserAsync(updateData);

            if (identityErrors.Count > 0)
            {
                var errors = identityErrors.Select(e => e.Description).ToArray();
                return BadRequest(new ResponseError { StatusCode = $"{(int)HttpStatusCode.BadRequest}", Messages = errors.ToArray() });
            }
            return Ok();
        }
        [HttpPost("refresh")]
        public async Task<ActionResult<AuthToken>> Refresh([FromBody] AuthToken request)
        {
            var tokenData = mapper.Map<AccessTokenData>(request);

            var newToken = await authService.RefreshTokenAsync(tokenData, expiryInDays);

            var tokenDto = mapper.Map<AuthToken>(newToken);
            return Ok(tokenDto);
        }

        #endregion
    }
}