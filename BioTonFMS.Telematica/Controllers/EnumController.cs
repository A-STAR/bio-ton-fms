using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KeyValuePair = BioTonFMS.Infrastructure.Extensions.KeyValuePair;

namespace BioTonFMS.Telematica.Controllers
{
    /// <summary>
    /// API cервиса возвращающего строковые представления перечислений
    /// </summary>
    [ApiController]
    [Route("/api/telematica/enums")]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class EnumController : ControllerBase
    {
        private readonly ILogger<EnumController> _logger;

        public EnumController(
            ILogger<EnumController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Возвращает строковые представления для перечисления VehicleTypeEnum
        /// </summary>
        /// <response code="200">Список успешно возвращён</response>
        [HttpGet("VehicleTypeEnum")]
        [ProducesResponseType(typeof(KeyValuePair[]), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetVehicleTypeEnum() => Ok(EnumExtension.GetKeyValuePairs<VehicleTypeEnum>());

        /// <summary>
        /// Возвращает строковые представления для перечисления TrackerTypeEnum
        /// </summary>
        /// <response code="200">Список успешно возвращён</response>
        [HttpGet("TrackerTypeEnum")]
        [ProducesResponseType(typeof(KeyValuePair[]), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTrackerTypeEnum() => Ok(EnumExtension.GetKeyValuePairs<TrackerTypeEnum>());

        /// <summary>
        /// Возвращает строковые представления для перечисления VehicleSubTypeEnum
        /// </summary>
        /// <response code="200">Список успешно возвращён</response>
        [HttpGet("VehicleSubTypeEnum")]
        [ProducesResponseType(typeof(KeyValuePair[]), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetVehicleSubTypeEnum() => Ok(EnumExtension.GetKeyValuePairs<VehicleSubTypeEnum>());
    }
}
