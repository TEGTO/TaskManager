namespace TaskManagerApi.Domain.Dtos.Auth
{
    public class AuthToken
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryDate { get; set; }
    }
}
