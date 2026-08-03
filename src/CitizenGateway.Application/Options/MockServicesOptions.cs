namespace CitizenGateway.Application.Options;

/// <summary>
/// Mock servis base URL'leri — docker-compose'ta hostname, lokalde localhost.
/// </summary>
public sealed class MockServicesOptions
{
    public const string SectionName = "MockServices";

    public string SporTesisiBaseUrl { get; set; } = "http://localhost:5101";
    public string KutuphaneBaseUrl { get; set; } = "http://localhost:5102";
    public string CozumMerkeziBaseUrl { get; set; } = "http://localhost:5103";
}
