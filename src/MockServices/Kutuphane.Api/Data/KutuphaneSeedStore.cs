using Kutuphane.Api.Models;
using MockServices.Shared;

namespace Kutuphane.Api.Data;

/// <summary>
/// Kütüphane demo verisi — Türkçe durum + okunabilir randevu konuları.
/// </summary>
public sealed class KutuphaneSeedStore
{
    private readonly Dictionary<string, KutuphaneResponse> _byTc;

    public KutuphaneSeedStore()
    {
        _byTc = SharedCitizenCatalog.All
            .Select((c, i) => Build(c, i))
            .ToDictionary(r => r.TcNo);
    }

    public KutuphaneResponse? Find(string tcNo) =>
        _byTc.TryGetValue(tcNo, out var record) ? record : null;

    private static KutuphaneResponse Build(SyntheticCitizen c, int index)
    {
        IReadOnlyList<KutuphaneRandevu> randevular = index switch
        {
            0 =>
            [
                new(new DateOnly(2026, 8, 10), "Sessiz Çalışma Salonu", "Ödev / çalışma masası", "Beklemede"),
                new(new DateOnly(2026, 7, 2), "Kitap Kulübü Salonu", "Kitap kulübü toplantısı", "Tamamlandı")
            ],
            1 =>
            [
                new(new DateOnly(2026, 8, 5), "Dijital Arşiv", "Belge tarama", "Beklemede")
            ],
            2 =>
            [
                new(new DateOnly(2026, 6, 18), "Çocuk Bölümü", "Etkinlik kaydı", "İptal")
            ],
            _ =>
            [
                new(
                    new DateOnly(2026, 5, 1).AddDays(index),
                    index % 2 == 0 ? "Sessiz Çalışma Salonu" : "Seminer Odası A",
                    index % 2 == 0 ? "Çalışma masası" : "Grup çalışması",
                    index % 3 == 0 ? "Tamamlandı" : "Beklemede")
            ]
        };

        var odunc = index switch
        {
            0 => 2,
            1 => 0,
            2 => 1,
            _ => index % 4
        };

        return new KutuphaneResponse(c.TcNo, c.AdSoyad, odunc, randevular);
    }
}
