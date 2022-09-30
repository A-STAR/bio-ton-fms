using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Text.Json;

namespace BioTonApp.Controllers
{
    [ApiController]
    public class SystemInfoController : ControllerBase
    {
        private readonly ILogger<TestRepoController> _logger;

        public SystemInfoController(
            ILogger<TestRepoController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// �������� ������ �������.
        /// </summary>
        /// <returns>������ � ������� ������</returns>
        [HttpGet]
        [Route("system/get-version")]
        public string GetVersion()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            return JsonSerializer.Serialize(version != null? $"{version.Major}.{version.Minor}.{version.Build}": "");
        }

        /// <summary>
        /// �������� ������ ���������� � ������� ������.
        /// </summary>
        /// <returns>������ � ����������� � ������</returns>
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