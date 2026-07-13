using Tripflow.Domain.Enums;

namespace Tripflow.Domain.Entities.Payments;

public sealed class PaymentProvider : BaseEntity
{
    private PaymentProvider() { }

    public PaymentProvider(string code, string name, PaymentProviderStatus status)
    {
        Code = code;
        Name = name;
        Status = status;
    }

    public string Code { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public PaymentProviderStatus Status { get; private set; }
}

