using AutoMapper;
using BioTonFMS.Infrastructure.EF.Repositories.FuelTypes;
using BioTonFMS.Telematica.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BioTonFMS.Telematica.Controllers
{
    /// <summary>
    /// API cервиса для работы с типом топлива
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("/api/telematica")]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class FuelTypeController : ControllerBase
    {
        private readonly ILogger<FuelTypeController> _logger;
        private readonly IFuelTypeRepository _fuelTypeRepository;
        private readonly IMapper _mapper;

        public FuelTypeController(
            ILogger<FuelTypeController> logger,
            IFuelTypeRepository fuelTypeRepository,
            IMapper mapper)
        {
            _fuelTypeRepository = fuelTypeRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Возвращает список типов топлива
        /// </summary>
        /// <response code="200">Список типов топлива успешно возвращён</response>
        [HttpGet("fueltypes")]
        [ProducesResponseType(typeof(FuelTypeDto[]), StatusCodes.Status200OK)]
        public IActionResult GetFuelTypes()
        {
            try
            {
                var fuelTypes = _fuelTypeRepository.GetFuelTypes();
                var result = _mapper.Map<FuelTypeDto[]>(fuelTypes);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при возврате списка типов топлива");
                throw;
            }
        }
    }
}