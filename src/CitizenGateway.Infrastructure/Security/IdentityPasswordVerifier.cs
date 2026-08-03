using CitizenGateway.Application.Abstractions;
using CitizenGateway.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace CitizenGateway.Infrastructure.Security;

/// <summary>
/// ASP.NET Identity PasswordHasher sarmalayıcısı — hash algoritması Infrastructure detayı.
/// </summary>
public sealed class IdentityPasswordVerifier : IPasswordVerifier
{
    private readonly PasswordHasher<ApplicationUser> _hasher = new();

    public bool Verify(ApplicationUser user, string password) =>
        _hasher.VerifyHashedPassword(user, user.PasswordHash, password)
            is PasswordVerificationResult.Success
            or PasswordVerificationResult.SuccessRehashNeeded;
}
