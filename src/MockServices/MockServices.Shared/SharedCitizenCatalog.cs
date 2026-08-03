using Bogus;

namespace MockServices.Shared;

/// <summary>
/// Sabit seed (42) ile 25 sentetik vatandaş üretir.
/// Neden sabit seed? Gateway DbSeeder ve mock servisler aynı TC listesini görsün;
/// demo'da "sorgula" her seferinde tutarlı sonuç dönsün.
/// </summary>
public static class SharedCitizenCatalog
{
    public const int Seed = 42;
    public const int CitizenCount = 25;

    private static readonly Lazy<IReadOnlyList<SyntheticCitizen>> Citizens = new(Build);

    public static IReadOnlyList<SyntheticCitizen> All => Citizens.Value;

    public static SyntheticCitizen? FindByTcNo(string tcNo) =>
        All.FirstOrDefault(c => c.TcNo == tcNo);

    private static IReadOnlyList<SyntheticCitizen> Build()
    {
        // tr locale: Türkçe ad-soyad üretir; yine de tamamen sahte/Bogus veridir.
        var faker = new Faker("tr");
        Randomizer.Seed = new Random(Seed);

        var random = new Random(Seed);
        var list = new List<SyntheticCitizen>(CitizenCount);

        for (var i = 0; i < CitizenCount; i++)
        {
            list.Add(new SyntheticCitizen(
                SyntheticTcNo.Generate(random),
                faker.Name.FullName()));
        }

        return list;
    }
}
