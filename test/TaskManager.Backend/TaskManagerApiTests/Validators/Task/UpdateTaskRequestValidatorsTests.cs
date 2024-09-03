using FluentValidation.TestHelper;
using TaskManagerApi.Domain.Dtos.Task;

namespace TaskManagerApi.Validators.Task.Tests
{
    [TestFixture]
    internal class UpdateTaskRequestValidatorsTests
    {
        private UpdateTaskRequestValidators validator;

        [SetUp]
        public void SetUp()
        {
            validator = new UpdateTaskRequestValidators();
        }

        [Test]
        public void Validate_TitleIsNull_ReturnsValidationError()
        {
            var model = new UpdateTaskRequest { Title = null };
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Title);
        }
        [Test]
        public void Validate_TitleIsEmpty_ReturnsValidationError()
        {
            var model = new UpdateTaskRequest { Title = "" };
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Title);
        }
        [Test]
        public void Validate_TitleIsTooLong_ReturnsValidationError()
        {
            var model = new UpdateTaskRequest { Title = new string('a', 257) };
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Title);
        }
        [Test]
        public void Validate_DescriptionIsNull_ReturnsValidationError()
        {
            var model = new UpdateTaskRequest { Description = null };
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Description);
        }
        [Test]
        public void Validate_DescriptionIsEmpty_ReturnsValidationError()
        {
            var model = new UpdateTaskRequest { Description = "" };
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Description);
        }
        [Test]
        public void Validate_DescriptionIsTooLong_ReturnsValidationError()
        {
            var model = new UpdateTaskRequest { Description = new string('a', 1025) };
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Description);
        }
        [Test]
        public void Validate_DueDateIsInPast_ReturnsValidationError()
        {
            var model = new UpdateTaskRequest { DueDate = DateTime.UtcNow.AddDays(-1) };
            var result = validator.TestValidate(model);
            result.ShouldHaveAnyValidationError()
                .WithErrorMessage("Due date must be equal or greater than Now");
        }
        [Test]
        public void Validate_ValidModel_PassesValidation()
        {
            var model = new UpdateTaskRequest
            {
                Title = "Updated Task Title",
                Description = "Updated Task Description",
                DueDate = DateTime.UtcNow.AddDays(1)
            };
            var result = validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}