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
        /// <param name="page">Page number (1-based)</param>
        /// <param name="limit">Number of items per page</param>
        /// <returns>Paginated query</returns>
        public static IQueryable<T> Paginate<T>(this IQueryable<T> query, int page, int limit)
        {
            if (page < 1) page = 1;
            if (limit < 1) limit = 20;
            if (limit > 100) limit = 100;

            return query
                .Skip((page - 1) * limit)
                .Take(limit);
        }


        /// <summary>
        /// Applies sorting to an IQueryable by property name using OrderBy string ("asc" or "desc")
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="query">Query to sort</param>
        /// <param name="sortBy">Property name to sort by (case-insensitive)</param>
        /// <param name="orderBy">Sort order: "asc" or "desc"</param>
        /// <param name="defaultSortBy">Default property name if sortBy is null or invalid</param>
        /// <returns>Sorted query</returns>
        public static IQueryable<T> SortBy<T>(
            this IQueryable<T> query,
            string? sortBy,
            string orderBy = "desc",
            string? defaultSortBy = null)
        {
            var sortDirection = orderBy?.ToLower() == "asc" ? SortDirection.Asc : SortDirection.Desc;
            return SortBy(query, sortBy, sortDirection, defaultSortBy);
        }

        /// <summary>
        /// Applies sorting to an IQueryable by property name (backward compatibility)
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
        /// Creates a PagedResult from a query using Page/Limit
        /// Optimized for large tables by executing Count and data queries separately
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="query">Query to paginate</param>
        /// <param name="page">Page number (1-based)</param>
        /// <param name="limit">Number of items per page</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>PagedResult with items and metadata</returns>
        public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
            this IQueryable<T> query,
            int page,
            int limit,
            CancellationToken cancellationToken = default)
        {
            if (page < 1) page = 1;
            if (limit < 1) limit = 20;
            if (limit > 100) limit = 100;

            // Optimize: Execute Count and data queries in parallel for better performance
            // For very large tables, Count can be slow, so we execute it separately
            var countTask = query.CountAsync(cancellationToken);
            
            // Prepare paginated query (don't execute yet)
            var paginatedQuery = query
                .Skip((page - 1) * limit)
                .Take(limit);

            // Execute both queries
            var totalCount = await countTask;
            var items = await paginatedQuery.ToListAsync(cancellationToken);

            return new PagedResult<T>(items, totalCount, page, limit);
        }

    }
}

