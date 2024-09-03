using Metalama.Attributes;
using Microsoft.EntityFrameworkCore;
using Shared.Repositories;
using TaskManagerApi.Data;
using TaskManagerApi.Enums;
using TaskStatus = TaskManagerApi.Enums.TaskStatus;
using UserTask = TaskManagerApi.Domain.Entities.Task;

namespace TaskManagerApi.Services
{
    public record class GetTasksByUserIdParams(
        Guid UserId,
        int PageNumber,
        int PageSize,
        TaskStatus? Status = null,
        DateTime? DueDate = null,
        TaskPriority? Priority = null);

    public partial class TaskService : ITaskService
    {
        private readonly IDatabaseRepository<TaskManagerDbContext> databaseRepository;

        public TaskService(IDatabaseRepository<TaskManagerDbContext> databaseRepository)
        {
            this.databaseRepository = databaseRepository;
        }

        #region ITaskService Members

        [Log]
        public async Task<UserTask> CreateTaskAsync(UserTask task, CancellationToken cancellationToken)
        {
            return await databaseRepository.AddAsync(task, cancellationToken);
        }
        [Log]
        public async Task<IEnumerable<UserTask>> GetTasksByUserIdAsync(GetTasksByUserIdParams request, CancellationToken cancellationToken)
        {
            var taskQueryable = await databaseRepository.GetQueryableAsync<UserTask>(cancellationToken);

            taskQueryable = taskQueryable.Where(t => t.UserId == request.UserId);

            if (request.Status.HasValue)
            {
                taskQueryable = taskQueryable.Where(t => t.Status == request.Status.Value);
            }

            if (request.DueDate.HasValue)
            {
                taskQueryable = taskQueryable.Where(t => t.DueDate <= request.DueDate);
            }

            if (request.Priority.HasValue)
            {
                taskQueryable = taskQueryable.Where(t => t.Priority == request.Priority.Value);
            }

            taskQueryable = taskQueryable.Skip((request.PageNumber - 1) * request.PageSize)
                                         .Take(request.PageSize);
            var tasks = await taskQueryable.ToListAsync(cancellationToken);

            return tasks;
        }
        [Log]
        public async Task<UserTask?> GetTaskByIdAsync(string id, CancellationToken cancellationToken)
        {
            var taskQueryable = await databaseRepository.GetQueryableAsync<UserTask>(cancellationToken);

            var task = await taskQueryable.FirstOrDefaultAsync(t => t.Id.ToString() == id, cancellationToken);

            return task;
        }
        [Log]
        public async Task UpdateTaskAsync(UserTask task, CancellationToken cancellationToken)
        {
            var taskQueryable = await databaseRepository.GetQueryableAsync<UserTask>(cancellationToken);

            var taskIdDb = await taskQueryable.FirstOrDefaultAsync(t => t.Id == task.Id, cancellationToken);
            if (taskIdDb != null)
            {
                taskIdDb.Copy(task);
                await databaseRepository.UpdateAsync(taskIdDb, cancellationToken);
            }
        }
        [Log]
        public async Task DeleteTaskByIdAsync(string id, CancellationToken cancellationToken)
        {
            var taskQueryable = await databaseRepository.GetQueryableAsync<UserTask>(cancellationToken);

            var task = await taskQueryable.FirstOrDefaultAsync(t => t.Id.ToString() == id, cancellationToken);

            if (task != null)
            {
                await databaseRepository.DeleteAsync(task, cancellationToken);
            }
        }

        #endregion
    }
}