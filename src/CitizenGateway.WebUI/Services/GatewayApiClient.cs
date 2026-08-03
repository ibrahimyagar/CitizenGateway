using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace CitizenGateway.WebUI.Services;

/// <summary>
/// Minimal Gateway istemcisi — önce personel login, sonra summary.
/// Auth UI yok: demo için sunucu tarafında yapılandırılmış kimlik kullanılır.
/// </summary>
public sealed class GatewayApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _http;
    private readonly GatewayOptions _options;

    public GatewayApiClient(HttpClient http, IOptions<GatewayOptions> options)
    {
        _http = http;
        _options = options.Value;
        _http.BaseAddress = new Uri(_options.BaseUrl.TrimEnd('/') + "/");
    }

    public async Task<CitizenSummaryViewModel> GetSummaryAsync(string tcNo, CancellationToken cancellationToken = default)
    {
        var token = await LoginAsync(cancellationToken);
        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/citizen/{tcNo}/summary");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        using var response = await _http.SendAsync(request, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException($"Gateway hatası ({(int)response.StatusCode}): {body}");

        return JsonSerializer.Deserialize<CitizenSummaryViewModel>(body, JsonOptions)
            ?? throw new InvalidOperationException("Summary yanıtı parse edilemedi.");
    }

    private async Task<string> LoginAsync(CancellationToken cancellationToken)
    {
        using var response = await _http.PostAsJsonAsync("api/auth/login", new
        {
            username = _options.Username,
            password = _options.Password
        }, cancellationToken);

        response.EnsureSuccessStatusCode();
        var login = await response.Content.ReadFromJsonAsync<LoginResponse>(JsonOptions, cancellationToken);
        return login?.AccessToken
            ?? throw new InvalidOperationException("Login accessToken boş.");
    }
}
