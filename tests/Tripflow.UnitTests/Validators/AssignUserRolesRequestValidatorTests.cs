using Tripflow.Application.DTOs.Requests.Roles;
using Tripflow.UnitTests.Fixtures.Validators;

namespace Tripflow.UnitTests.Validators;

public class AssignUserRolesRequestValidatorTests(AssignUserRolesRequestValidatorFixture fixture)
    : IClassFixture<AssignUserRolesRequestValidatorFixture>
{
    [Fact]
    public async Task Validate_Should_Return_No_Errors_When_Request_Is_Valid()
    {
        var validator = fixture.CreateValidatorWithExistingUser();
        var request = AssignUserRolesValidationRequest.From(fixture.UserId, new AssignUserRolesRequest
        {
            RoleNames = ["Consultant"]
        });

        var result = await validator.ValidateAsync(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_Should_Return_Error_When_User_Not_Found()
    {
        var validator = fixture.CreateValidatorWithMissingUser();
        var request = AssignUserRolesValidationRequest.From(fixture.UserId, new AssignUserRolesRequest
        {
            RoleNames = ["Consultant"]
        });

        var result = await validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Usuário não encontrado nesta empresa.");
    }

    [Fact]
    public async Task Validate_Should_Return_Error_When_No_Roles_Informed()
    {
        var validator = fixture.CreateValidatorWithExistingUser();
        var request = AssignUserRolesValidationRequest.From(fixture.UserId, new AssignUserRolesRequest());

        var result = await validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Informe ao menos uma role válida.");
    }
}
