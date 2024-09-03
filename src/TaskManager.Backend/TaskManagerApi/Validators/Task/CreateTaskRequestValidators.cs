using FluentValidation;
using TaskManagerApi.Domain.Dtos.Task;

namespace TaskManagerApi.Validators.Task
{
    public class CreateTaskRequestValidators : AbstractValidator<CreateTaskRequest>
    {
        public CreateTaskRequestValidators()
        {
            RuleFor(x => x.Title).NotNull().NotEmpty().MaximumLength(256);
            RuleFor(x => x.Description).NotNull().NotEmpty().MaximumLength(1024);
            RuleFor(x => x.DueDate.ToUniversalTime()).GreaterThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("Due date must be equal or greater than Now");
        }
    }
}
