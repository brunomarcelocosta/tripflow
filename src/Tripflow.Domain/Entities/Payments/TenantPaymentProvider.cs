using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Enums;
using Tripflow.Domain.Interfaces;

namespace Tripflow.Domain.Entities.Payments;

public sealed class TenantPaymentProvider : AuditableEntity, ITenantEntity
{
    private TenantPaymentProvider() { }

    public TenantPaymentProvider(Guid tenantId, Guid paymentProviderId, string? displayName, string? encryptedApiKey, string? encryptedSecret, string? webhookSecret, bool isDefault, PaymentProviderStatus status, string createdBy)
    {
        TenantId = tenantId;
        PaymentProviderId = paymentProviderId;
        DisplayName = displayName;
        EncryptedApiKey = encryptedApiKey;
        EncryptedSecret = encryptedSecret;
        WebhookSecret = webhookSecret;
        IsDefault = isDefault;
        Status = status;
        SetCreated(createdBy);
    }

    public Guid TenantId { get; private set; }
    public Tenant Tenant { get; private set; } = default!;

    public Guid PaymentProviderId { get; private set; }
    public PaymentProvider PaymentProvider { get; private set; } = default!;

    public string? DisplayName { get; private set; }
    public string? EncryptedApiKey { get; private set; }
    public string? EncryptedSecret { get; private set; }
    public string? WebhookSecret { get; private set; }
    public bool IsDefault { get; private set; }
    public PaymentProviderStatus Status { get; private set; }

    public void Update(string? displayName, string? encryptedApiKey, string? encryptedSecret, string? webhookSecret, bool isDefault, PaymentProviderStatus status, string updatedBy)
    {
        DisplayName = displayName;
        if (encryptedApiKey is not null)
            EncryptedApiKey = encryptedApiKey;
        if (encryptedSecret is not null)
            EncryptedSecret = encryptedSecret;
        if (webhookSecret is not null)
            WebhookSecret = webhookSecret;
        IsDefault = isDefault;
        Status = status;
        SetUpdated(updatedBy);
    }

    public void SetDefault(bool isDefault, string updatedBy)
    {
        IsDefault = isDefault;
        SetUpdated(updatedBy);
    }

    public void Activate(string updatedBy)
    {
        Status = PaymentProviderStatus.Active;
        SetUpdated(updatedBy);
    }

    public void Inactivate(string updatedBy)
    {
        Status = PaymentProviderStatus.Inactive;
        SetUpdated(updatedBy);
    }
}

