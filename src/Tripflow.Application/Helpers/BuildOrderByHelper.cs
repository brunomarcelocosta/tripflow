using System.Linq.Expressions;
using System.Reflection;

namespace Tripflow.Application.Helpers;

public static class BuildOrderByHelper
{
    private static readonly string[] DefaultPropertyNames = ["CreatedAtUtc", "Id"];

    public static Expression<Func<T, object>>? BuildOrderBy<T>(string? orderBy)
        where T : class
    {
        var propertyName = orderBy?.Trim();
        var property = GetProperty<T>(propertyName);

        if (property == null)
            return null;

        return BuildExpression<T>(property);
    }

    private static PropertyInfo? GetProperty<T>(string? propertyName) where T : class
    {
        var type = typeof(T);
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        if (!string.IsNullOrEmpty(propertyName))
        {
            var prop = properties.FirstOrDefault(p =>
                string.Equals(p.Name, propertyName, StringComparison.OrdinalIgnoreCase));

            if (prop != null)
                return prop;
        }

        foreach (var defaultName in DefaultPropertyNames)
        {
            var prop = properties.FirstOrDefault(p =>
                string.Equals(p.Name, defaultName, StringComparison.OrdinalIgnoreCase));

            if (prop != null)
                return prop;
        }

        return properties.FirstOrDefault();
    }

    private static Expression<Func<T, object>> BuildExpression<T>(PropertyInfo property) where T : class
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var propertyAccess = Expression.Property(parameter, property);
        var convert = Expression.Convert(propertyAccess, typeof(object));

        return Expression.Lambda<Func<T, object>>(convert, parameter);
    }
}

