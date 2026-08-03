using CitizenGateway.Contracts.External;

namespace CitizenGateway.Contracts.Citizens;

/// <summary>
/// Üç mock servisten konsolide özet.
/// PartialFailure=true: en az bir servis yanıt vermedi; FailedServices hangi departmanın düştüğünü söyler.
/// </summary>
public sealed record CitizenSummaryDto(
    string TcNo,
    string? AdSoyad,
    SporTesisiDto? SporTesisi,
    KutuphaneDto? Kutuphane,
    CozumMerkeziDto? CozumMerkezi,
    bool PartialFailure,
    IReadOnlyList<string> FailedServices);
