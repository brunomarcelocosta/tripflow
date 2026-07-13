using Tripflow.Application.DTOs.Requests.Pricing;
using Tripflow.Application.Validators.Pricing;
using Tripflow.Domain.Enums;

namespace Tripflow.UnitTests.Validators;

public class UpdatePaymentFeeRulesRequestValidatorTests
{
    private static UpdatePaymentFeeRulesRequestValidator BuildValidator() =>
        new(new UpdatePaymentFeeRuleItemValidator());

    [Fact]
    public async Task Validate_Should_Pass_For_Valid_Rules()
    {
        var request = new UpdatePaymentFeeRulesRequest
        {
            Rules = new[]
            {
                new UpdatePaymentFeeRuleItem
                {
                    PaymentMethod = PaymentMethod.CreditCard,
                    Installments = 1,
                    FeePercentage = 4.20m,
                    IsActive = true
                },
                new UpdatePaymentFeeRuleItem
                {
                    PaymentMethod = PaymentMethod.Pix,
                    Installments = 1,
                    FeePercentage = 0m,
                    IsActive = true
                }
            }
        };

        var result = await BuildValidator().ValidateAsync(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_Should_Fail_When_Rules_Is_Empty()
    {
        var request = new UpdatePaymentFeeRulesRequest { Rules = [] };

        var result = await BuildValidator().ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Informe pelo menos uma regra de taxa.");
    }

    [Fact]
    public async Task Validate_Should_Fail_When_Duplicate_PaymentMethod_And_Installments()
    {
        var request = new UpdatePaymentFeeRulesRequest
        {
            Rules = new[]
            {
                new UpdatePaymentFeeRuleItem
                {
                    PaymentMethod = PaymentMethod.CreditCard,
                    Installments = 2,
                    FeePercentage = 5m
                },
                new UpdatePaymentFeeRuleItem
                {
                    PaymentMethod = PaymentMethod.CreditCard,
                    Installments = 2,
                    FeePercentage = 6m
                }
            }
        };

        var result = await BuildValidator().ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Não é permitido enviar regras duplicadas para o mesmo método e parcelamento.");
    }

    [Fact]
    public async Task Validate_Should_Fail_When_Inner_Item_Is_Invalid()
    {
        var request = new UpdatePaymentFeeRulesRequest
        {
            Rules = new[]
            {
                new UpdatePaymentFeeRuleItem
                {
                    PaymentMethod = PaymentMethod.Pix,
                    Installments = 3,
                    FeePercentage = 0m
                }
            }
        };

        var result = await BuildValidator().ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Para PIX e Manual o número de parcelas deve ser 1.");
    }
}
