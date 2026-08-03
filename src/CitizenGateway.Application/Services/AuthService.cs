using CitizenGateway.Application.Abstractions;
using CitizenGateway.Application.DTOs;
using CitizenGateway.Domain.Exceptions;

namespace CitizenGateway.Application.Services;

/// <summary>
/// Kullanıcı adı/şifre doğrular; JWT üretmez (Api'nin işi).
/// Ayrım: Application "kim bu?", Api "token'ı nasıl imzalarım?".
/// </summary>
public sealed class AuthService
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
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            throw new DomainValidationException("Kullanıcı adı ve şifre zorunludur.");

        var user = await _users.GetByUsernameAsync(request.Username.Trim(), cancellationToken);
        if (user is null || !_passwordVerifier.Verify(user, request.Password))
            throw new DomainValidationException("Kullanıcı adı veya şifre hatalı.");

        return new AuthUserDto(
            user.Id,
            user.Username,
            user.Role,
            user.LinkedCitizen?.TcNo);
    }
}
