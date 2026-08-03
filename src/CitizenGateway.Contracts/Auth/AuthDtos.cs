using CitizenGateway.Domain.Enums;

namespace CitizenGateway.Contracts.Auth;

/// <summary>
/// Personel: Identifier = kurumsal e-posta.
/// Vatandaş: Identifier = T.C. kimlik no.
/// </summary>
public sealed record LoginRequestDto(
    LoginPortal Portal,
    string Identifier,
    string Password);

public sealed record AuthUserDto(
    Guid UserId,
    string Username,
    string DisplayName,
    UserRole Role,
    string? LinkedCitizenTcNo);

public sealed record LoginResponseDto(
    string AccessToken,
    DateTimeOffset ExpiresAt,
    string Username,
    string DisplayName,
    string Role,
    string? LinkedCitizenTcNo);
