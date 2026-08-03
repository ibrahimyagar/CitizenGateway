using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using CitizenGateway.Contracts.Audit;
using CitizenGateway.Contracts.Auth;
using CitizenGateway.Contracts.Citizens;
using CitizenGateway.Contracts.Health;
using CitizenGateway.Contracts.Requests;
using CitizenGateway.Domain.Enums;
using Microsoft.Extensions.Options;

namespace CitizenGateway.WebUI.Services;

/// <summary>
/// Gateway HTTP istemcisi — oturumdaki JWT ile çağrı yapar.
/// </summary>
public sealed class GatewayApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
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

    public async Task<LoginResponseDto> LoginAsync(
        LoginPortal portal,
        string identifier,
        string password,
        CancellationToken cancellationToken = default)
    {
        using var response = await _http.PostAsJsonAsync(
            "api/auth/login",
            new LoginRequestDto(portal, identifier, password),
            JsonOptions,
            cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException(ExtractError(body, (int)response.StatusCode));

        return JsonSerializer.Deserialize<LoginResponseDto>(body, JsonOptions)
            ?? throw new InvalidOperationException("Login yanıtı parse edilemedi.");
    }

    public Task<CitizenSummaryDto> GetSummaryAsync(string tcNo, CancellationToken cancellationToken = default) =>
        GetAsync<CitizenSummaryDto>($"api/citizens/{tcNo}/summary", cancellationToken);

    public Task<IReadOnlyList<CitizenListItemDto>> ListCitizensAsync(CancellationToken cancellationToken = default) =>
        GetAsync<IReadOnlyList<CitizenListItemDto>>("api/citizens", cancellationToken);

    public Task<IReadOnlyList<ServiceRequestDto>> GetRequestsAsync(string tcNo, CancellationToken cancellationToken = default) =>
        GetAsync<IReadOnlyList<ServiceRequestDto>>($"api/citizens/{tcNo}/requests", cancellationToken);

    public async Task<ServiceRequestDto> CreateRequestAsync(
        string tcNo,
        RequestType requestType,
        CancellationToken cancellationToken = default)
    {
        using var request = CreateAuthorizedRequest(HttpMethod.Post, $"api/citizens/{tcNo}/requests");
        request.Content = JsonContent.Create(new CreateServiceRequestDto(requestType), options: JsonOptions);

        using var response = await _http.SendAsync(request, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException(ExtractError(body, (int)response.StatusCode));

        return JsonSerializer.Deserialize<ServiceRequestDto>(body, JsonOptions)
            ?? throw new InvalidOperationException("Talep yanıtı parse edilemedi.");
    }

    public Task<IReadOnlyList<AuditLogDto>> GetAuditLogsAsync(CancellationToken cancellationToken = default) =>
        GetAsync<IReadOnlyList<AuditLogDto>>("api/audit-logs?take=50", cancellationToken);

    public Task<IReadOnlyList<ServiceRequestDto>> ListServiceRequestsAsync(
        RequestStatus? status,
        CancellationToken cancellationToken = default)
    {
        var path = status is null
            ? "api/service-requests?take=50"
            : $"api/service-requests?status={status}&take=50";
        return GetAsync<IReadOnlyList<ServiceRequestDto>>(path, cancellationToken);
    }

    public Task<ServiceRequestDto> ApproveRequestAsync(Guid id, CancellationToken cancellationToken = default) =>
        SendEmptyAsync<ServiceRequestDto>(HttpMethod.Post, $"api/service-requests/{id}/approve", cancellationToken);

    public Task<ServiceRequestDto> RejectRequestAsync(Guid id, CancellationToken cancellationToken = default) =>
        SendEmptyAsync<ServiceRequestDto>(HttpMethod.Post, $"api/service-requests/{id}/reject", cancellationToken);

    public async Task<GatewayHealthDto> GetHealthAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using var response = await _http.GetAsync("health", cancellationToken);
            if (!response.IsSuccessStatusCode)
                return new GatewayHealthDto { Status = "Unreachable" };

            return await response.Content.ReadFromJsonAsync<GatewayHealthDto>(JsonOptions, cancellationToken)
                ?? new GatewayHealthDto { Status = "Unknown" };
        }
        catch
        {
            return new GatewayHealthDto { Status = "Unreachable" };
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

    private async Task<T> SendEmptyAsync<T>(HttpMethod method, string path, CancellationToken cancellationToken)
    {
        using var request = CreateAuthorizedRequest(method, path);
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
