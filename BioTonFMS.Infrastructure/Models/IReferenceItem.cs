namespace BioTonFMS.Infrastructure.Models
{
    /// <summary>
    /// Контракт элемента справочника.
    /// </summary>
    public interface IReferenceItem
    {
        /// <summary>
        /// Идентификатор элемента справочника.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Наименование элемента справочника.
        /// </summary>
        string Name { get; }
    }
}