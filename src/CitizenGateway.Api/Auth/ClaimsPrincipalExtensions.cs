using System.Security.Claims;
using CitizenGateway.Domain.Enums;

namespace CitizenGateway.Api.Auth;

/// <summary>
/// Controller'ların ClaimsPrincipal'dan güvenli şekilde kimlik okuması.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var raw = user.FindFirstValue(GatewayClaimTypes.UserId)
            ?? user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(raw, out var id))
            throw new InvalidOperationException("JWT içinde kullanıcı kimliği (uid) yok.");

        return id;
    }

    public static UserRole GetUserRole(this ClaimsPrincipal user)
    {
        var role = user.FindFirstValue(ClaimTypes.Role)
            ?? throw new InvalidOperationException("JWT içinde rol claim'i yok.");

        return Enum.Parse<UserRole>(role);
    }

    public static string? GetLinkedTcNo(this ClaimsPrincipal user) =>
        user.FindFirstValue(GatewayClaimTypes.TcNo);
}
