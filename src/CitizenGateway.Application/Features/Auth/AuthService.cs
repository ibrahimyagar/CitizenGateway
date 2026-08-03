using System.Net.Mail;
using CitizenGateway.Application.Abstractions;
using CitizenGateway.Contracts.Auth;
using CitizenGateway.Domain.Enums;
using CitizenGateway.Domain.Exceptions;
using CitizenGateway.Domain.ValueObjects;

namespace CitizenGateway.Application.Features.Auth;

/// <summary>
/// Portal + kimlik + şifre doğrular; JWT üretmez (Api'nin işi).
/// </summary>
public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IPasswordVerifier _passwordVerifier;

    public AuthService(IUserRepository users, IPasswordVerifier passwordVerifier)
    {
        _users = users;
        _passwordVerifier = passwordVerifier;
    }

    public async Task<AuthUserDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Identifier) || string.IsNullOrWhiteSpace(request.Password))
            throw new DomainValidationException("Kimlik bilgisi ve şifre zorunludur.");

        if (!Enum.IsDefined(request.Portal))
            throw new DomainValidationException("Geçersiz giriş portalı.");

        var identifier = NormalizeIdentifier(request.Portal, request.Identifier);

        var user = await _users.GetByUsernameAsync(identifier, cancellationToken);
        if (user is null || !_passwordVerifier.Verify(user, request.Password))
            throw new DomainValidationException("Kimlik bilgisi veya şifre hatalı.");

        var expectedRole = request.Portal == LoginPortal.Personel ? UserRole.Personel : UserRole.Vatandas;
        if (user.Role != expectedRole)
            throw new DomainValidationException("Bu hesap seçilen giriş portalı ile uyumlu değil.");

        return new AuthUserDto(
            user.Id,
            user.Username,
            user.DisplayName,
            user.Role,
            user.LinkedCitizen?.TcNo);
    }

    private static string NormalizeIdentifier(LoginPortal portal, string raw)
    {
        var value = raw.Trim();

        if (portal == LoginPortal.Personel)
        {
            value = value.ToLowerInvariant();
            try
            {
                _ = new MailAddress(value);
            }
            catch
            {
                throw new DomainValidationException("Personel girişi için geçerli bir kurumsal e-posta girin.");
            }

            if (!value.EndsWith("@ornekkoy.bel.tr", StringComparison.Ordinal))
                throw new DomainValidationException("Personel e-postası @ornekkoy.bel.tr uzantılı olmalıdır.");

            return value;
        }

        // Vatandaş: TC doğrula (format).
        return TcNo.Create(value).Value;
    }
}
