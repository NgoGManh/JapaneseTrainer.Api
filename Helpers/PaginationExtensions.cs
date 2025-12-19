using System.Linq.Expressions;
using JapaneseTrainer.Api.DTOs.Common;
using Microsoft.EntityFrameworkCore;

namespace JapaneseTrainer.Api.Helpers
{
    /// <summary>
    /// Extension methods for pagination and sorting
    /// </summary>
    public static class PaginationExtensions
    {
        /// <summary>
        /// Applies pagination to an IQueryable
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="query">Query to paginate</param>
        /// <param name="pageNumber">Page number (1-based)</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <returns>Paginated query</returns>
        public static IQueryable<T> Paginate<T>(this IQueryable<T> query, int pageNumber, int pageSize)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 100) pageSize = 100;

            return query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
        }

        /// <summary>
        /// Applies sorting to an IQueryable by property name
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="query">Query to sort</param>
        /// <param name="sortBy">Property name to sort by (case-insensitive)</param>
        /// <param name="sortDirection">Sort direction</param>
        /// <param name="defaultSortBy">Default property name if sortBy is null or invalid</param>
        /// <returns>Sorted query</returns>
        public static IQueryable<T> SortBy<T>(
            this IQueryable<T> query,
            string? sortBy,
            SortDirection sortDirection = SortDirection.Asc,
            string? defaultSortBy = null)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
            {
                sortBy = defaultSortBy;
            }

            if (string.IsNullOrWhiteSpace(sortBy))
            {
                return query;
            }

            var propertyName = sortBy.Trim();
            var property = typeof(T).GetProperty(
                propertyName,
                System.Reflection.BindingFlags.IgnoreCase |
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Instance);

            if (property == null)
            {
                // If property not found, use default or return unsorted
                if (!string.IsNullOrWhiteSpace(defaultSortBy))
                {
                    propertyName = defaultSortBy;
                    property = typeof(T).GetProperty(
                        propertyName,
                        System.Reflection.BindingFlags.IgnoreCase |
                        System.Reflection.BindingFlags.Public |
                        System.Reflection.BindingFlags.Instance);
                }

                if (property == null)
                {
                    return query;
                }
            }

            var parameter = Expression.Parameter(typeof(T), "x");
            var propertyAccess = Expression.Property(parameter, property);
            var lambda = Expression.Lambda(propertyAccess, parameter);

            var methodName = sortDirection == SortDirection.Desc ? "OrderByDescending" : "OrderBy";
            var resultExpression = Expression.Call(
                typeof(Queryable),
                methodName,
                new Type[] { typeof(T), property.PropertyType },
                query.Expression,
                Expression.Quote(lambda));

            return query.Provider.CreateQuery<T>(resultExpression);
        }

        /// <summary>
        /// Applies sorting with a custom expression
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <typeparam name="TKey">Sort key type</typeparam>
        /// <param name="query">Query to sort</param>
        /// <param name="keySelector">Expression to select the sort key</param>
        /// <param name="sortDirection">Sort direction</param>
        /// <returns>Sorted query</returns>
        public static IQueryable<T> SortBy<T, TKey>(
            this IQueryable<T> query,
            Expression<Func<T, TKey>> keySelector,
            SortDirection sortDirection = SortDirection.Asc)
        {
            return sortDirection == SortDirection.Desc
                ? query.OrderByDescending(keySelector)
                : query.OrderBy(keySelector);
        }

        /// <summary>
        /// Creates a PagedResult from a query
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="query">Query to paginate</param>
        /// <param name="pageNumber">Page number (1-based)</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>PagedResult with items and metadata</returns>
        public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
            this IQueryable<T> query,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 100) pageSize = 100;

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<T>(items, totalCount, pageNumber, pageSize);
        }
    }
}

