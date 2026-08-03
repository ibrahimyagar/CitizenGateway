using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CitizenGateway.Application.DTOs;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CitizenGateway.Api.Auth;

/// <summary>
/// JWT üretim — Application AuthService kullanıcıyı doğrular, bu sınıf token imzalar.
/// Ayrım: iş kuralı Application'da, protokol detayı Api'de.
/// </summary>
public sealed class JwtTokenService
{
    private readonly JwtOptions _options;

    public JwtTokenService(IOptions<JwtOptions> options) => _options = options.Value;

    public (string Token, DateTimeOffset ExpiresAt) CreateToken(AuthUserDto user)
    {
        var expires = DateTimeOffset.UtcNow.AddMinutes(_options.ExpirationMinutes);

        var claims = new List<Claim>
        {
            new(GatewayClaimTypes.UserId, user.UserId.ToString()),
            new(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.Username),
            new(ClaimTypes.Name, user.Username),
            // Role claim: [Authorize(Roles = "Personel")] ile birebir eşleşir.
            new(ClaimTypes.Role, user.Role.ToString())
        };

        if (!string.IsNullOrWhiteSpace(user.LinkedCitizenTcNo))
            claims.Add(new Claim(GatewayClaimTypes.TcNo, user.LinkedCitizenTcNo));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var jwt = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: expires.UtcDateTime,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(jwt), expires);
    }
}
