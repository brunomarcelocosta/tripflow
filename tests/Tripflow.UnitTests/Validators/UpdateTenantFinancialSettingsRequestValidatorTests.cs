using Tripflow.Application.DTOs.Requests.Tenants;
using Tripflow.Application.Validators.Tenants;

namespace Tripflow.UnitTests.Validators;

public class UpdateTenantFinancialSettingsRequestValidatorTests
{
    private static UpdateTenantFinancialSettingsRequestValidator BuildValidator() => new();

    [Fact]
    public async Task Validate_Should_Pass_When_All_Fields_Null()
    {
        var request = new UpdateTenantFinancialSettingsRequest();

        var result = await BuildValidator().ValidateAsync(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_Should_Pass_When_Only_ProfitAmount_Set()
    {
        var request = new UpdateTenantFinancialSettingsRequest { DefaultProfitAmount = 100m };

        var result = await BuildValidator().ValidateAsync(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_Should_Pass_When_Only_ProfitPercentage_Set()
    {
        var request = new UpdateTenantFinancialSettingsRequest { DefaultProfitPercentage = 15m };

        var result = await BuildValidator().ValidateAsync(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_Should_Fail_When_Both_Profit_Fields_Set()
    {
        var request = new UpdateTenantFinancialSettingsRequest
        {
            DefaultProfitAmount = 100m,
            DefaultProfitPercentage = 10m
        };

        var result = await BuildValidator().ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Informe a margem padrão em valor OU em percentual, não os dois ao mesmo tempo.");
    }

    [Fact]
    public async Task Validate_Should_Fail_When_ProfitAmount_Is_Negative()
    {
        var request = new UpdateTenantFinancialSettingsRequest { DefaultProfitAmount = -1m };

        var result = await BuildValidator().ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "A margem padrão em valor não pode ser negativa.");
    }

    [Fact]
    public async Task Validate_Should_Fail_When_ProfitPercentage_Is_Negative()
    {
        var request = new UpdateTenantFinancialSettingsRequest { DefaultProfitPercentage = -0.01m };

        var result = await BuildValidator().ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "A margem padrão em percentual não pode ser negativa.");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(100.01)]
    [InlineData(150)]
    public async Task Validate_Should_Fail_When_PixDiscount_Is_Out_Of_Range(decimal value)
    {
        var request = new UpdateTenantFinancialSettingsRequest { DefaultPixDiscountPercentage = value };

        var result = await BuildValidator().ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "O desconto padrão de PIX deve estar entre 0 e 100.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(50)]
    [InlineData(100)]
    public async Task Validate_Should_Accept_PixDiscount_Within_Range(decimal value)
    {
        var request = new UpdateTenantFinancialSettingsRequest { DefaultPixDiscountPercentage = value };

        var result = await BuildValidator().ValidateAsync(request);

        Assert.True(result.IsValid);
    }
}
