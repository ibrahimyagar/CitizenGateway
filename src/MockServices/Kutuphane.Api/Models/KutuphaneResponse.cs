namespace Kutuphane.Api.Models;

/// <summary>
/// SENTETİK kütüphane randevu geçmişi — gerçek rezervasyon verisi değildir.
/// </summary>
public sealed record KutuphaneRandevu(
    DateOnly Tarih,
    string Salon,
    string Konu,
    string Durum);

public sealed record KutuphaneResponse(
    string TcNo,
    string AdSoyad,
    int AktifOduncSayisi,
    IReadOnlyList<KutuphaneRandevu> Randevular);
