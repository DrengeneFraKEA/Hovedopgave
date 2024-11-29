using System.Linq.Expressions;

namespace Hovedopgave.Server.Utils
{
    public static class StatisticsFilter
    {
        public static IQueryable<T> ApplyDateRangeFilter<T>(
            IQueryable<T> query,
            Expression<Func<T, DateTime>> dateSelector,
            DateTime? fromDate,
            DateTime? toDate)
        {
            // Apply the fromDate filter
            if (fromDate.HasValue)
            {
                query = query.Where(dateSelector.Body.Type == typeof(DateTime) ?
                    Expression.Lambda<Func<T, bool>>(
                        Expression.GreaterThanOrEqual(dateSelector.Body, Expression.Constant(fromDate.Value)),
                        dateSelector.Parameters)
                    : Expression.Lambda<Func<T, bool>>(
                        Expression.GreaterThanOrEqual(dateSelector.Body, Expression.Constant(fromDate.Value)),
                        dateSelector.Parameters));
            }

            // Apply the toDate filter
            if (toDate.HasValue)
            {
                query = query.Where(dateSelector.Body.Type == typeof(DateTime) ?
                    Expression.Lambda<Func<T, bool>>(
                        Expression.LessThanOrEqual(dateSelector.Body, Expression.Constant(toDate.Value)),
                        dateSelector.Parameters)
                    : Expression.Lambda<Func<T, bool>>(
                        Expression.LessThanOrEqual(dateSelector.Body, Expression.Constant(toDate.Value)),
                        dateSelector.Parameters));
            }

            return query;
        }
    }
}
