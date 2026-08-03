using CitizenGateway.Application.Abstractions;
using CitizenGateway.Application.DTOs.External;

namespace CitizenGateway.IntegrationTests.Support;

/// <summary>
/// Entegrasyon testlerinde HTTP mock servislere bağımlılığı kaldırır —
/// pipeline/auth/DB doğrulanırken dış ağ gürültüsü olmasın.
/// </summary>
public sealed class StubExternalServiceClient : IExternalServiceClient
{
    public Task<SporTesisiDto?> GetSporTesisiAsync(string tcNo, CancellationToken cancellationToken = default) =>
        Task.FromResult<SporTesisiDto?>(new SporTesisiDto(
            tcNo, "Stub Vatandas", true, "Test Tesis", 7, new DateOnly(2027, 6, 1)));

    public Task<KutuphaneDto?> GetKutuphaneAsync(string tcNo, CancellationToken cancellationToken = default) =>
        Task.FromResult<KutuphaneDto?>(new KutuphaneDto(tcNo, "Stub Vatandas", 1, []));

    public Task<CozumMerkeziDto?> GetCozumMerkeziAsync(string tcNo, CancellationToken cancellationToken = default) =>
        Task.FromResult<CozumMerkeziDto?>(new CozumMerkeziDto(tcNo, "Stub Vatandas", 0, []));
}
