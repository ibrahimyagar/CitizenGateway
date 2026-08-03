namespace CitizenGateway.WebUI.Services;

/// <summary>
/// Değerlendirme hesapları — DbSeeder ile birebir aynı kimlikler.
/// WebUI Infrastructure'a bağlanmasın diye burada tutulur.
/// </summary>
public static class DemoAccounts
{
    public const string PersonelEmail = "aylin.kara@ornekkoy.bel.tr";
    public const string PersonelPassword = "Personel123!";

    /// <summary>SharedCitizenCatalog seed=42 → Ayşe Demir TC.</summary>
    public const string VatandasTcNo = "71151275166";
    public const string VatandasPassword = "Vatandas123!";
    public const string VatandasDisplayName = "Ayşe Demir";
}
