using System.ComponentModel;

namespace BioTonFMS.Domain
{
    /// <summary>
    /// Тип протокола, поддерживаемый трекером
    /// </summary>
    public enum TrackerTypeEnum
    {
        /// <summary>
        /// Протокол GalileoSkyV50
        /// </summary>
        [Description("Протокол GalileoSkyV50")]
        GalileoSkyV50 = 1,
        /// <summary>
        /// Протокол WialonIPS
        /// </summary>
        [Description("Протокол WialonIPS")]
        WialonIPS = 2,
        /// <summary>
        /// Протокол Wialon
        /// </summary>
        [Description("Протокол Wialon")]
        Wialon = 3,
        /// <summary>
        /// Протокол Retranslator
        /// </summary>
        [Description("Протокол Retranslator")]
        Retranslator = 4
    }
}
