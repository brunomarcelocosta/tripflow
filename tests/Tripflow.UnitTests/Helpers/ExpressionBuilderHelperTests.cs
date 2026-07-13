using System.Linq.Expressions;
using Tripflow.Application.Helpers;
using Tripflow.UnitTests.Utils;

namespace Tripflow.UnitTests.Helpers;

public class ExpressionBuilderHelperTests
{
    [Fact]
    public void And_CombinesTwoExpressionsWithAndAlso()
    {
        var parameter = Expression.Parameter(typeof(TestEntity), "x");
        Expression<Func<TestEntity, bool>> expr = Expression.Lambda<Func<TestEntity, bool>>(Expression.Constant(true), parameter);
        var other = (Expression<Func<TestEntity, bool>>)(x => x.IsActive);

        var result = expr.And(other);

        var compiled = result.Compile();
        Assert.True(compiled(new TestEntity { IsActive = true }));
        Assert.False(compiled(new TestEntity { IsActive = false }));
    }

    [Fact]
    public void And_WithFalseExpression_ReturnsFalse()
    {
        var parameter = Expression.Parameter(typeof(TestEntity), "x");
        Expression<Func<TestEntity, bool>> expr = Expression.Lambda<Func<TestEntity, bool>>(Expression.Constant(true), parameter);
        var other = (Expression<Func<TestEntity, bool>>)(x => x.Name == "Test");

        var result = expr.And(other);

        var compiled = result.Compile();
        Assert.False(compiled(new TestEntity { Name = "Other" }));
        Assert.True(compiled(new TestEntity { Name = "Test" }));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void AndEqualsString_WithNullOrEmptyValue_ReturnsOriginalExpression(string? value)
    {
        var parameter = Expression.Parameter(typeof(TestEntity), "x");
        Expression<Func<TestEntity, bool>> expr = Expression.Lambda<Func<TestEntity, bool>>(Expression.Constant(true), parameter);

        var result = expr.AndEqualsString(parameter, value, "Name");

        Assert.Same(expr, result);
    }

    [Fact]
    public void AndEqualsString_WithValidValue_AddsEqualityCondition()
    {
        var parameter = Expression.Parameter(typeof(TestEntity), "x");
        Expression<Func<TestEntity, bool>> expr = Expression.Lambda<Func<TestEntity, bool>>(Expression.Constant(true), parameter);

        var result = expr.AndEqualsString(parameter, "  TestValue  ", "Name");

        var compiled = result.Compile();
        Assert.True(compiled(new TestEntity { Name = "TestValue" }));
        Assert.False(compiled(new TestEntity { Name = "Other" }));
    }

    [Fact]
    public void AndEqualsString_WithInvalidPropertyPath_ReturnsOriginalExpression()
    {
        var parameter = Expression.Parameter(typeof(TestEntity), "x");
        Expression<Func<TestEntity, bool>> expr = Expression.Lambda<Func<TestEntity, bool>>(Expression.Constant(true), parameter);

        var result = expr.AndEqualsString(parameter, "value", "PropertyInexistente");

        Assert.Same(expr, result);
    }

    [Fact]
    public void AndEquals_WithNullValue_ReturnsOriginalExpression()
    {
        var parameter = Expression.Parameter(typeof(TestEntity), "x");
        Expression<Func<TestEntity, bool>> expr = Expression.Lambda<Func<TestEntity, bool>>(Expression.Constant(true), parameter);

        var result = expr.AndEquals<TestEntity, Guid>(parameter, (Guid?)null, "Id");

        Assert.Same(expr, result);
    }

    [Fact]
    public void AndEquals_WithValidValue_AddsEqualityCondition()
    {
        var parameter = Expression.Parameter(typeof(TestEntity), "x");
        var id = Guid.NewGuid();
        Expression<Func<TestEntity, bool>> expr = Expression.Lambda<Func<TestEntity, bool>>(Expression.Constant(true), parameter);

        var result = expr.AndEquals<TestEntity, Guid>(parameter, id, "Id");

        var compiled = result.Compile();
        Assert.True(compiled(new TestEntity { Id = id }));
        Assert.False(compiled(new TestEntity { Id = Guid.NewGuid() }));
    }

    [Fact]
    public void AndEquals_WithInvalidPropertyPath_ReturnsOriginalExpression()
    {
        var parameter = Expression.Parameter(typeof(TestEntity), "x");
        Expression<Func<TestEntity, bool>> expr = Expression.Lambda<Func<TestEntity, bool>>(Expression.Constant(true), parameter);

        var result = expr.AndEquals<TestEntity, Guid>(parameter, Guid.NewGuid(), "PropertyInexistente");

        Assert.Same(expr, result);
    }

    [Fact]
    public void AndEqualsDate_WithNullDateTime_ReturnsOriginalExpression()
    {
        var parameter = Expression.Parameter(typeof(TestEntity), "x");
        Expression<Func<TestEntity, bool>> expr = Expression.Lambda<Func<TestEntity, bool>>(Expression.Constant(true), parameter);

        var result = expr.AndEqualsDate(parameter, (DateTime?)null, "CreatedAtUtc");

        Assert.Same(expr, result);
    }

    [Fact]
    public void AndEqualsDate_WithInvalidDateTimeProperty_ReturnsOriginalExpression()
    {
        var parameter = Expression.Parameter(typeof(TestEntity), "x");
        Expression<Func<TestEntity, bool>> expr = Expression.Lambda<Func<TestEntity, bool>>(Expression.Constant(true), parameter);

        var result = expr.AndEqualsDate(parameter, DateTime.UtcNow, "PropertyInexistente");

        Assert.Same(expr, result);
    }

    [Fact]
    public void AndEqualsDate_WithValidDateTime_AddsEqualityCondition()
    {
        var parameter = Expression.Parameter(typeof(TestEntity), "x");
        var date = new DateTime(2025, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        Expression<Func<TestEntity, bool>> expr = Expression.Lambda<Func<TestEntity, bool>>(Expression.Constant(true), parameter);

        var result = expr.AndEqualsDate(parameter, date, "CreatedAtUtc");

        var compiled = result.Compile();
        Assert.True(compiled(new TestEntity { CreatedAtUtc = date }));
        Assert.False(compiled(new TestEntity { CreatedAtUtc = date.AddDays(1) }));
    }

    [Fact]
    public void AndEqualsDate_WithNullDateOnly_ReturnsOriginalExpression()
    {
        var parameter = Expression.Parameter(typeof(TestEntity), "x");
        Expression<Func<TestEntity, bool>> expr = Expression.Lambda<Func<TestEntity, bool>>(Expression.Constant(true), parameter);

        var result = expr.AndEqualsDate<TestEntity>(parameter, (DateOnly?)null, "BirthDate");

        Assert.Same(expr, result);
    }

    [Fact]
    public void AndEqualsDate_WithValidDateOnly_AddsEqualityCondition()
    {
        var parameter = Expression.Parameter(typeof(TestEntity), "x");
        var date = new DateOnly(2025, 6, 1);
        Expression<Func<TestEntity, bool>> expr = Expression.Lambda<Func<TestEntity, bool>>(Expression.Constant(true), parameter);

        var result = expr.AndEqualsDate(parameter, date, "BirthDate");

        var compiled = result.Compile();
        Assert.True(compiled(new TestEntity { BirthDate = date }));
        Assert.False(compiled(new TestEntity { BirthDate = date.AddDays(1) }));
    }

    [Fact]
    public void AndEqualsDate_WithInvalidDateOnlyProperty_ReturnsOriginalExpression()
    {
        var parameter = Expression.Parameter(typeof(TestEntity), "x");
        Expression<Func<TestEntity, bool>> expr = Expression.Lambda<Func<TestEntity, bool>>(Expression.Constant(true), parameter);

        var result = expr.AndEqualsDate(parameter, new DateOnly(2025, 1, 1), "PropertyInexistente");

        Assert.Same(expr, result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void OrContainsNormalized_WithNullOrEmptySearch_ReturnsTrue(string? search)
    {
        var parameter = Expression.Parameter(typeof(TestEntity), "x");
        var result = ExpressionBuilderHelper.OrContainsNormalized<TestEntity>(parameter, search, true, "Name", "Code");

        var compiled = result.Compile();
        Assert.True(compiled(new TestEntity { Name = "Anything", Code = "XXX" }));
    }

    [Fact]
    public void OrContainsNormalized_WithNormalizeTrue_FindsCaseInsensitive()
    {
        var parameter = Expression.Parameter(typeof(TestEntity), "x");
        var result = ExpressionBuilderHelper.OrContainsNormalized<TestEntity>(parameter, "TEST", true, "Name", "Code");

        var compiled = result.Compile();
        Assert.True(compiled(new TestEntity { Name = "MyTestValue", Code = "X" }));
        Assert.True(compiled(new TestEntity { Name = "X", Code = "test123" }));
        Assert.False(compiled(new TestEntity { Name = "Other", Code = "XXX" }));
    }

    [Fact]
    public void OrContainsNormalized_WithNormalizeFalse_UsesCaseSensitive()
    {
        var parameter = Expression.Parameter(typeof(TestEntity), "x");
        var result = ExpressionBuilderHelper.OrContainsNormalized<TestEntity>(parameter, "Test", false, "Name");

        var compiled = result.Compile();
        Assert.True(compiled(new TestEntity { Name = "TestValue" }));
        Assert.True(compiled(new TestEntity { Name = "MyTest" }));
    }

    [Fact]
    public void OrContainsNormalized_WithMultipleProperties_ReturnsTrueIfAnyContains()
    {
        var parameter = Expression.Parameter(typeof(TestEntity), "x");
        var result = ExpressionBuilderHelper.OrContainsNormalized<TestEntity>(parameter, "find", true, "Name", "Code");

        var compiled = result.Compile();
        Assert.True(compiled(new TestEntity { Name = "findme", Code = "X" }));
        Assert.True(compiled(new TestEntity { Name = "X", Code = "findme" }));
        Assert.False(compiled(new TestEntity { Name = "X", Code = "Y" }));
    }

    [Fact]
    public void OrContainsNormalized_WithNonStringProperties_ReturnsTrue()
    {
        var parameter = Expression.Parameter(typeof(TestEntity), "x");
        var result = ExpressionBuilderHelper.OrContainsNormalized<TestEntity>(parameter, "find", true, "Id");

        var compiled = result.Compile();
        Assert.True(compiled(new TestEntity()));
    }

    [Fact]
    public void OrContains_DelegatesToOrContainsNormalizedWithNormalizeFalse()
    {
        var parameter = Expression.Parameter(typeof(TestEntity), "x");
        var result = ExpressionBuilderHelper.OrContains<TestEntity>(parameter, "Test", "Name");

        var compiled = result.Compile();
        Assert.True(compiled(new TestEntity { Name = "TestValue" }));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void AndContainsString_WithNullOrEmptyValue_ReturnsOriginalExpression(string? value)
    {
        var parameter = Expression.Parameter(typeof(TestEntity), "x");
        Expression<Func<TestEntity, bool>> expr = Expression.Lambda<Func<TestEntity, bool>>(Expression.Constant(true), parameter);

        var result = expr.AndContainsString(parameter, value, "Name");

        Assert.Same(expr, result);
    }

    [Fact]
    public void AndContainsString_WithValidValue_AddsContainsCondition()
    {
        var parameter = Expression.Parameter(typeof(TestEntity), "x");
        Expression<Func<TestEntity, bool>> expr = Expression.Lambda<Func<TestEntity, bool>>(Expression.Constant(true), parameter);

        var result = expr.AndContainsString(parameter, "part", "Name", false);

        var compiled = result.Compile();
        Assert.True(compiled(new TestEntity { Name = "partOfName" }));
        Assert.True(compiled(new TestEntity { Name = "part" }));
        Assert.False(compiled(new TestEntity { Name = "Other" }));
    }

    [Fact]
    public void AndContainsString_WithNormalizeTrue_FindsCaseInsensitive()
    {
        var parameter = Expression.Parameter(typeof(TestEntity), "x");
        Expression<Func<TestEntity, bool>> expr = Expression.Lambda<Func<TestEntity, bool>>(Expression.Constant(true), parameter);

        var result = expr.AndContainsString(parameter, "  TEST  ", "Name", true);

        var compiled = result.Compile();
        Assert.True(compiled(new TestEntity { Name = "MyTestValue" }));
    }

    [Fact]
    public void AndContainsString_WithInvalidPropertyPath_ReturnsOriginalExpression()
    {
        var parameter = Expression.Parameter(typeof(TestEntity), "x");
        Expression<Func<TestEntity, bool>> expr = Expression.Lambda<Func<TestEntity, bool>>(Expression.Constant(true), parameter);

        var result = expr.AndContainsString(parameter, "value", "PropertyInexistente");

        Assert.Same(expr, result);
    }

    [Fact]
    public void AndContainsString_WithNonStringProperty_ReturnsOriginalExpression()
    {
        var parameter = Expression.Parameter(typeof(TestEntity), "x");
        Expression<Func<TestEntity, bool>> expr = Expression.Lambda<Func<TestEntity, bool>>(Expression.Constant(true), parameter);

        var result = expr.AndContainsString(parameter, "value", "Id");

        Assert.Same(expr, result);
    }

    [Fact]
    public void AndEqualsString_WithNestedPropertyPath_Works()
    {
        var parameter = Expression.Parameter(typeof(TestEntity), "x");
        Expression<Func<TestEntity, bool>> expr = Expression.Lambda<Func<TestEntity, bool>>(Expression.Constant(true), parameter);

        var result = expr.AndEqualsString(parameter, "nested", "Nested.Name");

        var compiled = result.Compile();
        Assert.True(compiled(new TestEntity { Nested = new NestedTestEntity { Name = "nested" } }));
        Assert.False(compiled(new TestEntity { Nested = new NestedTestEntity { Name = "other" } }));
    }
}
