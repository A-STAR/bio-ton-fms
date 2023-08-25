using AutoMapper;
using BioTonFMS.Infrastructure.EF.Repositories.SensorGroups;
using BioTonFMS.Telematica.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BioTonFMS.Telematica.Controllers
{
    /// <summary>
    /// API cервиса для работы с группами датчиков
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("/api/telematica")]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class SensorGroupController : ControllerBase
    {
        private readonly ILogger<SensorGroupController> _logger;
        private readonly ISensorGroupRepository _sensorGroupRepository;
        private readonly IMapper _mapper;

        public SensorGroupController(
            ILogger<SensorGroupController> logger,
            ISensorGroupRepository sensorGroupRepository,
            IMapper mapper)
        {
            _sensorGroupRepository = sensorGroupRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Возвращает список групп датчиков
        /// </summary>
        /// <response code="200">Список групп датчиков успешно возвращён</response>
        [HttpGet("sensorGroups")]
        [ProducesResponseType(typeof(SensorGroupDto[]), StatusCodes.Status200OK)]
        public IActionResult GetSensorGroups()
        {
            try
            {
                var sensorGroups = _sensorGroupRepository.GetSensorGroups();
                var result = _mapper.Map<SensorGroupDto[]>(sensorGroups);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при возврате списка групп датчиков");
                throw;
            }
        }
    }
}