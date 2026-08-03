namespace CitizenGateway.Domain.Enums;

/// <summary>
/// Gateway üzerinden açılabilen talep türleri.
/// Enum tercih edildi: string magic-value yerine derleme zamanı güvenliği sağlar.
/// </summary>
public enum RequestType
{
    KursKaydi = 1,
    SikayetAcma = 2,
    RandevuTalebi = 3
}
