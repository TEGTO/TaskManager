
namespace TaskManagerApi.Services
{
    public interface ITaskService
    {
        public Task<Domain.Entities.Task?> GetTaskByIdAsync(string id, CancellationToken cancellationToken);
        public Task<IEnumerable<Domain.Entities.Task>> GetTasksByUserIdAsync(GetTasksByUserIdParams request, CancellationToken cancellationToken);
        public Task<Domain.Entities.Task> CreateTaskAsync(Domain.Entities.Task task, CancellationToken cancellationToken);
        public Task UpdateTaskAsync(Domain.Entities.Task task, CancellationToken cancellationToken);
        public Task DeleteTaskByIdAsync(string id, CancellationToken cancellationToken);
    }
}