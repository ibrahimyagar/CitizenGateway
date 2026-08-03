namespace CitizenGateway.Contracts.Health;

/// <summary>Gateway /health özet alanı — UI badge için yeterli yüzey.</summary>
public sealed class GatewayHealthDto
{
    public string Status { get; set; } = "Unknown";
    public string? Service { get; set; }
}
