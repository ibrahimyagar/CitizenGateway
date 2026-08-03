using CitizenGateway.Application.Abstractions;
using CitizenGateway.Domain.Entities;
using CitizenGateway.Domain.Enums;

namespace CitizenGateway.Application.Features.Audit;

/// <summary>
/// AuditLogEntry üretir ve repository'ye yazar — test edilebilir Application servisi.
/// </summary>
public sealed class AuditLogger : IAuditLogger
{
    private readonly IAuditLogRepository _repository;

    public AuditLogger(IAuditLogRepository repository) => _repository = repository;

    public Task LogAccessAsync(
        Guid userId,
        UserRole userRole,
        string accessedCitizenTcNo,
        string accessedEndpoint,
        string ipAddress,
        CancellationToken cancellationToken = default)
    {
        var entry = AuditLogEntry.Create(
            userId,
            userRole,
            accessedCitizenTcNo,
            accessedEndpoint,
            ipAddress);

        return _repository.AddAsync(entry, cancellationToken);
    }
}
