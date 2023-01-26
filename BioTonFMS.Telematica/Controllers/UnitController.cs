using AutoMapper;
using BioTonFMS.Infrastructure.EF.Repositories.Units;
using BioTonFMS.Telematica.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BioTonFMS.Telematica.Controllers
{
    /// <summary>
    /// API cервиса для работы с единицами измерения
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("/api/telematica")]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class UnitController : ControllerBase
    {
        private readonly ILogger<UnitController> _logger;
        private readonly IUnitRepository _unitRepository;
        private readonly IMapper _mapper;

        public UnitController(
            ILogger<UnitController> logger,
            IUnitRepository unitRepository,
            IMapper mapper)
        {
            _unitRepository = unitRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Возвращает список единиц измерения
        /// </summary>
        /// <response code="200">Список единиц измерения успешно возвращён</response>
        [HttpGet("units")]
        [ProducesResponseType(typeof(UnitDto[]), StatusCodes.Status200OK)]
        public IActionResult GetUnits()
        {
            try
            {
                var units = _unitRepository.GetUnits();
                var result = _mapper.Map<UnitDto[]>(units);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при возврате списка единиц измерения");
                throw;
            }
        }
    }
}