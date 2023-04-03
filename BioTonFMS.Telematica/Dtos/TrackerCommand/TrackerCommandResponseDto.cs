using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTonFMS.Telematica.Dtos.TrackerCommand
{
    /// <summary>
    /// Ответ на команду для трекера
    /// </summary>
    public class TrackerCommandResponseDto
    {
        /// <summary>
        /// строка с текстом ответа трекера
        /// </summary>
        public string CommandResponse { get; set; } = string.Empty;
    }
}
