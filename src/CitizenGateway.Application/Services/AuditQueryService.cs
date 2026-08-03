using CitizenGateway.Application.Abstractions;
using CitizenGateway.Application.DTOs;

namespace CitizenGateway.Application.Services;

public sealed class AuditQueryService
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
