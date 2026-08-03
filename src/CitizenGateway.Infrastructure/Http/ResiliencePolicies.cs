using System.Net;
using Polly;
using Polly.Extensions.Http;

namespace CitizenGateway.Infrastructure.Http;

/// <summary>
/// Mock servis çağrıları için ortak Polly politikaları.
/// Retry: geçici ağ hatalarında 3 deneme + exponential backoff.
/// Circuit breaker: 5 ardışık hatada 30 sn "devre açık" — çöken servisi yağmalamamak için.
/// </summary>
public static class ResiliencePolicies
{
    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy() =>
        HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(r => r.StatusCode == HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)));

    public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy() =>
        HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(30));
}
