using Tripflow.Application.DTOs.Requests.Users;
using Tripflow.UnitTests.Fixtures.Validators;

namespace Tripflow.UnitTests.Validators;

public class InviteUserRequestValidatorTests(InviteUserRequestValidatorFixture fixture) : IClassFixture<InviteUserRequestValidatorFixture>
{
    private static InviteUserRequest ValidRequest() => new()
    {
        FullName = "Novo Usuário",
        Email = "novo@test.com",
        RoleNames = ["Consultant"]
    };

    [Fact]
    public async Task Validate_Should_Return_No_Errors_When_Request_Is_Valid()
    {
        var validator = fixture.CreateValidatorWithoutDuplicateEmail();

        var result = await validator.ValidateAsync(ValidRequest());

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_Should_Return_Error_When_FullName_Is_Empty()
    {
        var validator = fixture.CreateValidatorWithoutDuplicateEmail();
        var request = new InviteUserRequest { FullName = "", Email = "novo@test.com", RoleNames = ["Consultant"] };

        var result = await validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Nome completo é obrigatório.");
    }

    [Fact]
    public async Task Validate_Should_Return_Error_When_Email_Is_Invalid()
    {
        var validator = fixture.CreateValidatorWithoutDuplicateEmail();
        var request = new InviteUserRequest { FullName = "Novo Usuário", Email = "invalido", RoleNames = ["Consultant"] };

        var result = await validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "O e-mail informado é inválido.");
    }

    [Fact]
    public async Task Validate_Should_Return_Error_When_No_Roles_Informed()
    {
        var validator = fixture.CreateValidatorWithoutDuplicateEmail();
        var request = new InviteUserRequest { FullName = "Novo Usuário", Email = "novo@test.com" };

        var result = await validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Informe ao menos uma role válida para o convite.");
    }

    [Fact]
    public async Task Validate_Should_Return_Error_When_Email_Already_Exists_In_Tenant()
    {
        var validator = fixture.CreateValidatorWithDuplicateEmail();

        var result = await validator.ValidateAsync(ValidRequest());

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Já existe um usuário com este e-mail nesta empresa.");
    }
}
