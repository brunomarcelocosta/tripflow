using Tripflow.Application.Builders;
using Tripflow.Application.DTOs.Requests.Subscriptions;
using Tripflow.Application.Validators.Subscriptions;
using Tripflow.Domain.Entities.Subscriptions;

namespace Tripflow.UnitTests.Builders;

public class LeadFilterExtensionsTests
{
    [Fact]
    public void ToExpression_Should_FilterByEmailAndCompanyName()
    {
        var filter = new LeadFilterRequest
        {
            Email = "demo@test.com",
            CompanyName = "TripFlow"
        }.ToExpression().Compile();

        Assert.True(filter(new Lead("TripFlow LTDA", "João", "demo@test.com", null, null, null, "public")));
        Assert.False(filter(new Lead("Outra Empresa", "Maria", "outro@test.com", null, null, null, "public")));
    }

    [Fact]
    public void ToExpression_Should_FilterByConvertedFlag()
    {
        var filter = new LeadFilterRequest { ConvertedToTenant = false }.ToExpression().Compile();
        var lead = new Lead("Empresa", "João", "a@test.com", null, null, null, "public");

        Assert.True(filter(lead));

        lead.MarkAsConverted(Guid.NewGuid(), "admin");
        Assert.False(filter(lead));
    }
}

public class ConvertLeadToTenantRequestValidatorTests
{
    private readonly ConvertLeadToTenantRequestValidator _validator = new();

    [Fact]
    public async Task Validate_Should_RequireLegalAndTradeName()
    {
        var result = await _validator.ValidateAsync(new ConvertLeadToTenantRequest());

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(ConvertLeadToTenantRequest.LegalName));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(ConvertLeadToTenantRequest.TradeName));
    }

    [Fact]
    public async Task Validate_Should_AcceptValidRequest()
    {
        var result = await _validator.ValidateAsync(new ConvertLeadToTenantRequest
        {
            LegalName = "Empresa LTDA",
            TradeName = "Empresa",
            DocumentNumber = "123",
            SubscriptionPlanId = Guid.NewGuid()
        });

        Assert.True(result.IsValid);
    }
}
