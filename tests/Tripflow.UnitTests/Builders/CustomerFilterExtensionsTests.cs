using Tripflow.Application.Builders;
using Tripflow.Application.DTOs.Requests.Customers;
using Tripflow.Domain.Enums;
using Tripflow.UnitTests.Utils;

namespace Tripflow.UnitTests.Builders;

public class CustomerFilterExtensionsTests
{
    [Fact]
    public void ToExpression_NoFilters_ReturnsAllowAll()
    {
        var request = new CustomerFilterRequest();
        var customer = CustomerTestHelper.Create();

        var compiled = request.ToExpression().Compile();

        Assert.True(compiled(customer));
    }

    [Fact]
    public void ToExpression_WithSearchInName_MatchesByName()
    {
        var request = new CustomerFilterRequest { Search = "joão" };
        var customer = CustomerTestHelper.Create(fullName: "João Pereira");

        Assert.True(request.ToExpression().Compile()(customer));
    }

    [Fact]
    public void ToExpression_WithSearchInEmail_MatchesByEmail()
    {
        var request = new CustomerFilterRequest { Search = "outro@" };
        var customer = CustomerTestHelper.Create(email: "outro@teste.com");

        Assert.True(request.ToExpression().Compile()(customer));
    }

    [Fact]
    public void ToExpression_WithSearch_NoMatch_ReturnsFalse()
    {
        var request = new CustomerFilterRequest { Search = "xyz" };
        var customer = CustomerTestHelper.Create();

        Assert.False(request.ToExpression().Compile()(customer));
    }

    [Fact]
    public void ToExpression_WithType_FiltersByType()
    {
        var request = new CustomerFilterRequest { Type = CustomerType.Company };
        var company = CustomerTestHelper.Create(type: CustomerType.Company);
        var person = CustomerTestHelper.Create(type: CustomerType.Person);

        var compiled = request.ToExpression().Compile();
        Assert.True(compiled(company));
        Assert.False(compiled(person));
    }

    [Fact]
    public void ToExpression_WithStatus_FiltersByStatus()
    {
        var request = new CustomerFilterRequest { Status = CustomerStatus.Active };
        var active = CustomerTestHelper.Create();
        var blocked = CustomerTestHelper.Create();
        blocked.Block("system");

        var compiled = request.ToExpression().Compile();
        Assert.True(compiled(active));
        Assert.False(compiled(blocked));
    }

    [Fact]
    public void ToExpression_WithDocumentNumber_FiltersByDocument()
    {
        var request = new CustomerFilterRequest { DocumentNumber = "999" };
        var match = CustomerTestHelper.Create(documentNumber: "999");
        var noMatch = CustomerTestHelper.Create(documentNumber: "111");

        var compiled = request.ToExpression().Compile();
        Assert.True(compiled(match));
        Assert.False(compiled(noMatch));
    }

    [Fact]
    public void ToExpression_WithEmail_FiltersByEmail()
    {
        var request = new CustomerFilterRequest { Email = "a@b.com" };
        var match = CustomerTestHelper.Create(email: "A@B.com");

        Assert.True(request.ToExpression().Compile()(match));
    }
}
