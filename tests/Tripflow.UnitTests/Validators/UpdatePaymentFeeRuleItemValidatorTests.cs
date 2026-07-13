using Tripflow.Application.DTOs.Requests.Pricing;
using Tripflow.Application.Validators.Pricing;
using Tripflow.Domain.Enums;

namespace Tripflow.UnitTests.Validators;

public class UpdatePaymentFeeRuleItemValidatorTests
{
    private static UpdatePaymentFeeRuleItemValidator BuildValidator() => new();

    [Fact]
    public async Task Validate_Should_Pass_For_CreditCard_Within_Bounds()
    {
        var item = new UpdatePaymentFeeRuleItem
        {
            PaymentMethod = PaymentMethod.CreditCard,
            Installments = 6,
            FeePercentage = 9.67m
        };

        var result = await BuildValidator().ValidateAsync(item);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_Should_Pass_For_Pix_With_One_Installment()
    {
        var item = new UpdatePaymentFeeRuleItem
        {
            PaymentMethod = PaymentMethod.Pix,
            Installments = 1,
            FeePercentage = 0m
        };

        var result = await BuildValidator().ValidateAsync(item);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_Should_Pass_For_Manual_With_One_Installment()
    {
        var item = new UpdatePaymentFeeRuleItem
        {
            PaymentMethod = PaymentMethod.Manual,
            Installments = 1,
            FeePercentage = 0m
        };

        var result = await BuildValidator().ValidateAsync(item);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_Should_Fail_For_Pix_With_More_Than_One_Installment()
    {
        var item = new UpdatePaymentFeeRuleItem
        {
            PaymentMethod = PaymentMethod.Pix,
            Installments = 2,
            FeePercentage = 0m
        };

        var result = await BuildValidator().ValidateAsync(item);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Para PIX e Manual o número de parcelas deve ser 1.");
    }

    [Fact]
    public async Task Validate_Should_Fail_For_CreditCard_With_Zero_Installments()
    {
        var item = new UpdatePaymentFeeRuleItem
        {
            PaymentMethod = PaymentMethod.CreditCard,
            Installments = 0,
            FeePercentage = 1m
        };

        var result = await BuildValidator().ValidateAsync(item);

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Validate_Should_Fail_For_CreditCard_With_More_Than_12_Installments()
    {
        var item = new UpdatePaymentFeeRuleItem
        {
            PaymentMethod = PaymentMethod.CreditCard,
            Installments = 13,
            FeePercentage = 10m
        };

        var result = await BuildValidator().ValidateAsync(item);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Para cartão de crédito o número de parcelas deve estar entre 1 e 12.");
    }

    [Fact]
    public async Task Validate_Should_Fail_When_Fee_Is_Negative()
    {
        var item = new UpdatePaymentFeeRuleItem
        {
            PaymentMethod = PaymentMethod.CreditCard,
            Installments = 1,
            FeePercentage = -0.5m
        };

        var result = await BuildValidator().ValidateAsync(item);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "A taxa não pode ser negativa.");
    }

    [Fact]
    public async Task Validate_Should_Fail_When_PaymentMethod_Is_Invalid()
    {
        var item = new UpdatePaymentFeeRuleItem
        {
            PaymentMethod = (PaymentMethod)999,
            Installments = 1,
            FeePercentage = 0m
        };

        var result = await BuildValidator().ValidateAsync(item);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "O método de pagamento informado é inválido.");
    }
}
