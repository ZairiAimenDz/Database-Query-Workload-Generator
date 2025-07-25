using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DatabaseWorkloadQueryGenerator.Infrastructure.Abstractions;

/// <summary>
/// Extension methods for setting up data seeding
/// </summary>
public static class DatabaseSeederExtensions
{
    /// <summary>
    /// Adds database seeding services to the service collection
    /// </summary>
    public static IServiceCollection AddDatabaseSeeding<TContext>(this IServiceCollection services)
        where TContext : DbContext
    {
        services.AddScoped<DatabaseSeeder<TContext>>();
        return services;
    }

    /// <summary>
    /// Adds a specific seeder to the service collection
    /// </summary>
    public static IServiceCollection AddSeeder<TContext, TSeeder>(this IServiceCollection services)
        where TContext : DbContext
        where TSeeder : class, IDataSeeder<TContext>
    {
        services.AddScoped<IDataSeeder<TContext>, TSeeder>();
        return services;
    }

    /// <summary>
    /// Configures the application to automatically migrate and seed the database on startup
    /// </summary>
    public static IHost MigrateAndSeedDatabase<TContext>(this WebApplication host)
        where TContext : DbContext
    {
        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<TContext>();
            var seeder = services.GetRequiredService<DatabaseSeeder<TContext>>();

            seeder.MigrateAndSeedAsync(context, services).GetAwaiter().GetResult();
        }

        return host;
    }
}