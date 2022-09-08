namespace BioTonFMS.Infrastructure.Models
{
    /// <summary>
    /// Контракт корня агрегации.
    /// </summary>
    /// <remarks>
    /// Aggregate Root is the single Entity that controls access to the children.  The rule of Cascading Delete is
    /// sometimes cited as a good way to tell if you have a group of <see cref="IEntity"/> or <see cref="IValueObject"/>
    /// that should be an Aggregate.
    /// </remarks>
    public interface IAggregateRoot
    {
    }
}