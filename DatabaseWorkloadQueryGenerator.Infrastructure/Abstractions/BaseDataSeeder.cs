using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseWorkloadQueryGenerator.Infrastructure.Abstractions;

/// <summary>
/// Abstract base class for implementing entity-specific seeders
/// </summary>
/// <typeparam name="TContext">The DbContext type</typeparam>
public abstract class BaseDataSeeder<TContext> : IDataSeeder<TContext> where TContext : DbContext
{
    public abstract Task SeedAsync(TContext context, IServiceProvider serviceProvider);

    public virtual int Order => 100;
}
