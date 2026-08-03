namespace MockServices.Shared;

/// <summary>
/// SENTETİK T.C. kimlik no üretici — gerçek kişi verisi ÜRETMEZ.
/// Domain'deki TcNo checksum algoritması ile uyumlu format üretir;
/// Gateway seed'i ile mock servislerin aynı havuzu paylaşabilmesi için ortak tutuldu.
/// </summary>
public static class SyntheticTcNo
{
    public static string Generate(Random random)
    {
        ArgumentNullException.ThrowIfNull(random);

        var digits = new int[11];
        digits[0] = random.Next(1, 10); // 0 ile başlayamaz
        for (var i = 1; i < 9; i++)
            digits[i] = random.Next(0, 10);

        var oddSum = digits[0] + digits[2] + digits[4] + digits[6] + digits[8];
        var evenSum = digits[1] + digits[3] + digits[5] + digits[7];
        var digit10 = ((oddSum * 7) - evenSum) % 10;
        if (digit10 < 0) digit10 += 10;
        digits[9] = digit10;
        digits[10] = digits.Take(10).Sum() % 10;

        return string.Concat(digits);
    }
}
