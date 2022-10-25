using System.ComponentModel;

namespace BioTonFMS.Domain
{
    /// <summary>
    /// Перечисление подтипов машин
    /// </summary>
    public enum VehicleSubTypeEnum
    {
        /// <summary>
        /// Легковой автомобиль
        /// </summary>
        [Description("Легковой автомобиль")]
        Car = 1,
        /// <summary>
        /// Грузовой автомобиль
        /// </summary>
        [Description("Грузовой автомобиль")]
        Truck = 2,
        /// <summary>
        /// Комбайн
        /// </summary>
        [Description("Комбайн")]
        Harvester = 3,
        /// <summary>
        /// Опрыскиватель
        /// </summary>
        [Description("Опрыскиватель")]
        Sprayer = 4,
        /// <summary>
        /// Трактор
        /// </summary>
        [Description("Трактор")]
        Tractor = 5,
        /// <summary>
        /// Бензовоз
        /// </summary>
        [Description("Бензовоз")]
        Tanker = 6,
        /// <summary>
        /// Tелескопический погрузчик
        /// </summary>
        [Description("Tелескопический погрузчик")]
        Telehandler = 7,
        /// <summary>
        /// Другой транспорт
        /// </summary>
        [Description("Другой транспорт")]
        Other = 8
    }
}
