using Tripflow.Application.DTOs.Requests.Roles;
using Tripflow.UnitTests.Fixtures.Validators;

namespace Tripflow.UnitTests.Validators;

public class RemoveUserRoleRequestValidatorTests(RemoveUserRoleRequestValidatorFixture fixture)
    : IClassFixture<RemoveUserRoleRequestValidatorFixture>
{
    [Fact]
    public async Task Validate_Should_Return_No_Errors_When_Entities_Exist()
    {
        var validator = fixture.CreateValidatorWithValidEntities();
        var request = new RemoveUserRoleValidationRequest(fixture.UserId, fixture.RoleId);

        var result = await validator.ValidateAsync(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_Should_Return_Error_When_User_Not_Found()
    {
        var validator = fixture.CreateValidatorWithMissingUser();
        var request = new RemoveUserRoleValidationRequest(fixture.UserId, fixture.RoleId);

        var result = await validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Usuário não encontrado nesta empresa.");
    }

    [Fact]
    public async Task Validate_Should_Return_Error_When_Role_Belongs_To_Other_Tenant()
    {
        var validator = fixture.CreateValidatorWithRoleFromOtherTenant();
        var request = new RemoveUserRoleValidationRequest(fixture.UserId, fixture.RoleId);

        var result = await validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Role não encontrada nesta empresa.");
    }
}
