using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Text.Json;

namespace BioTonFMSApp.Controllers
{
    /// <summary>
    /// Информация о системе
    /// </summary>
    [ApiController]
    [Produces("application/json")]
    public class SystemInfoController : ControllerBase
    {
        private readonly ILogger<TestRepoController> _logger;

        public SystemInfoController(
            ILogger<TestRepoController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Получить версию системы.
        /// </summary>
        /// <returns>Строка с номером версии</returns>
        [HttpGet]
        [Route("system/get-version")]
        public object GetVersion()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var versionString = version != null ? $"{version.Major}.{version.Minor}.{version.Build}" : "";
            return Ok(versionString);
        }

        /// <summary>
        /// Получить полную информацию о текущей сборке.
        /// </summary>
        /// <returns>Строка с информацией о сборке</returns>
        [HttpGet]
        [Route("system/get-build-info")]
        public string GetBuildInfo()
        {
            var version = Assembly
                .GetExecutingAssembly()
                .GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false)
                .Cast<AssemblyInformationalVersionAttribute>()
                .Select(e => e.InformationalVersion)
                .Single();

            return version;
        }
    }
}