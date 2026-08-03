using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using CitizenGateway.Application.Abstractions;
using CitizenGateway.Contracts.External;
using Microsoft.Extensions.Logging;

namespace CitizenGateway.Infrastructure.Http;

/// <summary>
/// Typed HttpClient adapter — her mock servis ayrı named client ile dönüşür.
/// 404 → null (vatandaş o serviste yok); diğer hatalar exception → PartialFailure üst katmanda.
/// </summary>
public sealed class ExternalServiceClient : IExternalServiceClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ExternalServiceClient> _logger;

    public const string SporTesisiClientName = "SporTesisi";
    public const string KutuphaneClientName = "Kutuphane";
    public const string CozumMerkeziClientName = "CozumMerkezi";

    public ExternalServiceClient(IHttpClientFactory httpClientFactory, ILogger<ExternalServiceClient> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public Task<SporTesisiDto?> GetSporTesisiAsync(string tcNo, CancellationToken cancellationToken = default) =>
        GetAsync<SporTesisiDto>(SporTesisiClientName, tcNo, cancellationToken);

    public Task<KutuphaneDto?> GetKutuphaneAsync(string tcNo, CancellationToken cancellationToken = default) =>
        GetAsync<KutuphaneDto>(KutuphaneClientName, tcNo, cancellationToken);

    public Task<CozumMerkeziDto?> GetCozumMerkeziAsync(string tcNo, CancellationToken cancellationToken = default) =>
        GetAsync<CozumMerkeziDto>(CozumMerkeziClientName, tcNo, cancellationToken);

    private async Task<T?> GetAsync<T>(string clientName, string tcNo, CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient(clientName);
        using var response = await client.GetAsync($"api/{tcNo}", cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogInformation("{Client}: TC {TcNo} için kayıt yok (404).", clientName, tcNo);
            return default;
        }

        // Polly retry sonrası hâlâ hata ise exception fırlat — SummaryService PartialFailure üretir.
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken);
    }
}
