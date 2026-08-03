using CitizenGateway.Application.Abstractions;
using CitizenGateway.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CitizenGateway.Infrastructure.Persistence.Repositories;

public sealed class AuditLogRepository : IAuditLogRepository
{
    private readonly GatewayDbContext _db;

    public AuditLogRepository(GatewayDbContext db) => _db = db;

    public async Task AddAsync(AuditLogEntry entry, CancellationToken cancellationToken = default)
    {
        await _db.AuditLogs.AddAsync(entry, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AuditLogEntry>> GetLatestAsync(
        int take = 100,
        CancellationToken cancellationToken = default)
    {
        return await _db.AuditLogs
            .AsNoTracking()
            .OrderByDescending(a => a.Timestamp)
            .Take(take)
            .ToListAsync(cancellationToken);
    }
}
