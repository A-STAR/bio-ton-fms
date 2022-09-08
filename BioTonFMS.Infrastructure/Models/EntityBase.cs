// ----------------------------------------------------------------------------
// <copyright file="EntityBase.cs" company="АО ИРТех">
// 
// АО "ИРТех", КОНФИДЕНЦИАЛЬНО.
// __________________
// 
// 2006 - 2022 АО ИРТех. 
// Все права защищены.
// 
// ПРЕДУПРЕЖДЕНИЕ:  Вся информация, находящаяся в этом файле является интеллектуальной
// собственностью компании АО "ИРТех" и ее поставщиков. Распространение и копирование
// данных материалов строго запрещено до получения письменного разрешения от компании
// АО "ИРТех".
// 
// </copyright>
// ----------------------------------------------------------------------------
namespace BioTonFMS.Infrastructure.Models
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Globalization;
    using System.Reflection;

    /// <summary>
    /// Базовая реализация бизнес-сущности.
    /// </summary>
    public abstract class EntityBase : IEntity
    {
        /// <inheritdoc />
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public virtual int Id { get; protected internal set; }

        /// <summary>
        /// Оператор равенства двух сущностей.
        /// </summary>
        /// <param name="entityA">Первая сущность.</param>
        /// <param name="entityB">Вторая сущность.</param>
        /// <returns><c>true</c>, если сущности равны.</returns>
        public static bool operator ==(EntityBase entityA, EntityBase entityB)
        {
            return Equals(entityA, entityB);
        }

        /// <summary>
        /// Оператор неравенства двух сущностей.
        /// </summary>
        /// <param name="entityA">Первая сущность.</param>
        /// <param name="entityB">Вторая сущность.</param>
        /// <returns><c>true</c>, если сущности не равны.</returns>
        public static bool operator !=(EntityBase entityA, EntityBase entityB)
        {
            return !(entityA == entityB);
        }

        /// <summary>
        /// Выполнить проверку эквивалентности двух сущностей по ключу.
        /// </summary>
        /// <param name="entityA">Первая сущность.</param>
        /// <param name="entityB">Вторая сущность.</param>
        /// <returns><c>true</c>, если сущности эквивалентны.</returns>
        /// <remarks>
        /// Метод сделан статичным для использования в Entity с <c>СompositeId().KeyReference</c>.
        /// Использование виртуальных методов приводит к воплощению объекта ключа, что в свою
        /// очередь генерирует не нужные SQL запросы.
        /// </remarks>
        public static bool Equals(IEntity<int> entityA, IEntity<int> entityB)
        {
            if (entityA == null ^ entityB == null)
            {
                return false;
            }

            if (entityA == null)
            {
                return true;
            }

            if (!entityA.GetType().IsEquivalentTo(entityA.GetType()))
            {
                return false;
            }

            if (entityA.Id == 0 && entityB.Id == 0)
            {
                return ReferenceEquals(entityA, entityB);
            }

            return entityA.Id == entityB.Id;
        }

        /// <summary>
        /// Получить hash-код по заданной сущности.
        /// </summary>
        /// <param name="entity">Сущность.</param>
        /// <returns>Hash-код.</returns>
        /// <remarks>
        /// Метод сделан статичным для использования в Entity с <c>СompositeId().KeyReference</c>.
        /// Использование виртуальных методов приводит к воплощению объекта ключа, что в свою
        /// очередь генерирует не нужные SQL запросы.
        /// </remarks>
        public static int GetHashCode(IEntity<int> entity)
        {
            if (entity == null)
            {
                return 0;
            }

            if (entity.Id == 0)
            {
                return entity.Id.GetHashCode();
            }

            return (entity.GetType().FullName + "#" + entity.Id.ToString(CultureInfo.InvariantCulture)).GetHashCode();
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Equals(this, obj as EntityBase);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return GetHashCode(this);
        }
    }
}