namespace BioTonFMSApp.Startup
{
    using BioTonFMS.Infrastructure.EF;
    using Microsoft.EntityFrameworkCore;

    public static class DatabaseExtensions
    {
        public static async Task ApplyMigrationsAsync(this IHost host, 
            IConfiguration configuration, 
            Action<DbMigrationOptions>? configureOptions = null)
        {
            var dbMigrationOptions = new DbMigrationOptions();
            configuration.GetSection("DbMigrationOptions").Bind(dbMigrationOptions);
            configureOptions?.Invoke(dbMigrationOptions);

            if (!dbMigrationOptions.ApplyMigrations) return;

            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var context = services.GetRequiredService<BioTonDBContext>();
                await context.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                // меняем на русский
                logger.LogError(ex, "Ошибка при создании базы данных");
            }
        }
    }
}
