using FluentValidation;

namespace EGL.Kinexa.Application.Features.ContactMessages.Validators;

public class CreateContactMessageValidator : AbstractValidator<Commands.CreateContactMessageCommand>
{
    public CreateContactMessageValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Valid email is required.")
            .MaximumLength(150);

        RuleFor(x => x.Subject)
            .NotEmpty().WithMessage("Subject is required.")
            .MaximumLength(200);

        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Message is required.");
    }
}
