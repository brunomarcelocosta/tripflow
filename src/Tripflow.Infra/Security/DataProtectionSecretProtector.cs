using Microsoft.AspNetCore.DataProtection;
using Tripflow.Domain.Interfaces.Security;

namespace Tripflow.Infra.Security;

public sealed class DataProtectionSecretProtector(IDataProtectionProvider provider) : ISecretProtector
{
    private readonly IDataProtector _protector = provider.CreateProtector("Tripflow.TenantPaymentProvider.Secrets");

    public string Protect(string value) => _protector.Protect(value);

    public string Unprotect(string protectedValue) => _protector.Unprotect(protectedValue);
}
