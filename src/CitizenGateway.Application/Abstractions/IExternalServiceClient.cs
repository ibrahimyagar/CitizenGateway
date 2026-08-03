using CitizenGateway.Contracts.External;

namespace CitizenGateway.Application.Abstractions;

/// <summary>
/// Üç mock departman servisine erişim kontratı.
/// Tek interface: unit testlerde Moq ile kolay stub; gerçek HTTP Infrastructure'da.
/// </summary>
public interface IExternalServiceClient
{
    Task<SporTesisiDto?> GetSporTesisiAsync(string tcNo, CancellationToken cancellationToken = default);
    Task<KutuphaneDto?> GetKutuphaneAsync(string tcNo, CancellationToken cancellationToken = default);
    Task<CozumMerkeziDto?> GetCozumMerkeziAsync(string tcNo, CancellationToken cancellationToken = default);
}
