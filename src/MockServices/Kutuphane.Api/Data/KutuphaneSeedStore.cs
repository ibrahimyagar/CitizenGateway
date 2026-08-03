using Bogus;
using Kutuphane.Api.Models;
using MockServices.Shared;

namespace Kutuphane.Api.Data;

/// <summary>
/// In-memory kütüphane seed — SharedCitizenCatalog ile ortak TC havuzu.
/// </summary>
public sealed class KutuphaneSeedStore
{
    private readonly Dictionary<string, KutuphaneResponse> _byTc;

    public KutuphaneSeedStore()
    {
        Randomizer.Seed = new Random(SharedCitizenCatalog.Seed + 1);
        var faker = new Faker("tr");
        var salonlar = new[] { "Sessiz Çalışma Salonu", "Çocuk Bölümü", "Dijital Arşiv", "Seminer Odası A" };
        var konular = new[] { "Çalışma masası", "Grup çalışması", "Kitap kulübü", "Dijital tarama" };
        var durumlar = new[] { "Tamamlandi", "Beklemede", "Iptal" };

        _byTc = SharedCitizenCatalog.All.ToDictionary(
            c => c.TcNo,
            c =>
            {
                var randevuCount = faker.Random.Int(1, 4);
                var randevular = Enumerable.Range(0, randevuCount)
                    .Select(_ => new KutuphaneRandevu(
                        DateOnly.FromDateTime(faker.Date.Between(DateTime.UtcNow.AddMonths(-6), DateTime.UtcNow.AddMonths(1))),
                        faker.PickRandom(salonlar),
                        faker.PickRandom(konular),
                        faker.PickRandom(durumlar)))
                    .OrderByDescending(r => r.Tarih)
                    .ToList();

                return new KutuphaneResponse(
                    c.TcNo,
                    c.AdSoyad,
                    faker.Random.Int(0, 5),
                    randevular);
            });
    }

    public KutuphaneResponse? Find(string tcNo) =>
        _byTc.TryGetValue(tcNo, out var record) ? record : null;
}
