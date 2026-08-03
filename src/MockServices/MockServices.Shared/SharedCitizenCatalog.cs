namespace MockServices.Shared;

/// <summary>
/// Sabit seed (42) ile 25 sentetik vatandaş.
/// İlk kayıt (Ayşe Demir) demo vatandaş hesabına bağlıdır — UI anlatısının kahramanı.
/// TC üretimi Random(42) ile sabit kalır; testlerdeki ValidTc1/ValidTc2 bozulmaz.
/// </summary>
public static class SharedCitizenCatalog
{
    public const int Seed = 42;
    public const int CitizenCount = 25;

    /// <summary>Demo vatandaş kullanıcısının bağlı olduğu kişi (katalog[0]).</summary>
    public const string DemoVatandasAdSoyad = "Ayşe Demir";

    private static readonly string[] DemoNames =
    [
        "Ayşe Demir",       // 0 — spor aktif, kütüphane ödünç, açık şikayet
        "Mehmet Kaya",      // 1 — spor pasif, kütüphane randevu bekliyor
        "Zeynep Arslan",    // 2 — çözüm merkezi yoğun
        "Can Yıldız",
        "Elif Çelik",
        "Burak Şahin",
        "Deniz Aydın",
        "Fatma Koç",
        "Emre Kurt",
        "Selin Özkan",
        "Hakan Yılmaz",
        "Merve Acar",
        "Onur Doğan",
        "Gülşen Polat",
        "Serkan Güneş",
        "İrem Taş",
        "Volkan Erdem",
        "Pınar Bulut",
        "Murat Aksoy",
        "Ceren Uçar",
        "Tolga Karaca",
        "Nazlı Öztürk",
        "Barış Çetin",
        "Seda Kılıç",
        "Kerem Aslan"
    ];

    private static readonly Lazy<IReadOnlyList<SyntheticCitizen>> Citizens = new(Build);

    public static IReadOnlyList<SyntheticCitizen> All => Citizens.Value;

    public static SyntheticCitizen? FindByTcNo(string tcNo) =>
        All.FirstOrDefault(c => c.TcNo == tcNo);

    private static IReadOnlyList<SyntheticCitizen> Build()
    {
        var random = new Random(Seed);
        var list = new List<SyntheticCitizen>(CitizenCount);

        for (var i = 0; i < CitizenCount; i++)
        {
            list.Add(new SyntheticCitizen(
                SyntheticTcNo.Generate(random),
                DemoNames[i]));
        }

        return list;
    }
}
