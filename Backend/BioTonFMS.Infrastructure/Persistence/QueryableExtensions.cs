namespace BioTonFMS.Infrastructure.Persistence
{
    using System;
    using System.Linq;

    using BioTonFMS.Infrastructure.Persistence.Providers;

    using LinqSpecs;

    /// <summary>
    /// Расширения для спецификаций и проекций.
    /// </summary>
    public static class QueryableExtensions
    {
        /// <summary>
        /// Начинает expression получения сущностей по спецификации.
        /// </summary>
        /// <typeparam name="T">Специфицируемая сущность.</typeparam>
        /// <param name="provider">See <see cref="IQueryableProvider{T}"/>.</param>
        /// <param name="specification">Спецификация запроса.</param>
        /// <exception cref="ArgumentNullException">Переданный аргумент имеет значение null.</exception>
        /// <returns>Linq запрос.</returns>
        public static IQueryable<T> SpecifiedBy<T>(this IQueryableProvider<T> provider, Specification<T> specification)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            if (specification == null)
            {
                throw new ArgumentNullException(nameof(specification));
            }

            return provider.Linq().Where(specification);
        }
    }
}