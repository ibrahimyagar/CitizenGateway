using CitizenGateway.Contracts.Auth;

namespace CitizenGateway.Application.Features.Auth;

public interface IAuthService
{
    Task<AuthUserDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);
}
