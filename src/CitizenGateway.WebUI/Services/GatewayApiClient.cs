using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace CitizenGateway.WebUI.Services;

/// <summary>
/// Gateway HTTP istemcisi — oturumdaki JWT ile çağrı yapar.
/// Login ayrıdır; sabit personel hesabına bağımlı değildir.
/// </summary>
public sealed class GatewayApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _http;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GatewayApiClient(
        HttpClient http,
        IOptions<GatewayOptions> options,
        IHttpContextAccessor httpContextAccessor)
    {
        _http = http;
        _httpContextAccessor = httpContextAccessor;
        _http.BaseAddress = new Uri(options.Value.BaseUrl.TrimEnd('/') + "/");
        _http.Timeout = TimeSpan.FromSeconds(20);
    }

    public async Task<LoginResponse> LoginAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        using var response = await _http.PostAsJsonAsync("api/auth/login", new { username, password }, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException(ExtractError(body, (int)response.StatusCode));

        return JsonSerializer.Deserialize<LoginResponse>(body, JsonOptions)
            ?? throw new InvalidOperationException("Login yanıtı parse edilemedi.");
    }

    public Task<CitizenSummaryViewModel> GetSummaryAsync(string tcNo, CancellationToken cancellationToken = default) =>
        GetAsync<CitizenSummaryViewModel>($"api/citizen/{tcNo}/summary", cancellationToken);

    public Task<IReadOnlyList<CitizenListItem>> ListCitizensAsync(CancellationToken cancellationToken = default) =>
        GetAsync<IReadOnlyList<CitizenListItem>>("api/citizens", cancellationToken);

    public Task<IReadOnlyList<ServiceRequestItem>> GetRequestsAsync(string tcNo, CancellationToken cancellationToken = default) =>
        GetAsync<IReadOnlyList<ServiceRequestItem>>($"api/citizen/{tcNo}/requests", cancellationToken);

    public async Task<ServiceRequestItem> CreateRequestAsync(string tcNo, string requestType, CancellationToken cancellationToken = default)
    {
        // API enum bekliyor; sayısal veya isim — isim gönderiyoruz.
        using var request = CreateAuthorizedRequest(HttpMethod.Post, $"api/citizen/{tcNo}/requests");
        request.Content = JsonContent.Create(new { requestType });

        using var response = await _http.SendAsync(request, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException(ExtractError(body, (int)response.StatusCode));

        return JsonSerializer.Deserialize<ServiceRequestItem>(body, JsonOptions)
            ?? throw new InvalidOperationException("Talep yanıtı parse edilemedi.");
    }

    public Task<IReadOnlyList<AuditLogItem>> GetAuditLogsAsync(CancellationToken cancellationToken = default) =>
        GetAsync<IReadOnlyList<AuditLogItem>>("api/audit-logs?take=50", cancellationToken);

    public async Task<HealthStatus> GetHealthAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using var response = await _http.GetAsync("health", cancellationToken);
            if (!response.IsSuccessStatusCode)
                return new HealthStatus { Status = "Unreachable" };

            return await response.Content.ReadFromJsonAsync<HealthStatus>(JsonOptions, cancellationToken)
                ?? new HealthStatus { Status = "Unknown" };
        }
        catch
        {
            return new HealthStatus { Status = "Unreachable" };
        }
    }

    private async Task<T> GetAsync<T>(string path, CancellationToken cancellationToken)
    {
        using var request = CreateAuthorizedRequest(HttpMethod.Get, path);
        using var response = await _http.SendAsync(request, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException(ExtractError(body, (int)response.StatusCode));

        return JsonSerializer.Deserialize<T>(body, JsonOptions)
            ?? throw new InvalidOperationException("Yanıt parse edilemedi.");
    }

    private HttpRequestMessage CreateAuthorizedRequest(HttpMethod method, string path)
    {
        var token = _httpContextAccessor.HttpContext?.User.FindFirstValue(WebUiClaimTypes.AccessToken)
            ?? throw new InvalidOperationException("Oturum bulunamadı. Tekrar giriş yapın.");

        var request = new HttpRequestMessage(method, path);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return request;
    }

    private static string ExtractError(string body, int status)
    {
        try
        {
            using var doc = JsonDocument.Parse(body);
            if (doc.RootElement.TryGetProperty("detail", out var detail))
                return detail.GetString() ?? $"HTTP {status}";
            if (doc.RootElement.TryGetProperty("title", out var title))
                return title.GetString() ?? $"HTTP {status}";
        }
        catch
        {
            // ignore
        }

        return string.IsNullOrWhiteSpace(body) ? $"HTTP {status}" : body;
    }
}
