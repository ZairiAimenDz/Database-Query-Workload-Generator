using Microsoft.EntityFrameworkCore;

namespace DatabaseWorkloadQueryGenerator.Infrastructure.Abstractions;

/// <summary>
/// Interface for implementing entity-specific seeders
/// </summary>
/// <typeparam name="TContext">The DbContext type</typeparam>
public interface IDataSeeder<TContext> where TContext : DbContext
{
    /// <summary>
    /// Seeds data for a specific entity
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="serviceProvider">Service provider for dependency injection</param>
    Task SeedAsync(TContext context, IServiceProvider serviceProvider);

    /// <summary>
    /// Defines the order in which seeders should run (lower numbers run first)
    /// </summary>
    int Order { get; }
}
