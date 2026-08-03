using CitizenGateway.Domain.Exceptions;

namespace CitizenGateway.Domain.ValueObjects;

/// <summary>
/// SENTETİK T.C. kimlik numarası value object'i.
/// ÖNEMLİ: Bu projede gerçek kişi verisi YOKTUR. Format kontrolü (11 hane, algoritma)
/// yalnızca demo/validasyon senaryolarını göstermek içindir; üretilen tüm numaralar sahte/Bogus'tur.
///
/// Neden value object? İlkel string yerine geçerli/geçersiz ayrımını Domain sınırında zorunlu kılar;
/// Application katmanı "boş string mi?" diye dağınık kontrol yapmak zorunda kalmaz.
/// </summary>
public sealed class TcNo : IEquatable<TcNo>
{
    public string Value { get; }

    private TcNo(string value) => Value = value;

    public static TcNo Create(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            throw new InvalidTcNoException("T.C. kimlik numarası boş olamaz.");

        var normalized = raw.Trim();

        if (normalized.Length != 11 || !normalized.All(char.IsDigit))
            throw new InvalidTcNoException("T.C. kimlik numarası 11 haneli rakam olmalıdır.");

        if (normalized[0] == '0')
            throw new InvalidTcNoException("T.C. kimlik numarası 0 ile başlayamaz.");

        // T.C. algoritması (checksum) — sentetik veri de geçerli formatta üretilsin diye uygulanır.
        if (!PassesChecksum(normalized))
            throw new InvalidTcNoException("T.C. kimlik numarası algoritma kontrolünden geçemedi.");

        return new TcNo(normalized);
    }

    /// <summary>
    /// TryCreate: use case'lerde exception yerine Result/bool tercih edildiğinde kullanılır.
    /// </summary>
    public static bool TryCreate(string? raw, out TcNo? tcNo)
    {
        try
        {
            tcNo = Create(raw);
            return true;
        }
        catch (InvalidTcNoException)
        {
            tcNo = null;
            return false;
        }
    }

    private static bool PassesChecksum(string tc)
    {
        var d = tc.Select(c => c - '0').ToArray();

        // 10. hane: (1+3+5+7+9)*7 - (2+4+6+8) mod 10
        var oddSum = d[0] + d[2] + d[4] + d[6] + d[8];
        var evenSum = d[1] + d[3] + d[5] + d[7];
        var digit10 = ((oddSum * 7) - evenSum) % 10;
        if (digit10 < 0) digit10 += 10;
        if (d[9] != digit10) return false;

        // 11. hane: ilk 10 hanenin toplamının mod 10'u
        var digit11 = d.Take(10).Sum() % 10;
        return d[10] == digit11;
    }

    public override string ToString() => Value;
    public bool Equals(TcNo? other) => other is not null && Value == other.Value;
    public override bool Equals(object? obj) => obj is TcNo other && Equals(other);
    public override int GetHashCode() => Value.GetHashCode(StringComparison.Ordinal);
    public static implicit operator string(TcNo tcNo) => tcNo.Value;
}
