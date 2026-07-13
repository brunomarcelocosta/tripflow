using Tripflow.Application.DTOs.Requests.Tenants;
using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Enums;
using Tripflow.UnitTests.Fixtures.UseCases;
using Tripflow.UnitTests.Utils;

namespace Tripflow.UnitTests.UseCases;

public class GetTenantsUseCaseTests(GetTenantsUseCaseFixture fixture) : IClassFixture<GetTenantsUseCaseFixture>
{
    [Fact]
    public async Task ExecuteAsync_Should_Return_Tenants_When_Valid_Request_WithData()
    {
        var useCase = fixture.Create(withData: true);
        var request = new TenantFilterRequest();

        var result = await useCase.ExecuteAsync(request, CancellationToken.None);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.NotEmpty(result.Data!.Items);
        Assert.Equal(1, result.Data.TotalItems);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Empty_When_Valid_Request_WithoutData()
    {
        var useCase = fixture.Create(withData: false);
        var request = new TenantFilterRequest();

        var result = await useCase.ExecuteAsync(request, CancellationToken.None);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Empty(result.Data!.Items);
        Assert.Equal(0, result.Data.TotalItems);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Correct_Pagination_When_Requesting_Specific_Page()
    {
        var tenants = new List<Tenant>
        {
            TenantTestHelper.Create(),
            TenantTestHelper.Create(legalName: "OUTRA RAZÃO", tradeName: "Outro")
        };
        var useCase = fixture.CreateWithCustomPagedResult(tenants, page: 2, pageSize: 5, totalItems: 12, totalPages: 3);
        var request = new TenantFilterRequest { Page = 2, PageSize = 5 };

        var result = await useCase.ExecuteAsync(request, CancellationToken.None);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data!.Page);
        Assert.Equal(5, result.Data.PageSize);
        Assert.Equal(12, result.Data.TotalItems);
        Assert.Equal(3, result.Data.TotalPages);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Map_Entities_To_Response_Correctly()
    {
        var tenant = TenantTestHelper.Create(
            legalName: "RAZÃO SOCIAL LTDA",
            tradeName: "Nome Fantasia",
            documentNumber: "12345678000199",
            email: "mapped@test.com");
        var useCase = fixture.CreateWithCustomPagedResult([tenant]);
        var request = new TenantFilterRequest();

        var result = await useCase.ExecuteAsync(request, CancellationToken.None);

        Assert.True(result.Success);
        var response = result.Data!.Items.First();
        Assert.Equal(tenant.Id, response.Id);
        Assert.Equal("RAZÃO SOCIAL LTDA", response.LegalName);
        Assert.Equal("Nome Fantasia", response.TradeName);
        Assert.Equal("12345678000199", response.DocumentNumber);
        Assert.Equal("mapped@test.com", response.Email);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Use_Status_Filter()
    {
        var useCase = fixture.Create(withData: true);
        var request = new TenantFilterRequest { Status = TenantStatus.Active, SortBy = "LegalName", SortDesc = false };

        var result = await useCase.ExecuteAsync(request, CancellationToken.None);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
    }
}
