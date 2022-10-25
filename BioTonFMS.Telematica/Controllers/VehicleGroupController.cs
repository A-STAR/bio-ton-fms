using AutoMapper;
using BioTonFMS.Infrastructure.EF.Repositories.VehicleGroups;
using BioTonFMS.Telematica.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BioTonFMS.Telematica.Controllers
{
    /// <summary>
    /// API cервиса для работы с группами машин
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("/api/telematica")]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class VehicleGroupController : ControllerBase
    {
        private readonly ILogger<VehicleGroupController> _logger;
        private readonly IVehicleGroupRepository _vehicleGroupRepository;
        private readonly IMapper _mapper;

        public VehicleGroupController(
            ILogger<VehicleGroupController> logger,
            IVehicleGroupRepository vehicleGroupRepository,
            IMapper mapper)
        {
            _vehicleGroupRepository = vehicleGroupRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Возвращает список групп машин
        /// </summary>
        /// <response code="200">Список групп машин успешно возвращён</response>
        [HttpGet("vehiclegroups")]
        [ProducesResponseType(typeof(VehicleGroupDto[]), StatusCodes.Status200OK)]
        public IActionResult GetVehicleGroups()
        {
            try
            {
                var vehicleGroups = _vehicleGroupRepository.GetVehicleGroups();
                var result = _mapper.Map<VehicleGroupDto[]>(vehicleGroups);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при возврате списка машин");
                throw;
            }
        }
    }
}