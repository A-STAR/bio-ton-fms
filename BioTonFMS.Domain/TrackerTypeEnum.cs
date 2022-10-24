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
        GalileoSkyV50,
        /// <summary>
        /// Протокол WialonIPS
        /// </summary>
        [Description("Протокол WialonIPS")]
        WialonIPS,
        /// <summary>
        /// Протокол Wialon
        /// </summary>
        [Description("Протокол Wialon")]
        Wialon,
        /// <summary>
        /// Протокол Retranslator
        /// </summary>
        [Description("Протокол Retranslator")]
        Retranslator
    }
}
