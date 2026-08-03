using CitizenGateway.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CitizenGateway.Infrastructure.Persistence;

/// <summary>
/// Gateway'in kendi PostgreSQL şeması — dış mock servislerin DB'si değildir.
/// Code-First: şema migration ile yönetilir.
/// </summary>
public sealed class GatewayDbContext : DbContext
{
    public GatewayDbContext(DbContextOptions<GatewayDbContext> options) : base(options)
    {
    }

    public DbSet<Citizen> Citizens => Set<Citizen>();
    public DbSet<ServiceRequest> ServiceRequests => Set<ServiceRequest>();
    public DbSet<AuditLogEntry> AuditLogs => Set<AuditLogEntry>();
    public DbSet<ApplicationUser> Users => Set<ApplicationUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GatewayDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
