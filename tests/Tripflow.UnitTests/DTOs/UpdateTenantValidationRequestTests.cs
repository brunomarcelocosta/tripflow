using Tripflow.Application.DTOs.Requests.Tenants;
using Tripflow.Domain.Enums;

namespace Tripflow.UnitTests.DTOs;

public class UpdateTenantValidationRequestTests
{
    [Fact]
    public void From_Should_Map_All_Fields_From_UpdateTenantRequest()
    {
        var id = Guid.NewGuid();
        var request = new UpdateTenantRequest(
            "Razão Social LTDA",
            "Nome Fantasia",
            "12345678000199",
            "tenant@test.com",
            "11999999999",
            TenantStatus.Suspended);

        var result = UpdateTenantValidationRequest.From(id, request);

        Assert.Equal(id, result.Id);
        Assert.Equal(request.LegalName, result.LegalName);
        Assert.Equal(request.TradeName, result.TradeName);
        Assert.Equal(request.DocumentNumber, result.DocumentNumber);
        Assert.Equal(request.Email, result.Email);
        Assert.Equal(request.Phone, result.Phone);
        Assert.Equal(request.Status, result.Status);
    }
}
