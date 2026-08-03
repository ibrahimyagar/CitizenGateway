namespace MockServices.Shared;

/// <summary>
/// Üç mock servisin paylaştığı sentetik vatandaş kimliği.
/// Aynı TC ile Spor / Kütüphane / Çözüm Merkezi sorgulanabilsin diye ortak model.
/// </summary>
public sealed record SyntheticCitizen(string TcNo, string AdSoyad);
