using CitizenGateway.Domain.Enums;

namespace CitizenGateway.Contracts.Audit;

public sealed record AuditLogDto(
    Guid Id,
    Guid UserId,
    UserRole UserRole,
    string AccessedCitizenTcNo,
    string AccessedEndpoint,
    DateTimeOffset Timestamp,
    string IpAddress);
