using MockServices.Shared;
using SporTesisi.Api.Models;

namespace SporTesisi.Api.Data;

/// <summary>
/// Anlaşılır demo profilleri — rastgele yerine sabit hikâye.
/// 0: Ayşe aktif üye · 1: Mehmet süresi dolmuş · diğerleri dengeli örnekler.
/// </summary>
public sealed class SporTesisiSeedStore
{
    private readonly Dictionary<string, SporTesisiResponse> _byTc;

    public SporTesisiSeedStore()
    {
        var tesisler = new[]
        {
            "Merkez Spor Salonu",
            "Sahil Yüzme Havuzu",
            "Gençlik Basket Sahası",
            "Atletizm Pisti"
        };

        _byTc = SharedCitizenCatalog.All
            .Select((c, i) =>
            {
                var profile = i switch
                {
                    0 => (Aktif: true, Tesis: tesisler[0], Kontor: 18, Bitis: new DateOnly(2027, 3, 15)),
                    1 => (Aktif: false, Tesis: tesisler[1], Kontor: 0, Bitis: new DateOnly(2025, 11, 30)),
                    2 => (Aktif: true, Tesis: tesisler[2], Kontor: 5, Bitis: new DateOnly(2026, 12, 1)),
                    _ => (
                        Aktif: i % 3 != 0,
                        Tesis: tesisler[i % tesisler.Length],
                        Kontor: (i * 3) % 25,
                        Bitis: new DateOnly(2026, 6, 1).AddMonths(i % 12))
                };

                return new SporTesisiResponse(
                    c.TcNo,
                    c.AdSoyad,
                    profile.Aktif,
                    profile.Tesis,
                    profile.Kontor,
                    profile.Bitis);
            })
            .ToDictionary(r => r.TcNo);
    }

    public SporTesisiResponse? Find(string tcNo) =>
        _byTc.TryGetValue(tcNo, out var record) ? record : null;
}
