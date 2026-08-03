using CitizenGateway.Domain.Entities;

namespace CitizenGateway.Application.Abstractions;

public interface IAuditLogRepository
{
    Task AddAsync(AuditLogEntry entry, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AuditLogEntry>> GetLatestAsync(int take = 100, CancellationToken cancellationToken = default);
}
