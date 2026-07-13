using Tripflow.Application.DTOs.Requests.Customers;
using Tripflow.Application.Validators.Customers;

namespace Tripflow.UnitTests.Validators;

public class UpdateCustomerPreferenceRequestValidatorTests
{
    private readonly UpdateCustomerPreferenceRequestValidator _validator = new();

    [Fact]
    public async Task EmptyRequest_Should_Pass()
    {
        var request = new UpdateCustomerPreferenceRequest(null, null, null, null, null, null);
        var result = await _validator.ValidateAsync(request);
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task TooLong_PreferredAirlines_Should_Fail()
    {
        var request = new UpdateCustomerPreferenceRequest(
            new string('a', 1001), null, null, null, null, null);
        var result = await _validator.ValidateAsync(request);
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task TooLong_TravelPreferences_Should_Fail()
    {
        var request = new UpdateCustomerPreferenceRequest(
            null, null, null, null, new string('b', 2001), null);
        var result = await _validator.ValidateAsync(request);
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task TooLong_GeneralNotes_Should_Fail()
    {
        var request = new UpdateCustomerPreferenceRequest(
            null, null, null, null, null, new string('c', 4001));
        var result = await _validator.ValidateAsync(request);
        Assert.False(result.IsValid);
    }
}
