using Tripflow.Application.Helpers;
using Tripflow.Domain.Entities.Tenants;
using Tripflow.UnitTests.Utils;

namespace Tripflow.UnitTests.Helpers;

public class TenantOrderByHelperTests
{
    [Theory]
    [InlineData("legalname")]
    [InlineData("tradename")]
    [InlineData("documentnumber")]
    [InlineData("email")]
    [InlineData("status")]
    [InlineData("createdatutc")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("unknown")]
    public void Build_Should_Return_Compilable_Expression(string? sortBy)
    {
        var result = TenantOrderByHelper.Build(sortBy);
        var compiled = result.Compile();
        var entity = TenantTestHelper.Create(
            legalName: "RAZÃO",
            tradeName: "FANTASIA",
            documentNumber: "123",
            email: "a@b.com");

        _ = compiled(entity);
    }

    [Fact]
    public void Build_WithLegalName_OrdersByLegalName()
    {
        var compiled = TenantOrderByHelper.Build("LegalName").Compile();
        var entity = TenantTestHelper.Create(legalName: "MINHA RAZÃO");
        Assert.Equal("MINHA RAZÃO", compiled(entity));
    }

    [Fact]
    public void Build_WithDocumentNumber_OrdersByDocumentNumber()
    {
        var compiled = TenantOrderByHelper.Build("documentnumber").Compile();
        var entity = TenantTestHelper.Create(documentNumber: "DOC123");
        Assert.Equal("DOC123", compiled(entity));
    }

    [Fact]
    public void Build_WithUpdatedAtUtc_OrdersByUpdatedAtUtc()
    {
        var tenant = TenantTestHelper.Create();
        tenant.Update("A", "B", "1", "e@e.com", "9", Domain.Enums.TenantStatus.Active, "user");
        var compiled = TenantOrderByHelper.Build("updatedatutc").Compile();
        Assert.NotNull(compiled(tenant));
    }
}
