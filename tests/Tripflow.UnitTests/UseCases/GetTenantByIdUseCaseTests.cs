using Moq;
using Tripflow.Application.DTOs.Requests.Tenants;
using Tripflow.UnitTests.Fixtures.UseCases;
using Tripflow.UnitTests.Utils;

namespace Tripflow.UnitTests.UseCases;

public class GetTenantByIdUseCaseTests(GetTenantByIdUseCaseFixture fixture) : IClassFixture<GetTenantByIdUseCaseFixture>
{
    [Fact]
    public async Task ExecuteAsync_Should_Return_Ok_With_Response_When_Entity_Exists()
    {
        var tenant = TenantTestHelper.Create();
        var useCase = fixture.CreateForSuccess(tenant);
        var request = new GetTenantByIdRequest(tenant.Id);

        var result = await useCase.ExecuteAsync(request);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(tenant.Id, result.Data!.Id);
        Assert.Equal(tenant.LegalName, result.Data.LegalName);
        Assert.Equal(tenant.TradeName, result.Data.TradeName);
        Assert.Equal(tenant.Status, result.Data.Status);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Failure_When_Validation_Fails()
    {
        var useCase = fixture.CreateForValidationFailure("Tenant não encontrado.");
        var request = new GetTenantByIdRequest(Guid.NewGuid());

        var result = await useCase.ExecuteAsync(request);

        Assert.False(result.Success);
        Assert.Null(result.Data);
        Assert.Equal("Tenant não encontrado.", result.Error);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Default_Validation_Message_When_Error_Message_Is_Null()
    {
        var useCase = fixture.CreateForValidationFailureWithoutMessage();
        var request = new GetTenantByIdRequest(Guid.NewGuid());

        var result = await useCase.ExecuteAsync(request);

        Assert.False(result.Success);
        Assert.Equal("Erro de validação.", result.Error);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Failure_When_Entity_Not_Found()
    {
        var useCase = fixture.CreateForNotFound();
        var request = new GetTenantByIdRequest(Guid.NewGuid());

        var result = await useCase.ExecuteAsync(request);

        Assert.False(result.Success);
        Assert.Null(result.Data);
        Assert.Equal("Tenant não encontrado.", result.Error);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Call_GetByIdAsync_Once_When_Success()
    {
        var tenant = TenantTestHelper.Create();
        var useCase = fixture.CreateForSuccess(tenant);
        var request = new GetTenantByIdRequest(tenant.Id);

        await useCase.ExecuteAsync(request);

        fixture.MockRepository.Verify(r => r.GetByIdAsync(request.Id), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Not_Call_GetByIdAsync_When_Validation_Fails()
    {
        var useCase = fixture.CreateForValidationFailure("Erro");
        var request = new GetTenantByIdRequest(Guid.NewGuid());

        await useCase.ExecuteAsync(request);

        fixture.MockRepository.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Map_Entity_To_Response_Correctly()
    {
        var tenant = TenantTestHelper.Create(
            legalName: "Empresa X LTDA",
            tradeName: "Empresa X",
            documentNumber: "99887766000155",
            email: "empresa@test.com",
            phone: "11888887777",
            status: Tripflow.Domain.Enums.TenantStatus.Trial);
        var useCase = fixture.CreateForSuccess(tenant);
        var request = new GetTenantByIdRequest(tenant.Id);

        var result = await useCase.ExecuteAsync(request);

        Assert.True(result.Success);
        Assert.Equal("Empresa X LTDA", result.Data!.LegalName);
        Assert.Equal("Empresa X", result.Data.TradeName);
        Assert.Equal("99887766000155", result.Data.DocumentNumber);
        Assert.Equal("empresa@test.com", result.Data.Email);
        Assert.Equal("11888887777", result.Data.Phone);
        Assert.Equal(Tripflow.Domain.Enums.TenantStatus.Trial, result.Data.Status);
    }
}
