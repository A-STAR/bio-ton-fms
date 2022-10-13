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
        Car = 1,
        /// <summary>
        /// Грузовой автомобиль
        /// </summary>
        Truck = 2,
        /// <summary>
        /// Комбайн
        /// </summary>
        Harvester = 3,
        /// <summary>
        /// Опрыскиватель
        /// </summary>
        Sprayer = 4,
        /// <summary>
        /// Трактор
        /// </summary>
        Tractor = 5,
        /// <summary>
        /// Бензовоз
        /// </summary>
        Tanker = 6,
        /// <summary>
        /// Tелескопический погрузчик
        /// </summary>
        Telehandler = 7,
        /// <summary>
        /// Другой транспорт
        /// </summary>
        Other = 8
    }
}
