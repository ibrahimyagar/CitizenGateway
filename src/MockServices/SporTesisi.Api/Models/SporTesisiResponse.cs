namespace SporTesisi.Api.Models;

/// <summary>
/// SENTETİK spor tesisi üyelik/kontör özeti — gerçek üyelik verisi değildir.
/// </summary>
public sealed record SporTesisiResponse(
    string TcNo,
    string AdSoyad,
    bool UyelikAktif,
    string TesisAdi,
    int KontorBakiye,
    DateOnly UyelikBitisTarihi);
