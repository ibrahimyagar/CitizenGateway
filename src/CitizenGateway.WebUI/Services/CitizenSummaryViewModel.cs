using System.Text.Json.Serialization;

namespace CitizenGateway.WebUI.Services;

/// <summary>Gateway /summary yanıtının WebUI tarafındaki sade modeli.</summary>
public sealed class CitizenSummaryViewModel
{
    public string TcNo { get; set; } = "";
    public string? AdSoyad { get; set; }
    public SporCard? SporTesisi { get; set; }
    public KutuphaneCard? Kutuphane { get; set; }
    public CozumCard? CozumMerkezi { get; set; }
    public bool PartialFailure { get; set; }
    public List<string> FailedServices { get; set; } = [];
}

public sealed class SporCard
{
    public bool UyelikAktif { get; set; }
    public string TesisAdi { get; set; } = "";
    public int KontorBakiye { get; set; }
    public DateOnly UyelikBitisTarihi { get; set; }
}

public sealed class KutuphaneCard
{
    public int AktifOduncSayisi { get; set; }
    public List<RandevuItem> Randevular { get; set; } = [];
}

public sealed class RandevuItem
{
    public DateOnly Tarih { get; set; }
    public string Salon { get; set; } = "";
    public string Konu { get; set; } = "";
    public string Durum { get; set; } = "";
}

public sealed class CozumCard
{
    public int AcikTalepSayisi { get; set; }
    public List<TalepItem> Talepler { get; set; } = [];
}

public sealed class TalepItem
{
    public string TalepNo { get; set; } = "";
    public string Konu { get; set; } = "";
    public string Kategori { get; set; } = "";
    public string Durum { get; set; } = "";
    public DateOnly AcilisTarihi { get; set; }
}

public sealed class LoginResponse
{
    [JsonPropertyName("accessToken")]
    public string AccessToken { get; set; } = "";
}
