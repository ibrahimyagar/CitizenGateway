namespace CitizenGateway.Domain.Enums;

/// <summary>
/// JWT role claim ile birebir eşleşecek roller.
/// Personel: tüm vatandaş verisine erişir. Vatandas: yalnızca kendi kaydı.
/// </summary>
public enum UserRole
{
    Personel = 1,
    Vatandas = 2
}
