namespace BioTonFMS.Infrastructure.Models
{
    /// <summary>
    /// Контракт бизнес сущности со стандартным типом идентификатора.
    /// </summary>
    public interface IEntity : IEntity<int>
    {
    }
}