using Bogus;
using CozumMerkezi.Api.Models;
using MockServices.Shared;

namespace CozumMerkezi.Api.Data;

/// <summary>
/// In-memory çözüm merkezi seed — SharedCitizenCatalog ile ortak TC havuzu.
/// </summary>
public sealed class CozumMerkeziSeedStore
{
    private readonly Dictionary<string, CozumMerkeziResponse> _byTc;

    public CozumMerkeziSeedStore()
    {
        Randomizer.Seed = new Random(SharedCitizenCatalog.Seed + 2);
        var faker = new Faker("tr");
        var kategoriler = new[] { "Temizlik", "ParkBahce", "Ulasim", "Aydinlatma", "Gurultu" };
        var durumlar = new[] { "Acik", "Incelemede", "Cozuldu", "Reddedildi" };
        var konular = new[]
        {
            "Çöp konteyneri taşması",
            "Park lambası arızası",
            "Kaldırım hasarı",
            "Gürültü şikayeti",
            "Yol çukuru bildirimi"
        };

        _byTc = SharedCitizenCatalog.All.ToDictionary(
            c => c.TcNo,
            c =>
            {
                var talepCount = faker.Random.Int(1, 3);
                var talepler = Enumerable.Range(0, talepCount)
                    .Select(i => new CozumTalebi(
                        $"CM-{faker.Random.Number(10000, 99999)}-{i + 1}",
                        faker.PickRandom(konular),
                        faker.PickRandom(kategoriler),
                        faker.PickRandom(durumlar),
                        DateOnly.FromDateTime(faker.Date.Past(1, DateTime.UtcNow))))
                    .OrderByDescending(t => t.AcilisTarihi)
                    .ToList();

                return new CozumMerkeziResponse(
                    c.TcNo,
                    c.AdSoyad,
                    talepler.Count(t => t.Durum is "Acik" or "Incelemede"),
                    talepler);
            });
    }

    public CozumMerkeziResponse? Find(string tcNo) =>
        _byTc.TryGetValue(tcNo, out var record) ? record : null;
}
