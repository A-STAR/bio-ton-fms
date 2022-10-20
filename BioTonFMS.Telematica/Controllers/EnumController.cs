using BioTonFMS.Domain;
using BioTonFMS.Domain.Extensions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


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
        public IActionResult GetVehicleTypeEnum()
        {
            var pairsList = new List<KeyValuePair>();

            foreach (var key in (VehicleTypeEnum[])Enum.GetValues(typeof(VehicleTypeEnum)))
            {
                var pair = new KeyValuePair
                {
                    Key = key.ToString(),
                    Value = key.GetDescription() ?? ""
                };
                pairsList.Add(pair);
            }
            return Ok(pairsList.OrderBy(p => p.Value).ToArray());
        }

        /// <summary>
        /// Пара ключ-значение
        /// </summary>
        public class KeyValuePair
        {
            /// <summary>
            /// Ключ
            /// </summary>
            public string Key { get; set; } = "";
            /// <summary>
            /// Значение
            /// </summary>
            public string Value { get; set; } = "";
        }
    }
}
