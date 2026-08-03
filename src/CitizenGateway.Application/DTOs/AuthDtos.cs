using CitizenGateway.Domain.Enums;

namespace CitizenGateway.Application.DTOs;

public sealed record LoginRequestDto(string Username, string Password);

/// <summary>
/// Kimlik doğrulama sonucu — JWT üretimi Api katmanında yapılır (issuer/secret orada).
/// </summary>
public sealed record AuthUserDto(
    Guid UserId,
    string Username,
    UserRole Role,
    string? LinkedCitizenTcNo);

public sealed record AuditLogDto(
    Guid Id,
    Guid UserId,
    UserRole UserRole,
    string AccessedCitizenTcNo,
    string AccessedEndpoint,
    DateTimeOffset Timestamp,
    string IpAddress);
