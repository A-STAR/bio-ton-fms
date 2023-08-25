namespace BioTonFMS.Infrastructure.Persistence.Providers
{
    /// <summary>
    /// Провайдер коллекции, способной выполнять выборку сущностей по их идентификаторам. Может быть RDMS и NoSQL.
    /// </summary>
    /// <typeparam name="T">
    /// Выбираемая сущность.
    /// </typeparam>
    /// <typeparam name="TKey">
    /// Ключ выборки, может быть <see cref="int"/> или пользовательский ссылаемый тип.
    /// </typeparam>
    public interface IKeyValueProvider<T, in TKey>
        where T : class
    {
        /// <summary>
        /// Получает сущность по ключу. Красивый вариант. Если поддерживается <c>LazyLoad</c> то он должен быть включен.
        /// </summary>
        /// <param name="key">
        /// Идентификатор бизнес-сущности.
        /// </param>
        /// <returns>Искомая сущность.</returns>
        T? this[TKey key] { get; }

        /// <summary>
        /// Удаляет сущность.
        /// </summary>
        /// <param name="entity">
        /// Сущность, которую необходимо удалить.
        /// </param>
        void Remove(T entity);

        /// <summary>
        /// Добавляет сущность.
        /// </summary>
        /// <param name="entity">
        /// Сущность, которую необходимо добавить.
        /// </param>
        void Add(T entity);

        /// <summary>
        /// Обновляет сущность.
        /// </summary>
        /// <param name="entity">
        /// Сущность, которую необходимо обновить.
        /// </param>
        void Update(T entity);
    }
}