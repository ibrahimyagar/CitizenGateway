namespace CitizenGateway.Contracts.Citizens;

/// <summary>Personel için vatandaş seçim listesi — demo keşfedilebilirliği.</summary>
public sealed record CitizenListItemDto(
    Guid Id,
    string TcNo,
    string AdSoyad,
    DateOnly DogumTarihi,
    string Telefon);
