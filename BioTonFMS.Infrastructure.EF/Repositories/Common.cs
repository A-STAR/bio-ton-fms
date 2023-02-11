using System.Linq.Expressions;
using BioTonFMS.Infrastructure.EF.Models;
using BioTonFMS.Infrastructure.EF.Models.Filters;
using BioTonFMS.Infrastructure.EF.Repositories.Models.Filters;
using BioTonFMS.Infrastructure.Models;
using BioTonFMS.Infrastructure.Utils.Builders;

namespace BioTonFMS.Infrastructure.EF.Repositories;

internal static class Common
{
    internal static IQueryable<TEntity> SetSortDirection<TEntity, TKey>(
        this IQueryable<TEntity> entities,
        SortDirection? direction,
        Expression<Func<TEntity, TKey>> property) where TEntity : EntityBase =>
        direction switch
        {
            SortDirection.Ascending => entities.OrderBy(property),
            SortDirection.Descending => entities.OrderByDescending(property),
            _ => entities
        };

    internal static Expression<Func<T, bool>> SetPredicate<T>(
        Expression<Func<T, bool>>? vehiclePredicate,
        Expression<Func<T, bool>> customPredicate) =>
        vehiclePredicate == null ? customPredicate : vehiclePredicate.And(customPredicate);
}