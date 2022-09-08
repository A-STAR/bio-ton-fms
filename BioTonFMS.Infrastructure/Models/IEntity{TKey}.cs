namespace BioTonFMS.Infrastructure.Models
{
    /// <summary>
    /// Контракт бизнес-сущности.
    /// </summary>
    /// <typeparam name="TKey">
    /// The key defining characteristic of an Entity is that it has an <typeparamref name="TKey"/>Identity type. 
    /// </typeparam>
    /// <remarks>
    /// Identity – is unique within the system, and no other Entity, no matter how similar is the same  Entity unless it
    /// has the same Identity. Whichever way you choose to represent it, an Entity is  defined by having an Identity.
    /// Remove Setters only, and make Getters representative of the  outside “shape” of the Entity. Entities can hold
    /// references to any <see cref="IAggregateRoot"/> , but never to any other Entity or 
    /// <see cref="IValueObject"/> within the Aggregate.
    /// </remarks>
    public interface IEntity<out TKey>
    {
        /// <summary>
        /// Идентификатор бизнес-сущности.
        /// </summary>
        TKey Id { get; }
    }
}