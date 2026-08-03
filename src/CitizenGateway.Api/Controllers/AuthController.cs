using CitizenGateway.Api.Auth;
using CitizenGateway.Application.DTOs;
using CitizenGateway.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CitizenGateway.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly JwtTokenService _jwtTokenService;

    public AuthController(AuthService authService, JwtTokenService jwtTokenService)
    {
        _authService = authService;
        _jwtTokenService = jwtTokenService;
    }

    /// <summary>Herkese açık — kullanıcı/şifre doğrular, JWT döner.</summary>
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LoginResponse>> Login(
        [FromBody] LoginRequestDto request,
        CancellationToken cancellationToken)
    {
        var user = await _authService.LoginAsync(request, cancellationToken);
        var (token, expiresAt) = _jwtTokenService.CreateToken(user);

        return Ok(new LoginResponse(token, expiresAt, user.Username, user.Role.ToString(), user.LinkedCitizenTcNo));
    }

    public sealed record LoginResponse(
        string AccessToken,
        DateTimeOffset ExpiresAt,
        string Username,
        string Role,
        string? LinkedCitizenTcNo);
}
