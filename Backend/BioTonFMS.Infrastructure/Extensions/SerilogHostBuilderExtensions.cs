using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Serilog;

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
