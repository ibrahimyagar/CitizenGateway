using CozumMerkezi.Api.Models;
using MockServices.Shared;

namespace CozumMerkezi.Api.Data;

/// <summary>
/// Çözüm merkezi demo verisi — mahalle sorunları Türkçe ve anlaşılır.
/// </summary>
public sealed class CozumMerkeziSeedStore
{
    private readonly Dictionary<string, CozumMerkeziResponse> _byTc;

    public CozumMerkeziSeedStore()
    {
        _byTc = SharedCitizenCatalog.All
            .Select((c, i) => Build(c, i))
            .ToDictionary(r => r.TcNo);
    }

    public CozumMerkeziResponse? Find(string tcNo) =>
        _byTc.TryGetValue(tcNo, out var record) ? record : null;

    private static CozumMerkeziResponse Build(SyntheticCitizen c, int index)
    {
        IReadOnlyList<CozumTalebi> talepler = index switch
        {
            0 =>
            [
                new("CM-10021-1", "Park lambası yanmıyor", "Aydınlatma", "İncelemede", new DateOnly(2026, 7, 20)),
                new("CM-09811-2", "Çöp konteyneri taşması", "Temizlik", "Çözüldü", new DateOnly(2026, 5, 3))
            ],
            1 =>
            [
                new("CM-10102-1", "Kaldırım hasarı", "Ulaşım", "Açık", new DateOnly(2026, 7, 28))
            ],
            2 =>
            [
                new("CM-10230-1", "Gürültü şikayeti", "Gürültü", "Açık", new DateOnly(2026, 8, 1)),
                new("CM-10230-2", "Yol çukuru bildirimi", "Ulaşım", "İncelemede", new DateOnly(2026, 7, 15)),
                new("CM-09001-3", "Park çimleri biçilmemiş", "ParkBahçe", "Reddedildi", new DateOnly(2026, 4, 10))
            ],
            _ =>
            [
                new(
                    $"CM-{11000 + index}-1",
                    index % 2 == 0 ? "Sokak aydınlatması" : "Çöp toplama gecikmesi",
                    index % 2 == 0 ? "Aydınlatma" : "Temizlik",
                    index % 3 == 0 ? "Çözüldü" : "Açık",
                    new DateOnly(2026, 3, 1).AddDays(index * 2))
            ]
        };

        var acik = talepler.Count(t => t.Durum is "Açık" or "İncelemede");
        return new CozumMerkeziResponse(c.TcNo, c.AdSoyad, acik, talepler);
    }
}
