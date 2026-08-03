using CitizenGateway.Api.Auth;
using CitizenGateway.Application.Features.Auth;
using CitizenGateway.Contracts.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CitizenGateway.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly JwtTokenService _jwtTokenService;

    public AuthController(IAuthService authService, JwtTokenService jwtTokenService)
    {
        _authService = authService;
        _jwtTokenService = jwtTokenService;
    }

    /// <summary>
    /// Personel: kurumsal e-posta + şifre.
    /// Vatandaş: T.C. kimlik no + şifre.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LoginResponseDto>> Login(
        [FromBody] LoginRequestDto request,
        CancellationToken cancellationToken)
    {
        var user = await _authService.LoginAsync(request, cancellationToken);
        var (token, expiresAt) = _jwtTokenService.CreateToken(user);

        return Ok(new LoginResponseDto(
            token,
            expiresAt,
            user.Username,
            user.DisplayName,
            user.Role.ToString(),
            user.LinkedCitizenTcNo));
    }
}
