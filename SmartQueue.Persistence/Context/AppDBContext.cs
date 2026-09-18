using Microsoft.EntityFrameworkCore;
using SmartQueue.Domain.Entities;
using System.Reflection;

namespace SmartQueue.Persistence.Context;

internal class AppDBContext : DbContext
{
    public AppDBContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var trackers = ChangeTracker.Entries<Customer>();
        foreach (var tracker in trackers)
        {
            if (tracker.State == EntityState.Added)
            { 
                tracker.Entity.CreatedAt = DateTime.UtcNow;

            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    public DbSet<Customer> Customers { get; set; }
}
