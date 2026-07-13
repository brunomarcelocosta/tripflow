using Tripflow.Application.Builders;
using Tripflow.Application.DTOs.Requests.Tenants;
using Tripflow.Domain.Enums;
using Tripflow.UnitTests.Utils;

namespace Tripflow.UnitTests.Builders;

public class TenantFilterBuilderTests
{
    [Fact]
    public void ToExpression_WithEmptyRequest_ReturnsAll()
    {
        var request = new TenantFilterRequest();
        var filter = request.ToExpression();
        var compiled = filter.Compile();

        var tenant = TenantTestHelper.Create();

        Assert.True(compiled(tenant));
    }

    [Fact]
    public void ToExpression_WithLegalName_FiltersByExactMatchUppercase()
    {
        var request = new TenantFilterRequest { LegalName = "  razão social  " };
        var filter = request.ToExpression();
        var compiled = filter.Compile();

        var match = TenantTestHelper.Create(legalName: "RAZÃO SOCIAL");
        var noMatch = TenantTestHelper.Create(legalName: "OUTRA RAZÃO");

        Assert.True(compiled(match));
        Assert.False(compiled(noMatch));
    }

    [Fact]
    public void ToExpression_WithTradeName_FiltersByExactMatchUppercase()
    {
        var request = new TenantFilterRequest { TradeName = "fantasia" };
        var filter = request.ToExpression();
        var compiled = filter.Compile();

        var match = TenantTestHelper.Create(tradeName: "FANTASIA");
        var noMatch = TenantTestHelper.Create(tradeName: "OUTRO");

        Assert.True(compiled(match));
        Assert.False(compiled(noMatch));
    }

    [Fact]
    public void ToExpression_WithDocumentNumber_FiltersByExactMatchUppercase()
    {
        var request = new TenantFilterRequest { DocumentNumber = "12345678000199" };
        var filter = request.ToExpression();
        var compiled = filter.Compile();

        var match = TenantTestHelper.Create(documentNumber: "12345678000199");
        var noMatch = TenantTestHelper.Create(documentNumber: "99999999000199");

        Assert.True(compiled(match));
        Assert.False(compiled(noMatch));
    }

    [Fact]
    public void ToExpression_WithEmail_FiltersByExactMatchUppercase()
    {
        var request = new TenantFilterRequest { Email = "tenant@test.com" };
        var filter = request.ToExpression();
        var compiled = filter.Compile();

        var match = TenantTestHelper.Create(email: "TENANT@TEST.COM");
        var noMatch = TenantTestHelper.Create(email: "other@test.com");

        Assert.True(compiled(match));
        Assert.False(compiled(noMatch));
    }

    [Fact]
    public void ToExpression_WithStatus_FiltersByStatus()
    {
        var request = new TenantFilterRequest { Status = TenantStatus.Inactive };
        var filter = request.ToExpression();
        var compiled = filter.Compile();

        var match = TenantTestHelper.Create(status: TenantStatus.Inactive);
        var noMatch = TenantTestHelper.Create(status: TenantStatus.Active);

        Assert.True(compiled(match));
        Assert.False(compiled(noMatch));
    }

    [Fact]
    public void ToExpression_WithEmptyLegalName_DoesNotApplyFilter()
    {
        var request = new TenantFilterRequest { LegalName = "   " };
        var filter = request.ToExpression();
        var compiled = filter.Compile();

        var tenant = TenantTestHelper.Create();

        Assert.True(compiled(tenant));
    }

    [Fact]
    public void ToExpression_WithMultipleFilters_CombinesWithAnd()
    {
        var request = new TenantFilterRequest
        {
            LegalName = "RAZÃO SOCIAL LTDA",
            TradeName = "NOME FANTASIA",
            Status = TenantStatus.Active
        };
        var filter = request.ToExpression();
        var compiled = filter.Compile();

        var match = TenantTestHelper.Create(
            legalName: "RAZÃO SOCIAL LTDA",
            tradeName: "NOME FANTASIA",
            status: TenantStatus.Active);
        var noMatch = TenantTestHelper.Create(
            legalName: "RAZÃO SOCIAL LTDA",
            tradeName: "OUTRO",
            status: TenantStatus.Active);

        Assert.True(compiled(match));
        Assert.False(compiled(noMatch));
    }
}
