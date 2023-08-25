using System.ComponentModel.DataAnnotations;

namespace BioTonFMS.Infrastructure.Extensions
{
    /// <summary>
    /// Информация о значении, являющемся ключом в другой таблице
    /// Позволяет клиенту минимизировать количество походов в API
    /// </summary>
    public struct ForeignKeyValue<TKey, TValue>
    {
        /// <summary>
        /// Создает экземпляр позволяя заполнить все значения
        /// </summary>
        /// <param name="id">Идентификатор сущности</param>
        /// <param name="value">Наиболее востребованная часть значения сущности. Чаще всего это имя</param>
        public ForeignKeyValue(TKey id, TValue value)
        {
            Id = id;
            Value = value;
        }

        /// <summary>
        /// Идентификатор сущности
        /// </summary>
        [Required]
        public TKey Id { get; }
        
        /// <summary>
        /// Наиболее востребованная часть значения сущности. Чаще всего это имя
        /// </summary>
        [Required]
        public TValue Value { get; }
    }
}
