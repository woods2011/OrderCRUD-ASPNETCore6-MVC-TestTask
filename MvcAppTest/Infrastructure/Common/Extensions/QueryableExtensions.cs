using System.Linq.Expressions;

namespace MvcAppTest.Infrastructure.Common.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> WhereIf<T>(
        this IQueryable<T> query,
        Expression<Func<T, bool>> predicate,
        bool condition)
    {
        return condition ? query.Where(predicate) : query;
    }
}