using FluentValidation.TestHelper;
using TaskManagerApi.Domain.Dtos.Task;

namespace TaskManagerApi.Validators.Task.Tests
{
    [TestFixture]
    internal class CreateTaskRequestValidatorsTests
    {
        private CreateTaskRequestValidators validator;

        [SetUp]
        public void SetUp()
        {
            validator = new CreateTaskRequestValidators();
        }

        [Test]
        public void Validate_TitleIsNull_ReturnsValidationError()
        {
            var model = new CreateTaskRequest { Title = null };
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Title);
        }
        [Test]
        public void Validate_TitleIsEmpty_ReturnsValidationError()
        {
            var model = new CreateTaskRequest { Title = "" };
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Title);
        }
        [Test]
        public void Validate_TitleIsTooLong_ReturnsValidationError()
        {
            var model = new CreateTaskRequest { Title = new string('a', 257) };
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Title);
        }
        [Test]
        public void Validate_DescriptionIsNull_ReturnsValidationError()
        {
            var model = new CreateTaskRequest { Description = null };
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Description);
        }
        [Test]
        public void Validate_DescriptionIsEmpty_ReturnsValidationError()
        {
            var model = new CreateTaskRequest { Description = "" };
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Description);
        }
        [Test]
        public void Validate_DescriptionIsTooLong_ReturnsValidationError()
        {
            var model = new CreateTaskRequest { Description = new string('a', 1025) };
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Description);
        }
        [Test]
        public void Validate_DueDateIsInPast_ReturnsValidationError()
        {
            var model = new CreateTaskRequest
            {
                Title = "Valid Title",
                Description = "Valid Description",
                DueDate = DateTime.UtcNow.AddDays(-1)
            }; ;
            var result = validator.TestValidate(model);
            result.ShouldHaveAnyValidationError()
                .WithErrorMessage("Due date must be equal or greater than Now");
        }
        [Test]
        public void Validate_ValidModel_PassesValidation()
        {
            var model = new CreateTaskRequest
            {
                Title = "Task Title",
                Description = "Task Description",
                DueDate = DateTime.UtcNow.AddDays(1)
            };
            var result = validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}