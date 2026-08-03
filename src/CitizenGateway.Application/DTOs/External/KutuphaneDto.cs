namespace CitizenGateway.Application.DTOs.External;

public sealed record KutuphaneRandevuDto(
    DateOnly Tarih,
    string Salon,
    string Konu,
    string Durum);

public sealed record KutuphaneDto(
    string TcNo,
    string AdSoyad,
    int AktifOduncSayisi,
    IReadOnlyList<KutuphaneRandevuDto> Randevular);
