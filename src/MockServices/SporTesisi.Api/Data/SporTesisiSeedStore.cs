using Bogus;
using MockServices.Shared;
using SporTesisi.Api.Models;

namespace SporTesisi.Api.Data;

/// <summary>
/// In-memory seed — DB yok; mock servis kendi sahte katalogunu RAM'de tutar.
/// SharedCitizenCatalog.Seed ile aynı Randomizer seed'i kullanır → TC tutarlılığı.
/// </summary>
public sealed class SporTesisiSeedStore
{
    private readonly Dictionary<string, SporTesisiResponse> _byTc;

    public SporTesisiSeedStore()
    {
        Randomizer.Seed = new Random(SharedCitizenCatalog.Seed);
        var faker = new Faker("tr");
        var tesisler = new[] { "Merkez Spor Salonu", "Sahil Yüzme Havuzu", "Gençlik Basket Sahası", "Atletizm Pisti" };

        _byTc = SharedCitizenCatalog.All.ToDictionary(
            c => c.TcNo,
            c => new SporTesisiResponse(
                c.TcNo,
                c.AdSoyad,
                faker.Random.Bool(0.75f),
                faker.PickRandom(tesisler),
                faker.Random.Int(0, 40),
                DateOnly.FromDateTime(faker.Date.Future(2, DateTime.UtcNow.Date))));
    }

    public SporTesisiResponse? Find(string tcNo) =>
        _byTc.TryGetValue(tcNo, out var record) ? record : null;
}
