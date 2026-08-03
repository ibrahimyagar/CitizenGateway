namespace CitizenGateway.Domain.Enums;

/// <summary>
/// Talebin yönlendirileceği mock dış servis.
/// Gateway'in hangi HTTP adapter'ı çağıracağını belirler.
/// </summary>
public enum TargetService
{
    SporTesisi = 1,
    Kutuphane = 2,
    CozumMerkezi = 3
}
