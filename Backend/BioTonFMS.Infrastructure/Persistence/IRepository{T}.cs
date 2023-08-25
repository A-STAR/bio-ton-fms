namespace BioTonFMS.Infrastructure.Persistence
{
    using BioTonFMS.Infrastructure.Models;

    /// <summary>
    /// Упрощенный вариант репозитория, когда в качестве ключа используется тип <see cref="int"/>.
    /// </summary>
    /// <typeparam name="T">
    /// Искомая сущность.
    /// </typeparam>
    public interface IRepository<T> : IRepository<T, int>
        where T : IEntity, IAggregateRoot
    {
    }
}