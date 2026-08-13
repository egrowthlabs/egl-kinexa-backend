using FluentValidation;

namespace EGL.Kinexa.Application.Features.QuoteRequests.Validators;

public class CreateQuoteRequestValidator : AbstractValidator<Commands.CreateQuoteRequestCommand>
{
    public CreateQuoteRequestValidator()
    {
        RuleFor(x => x.CustomerName)
            .NotEmpty().WithMessage("Customer name is required.")
            .MaximumLength(150);

        RuleFor(x => x.CustomerEmail)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Valid email is required.")
            .MaximumLength(150);

        RuleFor(x => x.CustomerPhone)
            .NotEmpty().WithMessage("Phone is required.")
            .MaximumLength(20);

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("At least one item is required.");
    }
}
