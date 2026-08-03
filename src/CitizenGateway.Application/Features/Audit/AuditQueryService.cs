using CitizenGateway.Application.Abstractions;
using CitizenGateway.Contracts.Audit;

namespace CitizenGateway.Application.Features.Audit;

public sealed class AuditQueryService : IAuditQueryService
{
    private readonly IAuditLogRepository _repository;

    public AuditQueryService(IAuditLogRepository repository) => _repository = repository;

    public async Task<IReadOnlyList<AuditLogDto>> GetLatestAsync(int take = 100, CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetLatestAsync(take, cancellationToken);
        return items.Select(a => new AuditLogDto(
            a.Id,
            a.UserId,
            a.UserRole,
            a.AccessedCitizenTcNo,
            a.AccessedEndpoint,
            a.Timestamp,
            a.IpAddress)).ToList();
    }
}