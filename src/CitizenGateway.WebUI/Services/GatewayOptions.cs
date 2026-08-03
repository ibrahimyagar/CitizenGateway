namespace CitizenGateway.WebUI.Services;

public sealed class GatewayOptions
{
    public const string SectionName = "Gateway";

    public string BaseUrl { get; set; } = "http://localhost:5100";
    public string Username { get; set; } = "personel";
    public string Password { get; set; } = "Personel123!";
}
