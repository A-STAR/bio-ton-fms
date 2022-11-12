using System.ComponentModel;

namespace BioTonFMS.Domain
{
    /// <summary>
    /// Перечисление подтипов машин
    /// </summary>
    public enum VehicleSubTypeEnum
    {
        /// <summary>
        /// Бензовоз
        /// </summary>
        [Description("Бензовоз")]
        Tanker = 1,
        /// <summary>
        /// Грузовой автомобиль
        /// </summary>
        [Description("Грузовой автомобиль")]
        Truck = 2,
        /// <summary>
        /// Другой транспорт
        /// </summary>
        [Description("Другой транспорт")]
        Other = 3,
        /// <summary>
        /// Комбайн
        /// </summary>
        [Description("Комбайн")]
        Harvester = 4,
        /// <summary>
        /// Легковой автомобиль
        /// </summary>
        [Description("Легковой автомобиль")]
        Car = 5,
        /// <summary>
        /// Опрыскиватель
        /// </summary>
        [Description("Опрыскиватель")]
        Sprayer = 6,
        /// <summary>
        /// Tелескопический погрузчик
        /// </summary>
        [Description("Tелескопический погрузчик")]
        Telehandler = 7,
        /// <summary>
        /// Трактор
        /// </summary>
        [Description("Трактор")]
        Tractor = 8
    }
}
