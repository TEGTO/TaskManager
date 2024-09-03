namespace TaskManagerApi.Domain.Dtos.Auth
{
    public class UserAuthenticationRequest
    {
        public string? Login { get; set; }
        public string? Password { get; set; }
    }
}
