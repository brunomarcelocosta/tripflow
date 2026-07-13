using Tripflow.Application.DTOs.Requests.Tenants;
using Tripflow.UnitTests.Fixtures.Validators;

namespace Tripflow.UnitTests.Validators;

public class GetTenantByIdRequestValidatorTests(GetTenantByIdRequestValidatorFixture fixture) : IClassFixture<GetTenantByIdRequestValidatorFixture>
{
    [Fact]
    public async Task Validate_Should_Return_No_Errors_When_Tenant_Exists()
    {
        var validator = fixture.CreateForExists();
        var request = new GetTenantByIdRequest(Guid.NewGuid());

        var result = await validator.ValidateAsync(request);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validate_Should_Return_Error_When_Id_Is_Empty()
    {
        var validator = fixture.CreateForExists();
        var request = new GetTenantByIdRequest(Guid.Empty);

        var result = await validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "O identificador do tenant é obrigatório.");
    }

    [Fact]
    public async Task Validate_Should_Return_Error_When_Tenant_Not_Found()
    {
        var validator = fixture.CreateForNotExists();
        var request = new GetTenantByIdRequest(Guid.NewGuid());

        var result = await validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Tenant não encontrado.");
    }
}
