using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DatabaseWorkloadQueryGenerator.Infrastructure.Abstractions;

/// <summary>
/// Service that orchestrates all registered seeders
/// </summary>
/// <typeparam name="TContext">The DbContext type</typeparam>
public class DatabaseSeeder<TContext> where TContext : DbContext
{
    private readonly IEnumerable<IDataSeeder<TContext>> _seeders;
    private readonly ILogger<DatabaseSeeder<TContext>> _logger;

    public DatabaseSeeder(
        IEnumerable<IDataSeeder<TContext>> seeders,
        ILogger<DatabaseSeeder<TContext>> logger)
    {
        _seeders = seeders;
        _logger = logger;
    }

    /// <summary>
    /// Applies all migrations and seeds data
    /// </summary>
    public async Task MigrateAndSeedAsync(TContext context, IServiceProvider serviceProvider)
    {
        try
        {
            _logger.LogInformation("Applying database migrations...");
            if ((await context.Database.GetPendingMigrationsAsync()).Any())
                await context.Database.MigrateAsync();

            // If there are model changes that we forgot to add migrations for
            if (context.Database.HasPendingModelChanges())
            {
                // Show an error
                _logger.LogError("There are model changes which have not yet been added to migrations. create a new migration to include them.");

                // Alert dev if debugging
                /*if(Debugger.IsAttached)
                    Debugger.Break();*/
            }

            _logger.LogInformation("Migrations applied successfully");

            _logger.LogInformation("Starting data seeding...");

            // Execute seeders in order
            foreach (var seeder in _seeders.OrderBy(s => s.Order))
            {
                _logger.LogInformation($"Running seeder: {seeder.GetType().Name}");
                await seeder.SeedAsync(context, serviceProvider);
            }

            _logger.LogInformation("Data seeding completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during database migration or seeding");
            throw;
        }
    }
}
