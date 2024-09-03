﻿namespace TaskManagerApi.Domain.Dtos.Auth
{
    public class UserAuthenticationResponse
    {
        public AuthToken AuthToken { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }
}
