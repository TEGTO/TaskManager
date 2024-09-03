using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace TaskManagerApi.Domain.Entities
{
    [Index(nameof(Email), IsUnique = true)]
    public class User : IdentityUser<Guid>, ITrackable
    {
        [MaxLength(100)]
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
