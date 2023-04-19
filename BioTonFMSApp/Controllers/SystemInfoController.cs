using BioTonFMS.Common.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace BioTonFMSApp.Controllers
{
    /// <summary>
    /// Информация о системе
    /// </summary>
    [ApiController]
    [Produces("application/json")]
    public class SystemInfoController : ControllerBase
    {
        private readonly IOptions<VersionInfoOptions> _versionInfoOptions;
        private readonly ILogger<SystemInfoController> _logger;

        public SystemInfoController(
            IOptions<VersionInfoOptions> versionInfoOptions,
            ILogger<SystemInfoController> logger)
        {
            _versionInfoOptions = versionInfoOptions;
            _logger = logger;
        }

        /// <summary>
        /// Получить версию системы.
        /// </summary>
        /// <returns>Строка с номером версии</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route("system/get-version")]
        public IActionResult GetVersion()
        {
            bool? showBranchInfo = _versionInfoOptions.Value?.ShowBranchInfo;
            string versionString = "";
            if (showBranchInfo.HasValue && showBranchInfo.Value)
            {
                versionString = GetVersionWithBranchInfo(); ;
            }
            else
            {
                versionString = GetVersionString();
            }
            return Ok(versionString);
        }

        /// <summary>
        /// Получить полную информацию о текущей сборке.
        /// </summary>
        /// <returns>Строка с информацией о сборке</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route("system/get-build-info")]
        public IActionResult GetBuildInfo()
        {
            string version = GetVersionWithBranchInfo();
            return Ok(version);
        }

        private static string GetVersionString()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var versionString = version is not null ? $"{version.Major}.{version.Minor}.{version.Build}" : "";
            return versionString;
        }

        private static string GetVersionWithBranchInfo()
        {
            return Assembly
                .GetExecutingAssembly()
                .GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false)
                .Cast<AssemblyInformationalVersionAttribute>()
                .Select(e => e.InformationalVersion)
                .Single();
        }
    }
}