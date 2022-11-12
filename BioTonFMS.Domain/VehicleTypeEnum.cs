using System.ComponentModel;

namespace BioTonFMS.Domain
{
    /// <summary>
    /// Перечисление типов машин
    /// </summary>
    public enum VehicleTypeEnum
    {
        /// <summary>
        /// Техника, которая используется для перевозок
        /// </summary>
        [Description("Для перевозок")]
        Transport = 1,
        /// <summary>
        /// Техника, которая принимает участие в работе на полях
        /// </summary>
        [Description("Для работы на полях")]
        Agro = 2
    }
}
