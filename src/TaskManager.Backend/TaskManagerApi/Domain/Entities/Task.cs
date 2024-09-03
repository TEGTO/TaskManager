using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskManagerApi.Enums;

namespace TaskManagerApi.Domain.Entities
{
    public class Task : ITrackable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } = default!;
        [Required]
        [MaxLength(256)]
        public string Title { get; set; } = default!;
        [MaxLength(1024)]
        public string? Description { get; set; }
        public Enums.TaskStatus Status { get; set; }
        public TaskPriority Priority { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        [Required]
        public Guid UserId { get; set; } = default!;
        public User User { get; set; }

        public void Copy(Task other)
        {
            Title = other.Title;
            Description = other.Description;
            Status = other.Status;
            Priority = other.Priority;
            DueDate = other.DueDate;
        }
    }
}