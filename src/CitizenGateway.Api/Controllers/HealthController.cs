using CitizenGateway.Application.Options;
using CitizenGateway.Infrastructure.Http;
using CitizenGateway.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CitizenGateway.Api.Controllers;

[ApiController]
[AllowAnonymous]
[Route("health")]
public sealed class HealthController : ControllerBase
{
    private readonly GatewayDbContext _db;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly MockServicesOptions _mockOptions;

    public HealthController(
        GatewayDbContext db,
        IHttpClientFactory httpClientFactory,
        IOptions<MockServicesOptions> mockOptions)
    {
        _db = db;
        _httpClientFactory = httpClientFactory;
        _mockOptions = mockOptions.Value;
    }

    /// <summary>
    /// Gateway + DB + 3 mock servis durumu.
    /// Dış servis down olsa bile 200 dönebilir; detayda Unhealthy görünür (PartialFailure mantığına benzer).
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var dbHealthy = await CheckDbAsync(cancellationToken);
        var spor = await CheckMockAsync(ExternalServiceClient.SporTesisiClientName, cancellationToken);
        var kutuphane = await CheckMockAsync(ExternalServiceClient.KutuphaneClientName, cancellationToken);
        var cozum = await CheckMockAsync(ExternalServiceClient.CozumMerkeziClientName, cancellationToken);

        var dependencies = new Dictionary<string, object>
        {
            ["database"] = dbHealthy ? "Healthy" : "Unhealthy",
            ["sporTesisi"] = new { status = spor ? "Healthy" : "Unhealthy", url = _mockOptions.SporTesisiBaseUrl },
            ["kutuphane"] = new { status = kutuphane ? "Healthy" : "Unhealthy", url = _mockOptions.KutuphaneBaseUrl },
            ["cozumMerkezi"] = new { status = cozum ? "Healthy" : "Unhealthy", url = _mockOptions.CozumMerkeziBaseUrl }
        };

        var allOk = dbHealthy && spor && kutuphane && cozum;
        return Ok(new
        {
            status = allOk ? "Healthy" : "Degraded",
            service = "CitizenGateway.Api",
            checkedAt = DateTimeOffset.UtcNow,
            dependencies
        });
    }

    private async Task<bool> CheckDbAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await _db.Database.CanConnectAsync(cancellationToken);
        }
        catch
        {
            return false;
        }
    }

    private async Task<bool> CheckMockAsync(string clientName, CancellationToken cancellationToken)
    {
        try
        {
            var client = _httpClientFactory.CreateClient(clientName);
            using var response = await client.GetAsync("health", cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
