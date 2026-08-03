using CitizenGateway.Contracts.Audit;

namespace CitizenGateway.Application.Features.Audit;

public interface IAuditQueryService
{
    Task<IReadOnlyList<AuditLogDto>> GetLatestAsync(int take = 100, CancellationToken cancellationToken = default);
}
