namespace CitizenGateway.Contracts.External;

/// <summary>SporTesisi mock API yanıt şekli — Gateway'in tanıdığı DTO.</summary>
public sealed record SporTesisiDto(
    string TcNo,
    string AdSoyad,
    bool UyelikAktif,
    string TesisAdi,
    int KontorBakiye,
    DateOnly UyelikBitisTarihi);
