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
        /// Протокол Wialon Retranslator
        /// </summary>
        [Description("Протокол Wialon Retranslator")]
        Retranslator = 2,
        /// <summary>
        /// Протокол WialonIPS
        /// </summary>
        [Description("Протокол WialonIPS")]
        WialonIPS = 3
    }
}
