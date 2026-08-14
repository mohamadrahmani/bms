using BMS.Application.Common.Filters;
using System.Globalization;
using System.Linq.Expressions;

public static class QueryableFilterExtensions
{
    public static IQueryable<T> ApplyDynamicFilters<T>(
        this IQueryable<T> query,
        List<FilterDto>? filters)
    {
        if (filters == null || filters.Count == 0)
            return query;

        foreach (var filter in filters)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.PropertyOrField(parameter, filter.Key);

            object? value = ConvertToType(filter.Value, property.Type);

            var constant = Expression.Constant(value, property.Type);
            var body = Expression.Equal(property, constant);

            var lambda = Expression.Lambda<Func<T, bool>>(body, parameter);

            query = query.Where(lambda);
        }

        return query;
    }

    private static object? ConvertToType(string? value, Type targetType)
    {
        // Nullable<T>
        Type actualType = Nullable.GetUnderlyingType(targetType) ?? targetType;

        if (string.IsNullOrWhiteSpace(value))
        {
            if (Nullable.GetUnderlyingType(targetType) != null)
                return null;

            throw new InvalidOperationException($"Value cannot be null for type {targetType.Name}");
        }

        // Enum
        if (actualType.IsEnum)
        {
            return Enum.Parse(actualType, value, ignoreCase: true);
        }

        // Guid
        if (actualType == typeof(Guid))
        {
            return Guid.Parse(value);
        }

        // DateTime
        if (actualType == typeof(DateTime))
        {
            return DateTime.Parse(value, CultureInfo.InvariantCulture);
        }

        // TimeSpan
        if (actualType == typeof(TimeSpan))
        {
            return TimeSpan.Parse(value, CultureInfo.InvariantCulture);
        }

        return Convert.ChangeType(value, actualType, CultureInfo.InvariantCulture);
    }
}