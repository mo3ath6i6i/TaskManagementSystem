using FluentValidation;
using TaskManagementSystem.Core.DTOs;

namespace TaskManagementSystem.Business.Services.Validators
{
    public class UpdateTaskDtoValidator : AbstractValidator<UpdateTaskDto>
    {
        public UpdateTaskDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(100).WithMessage("Title must not exceed 100 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Status is invalid.");

            RuleFor(x => x.Priority)
                .IsInEnum().WithMessage("Priority is invalid.");

            RuleFor(x => x.DueDate)
                .GreaterThan(DateTime.UtcNow).WithMessage("Due date must be in the future.");
        }
    }
}