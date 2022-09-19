using BioTonFMS.Infrastructure.EF;
using BioTonFMS.Migrations;

namespace BioTonFMSApp.Startup
{
    public static class DatabaseExnensions
    {
        public static void ApplyMigrations(this IHost host, IConfiguration configuration, Action<DbOptions>? configureOptions = null)
        {
            var dbOptions = new DbOptions();
            configuration.GetSection("Db").Bind(dbOptions);
            configureOptions?.Invoke(dbOptions);

            if (!dbOptions.ApplyMigrations) return;

            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            
            try
            {
                var context = services.GetRequiredService<BioTonDBContext>();
                DbMigrator.Apply(context);
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred creating the DB");
            }
        }
    }
}
