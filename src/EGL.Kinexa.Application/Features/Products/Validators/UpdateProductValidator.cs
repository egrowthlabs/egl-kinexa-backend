using FluentValidation;

namespace EGL.Kinexa.Application.Features.Products.Validators;

public class UpdateProductValidator : AbstractValidator<Commands.UpdateProductCommand>
{
    public UpdateProductValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("Valid CategoryId is required.");

        RuleFor(x => x.MedicalBranchId)
            .GreaterThan(0).WithMessage("Valid MedicalBranchId is required.");
    }
}
