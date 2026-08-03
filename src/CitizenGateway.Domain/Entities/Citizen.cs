namespace CitizenGateway.Domain.Entities;

/// <summary>
/// SENTETİK vatandaş kaydı — gerçek kişi verisi içermez.
/// Anemic model yerine factory + private set: geçersiz TC'nin Domain'e girmesini engeller.
/// </summary>
public class Citizen
{
    public Guid Id { get; private set; }

    /// <summary>11 haneli sentetik T.C. kimlik no (gerçek kişiye ait değildir).</summary>
    public string TcNo { get; private set; } = null!;

    public string AdSoyad { get; private set; } = null!;
    public DateOnly DogumTarihi { get; private set; }
    public string Telefon { get; private set; } = null!;

    // EF Core için parametresiz ctor — Domain dışından doğrudan new Citizen() beklenmez.
    private Citizen()
    {
    }

    public static Citizen Create(string tcNo, string adSoyad, DateOnly dogumTarihi, string telefon)
    {
        // Property adı TcNo ile VO çakışmasın diye tam nitelikli ad kullanılıyor.
        var validatedTc = ValueObjects.TcNo.Create(tcNo);

        if (string.IsNullOrWhiteSpace(adSoyad))
            throw new ArgumentException("AdSoyad boş olamaz.", nameof(adSoyad));

        if (string.IsNullOrWhiteSpace(telefon))
            throw new ArgumentException("Telefon boş olamaz.", nameof(telefon));

        return new Citizen
        {
            Id = Guid.NewGuid(),
            TcNo = validatedTc.Value,
            AdSoyad = adSoyad.Trim(),
            DogumTarihi = dogumTarihi,
            Telefon = telefon.Trim()
        };
    }
}
