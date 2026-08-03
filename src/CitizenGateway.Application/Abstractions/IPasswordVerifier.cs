using CitizenGateway.Domain.Entities;

namespace CitizenGateway.Application.Abstractions;

/// <summary>
/// Hash doğrulama kontratı — Identity/BCrypt detayı Infrastructure'da kalır.
/// </summary>
public interface IPasswordVerifier
{
    bool Verify(ApplicationUser user, string password);
}
