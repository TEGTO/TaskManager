using AutoMapper;
using Metalama.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagerApi.Domain.Dtos.Task;
using TaskManagerApi.Enums;
using TaskManagerApi.Services;
using TaskStatus = TaskManagerApi.Enums.TaskStatus;
using UserTask = TaskManagerApi.Domain.Entities.Task;

namespace TaskManagerApi.Controllers
{
    [Authorize]
    [Route("tasks")]
    [ApiController]
    public partial class TaskController : ControllerBase
    {
        private readonly ITaskService taskService;
        private readonly IAuthService authService;
        private readonly IMapper mapper;
        private readonly ILogger<TaskController> logger;

        public TaskController(ITaskService taskServicel, IAuthService authService, IMapper mapper, ILogger<TaskController> logger)
        {
            this.taskService = taskServicel;
            this.authService = authService;
            this.mapper = mapper;
            this.logger = logger;
        }

        #region Endpoints

        [Log]
        [HttpPost]
        public async Task<ActionResult<TaskResponse>> CreateTask([FromBody] CreateTaskRequest request, CancellationToken cancellationToken)
        {
            var task = await GetTaskFromRequestAsync(request);
            var responseTask = await taskService.CreateTaskAsync(task, cancellationToken);
            return Created($"/tasks/{responseTask.Id}", mapper.Map<TaskResponse>(responseTask));
        }
        [Log]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskResponse>>> GetTasks(
            int pageNumber,
            int pageSize,
            TaskStatus? status = null,
            DateTime? dueDate = null,
            TaskPriority? priority = null,
            CancellationToken cancellationToken = default)
        {
            var request = new GetTasksByUserIdParams(
                await GetUserIdAsync(),
                pageNumber,
                pageSize,
                status,
                dueDate,
                priority
            );

            var tasks = await taskService.GetTasksByUserIdAsync(request, cancellationToken);
            return Ok(tasks.Select(mapper.Map<TaskResponse>));
        }
        [Log]
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskResponse?>> GetTaskById(string id, CancellationToken cancellationToken)
        {
            var task = await taskService.GetTaskByIdAsync(id, cancellationToken);
            return Ok(mapper.Map<TaskResponse>(task));
        }
        [Log]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(Guid id, [FromBody] UpdateTaskRequest request, CancellationToken cancellationToken)
        {
            var task = await GetTaskFromRequestAsync(request);
            task.Id = id;
            await taskService.UpdateTaskAsync(task, cancellationToken);
            return Ok();
        }
        [Log]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaskById(string id, CancellationToken cancellationToken)
        {
            await taskService.DeleteTaskByIdAsync(id, cancellationToken);
            return Ok();
        }

        #endregion

        #region Private Helpers

        private async Task<UserTask> GetTaskFromRequestAsync<T>(T request) where T : class
        {
            var task = mapper.Map<UserTask>(request);
            task.UserId = await GetUserIdAsync();
            return task;
        }

        private async Task<Guid> GetUserIdAsync()
        {
            var user = await authService.GetUserAsync(User);
            return user.Id;
        }

        #endregion
    }
}