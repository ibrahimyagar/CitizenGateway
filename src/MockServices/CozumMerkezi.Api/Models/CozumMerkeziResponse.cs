namespace CozumMerkezi.Api.Models;

/// <summary>
/// SENTETİK şikayet/talep kaydı — gerçek çözüm merkezi verisi değildir.
/// </summary>
public sealed record CozumTalebi(
    string TalepNo,
    string Konu,
    string Kategori,
    string Durum,
    DateOnly AcilisTarihi);

public sealed record CozumMerkeziResponse(
    string TcNo,
    string AdSoyad,
    int AcikTalepSayisi,
    IReadOnlyList<CozumTalebi> Talepler);
