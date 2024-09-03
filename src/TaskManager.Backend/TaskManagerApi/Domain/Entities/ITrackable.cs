namespace TaskManagerApi.Domain.Entities
{
    public interface ITrackable
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
