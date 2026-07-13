using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Enums;

namespace Tripflow.UnitTests.Utils;

public static class TenantTestHelper
{
    public static Tenant Create(
        string legalName = "Razão Social LTDA",
        string tradeName = "Nome Fantasia",
        string? documentNumber = "12345678000199",
        string? email = "tenant@test.com",
        string? phone = "11999999999",
        TenantStatus status = TenantStatus.Active)
    {
        return new Tenant(legalName, tradeName, documentNumber, email, phone, status, "system");
    }
}
