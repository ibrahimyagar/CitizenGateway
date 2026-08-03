namespace CitizenGateway.Application.DTOs.External;

public sealed record CozumTalebiDto(
    string TalepNo,
    string Konu,
    string Kategori,
    string Durum,
    DateOnly AcilisTarihi);

public sealed record CozumMerkeziDto(
    string TcNo,
    string AdSoyad,
    int AcikTalepSayisi,
    IReadOnlyList<CozumTalebiDto> Talepler);
