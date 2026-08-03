namespace CitizenGateway.WebUI.Services;

/// <summary>WebUI → Gateway HTTP bağlantısı. Kimlik bilgisi cookie oturumundan gelir.</summary>
public sealed class GatewayOptions
{
    public const string SectionName = "Gateway";

    public string BaseUrl { get; set; } = "http://localhost:5100";
}
