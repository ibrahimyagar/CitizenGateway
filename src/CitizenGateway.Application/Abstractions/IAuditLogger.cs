using CitizenGateway.Domain.Enums;

namespace CitizenGateway.Application.Abstractions;

/// <summary>
/// Audit yazma kontratı — middleware/filter bu interface'i çağırır; unit testte Moq Verify edilir.
/// </summary>
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
