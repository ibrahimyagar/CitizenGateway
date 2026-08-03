using CitizenGateway.Domain.Enums;

namespace CitizenGateway.WebUI.Services;

/// <summary>Enum ve teknik alanları ekranda Türkçe / anlaşılır metne çevirir.</summary>
public static class DemoLabels
{
    public static string ForRequestType(RequestType type) => type switch
    {
        Domain.Enums.RequestType.KursKaydi => "Spor kursu kaydı",
        Domain.Enums.RequestType.RandevuTalebi => "Kütüphane randevusu",
        Domain.Enums.RequestType.SikayetAcma => "Çözüm merkezi bildirimi",
        _ => type.ToString()
    };

    public static string ForTargetService(TargetService service) => service switch
    {
        Domain.Enums.TargetService.SporTesisi => "Spor Tesisi",
        Domain.Enums.TargetService.Kutuphane => "Kütüphane",
        Domain.Enums.TargetService.CozumMerkezi => "Çözüm Merkezi",
        _ => service.ToString()
    };

    public static string ForRequestStatus(RequestStatus status) => status switch
    {
        Domain.Enums.RequestStatus.Beklemede => "Beklemede",
        Domain.Enums.RequestStatus.Onaylandi => "Onaylandı",
        Domain.Enums.RequestStatus.Reddedildi => "Reddedildi",
        _ => status.ToString()
    };

    public static string StatusPillClass(RequestStatus status) => status switch
    {
        Domain.Enums.RequestStatus.Beklemede => "cg-pill-warn",
        Domain.Enums.RequestStatus.Onaylandi => "cg-pill-ok",
        Domain.Enums.RequestStatus.Reddedildi => "cg-pill-bad",
        _ => "cg-pill-neutral"
    };

    public static string Role(bool isPersonel) =>
        isPersonel ? "Personel" : "Vatandaş";

    public static string FailedService(string service) => service switch
    {
        "SporTesisi" => "Spor Tesisi",
        "Kutuphane" => "Kütüphane",
        "CozumMerkezi" => "Çözüm Merkezi",
        _ => service
    };
}
