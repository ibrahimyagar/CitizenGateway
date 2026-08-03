using CitizenGateway.Domain.Enums;

namespace CitizenGateway.Application.Features.Audit;

public interface IAuditLogger
{
    Task LogAccessAsync(
        Guid userId,
        UserRole userRole,
        string accessedCitizenTcNo,
        string accessedEndpoint,
        string ipAddress,
        CancellationToken cancellationToken = default);
}
