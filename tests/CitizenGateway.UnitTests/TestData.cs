using CitizenGateway.Domain.Entities;

namespace CitizenGateway.UnitTests;

/// <summary>
/// SENTETİK test sabitleri — SharedCitizenCatalog seed=42 ile uyumlu geçerli TC'ler.
/// </summary>
internal static class TestData
{
    public const string ValidTc1 = "71151275166";
    public const string ValidTc2 = "72253325032";
    public const string InvalidTc = "123";

    public static Citizen CreateCitizen(string tcNo = ValidTc1, string adSoyad = "Test Vatandas") =>
        Citizen.Create(tcNo, adSoyad, new DateOnly(1990, 1, 15), "05551234567");
}
