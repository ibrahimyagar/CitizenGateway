namespace CitizenGateway.Domain.Enums;

/// <summary>
/// Talep yaşam döngüsü durumu.
/// Not: "Status" yerine RequestStatus — System/HTTP Status ile isim çakışmasını önlemek için.
/// </summary>
public enum RequestStatus
{
    Beklemede = 1,
    Onaylandi = 2,
    Reddedildi = 3
}
