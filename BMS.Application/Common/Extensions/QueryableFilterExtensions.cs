using BMS.Application.Common.Filters;
using System.Linq.Expressions;

public static class QueryableFilterExtensions
{
    public static IQueryable<T> ApplyDynamicFilters<T>(
        this IQueryable<T> query,
        List<FilterDto>? filters)
    {
        if (filters == null || !filters.Any())
            return query;

        foreach (var filter in filters)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.PropertyOrField(parameter, filter.Key);

            // دریافت نوع فیلد (مثلاً Guid یا string)
            Type propertyType = property.Type;

            object? typedValue;

            // هندلینگ ویژه برای Guid
            if (propertyType == typeof(Guid))
            {
                typedValue = Guid.Parse(filter.Value);
            }
            // هندلینگ ویژه برای Guid? (Nullable)
            else if (propertyType == typeof(Guid?))
            {
                typedValue = string.IsNullOrEmpty(filter.Value) ? null : Guid.Parse(filter.Value);
            }
            else
            {
                // برای سایر موارد مثل string, int و ...
                typedValue = Convert.ChangeType(filter.Value, propertyType);
            }

            var constant = Expression.Constant(typedValue, propertyType);
            var body = Expression.Equal(property, constant);

            var lambda = Expression.Lambda<Func<T, bool>>(body, parameter);

            query = query.Where(lambda);
        }

        return query;
    }
}
