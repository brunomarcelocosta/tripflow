using Tripflow.Application.Helpers;
using Tripflow.Domain.Entities.Tenants;
using Tripflow.UnitTests.Utils;

namespace Tripflow.UnitTests.Helpers;

public class BuildOrderByHelperTests
{
    [Fact]
    public void BuildOrderBy_WithLegalName_ReturnsExpression()
    {
        var result = BuildOrderByHelper.BuildOrderBy<Tenant>("LegalName");

        Assert.NotNull(result);
        var compiled = result!.Compile();
        var entity = TenantTestHelper.Create(legalName: "RAZÃO A");
        Assert.Equal("RAZÃO A", compiled(entity));
    }

    [Fact]
    public void BuildOrderBy_WithTradeName_ReturnsExpressionForTradeName()
    {
        var result = BuildOrderByHelper.BuildOrderBy<Tenant>("TradeName");

        Assert.NotNull(result);
        var compiled = result!.Compile();
        var entity = TenantTestHelper.Create(tradeName: "Fantasia X");
        Assert.Equal("Fantasia X", compiled(entity));
    }

    [Fact]
    public void BuildOrderBy_WithStatus_ReturnsExpressionForStatus()
    {
        var result = BuildOrderByHelper.BuildOrderBy<Tenant>("Status");

        Assert.NotNull(result);
        var compiled = result!.Compile();
        var entity = TenantTestHelper.Create();
        Assert.NotNull(compiled(entity));
    }

    [Fact]
    public void BuildOrderBy_WithCreatedAtUtc_ReturnsExpressionForCreatedAtUtc()
    {
        var result = BuildOrderByHelper.BuildOrderBy<Tenant>("CreatedAtUtc");

        Assert.NotNull(result);
        var compiled = result!.Compile();
        var entity = TenantTestHelper.Create();
        Assert.NotNull(compiled(entity));
    }

    [Fact]
    public void BuildOrderBy_CaseInsensitive_RecognizesProperty()
    {
        var result = BuildOrderByHelper.BuildOrderBy<Tenant>("legalname");

        Assert.NotNull(result);
        var compiled = result!.Compile();
        var entity = TenantTestHelper.Create(legalName: "RAZÃO B");
        Assert.Equal("RAZÃO B", compiled(entity));
    }

    [Fact]
    public void BuildOrderBy_WithNullOrderBy_UsesDefaultCreatedAtUtc()
    {
        var result = BuildOrderByHelper.BuildOrderBy<Tenant>(null);

        Assert.NotNull(result);
        var compiled = result!.Compile();
        var entity = TenantTestHelper.Create();
        Assert.NotNull(compiled(entity));
    }

    [Fact]
    public void BuildOrderBy_WithEmptyOrderBy_UsesDefaultCreatedAtUtc()
    {
        var result = BuildOrderByHelper.BuildOrderBy<Tenant>("");

        Assert.NotNull(result);
    }

    [Fact]
    public void BuildOrderBy_WithOrderByWithSpaces_TrimsAndFindsProperty()
    {
        var result = BuildOrderByHelper.BuildOrderBy<Tenant>("  TradeName  ");

        Assert.NotNull(result);
        var compiled = result!.Compile();
        var entity = TenantTestHelper.Create(tradeName: "Trimmed");
        Assert.Equal("Trimmed", compiled(entity));
    }

    [Fact]
    public void BuildOrderBy_WithNonExistentProperty_UsesDefault()
    {
        var result = BuildOrderByHelper.BuildOrderBy<Tenant>("PropertyInexistente");

        Assert.NotNull(result);
        var compiled = result!.Compile();
        var entity = TenantTestHelper.Create();
        Assert.NotNull(compiled(entity));
    }

    [Fact]
    public void BuildOrderBy_WithId_ReturnsExpressionForId()
    {
        var result = BuildOrderByHelper.BuildOrderBy<Tenant>("Id");

        Assert.NotNull(result);
        var compiled = result!.Compile();
        var entity = TenantTestHelper.Create();
        Assert.Equal(entity.Id, compiled(entity));
    }

    [Fact]
    public void BuildOrderBy_WithTypeWithoutProperties_ReturnsNull()
    {
        var result = BuildOrderByHelper.BuildOrderBy<EmptyEntity>(null);

        Assert.Null(result);
    }

    [Fact]
    public void BuildOrderBy_WithTypeWithoutDefaultProperties_UsesFirstAvailable()
    {
        var result = BuildOrderByHelper.BuildOrderBy<OnlyNameEntity>("unknown");

        Assert.NotNull(result);
        var compiled = result!.Compile();
        Assert.Equal("Test", compiled(new OnlyNameEntity { Name = "Test" }));
    }
}
