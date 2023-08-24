using AutoMapper;
using BioTonFMS.Infrastructure.EF.Repositories.SensorTypes;
using BioTonFMS.Telematica.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BioTonFMS.Telematica.Controllers
{
    /// <summary>
    /// API cервиса для работы с типами датчиков
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("/api/telematica")]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class SensorTypeController : ControllerBase
    {
        private readonly ILogger<SensorTypeController> _logger;
        private readonly ISensorTypeRepository _sensorTypeRepository;
        private readonly IMapper _mapper;

        public SensorTypeController(
            ILogger<SensorTypeController> logger,
            ISensorTypeRepository sensorTypeRepository,
            IMapper mapper)
        {
            _sensorTypeRepository = sensorTypeRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Возвращает список типов датчиков
        /// </summary>
        /// <response code="200">Список типов датчиков успешно возвращён</response>
        [HttpGet("sensorTypes")]
        [ProducesResponseType(typeof(SensorTypeDto[]), StatusCodes.Status200OK)]
        public IActionResult GetSensorTypes()
        {
            try
            {
                var sensorTypes = _sensorTypeRepository.GetSensorTypes();
                var result = _mapper.Map<SensorTypeDto[]>(sensorTypes);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при возврате списка типов датчиков");
                throw;
            }
        }
    }
}