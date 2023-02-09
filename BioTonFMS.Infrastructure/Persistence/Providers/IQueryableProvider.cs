namespace BioTonFMS.Infrastructure.Persistence.Providers
{

    /// <summary>
    /// Провайдер коллекции, способной выполнять выборку сущностей при помощи LINQ-запросов.
    /// </summary>
    /// <typeparam name="T">Тип сущности.</typeparam>
    public interface IQueryableProvider<T> : IFetchRequest<T>
    {
        /*
        /// <summary>
        /// Пометить запрос для пакетного выполнения.
        /// </summary>
        /// <param name="query">LINQ запрос.</param>
        /// <typeparam name="TQuery">Тип сущности.</typeparam>
        /// <returns>Результат выполнения запроса.</returns>
        IEnumerable<TQuery> ToFuture<TQuery>(IQueryable<TQuery> query);
        */
    }
}