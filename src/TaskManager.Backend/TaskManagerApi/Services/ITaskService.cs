
using TaskManagerApi.Enums;
using TaskStatus = TaskManagerApi.Enums.TaskStatus;
using UserTask = TaskManagerApi.Domain.Entities.Task;

namespace TaskManagerApi.Services
{
    public record class UserTaskParams(Guid UserId, Guid TaskId);
    public record class GetTasksByUserIdParams(
    Guid UserId,
    int PageNumber,
    int PageSize,
    TaskStatus? Status = null,
    DateTime? DueDate = null,
    TaskPriority? Priority = null);
    public interface ITaskService
    {
        public Task<UserTask?> GetTaskByIdAsync(UserTaskParams param, CancellationToken cancellationToken);
        public Task<IEnumerable<UserTask>> GetTasksByUserIdAsync(GetTasksByUserIdParams request, CancellationToken cancellationToken);
        public Task<UserTask> CreateTaskAsync(UserTask task, CancellationToken cancellationToken);
        public Task UpdateTaskAsync(UserTaskParams param, UserTask task, CancellationToken cancellationToken);
        public Task DeleteTaskByIdAsync(UserTaskParams param, CancellationToken cancellationToken);
    }
}