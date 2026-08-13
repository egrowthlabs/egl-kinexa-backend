using FluentValidation;

namespace EGL.Kinexa.Application.Features.Categories.Validators;

public class CreateCategoryValidator : AbstractValidator<Commands.CreateCategoryCommand>
{
    public CreateCategoryValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");
    }
}
