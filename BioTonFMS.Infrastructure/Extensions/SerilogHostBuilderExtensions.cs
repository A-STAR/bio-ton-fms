using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;

namespace BioTonFMS.Infrastructure.Extensions
{
    public static class SerilogHostBuilderExtensions
    {
        public static WebApplicationBuilder ConfigureSerilog(
            this WebApplicationBuilder builder)
        {
            var logger = new LoggerConfiguration()
              .ReadFrom.Configuration(builder.Configuration)
              .Enrich.FromLogContext()
              .CreateLogger();

            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog(logger);
            
            return builder;
        }
    }
}
