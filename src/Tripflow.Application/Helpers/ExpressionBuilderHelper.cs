using System.Linq.Expressions;
using System.Reflection;

namespace Tripflow.Application.Helpers;

public static class ExpressionBuilderHelper
{
    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr, Expression<Func<T, bool>> other)
    {
        var parameter = expr.Parameters[0];
        var replaced = ParameterReplacer.Replace(other.Parameters[0], parameter, other.Body);
        return Expression.Lambda<Func<T, bool>>(
            Expression.AndAlso(expr.Body, replaced),
            parameter);
    }

    public static Expression<Func<T, bool>> AndEqualsString<T>(
        this Expression<Func<T, bool>> expr,
        ParameterExpression parameter,
        string? value,
        string propertyPath)
    {
        if (string.IsNullOrWhiteSpace(value))
            return expr;

        var equalsExpr = BuildEqualsExpression<T>(parameter, value.Trim(), propertyPath);
        if (equalsExpr == null)
            return expr;

        var replaced = ParameterReplacer.Replace(equalsExpr.Parameters[0], parameter, equalsExpr.Body);
        return Expression.Lambda<Func<T, bool>>(
            Expression.AndAlso(expr.Body, replaced),
            parameter);
    }

    public static Expression<Func<T, bool>> AndEquals<T, TValue>(
        this Expression<Func<T, bool>> expr,
        ParameterExpression parameter,
        TValue? value,
        string propertyPath)
        where TValue : struct
    {
        if (!value.HasValue)
            return expr;

        var equalsExpr = BuildEqualsExpression<T, TValue>(parameter, value.Value, propertyPath);
        if (equalsExpr == null)
            return expr;

        var replaced = ParameterReplacer.Replace(equalsExpr.Parameters[0], parameter, equalsExpr.Body);
        return Expression.Lambda<Func<T, bool>>(
            Expression.AndAlso(expr.Body, replaced),
            parameter);
    }

    public static Expression<Func<T, bool>> AndEqualsDate<T>(
        this Expression<Func<T, bool>> expr,
        ParameterExpression parameter,
        DateTime? value,
        string propertyPath)
    {
        if (!value.HasValue)
            return expr;

        var equalsExpr = BuildEqualsExpression<T, DateTime>(parameter, value.Value, propertyPath);
        if (equalsExpr == null)
            return expr;

        var replaced = ParameterReplacer.Replace(equalsExpr.Parameters[0], parameter, equalsExpr.Body);
        return Expression.Lambda<Func<T, bool>>(
            Expression.AndAlso(expr.Body, replaced),
            parameter);
    }

    public static Expression<Func<T, bool>> AndEqualsDate<T>(
        this Expression<Func<T, bool>> expr,
        ParameterExpression parameter,
        DateOnly? value,
        string propertyPath)
    {
        if (!value.HasValue)
            return expr;

        var equalsExpr = BuildEqualsExpression<T, DateOnly>(parameter, value.Value, propertyPath);
        if (equalsExpr == null)
            return expr;

        var replaced = ParameterReplacer.Replace(equalsExpr.Parameters[0], parameter, equalsExpr.Body);
        return Expression.Lambda<Func<T, bool>>(
            Expression.AndAlso(expr.Body, replaced),
            parameter);
    }

    public static Expression<Func<T, bool>> OrContainsNormalized<T>(
        ParameterExpression parameter,
        string? search,
        bool normalize,
        params string[] propertyPaths)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Expression.Lambda<Func<T, bool>>(Expression.Constant(true), parameter);

        var searchValue = normalize ? search.Trim().ToLower() : search.Trim();
        var type = typeof(T);

        Expression? orExpr = null;

        foreach (var path in propertyPaths)
        {
            var propertyAccess = GetPropertyAccess(parameter, type, path);
            if (propertyAccess == null || propertyAccess.Type != typeof(string))
                continue;

            Expression containsExpr;
            if (normalize)
            {
                var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes)!;
                var containsMethod = typeof(string).GetMethod("Contains", [typeof(string)])!;
                var toLowerCall = Expression.Call(propertyAccess, toLowerMethod);
                var searchConstant = Expression.Constant(searchValue);
                containsExpr = Expression.Call(toLowerCall, containsMethod, searchConstant);
            }
            else
            {
                var containsMethod = typeof(string).GetMethod("Contains", [typeof(string)])!;
                containsExpr = Expression.Call(propertyAccess, containsMethod, Expression.Constant(searchValue));
            }

            orExpr = orExpr == null ? containsExpr : Expression.OrElse(orExpr, containsExpr);
        }

        if (orExpr == null)
            return Expression.Lambda<Func<T, bool>>(Expression.Constant(true), parameter);

        return Expression.Lambda<Func<T, bool>>(orExpr, parameter);
    }

    public static Expression<Func<T, bool>> OrContains<T>(
        ParameterExpression parameter,
        string? search,
        params string[] propertyPaths)
    {
        return OrContainsNormalized<T>(parameter, search, false, propertyPaths);
    }

    public static Expression<Func<T, bool>> AndContainsString<T>(
        this Expression<Func<T, bool>> expr,
        ParameterExpression parameter,
        string? value,
        string propertyPath,
        bool normalize = false)
    {
        if (string.IsNullOrWhiteSpace(value))
            return expr;

        var searchValue = normalize ? value.Trim().ToLower() : value.Trim();
        var propertyAccess = GetPropertyAccess(parameter, typeof(T), propertyPath);
        if (propertyAccess == null || propertyAccess.Type != typeof(string))
            return expr;

        Expression containsExpr;
        if (normalize)
        {
            var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes)!;
            var containsMethod = typeof(string).GetMethod("Contains", [typeof(string)])!;
            var toLowerCall = Expression.Call(propertyAccess, toLowerMethod);
            containsExpr = Expression.Call(toLowerCall, containsMethod, Expression.Constant(searchValue));
        }
        else
        {
            var containsMethod = typeof(string).GetMethod("Contains", [typeof(string)])!;
            containsExpr = Expression.Call(propertyAccess, containsMethod, Expression.Constant(searchValue));
        }

        var lambda = Expression.Lambda<Func<T, bool>>(containsExpr, parameter);
        var replaced = ParameterReplacer.Replace(lambda.Parameters[0], parameter, lambda.Body);
        return Expression.Lambda<Func<T, bool>>(
            Expression.AndAlso(expr.Body, replaced),
            parameter);
    }

    private static Expression<Func<T, bool>>? BuildEqualsExpression<T>(ParameterExpression parameter, string value, string propertyPath)
    {
        var propertyAccess = GetPropertyAccess(parameter, typeof(T), propertyPath);
        if (propertyAccess == null)
            return null;

        var constant = Expression.Constant(value);
        var equals = Expression.Equal(propertyAccess, constant);
        return Expression.Lambda<Func<T, bool>>(equals, parameter);
    }

    private static Expression<Func<T, bool>>? BuildEqualsExpression<T, TValue>(ParameterExpression parameter, TValue value, string propertyPath)
    {
        var propertyAccess = GetPropertyAccess(parameter, typeof(T), propertyPath);
        if (propertyAccess == null)
            return null;

        var constant = Expression.Constant(value, typeof(TValue));
        var equals = Expression.Equal(propertyAccess, constant);
        return Expression.Lambda<Func<T, bool>>(equals, parameter);
    }

    private static Expression? GetPropertyAccess(Expression parameter, Type type, string propertyPath)
    {
        var parts = propertyPath.Split('.');
        Expression? current = parameter;
        Type currentType = type;

        foreach (var part in parts)
        {
            var property = currentType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(p => string.Equals(p.Name, part, StringComparison.OrdinalIgnoreCase));
            if (property == null)
                return null;

            current = Expression.Property(current, property);
            currentType = property.PropertyType;
        }

        return current;
    }

    private sealed class ParameterReplacer : ExpressionVisitor
    {
        private readonly ParameterExpression _oldParameter;
        private readonly ParameterExpression _newParameter;

        private ParameterReplacer(ParameterExpression oldParameter, ParameterExpression newParameter)
        {
            _oldParameter = oldParameter;
            _newParameter = newParameter;
        }

        public static Expression Replace(ParameterExpression oldParameter, ParameterExpression newParameter, Expression body)
        {
            return new ParameterReplacer(oldParameter, newParameter).Visit(body);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == _oldParameter ? _newParameter : base.VisitParameter(node);
        }
    }
}
