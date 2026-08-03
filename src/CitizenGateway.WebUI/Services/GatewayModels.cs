namespace CitizenGateway.WebUI.Services;

public sealed class LoginResponse
{
    public string AccessToken { get; set; } = "";
    public string Username { get; set; } = "";
    public string Role { get; set; } = "";
    public string? LinkedCitizenTcNo { get; set; }
}

public sealed class CitizenListItem
{
    public Guid Id { get; set; }
    public string TcNo { get; set; } = "";
    public string AdSoyad { get; set; } = "";
    public DateOnly DogumTarihi { get; set; }
    public string Telefon { get; set; } = "";
}

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

public sealed class ServiceRequestItem
{
    public Guid Id { get; set; }
    public string TcNo { get; set; } = "";
    public string RequestType { get; set; } = "";
    public string Status { get; set; } = "";
    public string TargetService { get; set; } = "";
    public DateTimeOffset CreatedAt { get; set; }
}

public sealed class AuditLogItem
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserRole { get; set; } = "";
    public string AccessedCitizenTcNo { get; set; } = "";
    public string AccessedEndpoint { get; set; } = "";
    public DateTimeOffset Timestamp { get; set; }
    public string IpAddress { get; set; } = "";
}

public sealed class HealthStatus
{
    public string Status { get; set; } = "Unknown";
    public string? Service { get; set; }
}
