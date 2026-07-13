using FluentValidation;
using Tripflow.Application.DTOs.Requests.Payments;

namespace Tripflow.Application.Validators.Payments;

public sealed class CreatePaymentLinkRequestValidator : AbstractValidator<CreatePaymentLinkRequest>
{
    public CreatePaymentLinkRequestValidator()
    {
        RuleFor(x => x.ExpiresAtUtc)
            .Must(BeInFuture!)
            .WithMessage("A data de expiração do link deve ser futura.")
            .When(x => x.ExpiresAtUtc.HasValue);
    }

    private static bool BeInFuture(DateTime? expiresAtUtc)
        => !expiresAtUtc.HasValue || expiresAtUtc.Value > DateTime.UtcNow;
}
