using FluentValidation;

namespace EGL.Kinexa.Application.Features.MedicalBranches.Validators;

public class CreateMedicalBranchValidator : AbstractValidator<Commands.CreateMedicalBranchCommand>
{
    public CreateMedicalBranchValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");
    }
}
