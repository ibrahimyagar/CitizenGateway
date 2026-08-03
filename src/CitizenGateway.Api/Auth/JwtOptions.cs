namespace CitizenGateway.Api.Auth;

/// <summary>JWT ayarları — secret demo için appsettings'te; production'da Key Vault/env.</summary>
public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "CitizenGateway";
    public string Audience { get; set; } = "CitizenGateway.Clients";
    public string SigningKey { get; set; } = "OrnekkoyBelediyesi-Demo-SuperSecret-Key-32chars!";
    public int ExpirationMinutes { get; set; } = 60;
}

/// <summary>JWT claim adları — tek yerde sabitle, controller/guard aynı anahtarları kullansın.</summary>
public static class GatewayClaimTypes
{
    public const string TcNo = "tc_no";
    public const string UserId = "uid";
}
